using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Comm.CommUtils;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeReviewsQuery : Query
    {
        private readonly bool _anime;
        private readonly int _targetId;

        public AnimeReviewsQuery(int id, bool anime = true)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"https://myanimelist.net/{(anime ? "anime" : "manga")}/{id}/whatever/reviews"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _targetId = id;
            _anime = anime;
        }

        public async Task<List<AnimeReviewData>> GetAnimeReviews(bool force = false)
        {
            var output = force
                ? new List<AnimeReviewData>()
                : await DataCache.RetrieveReviewsData(_targetId, _anime) ?? new List<AnimeReviewData>();
            if (output.Count != 0) return output;

            var raw = await GetRequestResponse(false);
            if (string.IsNullOrEmpty(raw))
                return null;

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(raw);
                var reviewNodes = doc.DocumentNode.WhereOfDescendantsWithClass("div", "borderDark").Take(Settings.ReviewsToPull);

                foreach (var reviewNode in reviewNodes)
                {
                    try
                    {
                        var current = new AnimeReviewData();
                        //Details
                        var detailsNode = reviewNode.ChildNodes.First(node => node.Name == "div");
                        var pictureNode = detailsNode.WhereOfDescendantsWithClass("div", "picSurround")
                            .First() //1nd picSurround
                            .Descendants("a").First(); //2nd a tag
                        current.Author = pictureNode.Attributes["href"].Value.Split('/').Last();
                        current.AuthorAvatar = pictureNode.Descendants("img").First().Attributes["data-src"].Value;
                        //
                        current.HelpfulCount =
                            detailsNode.WhereOfDescendantsWithClass("div", "lightLink spaceit")
                                .Skip(1)
                                .First()
                                .InnerText.Trim()
                                .TrimWhitespaceInside();
                        //
                        var rightTableNode = reviewNode.FirstOfDescendantsWithClass("div", "mb8");
                        var rightTableNodeDivs = rightTableNode.Descendants("div").ToList();
                        current.Date = rightTableNode.ChildNodes[0].InnerText.Trim();
                        current.EpisodesSeen = rightTableNodeDivs[0].InnerText.Trim();
                        current.OverallRating = rightTableNodeDivs[1].InnerText.Trim().TrimWhitespaceInside();
                        //Review Content
                        var reviewNodeContent = reviewNode.FirstOfDescendantsWithClass("div", "spaceit textReadability word-break pt8 mt8");
                        foreach (var scoreRow in reviewNodeContent.ChildNodes[1].Descendants("tr").Skip(1))
                        {
                            var tds = scoreRow.Descendants("td").ToList();
                            current.Score.Add(new ReviewScore {Field = tds[0].InnerText,Score = tds[1].InnerText == "&nbsp;" ? "N/A" : tds[1].InnerText });
                        }
                        reviewNodeContent.ChildNodes.Remove(1);
                        var rawReview = reviewNodeContent.ChildNodes.Where(node => node.Name == "#text")
                            .Aggregate("", (s, node) => s += node.InnerText)
                            .Replace("read more", "")
                            .Replace("Helpful", "")
                            .Trim(' ', '\n', '\r');

                        var reviewSecondPart = (reviewNodeContent.ChildNodes.FirstOrDefault(node => node.Name == "span")?.InnerText ?? "")
                            .TrimWhitespaceInside(false).Trim(' ', '\n', '\r');

                        var oblivionStartIndex = reviewSecondPart.IndexOf("\r\n\r\n");
                        if (oblivionStartIndex != -1)
                            reviewSecondPart = reviewSecondPart.Remove(oblivionStartIndex, 4);

                        rawReview += reviewSecondPart;

                        current.Review =
                            WebUtility.HtmlDecode(rawReview.Trim(' ', '\n', '\r'));

                        output.Add(current);
                    }
                    catch (Exception)
                    {
                        //something unexpected
                    }
                }
            }
            catch (Exception)
            {
                //no reviews
            }
            DataCache.SaveAnimeReviews(_targetId, output, _anime);

            return output;
        }
    }
}