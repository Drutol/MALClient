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
        public List<string> Genres { get; set; } 
    }

    public static class DataCache
    {
        static DataCache()
        {
            LoadVolatileData();
            LoadSeasonalurls();
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

        public static async void SaveSeasonalData(List<SeasonalAnimeData> data,string tag)
        {
            await Task.Run(async () =>
            {
                var json =
                    JsonConvert.SerializeObject(new Tuple<DateTime, List<SeasonalAnimeData>>(DateTime.UtcNow, data));
                StorageFile file =
                    await
                        ApplicationData.Current.LocalFolder.CreateFileAsync($"seasonal_data{tag}.json",
                            CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            });
        }

        public static async Task<List<SeasonalAnimeData>> RetrieveSeasonalData(string tag)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync($"seasonal_data{tag}.json");
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
        public static Dictionary<string, string> SeasonalUrls; 

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
            if (_volatileDataCache.ContainsKey(id))
            {
                //We don't want to lose data here , only anime from seasonal contains genres data.
                if (data.Genres != null && data.Genres.Count > 0)
                    _volatileDataCache[id].Genres = data.Genres;
                _volatileDataCache[id].DayOfAiring = data.DayOfAiring;
                _volatileDataCache[id].GlobalScore = data.GlobalScore;
            }
            else
                _volatileDataCache[id] = data;

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

        #region SeasonalUrls
        public static async void SaveSeasonalUrls(Dictionary<string, string> seasonData)
        {
            try
            {
                SeasonalUrls = seasonData;
                var json = JsonConvert.SerializeObject(seasonData);
                StorageFile file =
                    await
                        ApplicationData.Current.LocalFolder.CreateFileAsync("seasonal_urls.json",
                            CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private static async void LoadSeasonalurls()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("seasonal_urls.json");
                var data = await FileIO.ReadTextAsync(file);
                SeasonalUrls = JsonConvert.DeserializeObject<Dictionary<string, string>>(data) ??
                                     new Dictionary<string,string>();
            }
            catch (Exception)
            {
                SeasonalUrls = new Dictionary<string, string>();
            }
        }
        #endregion

        #region Reviews
        public static async void SaveAnimeReviews(int id, List<AnimeReviewData> data)
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("AnimeDetails", CreationCollisionOption.OpenIfExists);
            await Task.Run(async () =>
            {
                var json =
                    JsonConvert.SerializeObject(new Tuple<DateTime, List<AnimeReviewData>>(DateTime.UtcNow, data));
                StorageFile file =
                    await
                        folder.CreateFileAsync($"reviews_{id}.json",
                            CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            });
        }

        public static async Task<List<AnimeReviewData>> RetrieveReviewsData(int animeId)
        {
            try
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("AnimeDetails", CreationCollisionOption.OpenIfExists);
                StorageFile file = await folder.GetFileAsync($"reviews_{animeId}.json");
                var data = await FileIO.ReadTextAsync(file);
                Tuple<DateTime, List<AnimeReviewData>> tuple =
                    JsonConvert.DeserializeObject<Tuple<DateTime, List<AnimeReviewData>>>(data);
                return CheckForOldDataDetails(tuple.Item1) ? tuple.Item2 : null;
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }
        #endregion

        #region DirectRecommendations
        public static async void SaveDirectRecommendationsData(int id, List<DirectRecommendationData> data)
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("AnimeDetails", CreationCollisionOption.OpenIfExists);
            await Task.Run(async () =>
            {
                var json =
                    JsonConvert.SerializeObject(new Tuple<DateTime, List<DirectRecommendationData>>(DateTime.UtcNow, data));
                StorageFile file =
                    await
                        folder.CreateFileAsync($"direct_recommendations_{id}.json",
                            CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            });
        }

        public static async Task<List<DirectRecommendationData>> RetrieveDirectRecommendationData(int id)
        {
            try
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("AnimeDetails", CreationCollisionOption.OpenIfExists);
                StorageFile file = await folder.GetFileAsync($"direct_recommendations_{id}.json");
                var data = await FileIO.ReadTextAsync(file);
                Tuple<DateTime, List<DirectRecommendationData>> tuple =
                    JsonConvert.DeserializeObject<Tuple<DateTime, List<DirectRecommendationData>>>(data);
                return CheckForOldDataDetails(tuple.Item1) ? tuple.Item2 : null;
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }


        #endregion
    }
}