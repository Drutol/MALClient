using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using MALClient.Adapters;
using Newtonsoft.Json;

namespace MALClient.UWP.Adapters
{
    public class DataCache : IDataCache
    {
        public async Task SaveData<T>(T data, string filename, string targetFolder)
        {
            var folder = string.IsNullOrEmpty(targetFolder)
                ? ApplicationData.Current.LocalFolder
                : await
                    ApplicationData.Current.LocalFolder.CreateFolderAsync(targetFolder,
                        CreationCollisionOption.OpenIfExists);
            await SaveData(data, filename, folder);
        }

        public async Task<T> RetrieveData<T>(string filename, string originFolder, int expiration)
        {
            var folder = string.IsNullOrEmpty(originFolder)
                ? ApplicationData.Current.LocalFolder
                : await
                    ApplicationData.Current.LocalFolder.CreateFolderAsync(originFolder,
                        CreationCollisionOption.OpenIfExists);
            return await RetrieveData<T>(filename, folder, expiration);
        }

        public async Task SaveDataRoaming<T>(T data, string filename)
        {
            await SaveData(data, filename, ApplicationData.Current.RoamingFolder);
        }

        public async Task<T> RetrieveDataRoaming<T>(string filename, int expiration)
        {
            return await RetrieveData<T>(filename, ApplicationData.Current.RoamingFolder, 0);
        }

        public async Task ClearApiRelatedCache()
        {
            StorageFile file;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync("mal_to_hum.json");
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception)
            {
                //
            }
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync("volatile_data.json");
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception)
            {
                //
            }
            try
            {
                var files = await ApplicationData.Current.LocalFolder.GetFilesAsync(CommonFileQuery.DefaultQuery);
                foreach (var listFile in files.Where(storageFile => storageFile.Name.Contains("_data_")))
                {
                    await listFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
            catch (Exception)
            {
                //
            }
            try
            {
                await (await ApplicationData.Current.LocalFolder.GetFolderAsync("AnimeDetails")).DeleteAsync(
                    StorageDeleteOption.PermanentDelete);
            }
            catch (Exception)
            {
                //
            }
        }

        public async Task ClearAnimeListData()
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync(CommonFileQuery.DefaultQuery);
            foreach (var listFile in files.Where(storageFile => storageFile.Name.Contains("_data_")))
            {
                await listFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        private static bool CheckForOldData(DateTime date, int days = 7)
        {
            var diff = DateTime.Now.ToUniversalTime().Subtract(date);
            if (diff.TotalDays >= days)
                return false;
            return true;
        }


        public async Task SaveData<T>(T data, string filename, StorageFolder targetFolder)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var folder = targetFolder ?? ApplicationData.Current.LocalFolder;
                    var json =
                        JsonConvert.SerializeObject(new Tuple<DateTime, T>(DateTime.UtcNow, data));
                    var file =
                        await
                            folder.CreateFileAsync(
                                $"{filename}",
                                CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, json);
                });
            }
            catch (Exception e)
            {
                //magic
            }
        }

        public async Task<T> RetrieveData<T>(string filename, StorageFolder originFolder, int expiration)
        {
            try
            {
                var folder = originFolder ?? ApplicationData.Current.LocalFolder;
                var file =
                    await
                        folder.GetFileAsync(
                            $"{filename}");
                var data = await FileIO.ReadTextAsync(file);
                var tuple =
                    JsonConvert.DeserializeObject<Tuple<DateTime, T>>(data);
                return expiration >= 1
                    ? CheckForOldData(tuple.Item1, expiration) ? tuple.Item2 : default(T)
                    : tuple.Item2;
            }
            catch (Exception)
            {
                //No file
            }
            return default(T);
        }
    }
}