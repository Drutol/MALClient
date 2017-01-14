using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeTitleQuery : Query
    {
        private readonly int _id;
        private readonly bool _anime;
        private static readonly Dictionary<int, string> AnimeTitles = new Dictionary<int, string>();
        private static readonly Dictionary<int, string> MangaTitles = new Dictionary<int, string>();

        private AnimeTitleQuery(int id, bool anime)
        {
            _id = id;
            _anime = anime;
            Request =
                WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/{(anime ? "anime" : "manga")}/{id}/stats"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        private async Task<string> GetTitle()
        {
            var raw = await GetRequestResponse();
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            return WebUtility.HtmlDecode(doc.DocumentNode.Descendants("h1").First().InnerText);
        }

        public static async Task<string> GetTitle(int id, bool anime)
        {
            if (anime)
            {
                if (AnimeTitles.ContainsKey(id))
                    return AnimeTitles[id];
            }
            else
            {
                if (MangaTitles.ContainsKey(id))
                    return MangaTitles[id];
            }

            var title = await new AnimeTitleQuery(id, anime).GetTitle();

            if (anime)
            {
                AnimeTitles[id] = title;
            }
            else
            {
                MangaTitles[id] = title;
            }

            return title;
        }
    }
}
