using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MalClient.Shared.Comm.CommUtils;
using MalClient.Shared.Models.AnimeScrapped;

namespace MalClient.Shared.Comm.Anime
{
    public class AnimeRecomendationsQuery : Query
    {
        public AnimeRecomendationsQuery()
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString("https://myanimelist.net/recommendations.php?s=recentrecs&t=anime"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<RecomendationData>> GetRecomendationsData()
        {
            var output = new List<RecomendationData>();
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            var recomNodes =
                doc.DocumentNode.Descendants("div")
                    .Where(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            HtmlClassMgr.ClassDefs["#Recommendations:recommNode:class"]).Take(20);
                //constant 20 recommendations

            foreach (var recomNode in recomNodes)
            {
                try
                {
                    var desc =
                        recomNode.ChildNodes.First(
                            node =>
                                node.Name == "div" &&
                                node.Attributes["class"].Value ==
                                HtmlClassMgr.ClassDefs["#Recommendations:recommNodeDesc:class"]);
                    if (desc != null)
                    {
                        var titleNodes =
                            recomNode.Descendants("a").Where(node => node.Attributes.Count == 2).Take(2).ToArray();
                        var titles = titleNodes.Select(node => node.Attributes["title"].Value).ToArray();
                        var ids =
                            titleNodes.Select(
                                node => Convert.ToInt32(node.Attributes["href"].Value.Substring(6).Split('/')[1]))
                                .ToArray();

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
                catch (Exception)
                {
                    //
                }
            }

            return output;
        }
    }
}