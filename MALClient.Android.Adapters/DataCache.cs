using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Java.IO;
using Java.Nio;
using MALClient.Adapters;
using Newtonsoft.Json;
using Console = System.Console;
using File = Java.IO.File;

namespace MALClient.Android.Adapters
{
    public class DataCache : IDataCache
    {
        public async Task SaveData<T>(T data, string filename, string targetFolder)
        {
            try
            {
                await Task.Run( async () =>
                {
                    targetFolder = string.IsNullOrEmpty(targetFolder) ? Application.Context.GetExternalFilesDir(null).Path : Path.Combine(Application.Context.GetExternalFilesDir(null).Path, targetFolder);
                    var file = new File(Path.Combine(targetFolder, filename));
                    if (!file.ParentFile.Exists())
                        file.ParentFile.Mkdir();
                    file.CreateNewFile();
                    var json = JsonConvert.SerializeObject(new Tuple<DateTime, T>(DateTime.UtcNow, data));
                    using (FileWriter writer = new FileWriter(file))
                    {
                        await writer.WriteAsync(json);
                        writer.Close();
                    }                    
                });
            }
            catch (Exception e)
            {
                //
            }
        }

        public async Task<T> RetrieveData<T>(string filename, string originFolder, int expiration) 
        {
            try
            {
                originFolder = string.IsNullOrEmpty(originFolder) ? Application.Context.GetExternalFilesDir(null).Path : Path.Combine(Application.Context.GetExternalFilesDir(null).Path, originFolder);
                var file = new File(Path.Combine(originFolder, filename));
                StringBuilder text = new StringBuilder();

                try
                {
                    using (BufferedReader br = new BufferedReader(new FileReader(file)))
                    {
                        string line;
                        while ((line = await br.ReadLineAsync()) != null)
                        {
                            text.Append(line);
                        }
                    }
                }
                catch (Exception e)
                {
                    return default(T);
                }


                var tuple = JsonConvert.DeserializeObject<Tuple<DateTime, T>>(text.ToString());
                
                return tuple.Item2;
               
            }
            catch (Exception e)
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
            try
            {
                var root = new File(Application.Context.GetExternalFilesDir(null).Path);
                foreach (var file in await root.ListFilesAsync())
                {
                    if (file.Name.Contains("_data_"))
                        file.Delete();
                }
            }
            catch (Exception)
            {

            }
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