using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using MALClient.Items;
using Newtonsoft.Json;
using WinRTXamlToolkit.IO.Serialization;

namespace MALClient
{
    public static class DataCache
    {
        #region UserData

        public static async void SaveDataForUser(string user, string data)
        {
            if(!Utils.IsCachingEnabled())
                return;
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync($"anime_data_{user.ToLower()}.xml",CreationCollisionOption.ReplaceExisting);
            var builder = new StringBuilder(data);
            builder.AppendLine(Utils.ConvertToUnixTimestamp(DateTime.Now).ToString());
            data = builder.ToString();
            await FileIO.WriteTextAsync(file, data);
        }

        public static async Task<Tuple<string,DateTime>> RetrieveDataForUser(string user)
        {
            if (!Utils.IsCachingEnabled())
                return null;
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync($"anime_data_{user.ToLower()}.xml");
                var data = await FileIO.ReadTextAsync(file);
                var lines = data.Split(new char[] {'\n', '\r'},StringSplitOptions.RemoveEmptyEntries);
                DateTime lastUpdateTime = new DateTime();
                if (!CheckForOldData(lines[lines.Length - 1],ref lastUpdateTime))
                {
                    await file.DeleteAsync();
                    return null;
                }
                data = "";
                for (int i = 0; i < lines.Length - 1; i++)
                {
                    data += lines[i];
                }
                return new Tuple<string, DateTime>(data,lastUpdateTime);
            }
            catch (Exception)
            {
                return null;
            }            
        }

        private static bool CheckForOldData(string timestamp, ref DateTime time)
        {
            var data = Utils.ConvertFromUnixTimestamp(double.Parse(timestamp));
            TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(data);
            time = data;
            if (diff.TotalSeconds > Utils.GetCachePersitence())
                return false;
            return true;
        }

        private static bool CheckForOldData(DateTime date)
        {
            TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(date);
            if (diff.TotalSeconds > 86400) //1day
                return false;
            return true;
        }
        #endregion

        public static async void SaveSeasonalData(List<SeasonalAnimeData> data)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new Tuple<DateTime,List<SeasonalAnimeData>>(DateTime.UtcNow,data));
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("seasonal_data.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, json);
        }

        public static async Task<List<SeasonalAnimeData>> RetrieveSeasonalData()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("seasonal_data.json");
                var data = await FileIO.ReadTextAsync(file);
                var tuple = JsonConvert.DeserializeObject<Tuple<DateTime, List<SeasonalAnimeData>>>(data);
                return CheckForOldData(tuple.Item1) ? tuple.Item2 : null;
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }
    }
}
