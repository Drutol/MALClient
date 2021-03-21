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
using Newtonsoft.Json;

namespace MALClient.XShared.Comm
{
    public class LibraryListQuery : Query
    {
        private readonly AnimeListWorkModes _mode;
        private string _source;

        public LibraryListQuery(string source, AnimeListWorkModes mode)
        {
            Debug.WriteLine($"Getting library for {mode}@{source}.");
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

        public async Task<List<ILibraryData>> GetLibrary(bool force = false, bool forceOtherUser = false)
        {
            var output = force
                ? new List<ILibraryData>()
                : await DataCache.RetrieveDataForUser(_source, _mode) ?? new List<ILibraryData>();
            if (output.Count > 0)
                return output;


            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();

                if (forceOtherUser)
                    ResourceLocator.TelemetryProvider.LogEvent("Falling back to forceOtherUser");

                var offset = 0;
                var i = 0;
                var loop = true; //loop_noop
                var failedOnce = false;

                string rawAnime = null;

                Debug.WriteLine($"Loading with offset {offset}");
                if (!forceOtherUser && _source.ToLower() == Credentials.UserName.ToLower())
                {
                    try
                    {
                        while (loop)
                        {
                            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
                            switch (CurrentApiType)
                            {
                                case ApiType.Mal:
                                    switch (_mode)
                                    {

                                        case AnimeListWorkModes.Anime:
                                            var animeResponse = await client.GetAsync(
                                                $"https://api.myanimelist.net/v2/users/{_source}/animelist?fields=" +
                                                $"title," +
                                                $"status," +
                                                $"media_type," +
                                                $"num_episodes," +
                                                $"my_list_status{{start_date," +
                                                $"tags," +
                                                $"finish_date," +
                                                $"comments," +
                                                $"priority," +
                                                $"num_episodes_rewatched," +
                                                $"num_watched_times," +
                                                $"list_updated_at}}&limit=1000&offset={offset}&nsfw=true", cts.Token);

                                            rawAnime = await animeResponse.Content.ReadAsStringAsync();

                                            if (string.IsNullOrEmpty(rawAnime))
                                                return await DataCache.RetrieveDataForUser(_source, _mode) ?? output;

                                            Root anime;
                                            try
                                            {
                                                anime = JsonConvert.DeserializeObject<Root>(rawAnime);
                                            }
                                            catch (Exception e)
                                            {
                                                ResourceLocator.TelemetryProvider.LogEvent(rawAnime);
                                                //ResourceLocator.SnackbarProvider.ShowText(
                                                //    $"Failed to get list from MAL, error: {rawAnime}");
                                                return await GetLibrary(force, true);
                                            }

                                            offset += anime.data?.Count ?? 0;

                                            if (anime.paging?.next == null)
                                            {
                                                loop = false;
                                            }


                                            if (offset == 0 || anime.data.Count == 0)
                                                Debugger.Break();

                                            foreach (var node in anime.data)
                                            {
                                                var item = node.node;
                                                var title = "";
                                                string alternateTitle = null;
                                                title = item.title;

                                                if (Settings.PreferEnglishTitles &&
                                                    ResourceLocator.EnglishTitlesProvider.TryGetEnglishTitleForSeries(
                                                        item.id, true, out var engTitle))
                                                {
                                                    alternateTitle = title;
                                                    title = engTitle;
                                                }

                                                try
                                                {
                                                    output.Add(new AnimeLibraryItemData
                                                    {
                                                        Title = title,
                                                        ImgUrl =
                                                            item?.main_picture?.medium ?? item?.main_picture?.large,
                                                        Type = (int) GetMediaType(),
                                                        MalId = item.id,
                                                        MyStatus = ParseAnimeStatus(item.my_list_status.status),
                                                        MyEpisodes = item.my_list_status.num_episodes_watched,
                                                        AllEpisodes = item.num_episodes,
                                                        MyStartDate = item.my_list_status.start_date,
                                                        MyEndDate = item.my_list_status.finish_date,
                                                        MyScore = item.my_list_status.score,
                                                        Notes = string.Join(",", item.my_list_status.tags),
                                                        IsRewatching = item.my_list_status.is_rewatching,
                                                        LastWatched = item.my_list_status.updated_at,
                                                        AlternateTitle = alternateTitle,
                                                        Priority = (AnimePriority) item.my_list_status.priority,
                                                    });
                                                }
                                                catch (Exception e)
                                                {
                                                    ResourceLocator.TelemetryProvider.TrackException(e,
                                                        "Load anime entry");
                                                }

                                                i++;

                                                AnimeType GetMediaType()
                                                {
                                                    switch (item.media_type)
                                                    {
                                                        case "tv":
                                                            return AnimeType.TV;
                                                        case "movie":
                                                            return AnimeType.Movie;
                                                        case "special":
                                                            return AnimeType.Special;
                                                        case "ova":
                                                            return AnimeType.OVA;
                                                        case "ona":
                                                            return AnimeType.ONA;
                                                        case "music":
                                                            return AnimeType.Music;
                                                    }

                                                    return AnimeType.TV;
                                                }
                                            }


                                            break;
                                        case AnimeListWorkModes.Manga:
                                            var mangaResponse = await client.GetAsync(
                                                $"https://api.myanimelist.net/v2/users/{_source}/mangalist?fields=" +
                                                $"title," +
                                                $"status," +
                                                $"media_type," +
                                                $"num_chapters," +
                                                $"num_volumes," +
                                                $"my_list_status{{start_date," +
                                                $"tags," +
                                                $"finish_date," +
                                                $"comments," +
                                                $"priority," +
                                                $"num_episodes_rewatched," +
                                                $"num_watched_times," +
                                                $"list_updated_at}}&limit=1000&offset={offset}&nsfw=true", cts.Token);

                                            var rawManga = await mangaResponse.Content.ReadAsStringAsync();

                                            if (string.IsNullOrEmpty(rawManga))
                                                return await DataCache.RetrieveDataForUser(_source, _mode) ?? output;

                                            var manga = JsonConvert.DeserializeObject<Root>(rawManga);
                                            offset += manga.data?.Count ?? 0;

                                            if (manga.paging?.next == null)
                                            {
                                                loop = false;
                                            }

                                            foreach (var node in manga.data)
                                            {
                                                var item = node.node;
                                                var title = "";
                                                string alternateTitle = null;
                                                title = item.title;

                                                if (Settings.PreferEnglishTitles &&
                                                    ResourceLocator.EnglishTitlesProvider.TryGetEnglishTitleForSeries(
                                                        item.id, false, out var engTitle))
                                                {
                                                    alternateTitle = title;
                                                    title = engTitle;
                                                }

                                                try
                                                {
                                                    output.Add(new MangaLibraryItemData
                                                    {
                                                        Title = title,
                                                        ImgUrl =
                                                            item?.main_picture?.medium ?? item?.main_picture?.large,
                                                        Id = item.id,
                                                        AllEpisodes = item.num_chapters,
                                                        MyEpisodes = item.my_list_status.num_chapters_read,
                                                        AllVolumes = item.num_volumes,
                                                        MyVolumes = item.my_list_status.num_volumes_read,
                                                        MalId = item.id,
                                                        Type = (int) GetMangaMediaType(),
                                                        MyScore = item.my_list_status.score,
                                                        Notes = string.Join(",", item.my_list_status.tags),
                                                        IsRewatching = item.my_list_status.is_rereading,
                                                        MyStatus = ParseMangaStatus(item.my_list_status.status),
                                                        MyStartDate = item.my_list_status.start_date,
                                                        MyEndDate = item.my_list_status.finish_date,
                                                        AlternateTitle = alternateTitle,
                                                        LastWatched = item.my_list_status.updated_at,
                                                        Priority = (AnimePriority) item.my_list_status.priority
                                                    });
                                                }
                                                catch (Exception e)
                                                {
                                                    ResourceLocator.TelemetryProvider.TrackException(e,
                                                        "Load manga entry");
                                                }

                                                i++;

                                                MangaType GetMangaMediaType()
                                                {
                                                    switch (item.media_type)
                                                    {
                                                        case "novel":
                                                            return MangaType.Novel;
                                                        case "manga":
                                                            return MangaType.Manga;
                                                        case "doujinshi":
                                                            return MangaType.Doujinshi;
                                                        case "oneshot":
                                                            return MangaType.OneShot;
                                                        case "manhwa":
                                                            return MangaType.Manhwa;
                                                        case "manhua":
                                                            return MangaType.Manhua;
                                                        default:
                                                            return MangaType.Manga;
                                                    }
                                                }
                                            }

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
                    }
                    catch (Exception e)
                    {
                        ResourceLocator.TelemetryProvider.TrackExceptionWithAttachment(e, "GetLibraryApi", rawAnime + $"\n{e.ToString()}");
                        return await GetLibrary(false, true);
                    }
                }
                else //other user
                {
                    while (loop)
                    {

                        try
                        {
                            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

                            switch (_mode)
                            {
                                case AnimeListWorkModes.Anime:
                                    var animeReponse = await client.GetAsync(
                                        $"https://myanimelist.net/animelist/{_source}/load.json?offset={offset}&status=7&order=5",
                                        cts.Token);

                                    rawAnime = await animeReponse.Content.ReadAsStringAsync();

                                    if (string.IsNullOrEmpty(rawAnime))
                                        return await DataCache.RetrieveDataForUser(_source, _mode) ?? output;

                                    var anime = JsonConvert.DeserializeObject<List<RootObject>>(rawAnime);
                                    offset += anime.Count;

                                    if (anime.Count < 300)
                                    {
                                        loop = false;
                                    }
                                    else
                                    {
                                        //MAL likes to throw "too many requests"
                                        await Task.Delay(200);
                                    }

                                    if (offset == 0 || anime.Count == 0)
                                        Debugger.Break();

                                    foreach (var item in anime)
                                    {
                                        var title = "";
                                        string alternateTitle = null;
                                        title = item.anime_title;

                                        if (Settings.PreferEnglishTitles &&
                                            ResourceLocator.EnglishTitlesProvider.TryGetEnglishTitleForSeries(
                                                item.anime_id, true, out var engTitle))
                                        {
                                            alternateTitle = title;
                                            title = engTitle;
                                        }

                                        item.anime_image_path =
                                            Regex.Replace(item.anime_image_path, @"\/r\/\d+x\d+", "");
                                        item.anime_image_path =
                                            item.anime_image_path.Substring(0,
                                                item.anime_image_path.IndexOf('?'));

                                        if (!string.IsNullOrEmpty(item.start_date_string))
                                        {
                                            var startDateTokens = item.start_date_string.Split('-');
                                            var yearToken = int.Parse(startDateTokens[2]);
                                            item.start_date_string =
                                                $"{(yearToken < 50 ? $"20{yearToken.ToString().PadLeft(2, '0')}" : $"19{yearToken.ToString().PadLeft(2, '0')}")}-{startDateTokens[true ? 0 : 1]}-{startDateTokens[true ? 1 : 0]}";
                                        }

                                        if (!string.IsNullOrEmpty(item.finish_date_string))
                                        {
                                            var endDateTokens = item.finish_date_string.Split('-');
                                            var yearToken = int.Parse(endDateTokens[2]);
                                            item.finish_date_string =
                                                $"{(yearToken < 50 ? $"20{yearToken.ToString().PadLeft(2, '0')}" : $"19{yearToken.ToString().PadLeft(2, '0')}")}-{endDateTokens[true ? 0 : 1]}-{endDateTokens[true ? 1 : 0]}";
                                        }


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
                                            LastWatched = DateTime.Today.Subtract(TimeSpan.FromMinutes(i)),
                                            AlternateTitle = alternateTitle,
                                            Priority = PriorityStringToPriority(item.priority_string),
                                        });
                                        i++;

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
                                        $"https://myanimelist.net/mangalist/{_source}/load.json?offset={offset}&status=7&order=5",
                                        cts.Token);

                                    var rawManga = await reponseManga.Content.ReadAsStringAsync();

                                    if (string.IsNullOrEmpty(rawManga))
                                        return await DataCache.RetrieveDataForUser(_source, _mode) ?? output;


                                    var manga = JsonConvert.DeserializeObject<List<MangaRootObject>>(rawManga);

                                    offset += manga.Count;

                                    if (manga.Count < 300)
                                        loop = false;

                                    foreach (var item in manga)
                                    {
                                        var title = "";
                                        string alternateTitle = null;
                                        title = item.manga_title;

                                        if (Settings.PreferEnglishTitles &&
                                            ResourceLocator.EnglishTitlesProvider.TryGetEnglishTitleForSeries(
                                                item.manga_id, false, out var engTitle))
                                        {
                                            alternateTitle = title;
                                            title = engTitle;
                                        }


                                        item.manga_image_path =
                                            Regex.Replace(item.manga_image_path, @"\/r\/\d+x\d+", "");
                                        item.manga_image_path =
                                            item.manga_image_path.Substring(0,
                                                item.manga_image_path.IndexOf('?'));

                                        if (!string.IsNullOrEmpty(item.start_date_string))
                                        {
                                            var startDateTokens = item.start_date_string.Split('-');
                                            var yearToken = int.Parse(startDateTokens[2]);
                                            item.start_date_string =
                                                $"{(yearToken < 50 ? $"20{yearToken.ToString().PadLeft(2, '0')}" : $"19{yearToken.ToString().PadLeft(2, '0')}")}-{startDateTokens[true ? 0 : 1]}-{startDateTokens[true ? 1 : 0]}";
                                        }

                                        if (!string.IsNullOrEmpty(item.finish_date_string))
                                        {
                                            var endDateTokens = item.finish_date_string.Split('-');
                                            var yearToken = int.Parse(endDateTokens[2]);
                                            item.finish_date_string =
                                                $"{(yearToken < 50 ? $"20{yearToken.ToString().PadLeft(2, '0')}" : $"19{yearToken.ToString().PadLeft(2, '0')}")}-{endDateTokens[true ? 0 : 1]}-{endDateTokens[true ? 1 : 0]}";
                                        }

                                        output.Add(new MangaLibraryItemData
                                        {
                                            Title = title,
                                            ImgUrl = item.manga_image_path,
                                            Id = item.manga_id,
                                            AllEpisodes = item.manga_num_chapters,
                                            MyEpisodes = item.num_read_chapters,
                                            AllVolumes = item.manga_num_volumes,
                                            MyVolumes = item.num_read_volumes,
                                            MalId = item.manga_id,
                                            Type = (int) GetMangaMediaType(),
                                            MyScore = item.score,
                                            Notes = item.tags,
                                            IsRewatching = item.is_rereading > 0,
                                            MyStatus = (AnimeStatus) item.status,
                                            MyStartDate = item.start_date_string,
                                            MyEndDate = item.finish_date_string,
                                            AlternateTitle = alternateTitle,
                                            LastWatched = DateTime.Today.Subtract(TimeSpan.FromMinutes(i)),
                                            Priority = PriorityStringToPriority(item.priority_string)
                                        });
                                        i++;

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

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(_mode),
                                        "You gave me something different than anime/manga... b..b-baka (GetLibrary)");
                            }
                        }
                        catch (Exception e)
                        {
                            ResourceLocator.TelemetryProvider.TrackException(e);
                            //Console.WriteLine($"Failed to read anime list, {e}");
                            if (failedOnce)
                            {
                                loop = false;

                                ResourceLocator.DispatcherAdapter.Run(() =>
                                    ResourceLocator.MessageDialogProvider.ShowMessageDialog(
                                        "Failed to authorize with MyAnimeList, please try signing in again.", "Error"));
                            }
                            else
                            {
                                failedOnce = true;
                                await Task.Delay(TimeSpan.FromSeconds(1));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ResourceLocator.TelemetryProvider.TrackException(e);
            }

            DataCache.SaveDataForUser(_source, output, _mode);
            return output;
        }

        private AnimeStatus ParseAnimeStatus(string status)
        {
            return status switch
            {
                "watching" => AnimeStatus.Watching,
                "completed" => AnimeStatus.Completed,
                "on_hold" => AnimeStatus.OnHold,
                "dropped" => AnimeStatus.Dropped,
                "plan_to_watch" => AnimeStatus.PlanToWatch,
                _ => AnimeStatus.PlanToWatch
            };
        }     
        
        private AnimeStatus ParseMangaStatus(string status)
        {
            return status switch
            {
                "reading" => AnimeStatus.Watching,
                "completed" => AnimeStatus.Completed,
                "on_hold" => AnimeStatus.OnHold,
                "dropped" => AnimeStatus.Dropped,
                "plan_to_read" => AnimeStatus.PlanToWatch,
                _ => AnimeStatus.PlanToWatch
            };
        }

        private AnimePriority PriorityStringToPriority(string itemPriorityString)
        {
            switch (itemPriorityString)
            {
                case "Low":
                    return AnimePriority.Low;
                case "Medium":
                    return AnimePriority.Medium;
                case "High":
                    return AnimePriority.High;
            }

            return AnimePriority.Low;
        }

        [Preserve(AllMembers = true)]
        public class MainPicture
        {
            public string medium { get; set; }
            public string large { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class MyListStatus
        {
            public string status { get; set; }
            public int score { get; set; }
            public int num_episodes_watched { get; set; }
            public bool is_rewatching { get; set; }
            public DateTime updated_at { get; set; }
            public string start_date { get; set; }
            public List<string> tags { get; set; }
            public string comments { get; set; }
            public int priority { get; set; }
            public string finish_date { get; set; }
            public bool is_rereading { get; set; }
            public int num_volumes_read { get; set; }
            public int num_chapters_read { get; set; }

        }
        [Preserve(AllMembers = true)]
        public class Node
        {
            public int id { get; set; }
            public string title { get; set; }
            public MainPicture main_picture { get; set; }
            public string status { get; set; }
            public string media_type { get; set; }
            public int num_episodes { get; set; }
            public int num_chapters { get; set; }
            public int num_volumes { get; set; }
            public MyListStatus my_list_status { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Datum
        {
            public Node node { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Paging
        {
            public string next { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Root
        {
            public List<Datum> data { get; set; }
            public Paging paging { get; set; }
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
            public int anime_id { get; set; }
            public string anime_image_path { get; set; }
            public string anime_media_type_string { get; set; }
            public string start_date_string { get; set; }
            public string finish_date_string { get; set; }
            public string priority_string { get; set; }
        }

        [Preserve(AllMembers = true)]
        public class MangaRootObject
        {
            public int status { get; set; }
            public int score { get; set; }
            public string tags { get; set; }
            public int? is_rereading { get; set; }
            public int num_read_chapters { get; set; }
            public int num_read_volumes { get; set; }
            public string manga_title { get; set; }
            public int manga_num_chapters { get; set; }
            public int manga_num_volumes { get; set; }
            public int manga_id { get; set; }
            public string manga_image_path { get; set; }
            public string manga_media_type_string { get; set; }
            public string start_date_string { get; set; }
            public string finish_date_string { get; set; }
            public string priority_string { get; set; }
        }
    }
}