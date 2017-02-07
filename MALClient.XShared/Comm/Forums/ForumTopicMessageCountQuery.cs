using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Forums
{
    public class ForumTopicMessageCountQuery : Query
    {
        public ForumTopicMessageCountQuery(string id)
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/forum/?topicid={id}&goto=lastpost"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<int?> GetMessageCount()
        {
            var raw = await GetRequestResponse();
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            try
            {
                var row = doc.WhereOfDescendantsWithClass("div", "forum_border_around").Last();
                var divs = row.ChildNodes.Where(node => node.Name == "div").ToList();
                var headerDivs = divs[0].Descendants("div").ToList();
                return int.Parse(WebUtility.HtmlDecode(headerDivs[1].InnerText.Trim()).Substring(1));
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
