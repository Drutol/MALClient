using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MALClient.Adapters;
using MALClient.Models.Models;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.Models.Models.Library;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

//Okay it's big copy paste... feel free to laugh

namespace MALClient.XShared.Utils
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
        private static readonly IDataCache DataCacheService;

        static DataCache()
        {
            DataCacheService = ResourceLocator.DataCacheService;
            LoadVolatileData();
            LoadSeasonalurls();
            RetrieveHumMalIdDictionary();      
        }

        public static async Task ClearApiRelatedCache()
        {
            await DataCacheService.ClearApiRelatedCache();
            _volatileDataCache.Clear();
            AnimeDetailsHummingbirdQuery.MalToHumId?.Clear();
        }

        public static async Task ClearAnimeListData()
        {
            await DataCacheService.ClearAnimeListData();
        }

        public static async Task SaveDataRoaming<T>(T data, string filename)
        {
            try
            {
                await DataCacheService.SaveDataRoaming(data, filename);
            }
            catch (Exception e)
            {
                //magic
            }
        }

        public static async Task SaveData<T>(T data, string filename, string targetFolder)
        {
            await DataCacheService.SaveData(data, filename,targetFolder);
        }

        public static async Task<T> RetrieveData<T>(string filename, string originFolder, int expiration)
        {
            return await DataCacheService.RetrieveData<T>(filename, originFolder, expiration);
        }

        public static async Task<T> RetrieveDataRoaming<T>(string filename,int expiration)
        {
            try
            {
                return await DataCacheService.RetrieveDataRoaming<T>(filename, expiration);
            }
            catch (Exception)
            {
                //No file
            }
            return default(T);
        }

        #region UserData

        public static async Task SaveDataForUser(string user, IEnumerable<ILibraryData> data, AnimeListWorkModes mode)
        {
            if (!Settings.IsCachingEnabled)
                return;
            try
            {
                if (mode == AnimeListWorkModes.Anime)
                {
                    await DataCacheService.SaveData(
                        new Tuple<DateTime, IEnumerable<AnimeLibraryItemData>>(DateTime.Now,
                            data.Select(item => item as AnimeLibraryItemData)),
                        $"{(mode == AnimeListWorkModes.Anime ? "anime" : "manga")}_data_{user.ToLower()}.json", "");
                }
                else
                {
                    await DataCacheService.SaveData(
                        new Tuple<DateTime, IEnumerable<MangaLibraryItemData>>(DateTime.Now,
                            data.Select(item => item as MangaLibraryItemData)),
                        $"{(mode == AnimeListWorkModes.Anime ? "anime" : "manga")}_data_{user.ToLower()}.json", "");
                }
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
                var decoded = new List<ILibraryData>();
                if (mode == AnimeListWorkModes.Anime)
                {
                    var jsonObj =
                        await
                            DataCacheService.RetrieveData<Tuple<DateTime, List<AnimeLibraryItemData>>>(
                                $"{(mode == AnimeListWorkModes.Anime ? "anime" : "manga")}_data_{user.ToLower()}.json",
                                "", 0);
                    if (!CheckForOldData(jsonObj.Item1))
                    {
                        return null;
                    }
                    decoded.AddRange(jsonObj.Item2.Select(item => item as ILibraryData));
                }
                else
                {
                    var jsonObj =
                        await
                            DataCacheService.RetrieveData<Tuple<DateTime, List<MangaLibraryItemData>>>(
                                $"{(mode == AnimeListWorkModes.Anime ? "anime" : "manga")}_data_{user.ToLower()}.json",
                                "", 0);
                    if (!CheckForOldData(jsonObj.Item1))
                    {
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
                    await
                        DataCacheService.SaveData(data,
                            $"seasonal_data{tag}.json", "");
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
                return
                    await
                        DataCacheService.RetrieveData<List<SeasonalAnimeData>>(
                            $"seasonal_data{tag}.json", "", 7);
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
                _volatileDataCache = (await DataCacheService.RetrieveData<Dictionary<int, VolatileDataCache>>("volatile_data.json","",0)) ??
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
                await DataCacheService.SaveData(_volatileDataCache, "volatile_data.json", "");
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

                await Task.Run(async () =>
                {
                    await
                        DataCacheService.SaveData(data, $"{data.Source}_{id}.json",
                            anime ? "AnimeDetails" : "MangaDetails");
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
                return await DataCacheService.RetrieveData<AnimeDetailsData>($"{source}_{id}.json",
                    anime ? "AnimeDetails" : "MangaDetails", 1);
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
                await DataCacheService.SaveData(seasonData, "seasonal_urls.json", "");
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
                SeasonalUrls = await RetrieveData<Dictionary<string, string>>("seasonal_urls.json", "", 0);
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
                await Task.Run(async () =>
                {
                    await DataCacheService.SaveData(data, $"reviews_{id}.json", anime ? "AnimeDetails" : "MangaDetails");
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
                return
                    await
                        DataCacheService.RetrieveData<List<AnimeReviewData>>($"reviews_{animeId}.json",
                            anime ? "AnimeDetails" : "MangaDetails", 14);
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
                await Task.Run(async () =>
                {
                        await DataCacheService.SaveData(data, $"direct_recommendations_{id}.json",
                            anime ? "AnimeDetails" : "MangaDetails");
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
                return await DataCacheService.RetrieveData<List<DirectRecommendationData>>(
                    $"direct_recommendations_{id}.json", anime ? "AnimeDetails" : "MangaDetails", 14);
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
                await Task.Run(async () =>
                {

                    await
                        DataCacheService.SaveData(data, $"related_anime_{id}.json",
                            anime ? "AnimeDetails" : "MangaDetails");
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
                return
                    await
                        DataCacheService.RetrieveData<List<RelatedAnimeData>>($"related_anime_{animeId}.json",
                            anime ? "AnimeDetails" : "MangaDetails", 14);
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
                    await
                        DataCacheService.SaveData(data, $"mal_details_{id}.json",
                            anime ? "AnimeDetails" : "MangaDetails");
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
                return
                    await
                        DataCacheService.RetrieveData<AnimeGeneralDetailsData>($"mal_details_{animeId}.json",
                            anime ? "AnimeDetails" : "MangaDetails", 14);
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }

        #endregion

        #region TopAnime

        public static async void SaveTopAnimeData(List<TopAnimeData> data, TopAnimeType type)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await DataCacheService.SaveData(data, $"top_{type}_data.json", "");
                });
            }
            catch (Exception)
            {
                //magic
            }
        }

        public static async Task<List<TopAnimeData>> RetrieveTopAnimeData(TopAnimeType type)
        {
            try
            {
                return await DataCacheService.RetrieveData<List<TopAnimeData>>($"top_{type}_data.json", "", 14);
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
                await DataCacheService.SaveData(AnimeDetailsHummingbirdQuery.MalToHumId, "mal_to_hum.json", "");
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
                result = await DataCacheService.RetrieveData<Dictionary<int, int>>("mal_to_hum.json", "", 0);
            }
            catch (Exception)
            {
                result = new Dictionary<int, int>();
            }
            AnimeDetailsHummingbirdQuery.MalToHumId = result;
        }

        #endregion

        #region ProfileData

        public static async void SaveProfileData(string user, ProfileData data)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await DataCacheService.SaveData(data, $"mal_profile_details_{user}.json", "ProfileData");
                });
            }
            catch (Exception)
            {
                //magic
            }
        }

        public static async Task<ProfileData> RetrieveProfileData(string user)
        {
            try
            {
                return
                    await
                        DataCacheService.RetrieveData<ProfileData>($"mal_profile_details_{user}.json", "ProfileData", 7);
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }

        #endregion

        #region ArticlesIndex

        public static async void SaveArticleIndexData(ArticlePageWorkMode mode, List<MalNewsUnitModel> data)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await
                        DataCacheService.SaveData(data,
                            mode == ArticlePageWorkMode.Articles ? "mal_article_index.json" : "mal_news_index.json",
                            "Articles");
                });
            }
            catch (Exception)
            {
                //magic
            }
        }

        public static async Task<List<MalNewsUnitModel>> RetrieveArticleIndexData(ArticlePageWorkMode mode)
        {
            try
            {
                return await DataCacheService.RetrieveData<List<MalNewsUnitModel>>(mode == ArticlePageWorkMode.Articles
                    ? "mal_article_index.json"
                    : "mal_news_index.json", "Articles", 4);
            }
            catch (Exception)
            {
                //No file
            }
            return null;
        }

        #endregion

        #region ArticlesContent

        public static async void SaveArticleContentData(string title, string htmlData, MalNewsType type)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await
                        DataCacheService.SaveData(htmlData,
                            $"mal_{(type == MalNewsType.Article ? "article" : "news")}_html_{title}.json", "Articles");
                });
            }
            catch (Exception e)
            {
                //magic
            }
        }

        public static async Task<string> RetrieveArticleContentData(string title, MalNewsType type)
        {
            try
            {
                return
                    await
                        DataCacheService.RetrieveData<string>(
                            $"mal_{(type == MalNewsType.Article ? "article" : "news")}_html_{title}.json", "Articles", 0);
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