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

            var isAmericanDateFormat = false;
            try
            {
                isAmericanDateFormat =
                    (await client.GetStringAsync("https://myanimelist.net/editprofile.php?go=listpreferences"))
                    .Contains("<option value=\"a\" selected>American (MM-DD-YY)");

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
                                        var animeReponse = await client.GetAsync(
                                            $"https://myanimelist.net/animelist/{_source}/load.json?offset={offset}&status=7&order=5",
                                            cts.Token);

                                        var rawAnime = await animeReponse.Content.ReadAsStringAsync();

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
                                            await Task.Delay(400);
                                        }

                                        if(offset == 0 || anime.Count == 0)
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
                                                item.anime_image_path.Substring(0, item.anime_image_path.IndexOf('?'));

                                            if (!string.IsNullOrEmpty(item.start_date_string))
                                            {
                                                var startDateTokens = item.start_date_string.Split('-');
                                                var yearToken = int.Parse(startDateTokens[2]);
                                                item.start_date_string =
                                                    $"{(yearToken < 50 ? $"20{yearToken.ToString().PadLeft(2,'0')}" : $"19{yearToken.ToString().PadLeft(2, '0')}")}-{startDateTokens[isAmericanDateFormat ? 0 : 1]}-{startDateTokens[isAmericanDateFormat ? 1 : 0]}";
                                            }

                                            if (!string.IsNullOrEmpty(item.finish_date_string))
                                            {
                                                var endDateTokens = item.finish_date_string.Split('-');
                                                var yearToken = int.Parse(endDateTokens[2]);
                                                item.finish_date_string =
                                                    $"{(yearToken < 50 ? $"20{yearToken.ToString().PadLeft(2, '0')}" : $"19{yearToken.ToString().PadLeft(2, '0')}")}-{endDateTokens[isAmericanDateFormat ? 0 : 1]}-{endDateTokens[isAmericanDateFormat ? 1 : 0]}";
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
                                                Priority = PriorityStringToPriority(item.priority_string)
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
                                                item.manga_image_path.Substring(0, item.manga_image_path.IndexOf('?'));

                                            if (!string.IsNullOrEmpty(item.start_date_string))
                                            {
                                                var startDateTokens = item.start_date_string.Split('-');
                                                var yearToken = int.Parse(startDateTokens[2]);
                                                item.start_date_string =
                                                    $"{(yearToken < 50 ? $"20{yearToken.ToString().PadLeft(2, '0')}" : $"19{yearToken.ToString().PadLeft(2, '0')}")}-{startDateTokens[isAmericanDateFormat ? 0 : 1]}-{startDateTokens[isAmericanDateFormat ? 1 : 0]}";
                                            }

                                            if (!string.IsNullOrEmpty(item.finish_date_string))
                                            {
                                                var endDateTokens = item.finish_date_string.Split('-');
                                                var yearToken = int.Parse(endDateTokens[2]);
                                                item.finish_date_string =
                                                    $"{(yearToken < 50 ? $"20{yearToken.ToString().PadLeft(2, '0')}" : $"19{yearToken.ToString().PadLeft(2, '0')}")}-{endDateTokens[isAmericanDateFormat ? 0 : 1]}-{endDateTokens[isAmericanDateFormat ? 1 : 0]}";
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
                            await Task.Delay(TimeSpan.FromSeconds(4));
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            if(output.Count % 300 == 0)
                Debugger.Break();

            DataCache.SaveDataForUser(_source, output, _mode);
            return output;
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