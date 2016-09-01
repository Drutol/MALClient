using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using MALClient.Adapters;
using Newtonsoft.Json;

namespace MALClient.Android.Adapters
{
    public class DataCache : IDataCache
    {
        public async Task SaveData<T>(T data, string filename, string targetFolder)
        {
            try
            {
                await Task.Run(() =>
                {
                    targetFolder = string.IsNullOrEmpty(targetFolder) ? Application.Context.FilesDir.Path : Path.Combine(Application.Context.FilesDir.Path, targetFolder);
                    var filepath = Path.Combine(targetFolder, filename);

                    var json = JsonConvert.SerializeObject(new Tuple<DateTime, T>(DateTime.UtcNow, data));
                    File.WriteAllText(filepath, json);
                });
            }
            catch (Exception)
            {
                //
            }
        }

        public async Task<T> RetrieveData<T>(string filename, string originFolder, int expiration)
        {
            try
            {
                originFolder = string.IsNullOrEmpty(originFolder) ? Application.Context.FilesDir.Path : Path.Combine(Application.Context.FilesDir.Path,originFolder);
                var filepath = Path.Combine(originFolder, filename);
                if (!File.Exists(filepath))
                {
                    return default(T);
                }
                var data = File.ReadAllText(filepath);

                var tuple = JsonConvert.DeserializeObject<Tuple<DateTime, T>>(data);
                
                return tuple.Item2;
               
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public async Task SaveDataRoaming<T>(T data, string filename)
        {
            await SaveData(data, filename, "");
        }

        public async Task<T> RetrieveDataRoaming<T>(string filename, int expiration)
        {
            return await RetrieveData<T>(filename, "", expiration);
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