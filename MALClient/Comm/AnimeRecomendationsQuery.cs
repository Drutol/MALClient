using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Items;

namespace MALClient.Comm
{
    class AnimeRecomendationsQuery : Query
    {
        public AnimeRecomendationsQuery()
        {
            Request = WebRequest.Create(Uri.EscapeUriString("http://myanimelist.net/recommendations.php?s=recentrecs&t=anime"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<RecomendationData>> GetRecomendationsData()
        {
           // var possibleData = Utils.GetMainPageInstance().RetrieveRecommendationData();
           // if (possibleData.Count != 0)
                //return possibleData;
            var output = new List<RecomendationData>();
            string raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(raw);
            var recomNodes =
                doc.DocumentNode.Descendants("div")
                    .Where(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            "spaceit borderClass").Take(20);

            foreach (HtmlNode recomNode in recomNodes)
            {
                var desc = recomNode.ChildNodes.First(node => node.Name == "div" && node.Attributes["class"].Value == "spaceit");
                if (desc != null)
                {
                    //var imgs = recomNode.Descendants("img").Select(node => node.Attributes["src"].Value).ToArray();
                    var titleNodes = recomNode.Descendants("a").Where(node => node.Attributes.Count == 2).Take(2).ToArray();
                    var titles = titleNodes.Select(node => node.Attributes["title"].Value).ToArray();
                    var ids = titleNodes.Select(node => Convert.ToInt32(node.Attributes["href"].Value.Substring(6).Split('/')[1])).ToArray();

                    output.Add(new RecomendationData
                    {
                        DependentId = ids[0],
                        RecommendationId = ids[1],

                        DependentTitle = titles[0],
                        RecommendationTitle = titles[1],

                        Description = WebUtility.HtmlDecode(desc.InnerText)
                    });
                }
            }


           // Utils.GetMainPageInstance().SaveRecommendationsData(output);

            return output;
        }
    }
}
