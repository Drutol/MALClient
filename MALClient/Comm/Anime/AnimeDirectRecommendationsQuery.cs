using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm
{
    internal class AnimeDirectRecommendationsQuery : Query
    {
        private readonly int _animeId;
        private readonly bool _animeMode;

        public AnimeDirectRecommendationsQuery(int id, bool anime = true)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"http://myanimelist.net/{(anime ? "anime" : "manga")}/{id}/whatever/userrecs"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _animeId = id;
            _animeMode = anime;
        }

        public async Task<List<DirectRecommendationData>> GetDirectRecommendations(bool force = false)
        {
            var output = force
                ? new List<DirectRecommendationData>()
                : await DataCache.RetrieveDirectRecommendationData(_animeId,_animeMode) ?? new List<DirectRecommendationData>();
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
                        "borderClass").Take(Settings.RecommsToPull);

            foreach (var recommNode in recommNodes)
            {
                try
                {
                    var current = new DirectRecommendationData();

                    var tds = recommNode.Descendants("td").Take(2).ToList();
                    current.ImageUrl = tds[0].Descendants("img").First().Attributes["src"].Value;
                    var pos = current.ImageUrl.LastIndexOf('t');
                        // we want to remove last "t" from url as this is much smaller image than we would want
                    if (pos != -1)
                    {
                        current.ImageUrl = current.ImageUrl.Remove(pos, 1);
                    }
                    current.Description = WebUtility.HtmlDecode(tds[1].Descendants("div").First(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            "borderClass bgColor1")
                        .Descendants("div")
                        .First().InnerText.Trim().Replace("&nbsp", "").Replace("read more", ""));
                    var titleNode = tds[1].ChildNodes[3].Descendants("a").First();
                    current.Title = titleNode.Descendants("strong").First().InnerText.Trim();
                    var link = titleNode.Attributes["href"].Value.Split('/');
                    current.Id = Convert.ToInt32(link[2]);
                    current.Type = link[1] == "anime"
                        ? RelatedItemType.Anime
                        : link[1] == "manga" ? RelatedItemType.Manga : RelatedItemType.Unknown;
                    output.Add(current);
                }
                catch (Exception)
                {
                    //who knows...raw html is scary
                }
            }

            DataCache.SaveDirectRecommendationsData(_animeId, output, _animeMode);

            return output;
        }
    }
}