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
        private readonly string _source;

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

        public async Task<List<ILibraryData>> GetLibrary(bool force = false)
        {
            var output = force
                ? new List<ILibraryData>()
                : await DataCache.RetrieveDataForUser(_source, _mode) ?? new List<ILibraryData>();
            if (output.Count > 0)
                return output;
            var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();

            try
            {
                var offset = 0;
                var i = 0;
                var loop = true; //loop_noop
                var failedOnce = false;
                while (loop)
                {
                    Debug.WriteLine($"Loading with offset {offset}");

                    //if (offset == 0)
                    //    Debugger.Break();
                    try
                    {
                        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

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
                                            $"list_updated_at}}&limit=1000&offset={offset}", cts.Token);

                                        var rawAnime = await animeResponse.Content.ReadAsStringAsync();

                                        if (string.IsNullOrEmpty(rawAnime))
                                            return await DataCache.RetrieveDataForUser(_source, _mode) ?? output;

                                        var anime = JsonConvert.DeserializeObject<Root>(rawAnime);
                                        offset += anime.data.Count;

                                        if (anime.paging?.next == null)
                                        {
                                            loop = false;
                                        }
  

                                        if(offset == 0 || anime.data.Count == 0)
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

                                            output.Add(new AnimeLibraryItemData
                                            {
                                                Title = title,
                                                ImgUrl =  item.main_picture.medium ?? item.main_picture.large,
                                                Type = (int) GetMediaType(),
                                                MalId = item.id,
                                                MyStatus = ParseAnimeStatus(item.my_list_status.status),
                                                MyEpisodes = item.my_list_status.num_episodes_watched,
                                                AllEpisodes = item.num_episodes,
                                                MyStartDate = item.my_list_status.start_date,
                                                MyEndDate = item.my_list_status.finish_date,
                                                MyScore = item.my_list_status.score,
                                                Notes = string.Join(",",item.my_list_status.tags),
                                                IsRewatching = item.my_list_status.is_rewatching,
                                                LastWatched = item.my_list_status.updated_at,
                                                AlternateTitle = alternateTitle,
                                                Priority = (AnimePriority)item.my_list_status.priority,
                                            });
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
                                            $"list_updated_at}}&limit=1000&offset={offset}", cts.Token);

                                        var rawManga = await mangaResponse.Content.ReadAsStringAsync();

                                        if (string.IsNullOrEmpty(rawManga))
                                            return await DataCache.RetrieveDataForUser(_source, _mode) ?? output;

                                        var manga = JsonConvert.DeserializeObject<Root>(rawManga);
                                        offset += manga.data.Count;

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

                                            output.Add(new MangaLibraryItemData
                                            {
                                                Title = title,
                                                ImgUrl = item.main_picture.medium ?? item.main_picture.large,
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
                                                Priority = (AnimePriority)item.my_list_status.priority
                                            });
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
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to read anime list, {e}");
                        if (failedOnce)
                        {
                            loop = false;
                        }
                        else
                        {
                            failedOnce = true;
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    }
                }
            }
            catch (Exception e)
            {

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