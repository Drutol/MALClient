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

        public async Task<int?> GetMessageCount(bool failIfLastMessageIsMine)
        {
            var raw = await GetRequestResponse();
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            try
            {
                var row = doc.WhereOfDescendantsWithClass("div", "forum_border_around").Last();
                if (failIfLastMessageIsMine)
                {
                    var tds = row.Descendants("tr").First().ChildNodes.Where(node => node.Name == "td").ToList();
                    var posterName = WebUtility.HtmlDecode(tds[0].Descendants("strong").First().InnerText.Trim());
                    if (Credentials.UserName.Equals(posterName, StringComparison.CurrentCultureIgnoreCase))
                        return null;
                }

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
