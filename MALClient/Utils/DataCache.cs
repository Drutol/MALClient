using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using MALClient.Comm.Anime;
using MALClient.Items;
using MALClient.Models;
using MALClient.Pages;
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
        public string AirStartDate { get; set; }
    }

    public static class DataCache
    {
        static DataCache()
        {
            LoadVolatileData();
            LoadSeasonalurls();
            RetrieveHumMalIdDictionary();
        }

        #region UserData

        public static async Task SaveDataForUser(string user, IEnumerable<ILibraryData> data, AnimeListWorkModes mode)
        {
            if (!Settings.IsCachingEnabled)
                return;
            try
            {
                var file =
                    await
                        ApplicationData.Current.LocalFolder.CreateFileAsync(
                            $"{(mode == AnimeListWorkModes.Anime ? "anime" : "manga")}_data_{user.ToLower()}.json",
                            CreationCollisionOption.ReplaceExisting);
                await
                    FileIO.WriteTextAsync(file,
                        mode == AnimeListWorkModes.Anime
                            ? JsonConvert.SerializeObject(new Tuple<DateTime, IEnumerable<AnimeLibraryItemData>>(DateTime.Now,
                                data.Select(item => item as AnimeLibraryItemData)))
                            : JsonConvert.SerializeObject(new Tuple<DateTime, IEnumerable<MangaLibraryItemData>>(DateTime.Now,
                                data.Select(item => item as MangaLibraryItemData))));
            }
            catch (Exception)
            {
                //
            }
        }

        public static async Task<List<ILibraryData>> RetrieveDataForUser(string user, AnimeListWorkModes mode)
        {
            if (!Settings.IsCachingEnabled)
                return null;
            try
            {
                var file =
                    await
                        ApplicationData.Current.LocalFolder.GetFileAsync(
                            $"{(mode == AnimeListWorkModes.Anime ? "anime" : "manga")}_data_{user.ToLower()}.json");
                var data = await FileIO.ReadTextAsync(file);
                var decoded = new List<ILibraryData>();
                if (mode == AnimeListWorkModes.Anime)
                {
                    var jsonObj = JsonConvert.DeserializeObject<Tuple<DateTime, List<AnimeLibraryItemData>>>(data);
                    if (!CheckForOldData(jsonObj.Item1))
                    {
                        await file.DeleteAsync();
                        return null;
                    }
                    decoded.AddRange(jsonObj.Item2.Select(item => item as ILibraryData));
                }
                else
                {
                    var jsonObj = JsonConvert.DeserializeObject<Tuple<DateTime, List<MangaLibraryItemData>>>(data);
                    if (!CheckForOldData(jsonObj.Item1))
                    {
                        await file.DeleteAsync();
                        return null;
                    }
                    decoded.AddRange(jsonObj.Item2.Select(item => item as ILibraryData));
                }

                return decoded;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool CheckForOldData(DateTime timestamp)
        {
            var diff = DateTime.Now.ToUniversalTime().Subtract(timestamp);
            if (diff.TotalSeconds > Settings.CachePersitence)
                return false;
            return true;
        }

        private static bool CheckForOldDataSeason(DateTime date)
        {
            var diff = DateTime.Now.ToUniversalTime().Subtract(date);
            if (diff.TotalSeconds > 86400) //1day
                return false;
            return true;
        }

        #endregion

        #region SeasonData

        public static async void SaveSeasonalData(List<SeasonalAnimeData> data, string tag)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var json =
                        JsonConvert.SerializeObject(new Tuple<DateTime, List<SeasonalAnimeData>>(DateTime.UtcNow, data));
                    var file =
                        await
                            ApplicationData.Current.LocalFolder.CreateFileAsync($"seasonal_data{tag}.json",
                                CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, json);
                });
            }
            catch (Exception)
            {
                // file replace exception?
            }

        }

        public static async Task<List<SeasonalAnimeData>> RetrieveSeasonalData(string tag)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync($"seasonal_data{tag}.json");
                var data = await FileIO.ReadTextAsync(file);
                var tuple =
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
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("volatile_data.json");
                var data = await FileIO.ReadTextAsync(file);
                _volatileDataCache = JsonConvert.DeserializeObject<Dictionary<int, VolatileDataCache>>(data) ??
                                     new Dictionary<int, VolatileDataCache>();
            }
            catch (Exception)
            {
                _volatileDataCache = new Dictionary<int, VolatileDataCache>();
            }
        }

        public static async Task SaveVolatileData()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_volatileDataCache);
                var file =
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
                _volatileDataCache[id].AirStartDate = data.AirStartDate;
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

        public static async void SaveAnimeDetails(int id, AnimeDetailsData data, bool anime = true)
        {
            try
            {
                var folder =
                    await
                        ApplicationData.Current.LocalFolder.CreateFolderAsync(anime ? "AnimeDetails" : "MangaDetails",
                            CreationCollisionOption.OpenIfExists);
                await Task.Run(async () =>
                {
                    var json =
                        JsonConvert.SerializeObject(new Tuple<DateTime, AnimeDetailsData>(DateTime.UtcNow, data));
                    var file =
                        await
                            folder.CreateFileAsync($"{data.Source}_{id}.json",
                                CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, json);
                });
            }
            catch (Exception)
            {
                //probably failed to create folder #windowsmagic
            }
        }

        public static async Task<AnimeDetailsData> RetrieveAnimeGeneralDetailsData(int id, DataSource source,
            bool anime = true)
        {
            try
            {
                var folder =
                    await
                        ApplicationData.Current.LocalFolder.CreateFolderAsync(anime ? "AnimeDetails" : "MangaDetails",
                            CreationCollisionOption.OpenIfExists);
                var file = await folder.GetFileAsync($"{source}_{id}.json");
                var data = await FileIO.ReadTextAsync(file);
                var tuple =
                    JsonConvert.DeserializeObject<Tuple<DateTime, AnimeDetailsData>>(data);
                return CheckForOldDataDetails(tuple.Item1) ? tuple.Item2 : null;
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }

        private static bool CheckForOldDataDetails(DateTime date, int days = 7)
        {
            var diff = DateTime.Now.ToUniversalTime().Subtract(date);
            if (diff.TotalDays >= days)
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
                var file =
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
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("seasonal_urls.json");
                var data = await FileIO.ReadTextAsync(file);
                SeasonalUrls = JsonConvert.DeserializeObject<Dictionary<string, string>>(data) ??
                               new Dictionary<string, string>();
            }
            catch (Exception)
            {
                SeasonalUrls = new Dictionary<string, string>();
            }
        }

        #endregion

        #region Reviews

        public static async void SaveAnimeReviews(int id, List<AnimeReviewData> data, bool anime)
        {
            try
            {
                var folder =
                    await
                        ApplicationData.Current.LocalFolder.CreateFolderAsync(anime ? "AnimeDetails" : "MangaDetails",
                            CreationCollisionOption.OpenIfExists);
                await Task.Run(async () =>
                {
                    var json =
                        JsonConvert.SerializeObject(new Tuple<DateTime, List<AnimeReviewData>>(DateTime.UtcNow, data));
                    var file =
                        await
                            folder.CreateFileAsync($"reviews_{id}.json",
                                CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, json);
                });
            }
            catch (Exception)
            {
                //magic
            }
        }

        public static async Task<List<AnimeReviewData>> RetrieveReviewsData(int animeId, bool anime)
        {
            try
            {
                var folder =
                    await
                        ApplicationData.Current.LocalFolder.CreateFolderAsync(anime ? "AnimeDetails" : "MangaDetails",
                            CreationCollisionOption.OpenIfExists);
                var file = await folder.GetFileAsync($"reviews_{animeId}.json");
                var data = await FileIO.ReadTextAsync(file);
                var tuple =
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

        public static async void SaveDirectRecommendationsData(int id, List<DirectRecommendationData> data,
            bool anime)
        {
            try
            {
                var folder =
                    await
                        ApplicationData.Current.LocalFolder.CreateFolderAsync(anime ? "AnimeDetails" : "MangaDetails",
                            CreationCollisionOption.OpenIfExists);
                await Task.Run(async () =>
                {
                    var json =
                        JsonConvert.SerializeObject(new Tuple<DateTime, List<DirectRecommendationData>>(
                            DateTime.UtcNow, data));
                    var file =
                        await
                            folder.CreateFileAsync($"direct_recommendations_{id}.json",
                                CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, json);
                });
            }
            catch (Exception)
            {
                //magic
            }
        }

        public static async Task<List<DirectRecommendationData>> RetrieveDirectRecommendationData(int id,
            bool anime)
        {
            try
            {
                var folder =
                    await
                        ApplicationData.Current.LocalFolder.CreateFolderAsync(anime ? "AnimeDetails" : "MangaDetails",
                            CreationCollisionOption.OpenIfExists);
                var file = await folder.GetFileAsync($"direct_recommendations_{id}.json");
                var data = await FileIO.ReadTextAsync(file);
                var tuple =
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

        #region RelatedAnime

        public static async void SaveRelatedAnimeData(int id, List<RelatedAnimeData> data, bool anime)
        {
            try
            {
                var folder =
                    await
                        ApplicationData.Current.LocalFolder.CreateFolderAsync(anime ? "AnimeDetails" : "MangaDetails",
                            CreationCollisionOption.OpenIfExists);
                await Task.Run(async () =>
                {
                    var json =
                        JsonConvert.SerializeObject(new Tuple<DateTime, List<RelatedAnimeData>>(DateTime.UtcNow, data));
                    var file =
                        await
                            folder.CreateFileAsync($"related_anime_{id}.json",
                                CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, json);
                });
            }
            catch (Exception)
            {
                //magic
            }
        }

        public static async Task<List<RelatedAnimeData>> RetrieveRelatedAnimeData(int animeId, bool anime)
        {
            try
            {
                var folder =
                    await
                        ApplicationData.Current.LocalFolder.CreateFolderAsync(anime ? "AnimeDetails" : "MangaDetails",
                            CreationCollisionOption.OpenIfExists);
                var file = await folder.GetFileAsync($"related_anime_{animeId}.json");
                var data = await FileIO.ReadTextAsync(file);
                var tuple =
                    JsonConvert.DeserializeObject<Tuple<DateTime, List<RelatedAnimeData>>>(data);
                return CheckForOldDataDetails(tuple.Item1) ? tuple.Item2 : null;
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }

        #endregion

        #region AnimeSerachResults

        public static async void SaveAnimeSearchResultsData(string id, AnimeGeneralDetailsData data, bool anime)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var folder =
                        await
                            ApplicationData.Current.LocalFolder.CreateFolderAsync(
                                anime ? "AnimeDetails" : "MangaDetails",
                                CreationCollisionOption.OpenIfExists);
                    var json =
                        JsonConvert.SerializeObject(new Tuple<DateTime, AnimeGeneralDetailsData>(DateTime.UtcNow, data));
                    var file =
                        await
                            folder.CreateFileAsync($"mal_details_{id}.json",
                                CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, json);
                });
            }
            catch (Exception)
            {
                //magic
            }
        }

        public static async Task<AnimeGeneralDetailsData> RetrieveAnimeSearchResultsData(string animeId, bool anime)
        {
            try
            {
                var folder =
                    await
                        ApplicationData.Current.LocalFolder.CreateFolderAsync(anime ? "AnimeDetails" : "MangaDetails",
                            CreationCollisionOption.OpenIfExists);
                var file = await folder.GetFileAsync($"mal_details_{animeId}.json");
                var data = await FileIO.ReadTextAsync(file);
                var tuple =
                    JsonConvert.DeserializeObject<Tuple<DateTime, AnimeGeneralDetailsData>>(data);
                return CheckForOldDataDetails(tuple.Item1, 1) ? tuple.Item2 : null;
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }

        #endregion

        #region TopAnime

        public static async void SaveTopAnimeData(List<TopAnimeData> data, bool anime)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var json =
                        JsonConvert.SerializeObject(new Tuple<DateTime, List<TopAnimeData>>(DateTime.UtcNow, data));
                    var file =
                        await
                            ApplicationData.Current.LocalFolder.CreateFileAsync(
                                $"top_{(anime ? "anime" : "manga")}.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, json);
                });
            }
            catch (Exception)
            {
                //magic
            }
        }

        public static async Task<List<TopAnimeData>> RetrieveTopAnimeData(bool anime)
        {
            try
            {
                var file =
                    await ApplicationData.Current.LocalFolder.GetFileAsync($"top_{(anime ? "anime" : "manga")}.json");
                var data = await FileIO.ReadTextAsync(file);
                var tuple =
                    JsonConvert.DeserializeObject<Tuple<DateTime, List<TopAnimeData>>>(data);
                return CheckForOldDataDetails(tuple.Item1) ? tuple.Item2 : null;
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }

        #endregion

        #region MalToHum
        public static async Task SaveHumMalIdDictionary()
        {
            try
            {
                var json = JsonConvert.SerializeObject(AnimeDetailsHummingbirdQuery.MalToHumId);
                var file =
                    await
                        ApplicationData.Current.LocalFolder.CreateFileAsync("mal_to_hum.json",
                            CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public static async void RetrieveHumMalIdDictionary()
        {
            var result = new Dictionary<int, int>();
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("mal_to_hum.json");
                var data = await FileIO.ReadTextAsync(file);
                result = JsonConvert.DeserializeObject<Dictionary<int, int>>(data) ??
                         new Dictionary<int, int>();
            }
            catch (Exception)
            {
                result = new Dictionary<int, int>();
            }
            AnimeDetailsHummingbirdQuery.MalToHumId = result;
        }

        #endregion


    }
}