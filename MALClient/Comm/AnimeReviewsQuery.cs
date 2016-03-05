using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;

namespace MALClient.Comm
{
    class AnimeReviewsQuery : Query
    {
        private int _animeId;

        public AnimeReviewsQuery(int id)
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/anime/{id}/whatever/reviews"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _animeId = id;
        }

        public async Task<List<AnimeReviewData>> GetAnimeReviews(bool force = false)
        {
            List<AnimeReviewData> output = force
                ? new List<AnimeReviewData>()
                : await DataCache.RetrieveReviewData(_animeId) ?? new List<AnimeReviewData>();
            if (output.Count != 0) return output;

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;

            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            List<HtmlNode> reviewNodes = doc.DocumentNode.Descendants("div")
                .Where(
                        node =>
                            node.Attributes.Contains("class") &&
                            node.Attributes["class"].Value ==
                            "borderDark pt4 pb8 pl4 pr4 mb8").Take(4).ToList();

            foreach (var reviewNode in reviewNodes)
            {
                AnimeReviewData current = new AnimeReviewData();
                //Details
                var detailsNode = reviewNode.Descendants("div").First();
                var pictureNode = detailsNode.Descendants("div").Where(node =>
                    node.Attributes.Contains("class") &&
                    node.Attributes["class"].Value =="picSurround")
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
                var rightTableNodeDivs = detailsNode.Descendants("td").Skip(2).First().Descendants("div").ToList();
                current.Date = rightTableNodeDivs[0].InnerText;
                current.EpisodesSeen = rightTableNodeDivs[1].InnerText;
                current.OverallRating = rightTableNodeDivs[2].InnerText;
                //Review Content
                var reviewNodeContent = reviewNode.Descendants("div").First(node =>
                    node.Attributes.Contains("class") &&
                    node.Attributes["class"].Value == "spaceit textReadability");
                reviewNodeContent.ChildNodes.Remove(1);
                current.Review = WebUtility.HtmlDecode(reviewNodeContent.InnerText.Trim());

                output.Add(current);
            }

            DataCache.SaveAnimeReviews(_animeId,output);

            return output;
        } 
    }
}
