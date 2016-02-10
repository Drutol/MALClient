using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using MALClient.Items;
using MALClient.Models;
using Newtonsoft.Json;

namespace MALClient
{
    /// <summary>
    ///     Contains stuff like GlobalScore and air date
    /// </summary>
    public class VolatileDataCache
    {
        public float GlobalScore { get; set; }
        public int DayOfAiring { get; set; }
    }

    public static class DataCache
    {
        static DataCache()
        {
            LoadVolatileData();
        }

        #region UserData

        public static async void SaveDataForUser(string user, string data)
        {
            if (!Utils.IsCachingEnabled())
                return;
            try
            {
                StorageFile file =
                    await
                        ApplicationData.Current.LocalFolder.CreateFileAsync($"anime_data_{user.ToLower()}.json",
                            CreationCollisionOption.ReplaceExisting);
                await
                    FileIO.WriteTextAsync(file,
                        JsonConvert.SerializeObject(new Tuple<DateTime, string>(DateTime.Now, data)));
            }
            catch (Exception)
            {
                //
            }
        }

        public static async Task<Tuple<string, DateTime>> RetrieveDataForUser(string user)
        {
            if (!Utils.IsCachingEnabled())
                return null;
            try
            {
                StorageFile file =
                    await ApplicationData.Current.LocalFolder.GetFileAsync($"anime_data_{user.ToLower()}.json");
                var data = await FileIO.ReadTextAsync(file);
                Tuple<DateTime, string> decoded = JsonConvert.DeserializeObject<Tuple<DateTime, string>>(data);
                if (!CheckForOldData(decoded.Item1))
                {
                    await file.DeleteAsync();
                    return null;
                }
                return new Tuple<string, DateTime>(decoded.Item2, decoded.Item1);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool CheckForOldData(DateTime timestamp)
        {
            TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(timestamp);
            if (diff.TotalSeconds > Utils.GetCachePersitence())
                return false;
            return true;
        }

        private static bool CheckForOldDataSeason(DateTime date)
        {
            TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(date);
            if (diff.TotalSeconds > 86400) //1day
                return false;
            return true;
        }

        #endregion

        #region SeasonData

        public static async void SaveSeasonalData(List<SeasonalAnimeData> data)
        {
            await Task.Run(async () =>
            {
                var json =
                    JsonConvert.SerializeObject(new Tuple<DateTime, List<SeasonalAnimeData>>(DateTime.UtcNow, data));
                StorageFile file =
                    await
                        ApplicationData.Current.LocalFolder.CreateFileAsync("seasonal_data.json",
                            CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            });
        }

        public static async Task<List<SeasonalAnimeData>> RetrieveSeasonalData()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("seasonal_data.json");
                var data = await FileIO.ReadTextAsync(file);
                Tuple<DateTime, List<SeasonalAnimeData>> tuple =
                    JsonConvert.DeserializeObject<Tuple<DateTime, List<SeasonalAnimeData>>>(data);
                return CheckForOldDataSeason(tuple.Item1) ? tuple.Item2 : null;
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }

        #endregion

        #region VolatileData

        private static Dictionary<int, VolatileDataCache> _volatileDataCache;

        private static async void LoadVolatileData()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("volatile_data.json");
                var data = await FileIO.ReadTextAsync(file);
                _volatileDataCache = JsonConvert.DeserializeObject<Dictionary<int, VolatileDataCache>>(data) ??
                                     new Dictionary<int, VolatileDataCache>();
            }
            catch (Exception)
            {
                _volatileDataCache = new Dictionary<int, VolatileDataCache>();
            }
        }

        public static async void SaveVolatileData()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_volatileDataCache);
                StorageFile file =
                    await
                        ApplicationData.Current.LocalFolder.CreateFileAsync("volatile_data.json",
                            CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public static void RegisterVolatileData(int id, VolatileDataCache data)
        {
            _volatileDataCache[id] = data;
        }

        public static void DeregisterVolatileData(int id)
        {
            try
            {
                _volatileDataCache[id].DayOfAiring = -1;
            }
            catch (Exception)
            {
                /*ignore*/
            }
        }

        public static bool TryRetrieveDataForId(int id, out VolatileDataCache data)
        {
            try
            {
                return _volatileDataCache.TryGetValue(id, out data);
            }
            catch (Exception)
            {
                data = null;
                return false;
            }
        }

        #endregion

        #region AnimeDetailsData

        public static async void SaveAnimeDetails(int id, AnimeGeneralDetailsData data)
        {          
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("AnimeDetails", CreationCollisionOption.OpenIfExists);
            await Task.Run(async () =>
            {
                var json =
                    JsonConvert.SerializeObject(new Tuple<DateTime, AnimeGeneralDetailsData>(DateTime.UtcNow, data));
                StorageFile file =
                    await
                        folder.CreateFileAsync($"{id}.json",
                            CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            }); 
        }

        public static async Task<AnimeGeneralDetailsData> RetrieveAnimeGeneralDetailsData(int id)
        {
            try
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("AnimeDetails", CreationCollisionOption.OpenIfExists);
                StorageFile file = await folder.GetFileAsync($"{id}.json");
                var data = await FileIO.ReadTextAsync(file);
                Tuple<DateTime, AnimeGeneralDetailsData> tuple =
                    JsonConvert.DeserializeObject<Tuple<DateTime, AnimeGeneralDetailsData>>(data);
                return CheckForOldDataDetails(tuple.Item1) ? tuple.Item2 : null;
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }

        private static bool CheckForOldDataDetails(DateTime date)
        {
            TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(date);
            if (diff.TotalDays > 7)
                return false;
            return true;
        }
        #endregion
    }
}