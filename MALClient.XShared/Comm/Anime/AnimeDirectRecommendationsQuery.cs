using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Comm.CommUtils;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeDirectRecommendationsQuery : Query
    {
        private readonly int _animeId;
        private readonly bool _animeMode;

        public AnimeDirectRecommendationsQuery(int id, bool anime = true)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"https://myanimelist.net/{(anime ? "anime" : "manga")}/{id}/whatever/userrecs"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _animeId = id;
            _animeMode = anime;
        }

        public async Task<List<DirectRecommendationData>> GetDirectRecommendations(bool force = false)
        {
            var output = force
                ? new List<DirectRecommendationData>()
                : await DataCache.RetrieveDirectRecommendationData(_animeId, _animeMode) ??
                  new List<DirectRecommendationData>();
            if (output.Count != 0) return output;

            var raw = await GetRequestResponse(false);
            if (string.IsNullOrEmpty(raw))
                return null;

            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            try
            {
                var recommNodes = doc.DocumentNode.Descendants("div")
                    .Where(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            HtmlClassMgr.ClassDefs["#DirectRecomm:recommNode:class"]).Take(Settings.RecommsToPull);

                foreach (var recommNode in recommNodes)
                {
                    try
                    {
                        var current = new DirectRecommendationData();

                        var tds = recommNode.Descendants("td").Take(2).ToList();
                        var img = tds[0].Descendants("img").First().Attributes["data-src"].Value;
                        if (!img.Contains("questionmark"))
                        {
                            img = Regex.Replace(img, @"\/r\/\d+x\d+", "");
                            current.ImageUrl = img.Substring(0, img.IndexOf('?'));
                        }
                        current.Description = WebUtility.HtmlDecode(tds[1].Descendants("div").First(
                            node =>
                                node.Attributes.Contains("class") &&
                                node.Attributes["class"].Value ==
                                HtmlClassMgr.ClassDefs["#DirectRecomm:recommNode:descClass"])
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
            }
            catch (Exception)
            {
                //something we wrong
            }

            DataCache.SaveDirectRecommendationsData(_animeId, output, _animeMode);

            return output;
        }
    }
}