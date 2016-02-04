using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Core;
using Windows.Data.Html;
using Windows.UI.Core;
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

        public async Task<List<SeasonalAnimeData>> GetSeasonalAnime(bool force = false)
        {
            List<SeasonalAnimeData> output = force ? new List<SeasonalAnimeData>() : await DataCache.RetrieveSeasonalData() ?? new List<SeasonalAnimeData>();
            if (output.Count != 0) return output;
            string raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(raw);
            var mainNode =
                doc.DocumentNode.Descendants("div")
                    .First(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            "seasonal-anime-list js-seasonal-anime-list js-seasonal-anime-list-key-1 clearfix");

            var nodes = mainNode.ChildNodes.Where(node => node.Name == "div");

            int i = 0;
            foreach (var htmlNode in nodes)
            {
                if (htmlNode.Attributes["class"]?.Value != "seasonal-anime js-seasonal-anime")
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
                var infoNode =
                    htmlNode.Descendants("div")
                        .First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "info");
                int day;
                try
                {
                    string date = infoNode.ChildNodes[1].InnerText.Trim().Substring(0,13).Replace(",","");
                    day = (int)DateTime.Parse(date).DayOfWeek;
                    day++;
                }

                catch (Exception)
                {
                    day = -1;
                }
                    
                float score;
                if (!float.TryParse(scoreTxt, out score))
                    score = 0;
                output.Add(new SeasonalAnimeData
                {
                    Title = WebUtility.HtmlDecode(imageNode.InnerText.Trim()), //there are some \n that we need to get rid of
                    MalLink = link,
                    Id = int.Parse(link.Substring(7).Split('/')[2]), //extracted from anime link
                    ImgUrl = img.Split('(', ')')[1], // from image style attr it's between ( )
                    Synopsis =
                        HtmlUtilities.ConvertToText(htmlNode.Descendants("div")
                            .First(
                                node =>
                                    node.Attributes.Contains("class") &&
                                    node.Attributes["class"].Value == "synopsis js-synopsis")
                            .InnerText),
                    Score = score, //0 for N/A
                    Episodes =
                        htmlNode.Descendants("div")
                            .First(
                                node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "eps")
                            .Descendants("a")
                            .First()
                            .InnerText.Split(new[] {" ",}, StringSplitOptions.RemoveEmptyEntries)[0],
                    Index = i,
                    Genres = htmlNode.Descendants("div").First(node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value == "genres-inner js-genre-inner").InnerText
                        .Replace('\n',';')
                        .Split(new[] {';'} , StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()) 
                        .ToList(),
                    AirDay = day,
                                        
                });
                i++;
                if (i == 30)
                    break;
            }

            await Task.Run(() => DataCache.SaveSeasonalData(output));

            //We are done.
            return output;
        }
    }
}
