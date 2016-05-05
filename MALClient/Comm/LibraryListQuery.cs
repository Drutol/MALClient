using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Models;
using MALClient.Models.ApiResponses;
using MALClient.Pages;
using MALClient.ViewModels;
using Newtonsoft.Json;

namespace MALClient.Comm
{
    public class LibraryListQuery : Query
    {
        private AnimeListWorkModes _mode;
        private string _source;
        public LibraryListQuery(string source,AnimeListWorkModes mode)
        {
            _mode = mode;
            _source = source;
            string type = _mode == AnimeListWorkModes.Anime ? "anime" : "manga";
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/malappinfo.php?u={source}&status=all&type={type}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:
                    switch (mode)
                    {
                        case AnimeListWorkModes.Anime:
                            Request = WebRequest.Create(Uri.EscapeUriString($"https://hummingbird.me/api/v1/users/{source}/library"));
                            Request.ContentType = "application/x-www-form-urlencoded";
                            Request.Method = "GET";
                            break;
                        case AnimeListWorkModes.Manga:
                            Request = WebRequest.Create(Uri.EscapeUriString($"https://hummingbird.me/manga_library_entries?user_id={source}"));
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
            var output = force ? new List<ILibraryData>() : await DataCache.RetrieveDataForUser(_source, _mode) ?? new List<ILibraryData>();
            if (output.Count > 0)
                return output;
            string raw = await GetRequestResponse();
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
                                    MyScore = Convert.ToInt32(item.Element("my_score").Value)
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
                                    AllVolumes = Convert.ToInt32(item.Element("series_volumes").Value)
                                });
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(_mode), "You gave me something different than anime/manga... b..b-baka (GetLibrary)");
                    }
                    break;
                case ApiType.Hummingbird:

                    switch (_mode)
                    {
                        case AnimeListWorkModes.Anime:
                            List<HumRootObject> jsonObj = JsonConvert.DeserializeObject<List<HumRootObject>>(raw,new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore});
                            foreach (var entry in jsonObj)
                            {
                                AnimeType type = AnimeType.TV;
                                try
                                {
                                    float score = 0;
                                    if (entry.rating?.value != null)
                                        float.TryParse(entry.rating.value.ToString(), out score);
                                    if (entry.anime.show_type != null)
                                        AnimeType.TryParse(entry.anime.show_type, true, out type);
                                    output.Add(new AnimeLibraryItemData
                                    {
                                        Title = entry.anime.title,
                                        ImgUrl = entry.anime.cover_image,
                                        Type = (int) type,
                                        MalId = entry.anime.mal_id,
                                        Id = Convert.ToInt32(entry.anime.id.ToString()),
                                        AllEpisodes = entry.anime.episode_count,
                                        MyStartDate = AnimeItemViewModel.InvalidStartEndDate, //TODO : Do sth
                                        MyEndDate = AnimeItemViewModel.InvalidStartEndDate,
                                        MyEpisodes = Convert.ToInt32(entry.episodes_watched.ToString()),
                                        MyScore = score,
                                        MyStatus = HummingbirdStatusToMal(entry.status)
                                    });
                                }
                                catch (Exception e)
                                {
                                    //
                                }

                            }
                            break;
                        case AnimeListWorkModes.Manga: //rough undocumented endpoint raid
                            dynamic jsonMangaObj = JsonConvert.DeserializeObject(raw);
                            Dictionary<string, dynamic> mangaData = new Dictionary<string, dynamic>(); //library data and manga dta are not connected
                            foreach (var manga in jsonMangaObj.manga)
                                mangaData.Add(manga.id.ToString(), manga);
                            foreach (var mangaLibraryEntry in jsonMangaObj.manga_library_entries)
                            {
                                var details = mangaData[mangaLibraryEntry.manga_id.ToString()];
                                try
                                {
                                    MangaType type = MangaType.Manga;
                                    MangaType.TryParse(details.manga_type.ToString(), true, out type);
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
                                        MyStartDate = AnimeItemViewModel.InvalidStartEndDate,
                                        MyEndDate = AnimeItemViewModel.InvalidStartEndDate,
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
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DataCache.SaveDataForUser(Credentials.UserName, output, _mode);
            return output;
        }

        public async Task<XElement> GetProfileStats(bool wantMsg = true)
        {
            var raw = await GetRequestResponse(wantMsg);
            if (string.IsNullOrEmpty(raw))
                return null;
            try
            {
                XElement doc = XElement.Parse(raw);
                return doc.Element("myinfo");
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static AnimeStatus HummingbirdStatusToMal(string humStatus)
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