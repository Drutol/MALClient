using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Android.Runtime;
using MALClient.Models.Enums;
using MALClient.Models.Models.ApiResponses;
using MALClient.Models.Models.Library;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using ModernHttpClient;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm
{
    public class LibraryListQuery : Query
    {
        private readonly AnimeListWorkModes _mode;
        private readonly string _source;

        public LibraryListQuery(string source, AnimeListWorkModes mode)
        {
            _mode = mode;
            _source = source;
            var type = _mode == AnimeListWorkModes.Anime ? "anime" : "manga";
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request =
                        WebRequest.Create(
                            Uri.EscapeUriString(
                                $"https://myanimelist.net/malappinfo.php?u={source}&status=all&type={type}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:
                    switch (mode)
                    {
                        case AnimeListWorkModes.Anime:
                            Request =
                                WebRequest.Create(
                                    Uri.EscapeUriString($"https://hummingbird.me/api/v1/users/{source}/library"));
                            Request.ContentType = "application/x-www-form-urlencoded";
                            Request.Method = "GET";
                            break;
                        case AnimeListWorkModes.Manga:
                            Request =
                                WebRequest.Create(
                                    Uri.EscapeUriString($"https://hummingbird.me/manga_library_entries?user_id={source}"));
                            Request.ContentType = "application/x-www-form-urlencoded";
                            Request.Method = "GET";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<List<ILibraryData>> GetLibrary(bool force = false)
        {
            var output = force
                ? new List<ILibraryData>()
                : await DataCache.RetrieveDataForUser(_source, _mode) ?? new List<ILibraryData>();
            if (output.Count > 0)
                return output;
            var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();
            int offset = 0;
            bool loop = true; //loop_noop
            while (loop)
            {
                Debug.WriteLine($"Loading with offset {offset}");
                try
                {
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));


                    switch (CurrentApiType)
                    {
                        case ApiType.Mal:
                            //var parsedData = XDocument.Parse(raw);

                            switch (_mode)
                            {
                                case AnimeListWorkModes.Anime:
                                    var animeReponse = await client.GetAsync(
                                        $"https://myanimelist.net/animelist/{_source}/load.json?offset={offset}&status=7", cts.Token);

                                    var rawAnime = await animeReponse.Content.ReadAsStringAsync();

                                    if (string.IsNullOrEmpty(rawAnime))
                                        return await DataCache.RetrieveDataForUser(_source, _mode) ?? output;

                                    var anime = JsonConvert.DeserializeObject<List<RootObject>>(rawAnime);
                                    offset += anime.Count;

                                    if (anime.Count < 300)
                                        loop = false;

                                    foreach (var item in anime)
                                    {
                                        string title = "";
                                        string alternateTitle = null;
                                        title = item.anime_title;

                                        item.anime_image_path =
                                            Regex.Replace(item.anime_image_path, @"\/r\/\d+x\d+", "");
                                        item.anime_image_path =
                                            item.anime_image_path.Substring(0, item.anime_image_path.IndexOf('?'));

                                        output.Add(new AnimeLibraryItemData
                                        {
                                            Title = title,
                                            ImgUrl = item.anime_image_path,
                                            Type = (int) GetMediaType(),
                                            MalId = item.anime_id,
                                            MyStatus = (AnimeStatus) item.status,
                                            MyEpisodes = item.num_watched_episodes,
                                            AllEpisodes = item.anime_num_episodes,
                                            MyStartDate = item.start_date_string,
                                            MyEndDate = item.finish_date_string,
                                            MyScore = item.score,
                                            Notes = item.tags,
                                            IsRewatching = item.is_rewatching > 0,
                                            LastWatched = DateTime.Now,
                                            AlternateTitle = alternateTitle,
                                        });

                                        AnimeType GetMediaType()
                                        {
                                            switch (item.anime_media_type_string)
                                            {
                                                case "TV":
                                                    return AnimeType.TV;
                                                case "Movie":
                                                    return AnimeType.Movie;
                                                case "Speical":
                                                    return AnimeType.Special;
                                                case "OVA":
                                                    return AnimeType.OVA;
                                                case "ONA":
                                                    return AnimeType.ONA;
                                                case "Music":
                                                    return AnimeType.Music;
                                            }

                                            return AnimeType.TV;
                                        }
                                    }

                                    break;
                                case AnimeListWorkModes.Manga:
                                    var reponseManga = await client.GetAsync(
                                        $"https://myanimelist.net/mangalist/{_source}/load.json?offset={offset}&status=7", cts.Token);

                                    var rawManga = await reponseManga.Content.ReadAsStringAsync();

                                    if (string.IsNullOrEmpty(rawManga))
                                        return await DataCache.RetrieveDataForUser(_source, _mode) ?? output;


                                    var manga = JsonConvert.DeserializeObject<List<MangaRootObject>>(rawManga);

                                    offset += manga.Count;

                                    if (manga.Count < 300)
                                        loop = false;

                                    foreach (var item in manga)
                                    {
                                        item.manga_image_path =
                                            Regex.Replace(item.manga_image_path, @"\/r\/\d+x\d+", "");
                                        item.manga_image_path =
                                            item.manga_image_path.Substring(0, item.manga_image_path.IndexOf('?'));

                                        output.Add(new MangaLibraryItemData
                                        {
                                            Title = item.manga_title,
                                            ImgUrl = item.manga_image_path,
                                            Id = item.manga_id,
                                            AllEpisodes = item.manga_num_chapters,
                                            MyEpisodes = item.num_read_chapters,
                                            AllVolumes = item.manga_num_volumes,
                                            MyVolumes = item.num_read_volumes,
                                            MalId = item.manga_id,
                                            Type = (int)GetMangaMediaType(),
                                            MyScore = item.score,
                                            Notes = item.tags,
                                            MyStatus = (AnimeStatus)item.status,
                                            MyStartDate = item.start_date_string,
                                            MyEndDate = item.finish_date_string,
                                            AlternateTitle = item.manga_title
                                        });

                                        MangaType GetMangaMediaType()
                                        {
                                            switch (item.manga_media_type_string)
                                            {
                                                case "Novel":
                                                    return MangaType.Novel;
                                                case "Manga":
                                                    return MangaType.Manga;
                                                case "Doujinshi":
                                                    return MangaType.Doujinshi;
                                                case "OneShot":
                                                    return MangaType.OneShot;
                                                case "Manhwa":
                                                    return MangaType.Manhwa;
                                                case "Manhua":
                                                    return MangaType.Manhua;
                                                default:
                                                    return MangaType.Manga;
                                            }
                                        }
                                    }


                                    //var manga = parsedData.Root.Elements("manga").ToList();
                                    //foreach (var item in manga)
                                    //{
                                    //    string title = "";
                                    //    string alternateTitle = null;
                                    //    if (Settings.PreferEnglishTitles)
                                    //    {
                                    //        var elem = item.Element("series_synonyms");
                                    //        if (!string.IsNullOrWhiteSpace(elem?.Value))
                                    //        {
                                    //            title = elem.Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Trim();
                                    //            alternateTitle = item.Element("series_title").Value;
                                    //        }

                                    //        if (string.IsNullOrEmpty(title))
                                    //            title = item.Element("series_title").Value;
                                    //    }
                                    //    else
                                    //    {
                                    //        title = item.Element("series_title").Value;
                                    //        var elem = item.Element("series_synonyms");
                                    //        if (!string.IsNullOrWhiteSpace(elem?.Value))
                                    //        {
                                    //            alternateTitle = elem.Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Trim();
                                    //        }
                                    //    }
                                    //    output.Add(new MangaLibraryItemData
                                    //    {
                                    //        Title = title,
                                    //        ImgUrl = item.Element("series_image").Value,
                                    //        Type = Convert.ToInt32(item.Element("series_type").Value),
                                    //        MalId = Convert.ToInt32(item.Element("series_mangadb_id").Value),
                                    //        MyStatus = (AnimeStatus) Convert.ToInt32(item.Element("my_status").Value),
                                    //        MyEpisodes = Convert.ToInt32(item.Element("my_read_chapters").Value),
                                    //        AllEpisodes = Convert.ToInt32(item.Element("series_chapters").Value),
                                    //        MyStartDate = item.Element("my_start_date").Value,
                                    //        MyEndDate = item.Element("my_finish_date").Value,
                                    //        MyScore = Convert.ToInt32(item.Element("my_score").Value),
                                    //        MyVolumes = Convert.ToInt32(item.Element("my_read_volumes").Value),
                                    //        AllVolumes = Convert.ToInt32(item.Element("series_volumes").Value),
                                    //        IsRewatching = item.Element("my_rereadingg").Value == "1",
                                    //        Notes = item.Element("my_tags").Value,
                                    //        AlternateTitle = alternateTitle,
                                    //        LastWatched = Utils.Utilities.ConvertFromUnixTimestamp(int.Parse(item.Element("my_last_updated").Value))

                                    //    });
                                    //}
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(_mode),
                                        "You gave me something different than anime/manga... b..b-baka (GetLibrary)");
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception e)
                {
#if ANDROID
                    ResourceLocator.SnackbarProvider.ShowText("MAL refused access to animelist for some reason... I'm still trying to figure this one.");
#endif
                    loop = false;
                }
            }


            DataCache.SaveDataForUser(_source, output, _mode);
            return output;
        }

        [Preserve(AllMembers = true)]
        public class RootObject
        {
            public int status { get; set; }
            public int score { get; set; }
            public string tags { get; set; }
            public int? is_rewatching { get; set; }
            public int num_watched_episodes { get; set; }
            public string anime_title { get; set; }
            public int anime_num_episodes { get; set; }
            public int anime_airing_status { get; set; }
            public int anime_id { get; set; }
            public object anime_studios { get; set; }
            public object anime_licensors { get; set; }
            public object anime_season { get; set; }
            public bool has_episode_video { get; set; }
            public bool has_promotion_video { get; set; }
            public bool has_video { get; set; }
            public string video_url { get; set; }
            public string anime_url { get; set; }
            public string anime_image_path { get; set; }
            public bool is_added_to_list { get; set; }
            public string anime_media_type_string { get; set; }
            public string anime_mpaa_rating_string { get; set; }
            public string start_date_string { get; set; }
            public string finish_date_string { get; set; }
            public string anime_start_date_string { get; set; }
            public string anime_end_date_string { get; set; }
            public int? days_string { get; set; }
            public string storage_string { get; set; }
            public string priority_string { get; set; }
        }

        [Preserve(AllMembers = true)]
        public class MangaRootObject
        {
            public int id { get; set; }
            public int status { get; set; }
            public int score { get; set; }
            public string tags { get; set; }
            public int is_rereading { get; set; }
            public int num_read_chapters { get; set; }
            public int num_read_volumes { get; set; }
            public string manga_title { get; set; }
            public int manga_num_chapters { get; set; }
            public int manga_num_volumes { get; set; }
            public int manga_publishing_status { get; set; }
            public int manga_id { get; set; }
            public object manga_magazines { get; set; }
            public string manga_url { get; set; }
            public string manga_image_path { get; set; }
            public bool is_added_to_list { get; set; }
            public string manga_media_type_string { get; set; }
            public string start_date_string { get; set; }
            public string finish_date_string { get; set; }
            public string manga_start_date_string { get; set; }
            public string manga_end_date_string { get; set; }
            public int? days_string { get; set; }
            public object retail_string { get; set; }
            public string priority_string { get; set; }
        }

        public static AnimeStatus HummingbirdStatusToMal(string humStatus)
        {
            switch (humStatus)
            {
                case "currently-watching":
                    return AnimeStatus.Watching;
                case "plan-to-watch":
                    return AnimeStatus.PlanToWatch;
                case "completed":
                    return AnimeStatus.Completed;
                case "on-hold":
                    return AnimeStatus.OnHold;
                case "dropped":
                    return AnimeStatus.Dropped;
                default:
                    throw new ArgumentOutOfRangeException(nameof(humStatus), "Hummingbird has gone crazy");
            }
        }

        private static AnimeStatus HummingbirdMangaStatusToMal(string humStatus)
        {
            switch (humStatus)
            {
                case "Currently Reading":
                    return AnimeStatus.Watching;
                case "Plan to Read":
                    return AnimeStatus.PlanToWatch;
                case "Completed":
                    return AnimeStatus.Completed;
                case "On Hold":
                    return AnimeStatus.OnHold;
                case "Dropped":
                    return AnimeStatus.Dropped;
                default:
                    throw new ArgumentOutOfRangeException(nameof(humStatus), "Hummingbird has gone crazy");
            }
        }
    }
}