using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using MALClient.Adapters;
using MALClient.UWP.Shared;
using ModernHttpClient;
using MALClient.XShared.Utils;

namespace MALClient.UWP.Adapters
{
    public class ImageDownloaderService : IImageDownloaderService
    {
        private readonly HttpClient _client;

        public ImageDownloaderService()
        {
            _client = new HttpClient(new NativeMessageHandler());
            _client.ConfigureToAcceptCompressedContent();
        }


        public async void DownloadImage(string url, string suggestedFilename,bool animeCover)
        {
            if (url == null)
                return;
            try
            {
                var sp = new FileSavePicker();
                sp.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                sp.FileTypeChoices.Add("Portable Network Graphics (*.png)", new List<string> {".png"});
                sp.SuggestedFileName = $"{suggestedFilename}-cover_art";

                var file = await sp.PickSaveFileAsync();
                if (file == null)
                    return;
                await Download(url, file,animeCover);
                UWPUtilities.GiveStatusBarFeedback("File saved successfully.");
            }
            catch (Exception)
            {
                UWPUtilities.GiveStatusBarFeedback("Error. File didn't save properly.");
            }
        }

        public async void DownloadImageDefault(string url, string suggestedFilename,bool animeCover)
        {
            try
            {
                var lib = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                var folder = await lib.SaveFolder.CreateFolderAsync("MALCLient Images",CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync(UWPUtilities.SanitizeFileName(suggestedFilename) + ".png");
                await Download(url, file,animeCover);
                UWPUtilities.GiveStatusBarFeedback("File saved successfully.");
            }
            catch (Exception)
            {
                UWPUtilities.GiveStatusBarFeedback("Error. File didn't save properly.");
            }
        }

        private async Task Download(string url,StorageFile file,bool cover)
        {
            string betterUrl = url;
            Stream responseStream = null;

            if (cover)
            {
                var pos = betterUrl.IndexOf(".jpg");
                if (pos != -1)
                    betterUrl = betterUrl.Insert(pos, "l");
            }

            // Get image stream...
            try
            {
                using (var response = await _client.GetAsync(betterUrl)
                                                   .ConfigureAwait(false))
                {
                    responseStream = await response.GetDecompressionStreamAsync()
                                                   .ConfigureAwait(false);
                }               
            }
            catch(Exception)
            {
                using (var response = await _client.GetAsync(url)
                                                   .ConfigureAwait(false))
                {
                    responseStream = await response.GetDecompressionStreamAsync()
                                                   .ConfigureAwait(false);
                }
            }


            if (responseStream == null)
            {
                throw new Exception($"Failed to download image for: ${url}");
            }

            // Write stream to file...
            using (responseStream)
            {
                using (var reader = new StreamReader(responseStream))
                {
                    var fs = await file.OpenStreamForWriteAsync();
                    var writer = new DataWriter(fs.AsOutputStream());

                    writer.WriteString(await reader.ReadToEndAsync()
                                                   .ConfigureAwait(false));

                    await writer.StoreAsync();
                    await writer.FlushAsync();

                    writer.Dispose();
                    fs.Dispose();
                }
            }
        }
    }
}
