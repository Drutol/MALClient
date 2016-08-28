using System;
using System.Threading.Tasks;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class DataCache : IDataCache
    {
        public async Task SaveData<T>(T data, string filename, string targetFolder)
        {
            //var folder = string.IsNullOrEmpty(targetFolder)
            //    ? ApplicationData.Current.LocalFolder
            //    : await
            //        ApplicationData.Current.LocalFolder.CreateFolderAsync(targetFolder,
            //            CreationCollisionOption.OpenIfExists);
            //await SaveData(data, filename, folder);
        }

        public async Task<T> RetrieveData<T>(string filename, string originFolder, int expiration)
        {
            //var folder = string.IsNullOrEmpty(originFolder)
            //    ? ApplicationData.Current.LocalFolder
            //    : await
            //        ApplicationData.Current.LocalFolder.CreateFolderAsync(originFolder,
            //            CreationCollisionOption.OpenIfExists);
            //return await RetrieveData<T>(filename, folder, expiration);
            return default(T);
        }

        public async Task SaveDataRoaming<T>(T data, string filename)
        {
            //await SaveData(data, filename, ApplicationData.Current.RoamingFolder);
        }

        public async Task<T> RetrieveDataRoaming<T>(string filename, int expiration)
        {
            //return await RetrieveData<T>(filename, ApplicationData.Current.RoamingFolder, 0);
            return default(T);
        }

        public async Task ClearApiRelatedCache()
        {
            //StorageFile file;
            //try
            //{
            //    file = await ApplicationData.Current.LocalFolder.GetFileAsync("mal_to_hum.json");
            //    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            //}
            //catch (Exception)
            //{
            //    //
            //}
            //try
            //{
            //    file = await ApplicationData.Current.LocalFolder.GetFileAsync("volatile_data.json");
            //    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            //}
            //catch (Exception)
            //{
            //    //
            //}
            //try
            //{
            //    var files = await ApplicationData.Current.LocalFolder.GetFilesAsync(CommonFileQuery.DefaultQuery);
            //    foreach (var listFile in files.Where(storageFile => storageFile.Name.Contains("_data_")))
            //    {
            //        await listFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            //    }
            //}
            //catch (Exception)
            //{
            //    //
            //}
            //try
            //{
            //    await (await ApplicationData.Current.LocalFolder.GetFolderAsync("AnimeDetails")).DeleteAsync(
            //        StorageDeleteOption.PermanentDelete);
            //}
            //catch (Exception)
            //{
            //    //
            //}
        }

        public async Task ClearAnimeListData()
        {
            //var files = await ApplicationData.Current.LocalFolder.GetFilesAsync(CommonFileQuery.DefaultQuery);
            //foreach (var listFile in files.Where(storageFile => storageFile.Name.Contains("_data_")))
            //{
            //    await listFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            //}
        }

        private static bool CheckForOldData(DateTime date, int days = 7)
        {
            var diff = DateTime.Now.ToUniversalTime().Subtract(date);
            if (diff.TotalDays >= days)
                return false;
            return true;
        }


        
    }
}