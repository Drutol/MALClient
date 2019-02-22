// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using ModernHttpClient;

namespace MALClient.UWP.Shared.Managers
{
    /// <summary>
    /// Provides methods and tools to cache images in a temporary local folder
    /// </summary>
    public static class ImageCache
    {
        private const string CacheFolderName = "ImageCache";

        private static readonly HttpClient _client;
        private static readonly SemaphoreSlim _cacheFolderSemaphore = new SemaphoreSlim(1);
        private static readonly Dictionary<string, Task> _concurrentTasks = new Dictionary<string, Task>();

        private static StorageFolder _cacheFolder;

        static ImageCache()
        {
            CacheDuration = TimeSpan.FromDays(3);

            _client = new HttpClient(new NativeMessageHandler());
            _client.ConfigureToAcceptCompressedContent();
        }

        /// <summary>
        /// Gets or sets the life duration of every cache entry.
        /// </summary>
        public static TimeSpan CacheDuration { get; set; }

        /// <summary>
        /// call this method to clear the entire cache.
        /// </summary>
        /// <param name="duration">Use this parameter to define a timespan from now to select cache entries to delete.</param>
        /// <returns>Task</returns>
        public static async Task ClearAsync(TimeSpan? duration = null)
        {
            duration = duration ?? TimeSpan.FromSeconds(0);
            DateTime expirationDate = DateTime.Now.Subtract(duration.Value);
            try
            {
                var folder = await GetCacheFolderAsync();
                foreach (var file in await folder.GetFilesAsync())
                {
                    try
                    {
                        if (file.DateCreated < expirationDate)
                        {
                            await file.DeleteAsync();
                        }
                    }
                    catch
                    {
                        // Just ignore errors for now
                    }
                }
            }
            catch
            {
                // Just ignore errors for now
            }
        }

        public static async void PerformScheduledCacheCleanup()
        {
            var date = ResourceLocator.ApplicationDataService["LastImageCacheCleanup"];
            if (date == null)
            {
                ResourceLocator.ApplicationDataService["LastImageCacheCleanup"] = DateTime.Now.ToBinary();
                return;
            }
            var lastCleanup = DateTime.FromBinary((long) date);
            if (DateTime.Now.Subtract(lastCleanup).TotalDays >= 1) //perform it once a day
            {
                await ClearAsync(CacheDuration);
                ResourceLocator.ApplicationDataService["LastImageCacheCleanup"] = DateTime.Now.ToBinary();
            }
        }

        /// <summary>
        /// Load a specific image from the cache. If the image is not in the cache, ImageCache will try to download and store it.
        /// </summary>
        /// <param name="uri">Uri of the image.</param>
        /// <returns>a BitmapImage</returns>
        public static async Task<Uri> GetFromCacheAsync(Uri uri)
        {
            if (!Settings.EnableImageCache)
                return uri;

            Task busy;
            string key = GetCacheFileName(uri);

            lock (_concurrentTasks)
            {
                if (_concurrentTasks.ContainsKey(key))
                {
                    busy = _concurrentTasks[key];
                }
                else
                {
                    busy = EnsureFileAsync(uri);
                    _concurrentTasks.Add(key, busy);
                }
            }

            try
            {
                await busy;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return uri;
            }
            finally
            {
                lock (_concurrentTasks)
                {
                    if (_concurrentTasks.ContainsKey(key))
                    {
                        _concurrentTasks.Remove(key);
                    }
                }
            }

            return CreateBitmapImage(key);
        }

        /// <summary>
        /// Gets the local cache file name associated with a specified Uri.
        /// </summary>
        /// <param name="uri">Uri of the resource.</param>
        /// <returns>Filename associated with the Uri.</returns>
        public static string GetCacheFileName(Uri uri)
        {
            ulong uriHash = CreateHash64(uri);

            return $"{uriHash}.webp";
        }

        private static Uri CreateBitmapImage(string fileName)
        {
            return new Uri($"ms-appdata:///temp/{CacheFolderName}/{fileName}");
        }

        private static async Task EnsureFileAsync(Uri uri)
        {
            DateTime expirationDate = DateTime.Now.Subtract(CacheDuration);

            var folder = await GetCacheFolderAsync();

            string fileName = GetCacheFileName(uri);
            var baseFile = await folder.TryGetItemAsync(fileName) as StorageFile;
            if (await IsFileOutOfDate(baseFile, expirationDate))
            {
                baseFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                try
                {
                    await GetHttpStreamToStorageFileAsync(uri, baseFile);
                }
                catch
                {
                    await baseFile.DeleteAsync();
                }
            }
        }

        private static async Task<bool> IsFileOutOfDate(StorageFile file, DateTime expirationDate)
        {
            if (file != null)
            {
                var properties = await file.GetBasicPropertiesAsync();
                return properties.DateModified < expirationDate;
            }

            return true;
        }

        private static async Task<StorageFolder> GetCacheFolderAsync()
        {
            if (_cacheFolder == null)
            {
                await _cacheFolderSemaphore.WaitAsync();
                try
                {
                    _cacheFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
                }
                catch
                {
                }
                finally
                {
                    _cacheFolderSemaphore.Release();
                }
            }

            return _cacheFolder;
        }

        private static ulong CreateHash64(Uri uri)
        {
            return CreateHash64(uri.Host + uri.PathAndQuery);
        }

        private static ulong CreateHash64(string str)
        {
            byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(str);

            ulong value = (ulong)utf8.Length;
            for (int n = 0; n < utf8.Length; n++)
            {
                value += (ulong)utf8[n] << ((n * 5) % 56);
            }

            return value;
        }

        /// <summary>
        /// Get the response stream returned by a HTTP get request and save it to a local file.
        /// </summary>
        /// <param name="uri">Uri to request.</param>
        /// <param name="targetFile">StorageFile to save the stream to.</param>
        /// <returns>True if success.</returns>
        public static async Task GetHttpStreamToStorageFileAsync(
            this Uri uri,
            StorageFile targetFile)
        {
            using (var content = await GetHttpContentAsync(uri)
                                            .ConfigureAwait(false))
            {
                using (var fileStream = await targetFile.OpenStreamForWriteAsync())
                {
                    using (var responseSream = await content.GetDecompressionStreamAsync()
                                                             .ConfigureAwait(false))
                    {
                        int bytesRead = 0;
                        byte[] buffer = new byte[8192]; //8kb

                        while ((bytesRead = responseSream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }

                        await fileStream.FlushAsync()
                                        .ConfigureAwait(false);
                    }
                }
            }
        }

        private static async Task<HttpResponseMessage> GetHttpContentAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException();
            }

            var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead)
                                        .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}