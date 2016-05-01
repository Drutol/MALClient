using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Models;

namespace MALClient.Comm.Anime
{
    class AnimeGeneralDetailsQuery
    {
        public async Task<AnimeGeneralDetailsData> GetAnimeDetails(bool force,string id,string title,bool animeMode)
        {
            var output = force ? null : await DataCache.RetrieveAnimeSearchResultsData(id, animeMode);
            var data = animeMode
                ? await new AnimeSearchQuery(Utils.CleanAnimeTitle(title)).GetRequestResponse(false)
                : await new MangaSearchQuery(Utils.CleanAnimeTitle(title)).GetRequestResponse(false);
            data = WebUtility.HtmlDecode(data);
            data = data.Replace("&mdash", "").Replace("&rsquo", "").Replace("&", "");

            var parsedData = XDocument.Parse(data);

            var elements = parsedData.Element(animeMode ? "anime" : "manga").Elements("entry");
            var xmlObj = elements.First(element => element.Element("id").Value == id);

            output = new AnimeGeneralDetailsData();
            output.ParseXElement(xmlObj,animeMode);

            DataCache.SaveAnimeSearchResultsData(id, output , animeMode);

            return output;
        }
    }
}
