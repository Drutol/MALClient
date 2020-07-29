using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm.MagicalRawQueries.Messages
{
    public class MalMessagesQuery
    {
        public async Task<List<MalMessageModel>> GetMessages(int page = 1)
        {
            var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();
            string path = $"https://myanimelist.net/mymessages.php?go=&show={page*20 - 20}";
            var res = await client.GetAsync(path);
            var body = await res.Content.ReadAsStringAsync();


            var output = new List<MalMessageModel>();
            if (body.Contains("You have 0 messages"))
                return output;

            var doc = new HtmlDocument();
            doc.LoadHtml(body);
            output.AddRange(
                doc.WhereOfDescendantsWithClass("div", "message unread spot1 clearfix")
                    .Select(msgNode => ParseInboxHtmlToMalMessage(msgNode, false))
                    .Where(model => model != null));
            output.AddRange(
                doc.WhereOfDescendantsWithClass("div", "message read spot1 clearfix")
                    .Select(msgNode => ParseInboxHtmlToMalMessage(msgNode, true))
                    .Where(model => model != null));


            return output;
        }

        public async Task<List<MalMessageModel>> GetSentMessages(int page = 1)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();
                string path = $"https://myanimelist.net/mymessages.php?go=sent";
                var res = await client.GetAsync(path);
                var body = await res.Content.ReadAsStringAsync();


                var output = new List<MalMessageModel>();

                var doc = new HtmlDocument();
                doc.LoadHtml(body);
                output.AddRange(
                    doc.WhereOfDescendantsWithClass("div", "message read spot2 clearfix")
                        .Select(ParseOutboxHtmlToMalMessage));


                return output;
            }
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Messages");
            }
            return new List<MalMessageModel>();
        }

        /// <summary>
        ///     When we send new message we don't really know its id so we have to pull it.
        ///     Once we have that we will be able to pull thread id.
        /// </summary>
        public async Task<string> GetFirstSentMessageId()
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();
                var res = await client.GetAsync("https://myanimelist.net/mymessages.php?go=sent");
                var body = await res.Content.ReadAsStringAsync();

                try
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(body);
                    //?go=read&id=8473147&f=1
                    var id =
                        doc.FirstOfDescendantsWithClass("div", "message read spot2 clearfix")
                            .Descendants("a")
                            .Skip(1)
                            .First()
                            .Attributes["href"].Value.Split('=')[2];
                    return id.Substring(0, id.Length - 2);
                }
                catch (Exception)
                {
                    return "0";
                }
            }
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Messages");
            }
            return "0";
        }

        private MalMessageModel ParseOutboxHtmlToMalMessage(HtmlNode msgNode)
        {
            try
            {
                var current = new MalMessageModel();
                current.Target = msgNode.FirstOfDescendantsWithClass("div", "mym mym_user").InnerText.Trim();
                current.Sender = Credentials.UserName;
                var contentNode = msgNode.FirstOfDescendantsWithClass("div", "mym mym_subject");
                current.Subject =
                    WebUtility.HtmlDecode(contentNode.Descendants("a").First().ChildNodes[0].InnerText.Trim().Trim('-'));
                current.Content = WebUtility.HtmlDecode(contentNode.Descendants("span").First().InnerText.Trim());
                current.Date = msgNode.FirstOfDescendantsWithClass("span", "mym_date").InnerText.Trim();
                current.IsMine = true;
                return current;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private MalMessageModel ParseInboxHtmlToMalMessage(HtmlNode msgNode, bool read)
        {
            try
            {
                var current = new MalMessageModel();
                current.Sender = msgNode.FirstOfDescendantsWithClass("div", "mym mym_user").InnerText.Trim();
                current.Target = Credentials.UserName;
                var contentNode = msgNode.FirstOfDescendantsWithClass("div", "mym mym_subject");
                current.Subject =
                    WebUtility.HtmlDecode(contentNode.Descendants("a").First().ChildNodes[0].InnerText.Trim().Trim('-'));
                current.Content = WebUtility.HtmlDecode(contentNode.Descendants("span").First().InnerText.Trim());
                current.Id =
                    contentNode.FirstOfDescendantsWithClass("a", "subject-link").Attributes["href"].Value.Split('=')
                        .Last();
                current.Date = msgNode.FirstOfDescendantsWithClass("span", "mym_date").InnerText.Trim();
                current.IsRead = read;

                var ids =
                    msgNode.FirstOfDescendantsWithClass("span", "mym_actions").Descendants("a").First().Attributes["href"]
                        .Value.Split('=');
                current.ThreadId = ids[3].Substring(0, ids[3].IndexOf('&'));
                current.ReplyId = ids[2].Substring(0, ids[3].IndexOf('&'));
                return current;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}