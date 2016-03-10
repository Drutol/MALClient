using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm
{
    class DirectRecommendationsQuery : Query
    {
        private int _animeId;

        public DirectRecommendationsQuery(int id)
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/anime/{id}/whatever/userrecs"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _animeId = id;
        }

        public async Task<List<DirectRecommendationData>> GetDirectRecommendations(bool force = false)
        {
            List<DirectRecommendationData> output = force
            ? new List<DirectRecommendationData>()
            : await DataCache.RetrieveDirectRecommendationData(_animeId) ?? new List<DirectRecommendationData>();
            if (output.Count != 0) return output;

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;

            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            var recommNodes = doc.DocumentNode.Descendants("div")
                .Where(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            "borderClass").Take(8);

            foreach (var recommNode in recommNodes)
            {
                var current = new DirectRecommendationData();

                var tds = recommNode.Descendants("td").Take(2).ToList();
                current.ImageUrl = tds[0].Descendants("img").First().Attributes["src"].Value;
                current.Description = WebUtility.HtmlDecode(tds[1].Descendants("div").First(
                    node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        "borderClass bgColor1")
                        .Descendants("div")
                        .First().InnerText.Trim());
                var titleNode = tds[1].ChildNodes[3].Descendants("a").First();
                current.Title = titleNode.Descendants("strong").First().InnerText.Trim();
                current.Id = Convert.ToInt32(titleNode.Attributes["href"].Value.Split('/')[2]);

                output.Add(current);
            }

            DataCache.SaveDirectRecommendationsData(_animeId,output);

            return output;
        }
    }
}
