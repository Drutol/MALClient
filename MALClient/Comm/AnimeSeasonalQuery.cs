using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Data.Html;
using HtmlAgilityPack;
using MALClient.Items;
using MALClient.ViewModels;

namespace MALClient.Comm
{
    internal class AnimeSeasonalQuery : Query
    {
        private bool _overriden;
        private AnimeSeason _season;

        public AnimeSeasonalQuery(AnimeSeason season)
        {
            _season = season;
            _overriden = _season.Url != "http://myanimelist.net/anime/season";
            Request = WebRequest.Create(Uri.EscapeUriString(_season.Url));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<SeasonalAnimeData>> GetSeasonalAnime(bool force = false)
        {
            List<SeasonalAnimeData> output = force || DataCache.SeasonalUrls.Count == 0 //either force or urls are empty after update
                ? new List<SeasonalAnimeData>()
                : await DataCache.RetrieveSeasonalData(_overriden ? _season.Name : "") ?? new List<SeasonalAnimeData>(); //current season without suffix
            if (output.Count != 0) return output;
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            HtmlNode mainNode =
                doc.DocumentNode.Descendants("div")
                    .First(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            "seasonal-anime-list js-seasonal-anime-list js-seasonal-anime-list-key-1 clearfix");

            //Get season data - we are getting this only from current season
            if (!_overriden)
            {
                var seasonInfoNodes = doc.DocumentNode.Descendants("div").First(
                    node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value ==
                        "horiznav_nav").Descendants("li").ToList();
                Dictionary<string, string> seasonData = new Dictionary<string, string>();
                for (int j = 1; j <= 4; j++)
                {
                    try
                    {
                        seasonData.Add(seasonInfoNodes[j].Descendants("a").First().InnerText.Trim(),
                            seasonInfoNodes[j].Descendants("a").First().Attributes["href"].Value);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }
                DataCache.SaveSeasonalUrls(seasonData);
            }

            //Get anime data
            IEnumerable<HtmlNode> nodes = mainNode.ChildNodes.Where(node => node.Name == "div");

            var i = 0;
            foreach (HtmlNode htmlNode in nodes)
            {
                if (htmlNode.Attributes["class"]?.Value != "seasonal-anime js-seasonal-anime")
                    continue;

                HtmlNode imageNode =
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
                HtmlNode infoNode =
                    htmlNode.Descendants("div")
                        .First(node => node.Attributes.Contains("class") && node.Attributes["class"].Value == "info");
                int day;
                try
                {
                    var date = infoNode.ChildNodes[1].InnerText.Trim().Substring(0, 13).Replace(",", "");
                    day = (int) DateTime.Parse(date).DayOfWeek;
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
                    Title = WebUtility.HtmlDecode(imageNode.InnerText.Trim()),
                    //there are some \n that we need to get rid of
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
                            .InnerText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[0],
                    Index = i,
                    Genres = htmlNode.Descendants("div").First(node =>
                        node.Attributes.Contains("class") &&
                        node.Attributes["class"].Value == "genres-inner js-genre-inner").InnerText
                        .Replace('\n', ';')
                        .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim())
                        .ToList(),
                    AirDay = day
                });
                i++;
                if (i == 30)
                    break;
            }

            await Task.Run(() => DataCache.SaveSeasonalData(output,_overriden ? _season.Name : ""));

            //We are done.
            return output;
        }
    }
}