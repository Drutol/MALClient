using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm
{
    internal class AnimeTopQuery : Query
    {
        private readonly bool _animeMode;

        public AnimeTopQuery(bool animeMode = true)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"http://myanimelist.net/{(animeMode ? "topanime" : "topmanga")}.php"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _animeMode = animeMode;
        }

        public async Task<List<TopAnimeData>> GetTopAnimeData(bool force = false)
        {
            var output = force
                ? new List<TopAnimeData>()
                : (await DataCache.RetrieveTopAnimeData(_animeMode) ?? new List<TopAnimeData>());
            if (output.Count > 0)
                return output;
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;


            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            var topNodes =
                doc.DocumentNode.Descendants("table")
                    .First(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            HttpClassMgr.ClassDefs["#Top:mainNode:class"]);
            var i = 0;
            foreach (var item in topNodes.Descendants("tr").Where(node =>
                node.Attributes.Contains("class") &&
                node.Attributes["class"].Value ==
                HttpClassMgr.ClassDefs["#Top:topNode:class"]))
            {
                try
                {
                    var current = new TopAnimeData();
                    var epsText = item.Descendants("div").First(node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        HttpClassMgr.ClassDefs["#Top:topNode:eps:class"]).ChildNodes[0].InnerText;
                    epsText = epsText.Substring(epsText.IndexOf('(') + 1);
                    epsText = epsText.Substring(0, epsText.IndexOf(' '));
                    current.Episodes = epsText;
                    var img = item.Descendants("img").First().Attributes["src"].Value;
                    var pos = img.LastIndexOf('t');
                    // we want to remove last "t" from url as this is much smaller image than we would want
                    current.ImgUrl = pos != -1 ? img.Remove(pos, 1) : img;
                    var titleNode = item.Descendants("a").First(node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        HttpClassMgr.ClassDefs[_animeMode ? "#Top:topNode:titleNode:class" : "#Top:topMangaNode:titleNode:class"]);
                    current.Title = WebUtility.HtmlDecode(titleNode.InnerText).Trim();
                    current.Id = Convert.ToInt32(titleNode.Attributes["href"].Value.Substring(7).Split('/')[2]);
                    current.Score = float.Parse(item.Descendants("span").First(node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        HttpClassMgr.ClassDefs["#Top:topNode:score:class"]).InnerText.Trim());
                    current.Index = ++i;


                    output.Add(current);
                }
                catch (Exception)
                {
                    //
                }
            }
            DataCache.SaveTopAnimeData(output, _animeMode);

            return output;
        }
    }
}