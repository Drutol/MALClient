using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm
{
    internal class AnimeReviewsQuery : Query
    {
        private readonly bool _anime;
        private readonly int _targetId;

        public AnimeReviewsQuery(int id, bool anime = true)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"http://myanimelist.net/{(anime ? "anime" : "manga")}/{id}/whatever/reviews"));
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

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(raw);
                var reviewNodes = doc.DocumentNode.Descendants("div")
                    .Where(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            "borderDark pt4 pb8 pl4 pr4 mb8").Take(Settings.ReviewsToPull);

                foreach (var reviewNode in reviewNodes)
                {
                    try
                    {
                        var current = new AnimeReviewData();
                        //Details
                        var detailsNode = reviewNode.Descendants("div").First();
                        var pictureNode = detailsNode.Descendants("div").Where(node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value == "picSurround")
                            .Skip(1)
                            .First() //2nd picSurround
                            .Descendants("a").First(); //2nd a tag
                        current.Author = pictureNode.Attributes["href"].Value.Split('/')[2];
                        current.AuthorAvatar = pictureNode.Descendants("img").First().Attributes["src"].Value;
                        //
                        current.HelpfulCount = detailsNode.Descendants("div")
                            .First(node =>
                                node.Attributes.Contains("class") &&
                                node.Attributes["class"].Value ==
                                "lightLink spaceit").InnerText;
                        //
                        var rightTableNodeDivs =
                            detailsNode.Descendants("td").Skip(2).First().Descendants("div").ToList();
                        current.Date = rightTableNodeDivs[0].InnerText;
                        current.EpisodesSeen = rightTableNodeDivs[1].InnerText;
                        current.OverallRating = rightTableNodeDivs[2].InnerText;
                        //Review Content
                        var reviewNodeContent = reviewNode.Descendants("div").First(node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value == "spaceit textReadability");
                        reviewNodeContent.ChildNodes.Remove(1);
                        current.Review =
                            WebUtility.HtmlDecode(reviewNodeContent.InnerText.Trim().Replace("read more", ""));

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