using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MalClient.Shared.Models.Anime;
using MALClient.Items;
using MALClient.Utils;

namespace MALClient.Comm
{
    public class AnimeSeasonalQuery : Query
    {
        private readonly bool _overriden;
        private readonly AnimeSeason _season;

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
            var output = force || DataCache.SeasonalUrls?.Count == 0 //either force or urls are empty after update
                ? new List<SeasonalAnimeData>()
                : await DataCache.RetrieveSeasonalData(_overriden ? _season.Name : "") ?? new List<SeasonalAnimeData>();
            //current season without suffix
            if (output.Count != 0) return output;
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return output;


            //Get season data - we are getting this only from current season
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(raw);
                var mainNode =
                    doc.DocumentNode.Descendants("div")
                        .First(
                            node =>
                                node.Attributes.Contains("class") &&
                                node.Attributes["class"].Value ==
                                HtmlClassMgr.ClassDefs["#Seasonal:mainNode:class"]);
                if (!_overriden)
                {
                    var seasonInfoNodes = doc.DocumentNode.Descendants("div").First(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            HtmlClassMgr.ClassDefs["#Seasonal:seasonInfo:class"]).Descendants("li").ToList();
                    var seasonData = new Dictionary<string, string>();
                    for (var j = 1; j <= 4; j++)
                    {
                        try
                        {
                            seasonData.Add(seasonInfoNodes[j].Descendants("a").First().InnerText.Trim(),
                                seasonInfoNodes[j].Descendants("a").First().Attributes["href"].Value);

                            if (seasonInfoNodes[j].Descendants("a").First().Attributes["class"].Value ==
                                HtmlClassMgr.ClassDefs["#Seasonal:seasonInfoCurrent:class"])
                                seasonData.Add("current", j.ToString());
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
                    DataCache.SaveSeasonalUrls(seasonData);
                }

                //Get anime data
                var nodes = mainNode.ChildNodes.Where(node => node.Name == "div").Take(Settings.SeasonalToPull);

                var i = 0;
                foreach (var htmlNode in nodes)
                {
                    try
                    {

                        if (htmlNode.Attributes["class"]?.Value != HtmlClassMgr.ClassDefs["#Seasonal:entryNode:class"])
                            continue;

                        var imageNode =
                            htmlNode.FirstOfDescendantsWithClass("div", "image lazyload");
                        var link = imageNode.ChildNodes.First(node => node.Name == "a").Attributes["href"].Value;
                        var img = imageNode.Attributes["data-bg"].Value;
                        var scoreTxt =
                            htmlNode.Descendants("span")
                                .First(
                                    node =>
                                        node.Attributes.Contains("class") &&
                                        node.Attributes["class"].Value ==
                                        HtmlClassMgr.ClassDefs["#Seasonal:entryNode:score:class"])
                                .InnerText;
                        var infoNode =
                            htmlNode.Descendants("div")
                                .First(
                                    node =>
                                        node.Attributes.Contains("class") &&
                                        node.Attributes["class"].Value ==
                                        HtmlClassMgr.ClassDefs["#Seasonal:entryNode:info:class"]);
                        int day;
                        string airStartDate = null;
                        try
                        {
                            var date = infoNode.ChildNodes[1].InnerText.Trim().Substring(0, 13).Replace(",", "");
                            var dateObj = DateTime.Parse(date);
                            day = (int) dateObj.DayOfWeek;
                            airStartDate = dateObj.ToString("yyyy-MM-dd");
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
                            Id = int.Parse(link.Substring(7).Split('/')[2]), //extracted from anime link
                            ImgUrl = img, // from image style attr it's between ( )
                            Score = score, //0 for N/A
                            Episodes =
                                htmlNode.Descendants("div")
                                    .First(
                                        node =>
                                            node.Attributes.Contains("class") &&
                                            node.Attributes["class"].Value ==
                                            HtmlClassMgr.ClassDefs["#Seasonal:entryNode:eps:class"])
                                    .Descendants("a")
                                    .First()
                                    .InnerText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)[0],
                            Index = i,
                            Genres = htmlNode.Descendants("div").First(node =>
                                node.Attributes.Contains("class") &&
                                node.Attributes["class"].Value ==
                                HtmlClassMgr.ClassDefs["#Seasonal:entryNode:genres:class"])
                                .InnerText
                                .Replace('\n', ';')
                                .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                                .Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim())
                                .ToList(),
                            AirDay = day,
                            AirStartDate = airStartDate
                        });
                        i++;
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                //sumthing
            }


            DataCache.SaveSeasonalData(output, _overriden ? _season.Name : "");

            //We are done.
            return output;
        }
    }
}