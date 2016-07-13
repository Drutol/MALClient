using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models;
using MALClient.Utils;

namespace MALClient.Comm.MagicalRawQueries.Messages
{
    public class MalMessageDetailsQuery
    {
        public async Task<MalMessageModel> GetMessageDetails(MalMessageModel msg)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();
                var response = await client.GetAsync($"/mymessages.php?go=read&id={msg.Id}");
                var raw = await response.Content.ReadAsStringAsync();

                var doc = new HtmlDocument();
                doc.LoadHtml(raw);

                var msgNode = doc.FirstOfDescendantsWithClass("td", "dialog-text");
                var msgContent = msgNode.ChildNodes.Skip(3)
                    .Where(node => node.NodeType == HtmlNodeType.Text)
                    .Aggregate("", (current, textNode) => current + textNode.InnerText);

                msg.Content = WebUtility.HtmlDecode(msgContent.Trim());
                var ids =
                    doc.FirstOfDescendantsWithClass("input", "inputButton btn-middle flat").Attributes["onclick"].Value
                        .Split('=');
                msg.ThreadId = ids[4].Substring(0, ids[3].IndexOf('&'));
                msg.ReplyId = ids[3].Substring(0, ids[3].IndexOf('&'));
                return msg;
            }
            catch (WebException)
            {
                MalHttpContextProvider.ErrorMessage("Messages");
            }

            return new MalMessageModel();
        }

        public async Task<List<MalMessageModel>> GetMessagesInThread(MalMessageModel msg)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();
                var response = await client.GetAsync($"/mymessages.php?go=read&id={msg.Id}&threadid={msg.ThreadId}");
                var raw = await response.Content.ReadAsStringAsync();

                var doc = new HtmlDocument();
                doc.LoadHtml(raw);

                var output = new List<MalMessageModel>();

                foreach (
                    var msgHistoryNode in
                        doc.FirstOfDescendantsWithClass("table", "pmessage-message-history").Descendants("tr"))
                {
                    var current = new MalMessageModel();
                    var tds = msgHistoryNode.Descendants("td").ToList();
                    current.Content = WebUtility.HtmlDecode(tds[2].InnerText.Trim());
                    if (string.IsNullOrEmpty(current.Content))
                        continue;


                    current.Subject = msg.Subject;
                    current.Date = tds[0].InnerText.Trim();
                    current.Sender = tds[1].InnerText.Trim();
                    output.Add(current);
                }


                return output;
            }
            catch (Exception)
            {
                MalHttpContextProvider.ErrorMessage("Messages");
            }

            return new List<MalMessageModel>();

        }
    }
}