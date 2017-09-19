using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.MalSpecific
{
    public class PopularVideosQuery : Query
    {
        public PopularVideosQuery()
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString(
                        "https://myanimelist.net/watch/promotion/popular"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<AnimeVideoData>> GetVideos()
        {
            var output = new List<AnimeVideoData>();
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return output;

            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            try
            {
                foreach (var videoNode in doc.WhereOfDescendantsWithClass("div", "video-list-outer-vertical"))
                {
                    var current = new AnimeVideoData();

                    var link = videoNode.ChildNodes.First(node => node.Name == "a");
                    current.Thumb = link.Attributes["data-bg"].Value;
                    if(current.Thumb.Contains("banned"))
                        continue;
                    current.AnimeId = int.Parse(link.Attributes["data-anime-id"].Value);

                    var href = link.Attributes["href"].Value;
                    var pos = href.IndexOf('?');
                    href = href.Substring(0, pos);
                    current.YtLink = $"https://www.youtube.com/watch?v={href.Split('/').Last()}";

                    current.Name =
                        WebUtility.HtmlDecode(
                            videoNode.FirstOfDescendantsWithClass("div", "info-container").InnerText.Trim());

                    current.AnimeTitle = WebUtility.HtmlDecode(videoNode.Descendants("a").Last().InnerText.Trim());

                    output.Add(current);
                }
            }
            catch (Exception)
            {
                //
            }

            return output;
        }
    }
}
