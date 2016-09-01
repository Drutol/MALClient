using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Models.Enums;
using MALClient.Models.Models.ApiResponses;
using MALClient.Models.Models.Library;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
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
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return output;

            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    var parsedData = XDocument.Parse(raw);

                    switch (_mode)
                    {
                        case AnimeListWorkModes.Anime:
                            var anime = parsedData.Root.Elements("anime").ToList();
                            foreach (var item in anime)
                            {
                                output.Add(new AnimeLibraryItemData
                                {
                                    Title = item.Element("series_title").Value,
                                    ImgUrl = item.Element("series_image").Value,
                                    Type = Convert.ToInt32(item.Element("series_type").Value),
                                    MalId = Convert.ToInt32(item.Element("series_animedb_id").Value),
                                    MyStatus = (AnimeStatus) Convert.ToInt32(item.Element("my_status").Value),
                                    MyEpisodes = Convert.ToInt32(item.Element("my_watched_episodes").Value),
                                    AllEpisodes = Convert.ToInt32(item.Element("series_episodes").Value),
                                    MyStartDate = item.Element("my_start_date").Value,
                                    MyEndDate = item.Element("my_finish_date").Value,
                                    MyScore = Convert.ToInt32(item.Element("my_score").Value),
                                    Notes = item.Element("my_tags").Value
                                });
                            }
                            break;
                        case AnimeListWorkModes.Manga:
                            var manga = parsedData.Root.Elements("manga").ToList();
                            foreach (var item in manga)
                            {
                                output.Add(new MangaLibraryItemData
                                {
                                    Title = item.Element("series_title").Value,
                                    ImgUrl = item.Element("series_image").Value,
                                    Type = Convert.ToInt32(item.Element("series_type").Value),
                                    MalId = Convert.ToInt32(item.Element("series_mangadb_id").Value),
                                    MyStatus = (AnimeStatus) Convert.ToInt32(item.Element("my_status").Value),
                                    MyEpisodes = Convert.ToInt32(item.Element("my_read_chapters").Value),
                                    AllEpisodes = Convert.ToInt32(item.Element("series_chapters").Value),
                                    MyStartDate = item.Element("my_start_date").Value,
                                    MyEndDate = item.Element("my_finish_date").Value,
                                    MyScore = Convert.ToInt32(item.Element("my_score").Value),
                                    MyVolumes = Convert.ToInt32(item.Element("my_read_volumes").Value),
                                    AllVolumes = Convert.ToInt32(item.Element("series_volumes").Value),
                                    Notes = item.Element("my_tags").Value
                                });
                            }
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