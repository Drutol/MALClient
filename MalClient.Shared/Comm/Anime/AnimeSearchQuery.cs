using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using MalClient.Shared.Models.Anime;
using MalClient.Shared.Utils;
using Newtonsoft.Json;

namespace MalClient.Shared.Comm.Anime
{
    public class AnimeSearchQuery : Query
    {
        public AnimeSearchQuery(string query)
        {
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request =
                        WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/anime/search.xml?q={query}"));
                    Request.Credentials = Credentials.GetHttpCreditentials();
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:
                    Request =
                        WebRequest.Create(Uri.EscapeUriString($"https://hummingbird.me/api/v1/search/anime?query={query}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<List<AnimeGeneralDetailsData>> GetSearchResults()
        {
            var output = new List<AnimeGeneralDetailsData>();

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return output;

            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    var parsed = XElement.Parse(raw);
                    foreach (var element in parsed.Elements("entry"))
                    {
                        var item = new AnimeGeneralDetailsData();
                        item.ParseXElement(element, true);
                        output.Add(item);
                    }
                    break;
                case ApiType.Hummingbird:
                    dynamic jsonObj = JsonConvert.DeserializeObject(raw);
                    foreach (var entry in jsonObj)
                    {
                        try
                        {
                            var allEps = 0;
                            if (entry.episode_count != null)
                                allEps = Convert.ToInt32(entry.episode_count.ToString());
                            output.Add(new AnimeGeneralDetailsData
                            {
                                Title = entry.title.ToString(),
                                ImgUrl = entry.cover_image.ToString(),
                                Type = entry.show_type.ToString(),
                                Id = Convert.ToInt32(entry.id.ToString()),
                                MalId = Convert.ToInt32(entry.mal_id.ToString()),
                                AllEpisodes = allEps,
                                StartDate = "0000-00-00", //TODO : Do sth
                                EndDate = "0000-00-00",
                                Status = entry.status.ToString(),
                                Synopsis = entry.synopsis.ToString(),
                                GlobalScore = float.Parse(entry.community_rating.ToString()),
                                Synonyms = new List<string>()
                            });
                        }
                        catch (Exception e)
                        {
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return output;
        }
    }
}