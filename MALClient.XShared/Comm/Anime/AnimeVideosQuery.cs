using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeVideosQuery : Query
    {
        private readonly int _id;

        public AnimeVideosQuery(int id)
        {
            _id = id;
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString(
                        $"https://myanimelist.net/anime/{id}/whatever/video"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<AnimeVideoData>> GetVideos(bool force)
        {
            var output = force
                ? new List<AnimeVideoData>()
                : await DataCache.RetrieveData<List<AnimeVideoData>>($"videos_{_id}", "AnimeDetails", 7) ??
                  new List<AnimeVideoData>();

            if (output.Any())
                return output;

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return output;

            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            try
            {
                foreach (
                    var video in
                    doc.FirstOfDescendantsWithClass("div", "video-block promotional-video mt16")
                        .WhereOfDescendantsWithClass("div", "video-list-outer po-r pv"))
                {
                    try
                    {
                        var current = new AnimeVideoData();
                        var img = video.Descendants("img").First();
                        current.Thumb = img.Attributes["data-src"].Value;
                        if (current.Thumb.Contains("banned"))
                            continue;
                        var href = video.Descendants("a").First().Attributes["href"].Value;
                        var pos = href.IndexOf('?');
                        href = href.Substring(0, pos);
                        current.YtLink = $"https://www.youtube.com/watch?v={href.Split('/').Last()}";

                        current.Name = WebUtility.HtmlDecode(img.Attributes["data-title"].Value);

                        output.Add(current);
                    }
                    catch (Exception)
                    {
                        //html
                    }

                }
            }
            catch (Exception)
            {
                //no videos
            }

            DataCache.SaveData(output, $"videos_{_id}", "AnimeDetails");

            return output;

        }
    }
}
