using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Items;
using MALClient.Models;

namespace MALClient.Comm
{
    class AnimeTopQuery : Query
    {
        private bool _animeMode;

        public AnimeTopQuery(bool animeMode = true)
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/{(animeMode ? "topanime" : "topmanga")}.php"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _animeMode = animeMode;
        }

        public async Task<List<TopAnimeData>> GetTopAnimeData(bool force = false)
        {
            var output = new List<TopAnimeData>();
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
                            "top-ranking-table"); //constant 20 recommendations
            int i = 0;
            foreach (var item in topNodes.Descendants("tr").Where(node =>
                node.Attributes.Contains("class") &&
                node.Attributes["class"].Value ==
                "ranking-list"))
            {
                try
                {
                    var current = new TopAnimeData();
                    string epsText = item.Descendants("div").First(node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        "information di-ib mt4").ChildNodes[0].InnerText;
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
                        "hoverinfo_trigger fs14 fw-b");
                    current.Title = WebUtility.HtmlDecode(titleNode.InnerText).Trim();
                    current.Id = Convert.ToInt32(titleNode.Attributes["href"].Value.Substring(7).Split('/')[2]);
                    current.Score = float.Parse(item.Descendants("span").First(node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        "text on").InnerText.Trim());
                    current.Index = ++i;


                    output.Add(current);
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
