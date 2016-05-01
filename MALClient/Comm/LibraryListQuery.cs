using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Models;
using MALClient.Pages;
using MALClient.ViewModels;
using Newtonsoft.Json;

namespace MALClient.Comm
{
    public class LibraryListQuery : Query
    {
        private AnimeListWorkModes _mode;

        public LibraryListQuery(AnimeListWorkModes mode)
        {
            _mode = mode;
            string type = _mode == AnimeListWorkModes.Anime ? "anime" : "manga";
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/malappinfo.php?u={Credentials.UserName}&status=all&type={type}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:                
                    Request = WebRequest.Create(Uri.EscapeUriString($"https://hummingbird.me/api/v1/users/{Credentials.UserName}/library"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public async Task<List<ILibraryData>> GetLibrary(bool force = false)
        {
            var output = force
                ? new List<ILibraryData>()
                : await DataCache.RetrieveDataForUser(Credentials.UserName, _mode) ?? new List<ILibraryData>();
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
                                MyStatus = (AnimeStatus)Convert.ToInt32(item.Element("my_status").Value),
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
                                    MyStatus = (AnimeStatus)Convert.ToInt32(item.Element("my_status").Value),
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
                            throw new ArgumentOutOfRangeException(nameof(_mode),"You gave me something different than anime/manga... b..b-baka (GetLibrary)");
                    }
                    break;
                case ApiType.Hummingbird:
                    dynamic jsonObj = JsonConvert.DeserializeObject(raw);
                    switch (_mode)
                    {
                        case AnimeListWorkModes.Anime:
                            foreach (var entry in jsonObj)
                            {
                                float score = 0;
                                float.TryParse(entry.rating.value.ToString(), out score);
                                AnimeType type = AnimeType.TV;
                                AnimeType.TryParse(entry.anime.show_type.ToString(), true, out type);
                                
                                output.Add(new AnimeLibraryItemData
                                {
                                    Title = entry.anime.title.ToString(),
                                    ImgUrl = entry.anime.cover_image.ToString(),
                                    Type = (int)type,
                                    MalId = Convert.ToInt32(entry.anime.mal_id.ToString()),
                                    Id = Convert.ToInt32(entry.anime.id.ToString()),
                                    AllEpisodes = Convert.ToInt32(entry.anime.episode_count.ToString()),
                                    MyStartDate = AnimeItemViewModel.InvalidStartEndDate, //TODO : Do sth
                                    MyEndDate = AnimeItemViewModel.InvalidStartEndDate,
                                    MyEpisodes = Convert.ToInt32(entry.episodes_watched.ToString()),
                                    MyScore = score,
                                    MyStatus = HummingbirdStatusToMal(entry.status.ToString())
                                });
                            }
                            break;
                        case AnimeListWorkModes.Manga:
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
                   throw new ArgumentOutOfRangeException(nameof(humStatus),"Hummingbird has gone crazy");
            }
        }
    }
}