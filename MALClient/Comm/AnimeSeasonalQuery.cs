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
    class AnimeSeasonalQuery : Query
    {
        public AnimeSeasonalQuery()
        {
            Request = WebRequest.Create(Uri.EscapeUriString("http://myanimelist.net/anime/season"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<SeasonalAnimeData>> GetSeasonalAnime()
        {
            string raw = await GetRequestResponse();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(raw);
            List<HtmlNode> nodessss;
            var mainNode =
                doc.DocumentNode.Descendants("div")
                    .First(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            "seasonal-anime-list js-seasonal-anime-list js-seasonal-anime-list-key-1 clearfix");



            var nodes = mainNode.ChildNodes.Where(node => node.Name == "div");
            List<SeasonalAnimeData> output = new List<SeasonalAnimeData>();
            int i = 0;
            foreach (var htmlNode in nodes)
            {
                if(htmlNode.Attributes["class"]?.Value != "seasonal-anime js-seasonal-anime")
                    continue;

                var imageNode =
                    htmlNode.Descendants("div")
                        .First(
                            node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "image");
                var link = imageNode.ChildNodes.First(node => node.Name == "a").Attributes["href"].Value;
                var img = imageNode.Attributes["style"].Value;
                var scoreTxt =
                    htmlNode.Descendants("span")
                        .First(
                            node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "score")
                        .InnerText;
                float score;
                if(!float.TryParse(scoreTxt, out score))
                    score = 0;
                output.Add(new SeasonalAnimeData
                {
                    Title = imageNode.InnerText,
                    MalLink = link,
                    Id = int.Parse(link.Substring(7).Split('/')[2]),
                    ImgUrl = img.Split('(', ')')[1],
                    Synopsis = htmlNode.Descendants("div").First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "synopsis js-synopsis").InnerHtml,
                    Score = score,
                    Episodes = htmlNode.Descendants("div").First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "eps").Descendants("a").First().InnerText.Split(new[] { " ", },StringSplitOptions.RemoveEmptyEntries)[0],
                    Index = i,
                });
                i++;
            }

            return output;
        }
    }
}
