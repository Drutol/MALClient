using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MalClient.Shared.Comm.MagicalRawQueries;
using MalClient.Shared.Utils;

namespace MalClient.Shared.Comm.Forums
{
    public class ForumTopicContentQuery
    {
        public async Task<string> GetContent(string topicID)
        {
            topicID = "1499207";
            var client = await MalHttpContextProvider.GetHttpContextAsync();
            var response = await client.GetAsync($"/forum/?topicid={topicID}");
            var raw = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            //var htmlData =
            //    doc.DocumentNode.Descendants("div")
            //        .First(node => node.Attributes.Contains("id") && node.Attributes["id"].Value == "content");
            //return htmlData.OuterHtml;
            return doc.FirstOfDescendantsWithClass("div","wrapper").OuterHtml;
        }
    }
}
