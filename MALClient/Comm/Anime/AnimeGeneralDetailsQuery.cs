using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Models;
using MALClient.ViewModels;
using Newtonsoft.Json;

namespace MALClient.Comm.Anime
{
    class AnimeGeneralDetailsQuery : Query
    {
        public async Task<AnimeGeneralDetailsData> GetAnimeDetails(bool force,string id,string title,bool animeMode)
        {
            var output = force ? null : await DataCache.RetrieveAnimeSearchResultsData(id, animeMode);
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    var data = animeMode ? await new AnimeSearchQuery(Utils.CleanAnimeTitle(title)).GetRequestResponse(false) : await new MangaSearchQuery(Utils.CleanAnimeTitle(title)).GetRequestResponse(false);
                    data = WebUtility.HtmlDecode(data);
                    data = data.Replace("&mdash", "").Replace("&rsquo", "").Replace("&", "");

                    var parsedData = XDocument.Parse(data);

                    var elements = parsedData.Element(animeMode ? "anime" : "manga").Elements("entry");
                    var xmlObj = elements.First(element => element.Element("id").Value == id);

                    output = new AnimeGeneralDetailsData();
                    output.ParseXElement(xmlObj, animeMode);

                    DataCache.SaveAnimeSearchResultsData(id, output, animeMode);
                    break;
                case ApiType.Hummingbird:
                    Request =
                    WebRequest.Create(
                        Uri.EscapeUriString($"https://hummingbird.me/api/v1/anime/{id}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";

                    string raw = await GetRequestResponse();
                    if (string.IsNullOrEmpty(raw))
                        break;

                    dynamic jsonObj = JsonConvert.DeserializeObject(raw);
                    output = new AnimeGeneralDetailsData
                    {
                        Title = jsonObj.title.ToString(),
                        ImgUrl = jsonObj.cover_image.ToString(),
                        Type = jsonObj.show_type.ToString(),
                        Id = Convert.ToInt32(jsonObj.id.ToString()),
                        MalId = Convert.ToInt32(jsonObj.mal_id.ToString()),
                        AllEpisodes = Convert.ToInt32(jsonObj.episode_count.ToString()),
                        StartDate = AnimeItemViewModel.InvalidStartEndDate, //TODO : Do sth
                        EndDate = AnimeItemViewModel.InvalidStartEndDate, 
                        Status = jsonObj.status,
                        Synopsis = jsonObj.synopsis,
                        GlobalScore = jsonObj.community_rating,
                        Synonyms = new List<string> { jsonObj.alternate_title.ToString()} 
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return output;
        }
    }
}
