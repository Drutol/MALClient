using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Android.Runtime;
using MALClient.Models.Enums;
using MALClient.Models.Models.ApiResponses;
using MALClient.Models.Models.Library;
using MALClient.XShared.Utils;
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
            var client = new HttpClient(new NativeMessageHandler());
            var raw = await client.GetStringAsync(
                $"https://myanimelist.net/animelist/{_source}/load.json?offset=1&status=7&order=5");
            if (string.IsNullOrEmpty(raw))
                return await DataCache.RetrieveDataForUser(_source, _mode) ?? output;

            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    //var parsedData = XDocument.Parse(raw);

                    switch (_mode)
                    {
                        case AnimeListWorkModes.Anime:
                            var anime = JsonConvert.DeserializeObject<List<RootObject>>(raw);
                            foreach (var item in anime)
                            {
                                string title = "";
                                string alternateTitle = null;
                                if (Settings.PreferEnglishTitles)
                                {
                                    //var elem = item.Element("series_synonyms");
                                    //if (!string.IsNullOrWhiteSpace(elem?.Value))
                                    //{
                                    //   title = elem.Value.Split(new [] {';'}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Trim();
                                    //   alternateTitle = item.Element("series_title").Value;
                                    //}

                                        title = item.anime_title;
                                }
                                else
                                {
                                    title = item.anime_title;
                                    //var elem = item.Element("series_synonyms");
                                    //if (!string.IsNullOrWhiteSpace(elem?.Value))
                                    //{
                                    //    alternateTitle = elem.Value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Trim();
                                    //}
                                }

                                item.anime_image_path = Regex.Replace(item.anime_image_path, @"\/r\/\d+x\d+", "");
                                item.anime_image_path = item.anime_image_path.Substring(0, item.anime_image_path.IndexOf('?'));

                                output.Add(new AnimeLibraryItemData
                                {
                                    Title = title,
                                    ImgUrl = item.anime_image_path,
                                    Type = (int)GetMediaType(),
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
                case ApiType.Hummingbird:

                    switch (_mode)
                    {
                        case AnimeListWorkModes.Anime:
                            var jsonObj = JsonConvert.DeserializeObject<List<HumRootObject>>(raw,
                                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
                            foreach (var entry in jsonObj)
                            {
                                var type = AnimeType.TV;
                                try
                                {
                                    float score = 0;
                                    if (entry.rating?.value != null)
                                        float.TryParse(entry.rating.value.ToString(), out score);
                                    if (entry.anime.show_type != null)
                                        Enum.TryParse(entry.anime.show_type, true, out type);
                                    var lastWatch = new DateTime();
                                    try
                                    {
                                        lastWatch = DateTime.Parse(entry.last_watched);
                                    }
                                    catch (Exception)
                                    {
                                        lastWatch = DateTime.MinValue;
                                    }

                                    output.Add(new AnimeLibraryItemData
                                    {
                                        Title = entry.anime.title,
                                        ImgUrl = entry.anime.cover_image,
                                        Type = (int) type,
                                        MalId = entry.anime.mal_id,
                                        Id = entry.anime.id,
                                        AllEpisodes = entry.anime.episode_count,
                                        MyStartDate = "0000-00-00", //TODO : Do sth
                                        MyEndDate = "0000-00-00",
                                        MyEpisodes = Convert.ToInt32(entry.episodes_watched.ToString()),
                                        MyScore = score,
                                        MyStatus = HummingbirdStatusToMal(entry.status),
                                        LastWatched = lastWatch,
                                        Notes = entry.notes?.ToString()
                                    });
                                    if (entry.anime.status == "Currently Airing" ||
                                        entry.anime.status == "Not Yet Aired")
                                    {
                                        try
                                        {
                                            var dateObj = DateTime.Parse(entry.anime.started_airing);
                                            var day = (int)dateObj.DayOfWeek;
                                            day++;
                                            DataCache.RegisterVolatileData(entry.anime.id, new VolatileDataCache
                                            {
                                                DayOfAiring = day,
                                                GlobalScore = (float)entry.anime.community_rating,
                                            });
                                        }
                                        catch (Exception)
                                        {
                                            //no date
                                        }

                                    }


                                }
                                catch (Exception)
                                {
                                    //
                                }
                            }
                            break;
                        case AnimeListWorkModes.Manga: //rough undocumented endpoint raid
                            try
                            {
                                dynamic jsonMangaObj = JsonConvert.DeserializeObject(raw);
                                var mangaData = new Dictionary<string, dynamic>();
                                //library data and manga dta are not connected
                                foreach (var manga in jsonMangaObj.manga)
                                    mangaData.Add(manga.id.ToString(), manga);
                                foreach (var mangaLibraryEntry in jsonMangaObj.manga_library_entries)
                                {
                                    var details = mangaData[mangaLibraryEntry.manga_id.ToString()];
                                    try
                                    {
                                        var type = MangaType.Manga;
                                        Enum.TryParse(details.manga_type.ToString(), true, out type);
                                        float score = 0;
                                        if (details.rating != null)
                                            score = float.Parse(mangaLibraryEntry.rating.value.ToString());
                                        output.Add(new MangaLibraryItemData
                                        {
                                            Title = details.romaji_title.ToString(),
                                            ImgUrl = details.cover_image.ToString(),
                                            Type = (int)type,
                                            MalId = -1,
                                            Id = Convert.ToInt32(mangaLibraryEntry.id.ToString()),
                                            MyStatus = HummingbirdMangaStatusToMal(mangaLibraryEntry.status.ToString()),
                                            MyEpisodes = Convert.ToInt32(mangaLibraryEntry.chapters_read.ToString()),
                                            AllEpisodes = Convert.ToInt32(details.chapter_count.ToString()),
                                            MyStartDate = "0000-00-00",
                                            MyEndDate = "0000-00-00",
                                            MyScore = score,
                                            MyVolumes = Convert.ToInt32(mangaLibraryEntry.volumes_read.ToString()),
                                            AllVolumes = Convert.ToInt32(details.volume_count.ToString()),
                                            SlugId = mangaLibraryEntry.manga_id.ToString()
                                        });
                                        if (output.Last().ImgUrl == "/cover_images/original/missing.png")
                                        {
                                            output.Last().ImgUrl = details.poster_image.ToString();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                //aaand we've derailed
                            }
                            
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
            public int is_rewatching { get; set; }
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