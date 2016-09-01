using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils.Enums;

namespace MALClient.XShared.Comm.Forums
{
    public static class ForumSearchQuery
    {
        public static async Task<ForumBoardContent> GetSearchResults(string query, ForumBoards? searchScope)
        {
            var scope = searchScope == null ? -1 : (int) searchScope;
            var output = new ForumBoardContent {Pages = 0};
            if (query.Length > 2)
                try
                {
                    var client = await MalHttpContextProvider.GetHttpContextAsync();

                    var resp = await client.GetAsync($"/forum/?action=search&u=&uloc=1&loc={scope}&q={query}");

                    var doc = new HtmlDocument();
                    doc.LoadHtml(await resp.Content.ReadAsStringAsync());

                    var topicContainer =
                        doc.DocumentNode.Descendants("table")
                            .First(
                                node => node.Attributes.Contains("id") && node.Attributes["id"].Value == "forumTopics");
                    foreach (var topicRow in topicContainer.Descendants("tr").Skip(1)) //skip forum table header
                    {
                        try
                        {
                            output.ForumTopicEntries.Add(ForumBoardTopicsQuery.ParseHtmlToTopic(topicRow));
                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                }
                catch (WebException)
                {
                    //
                }
            return output;

        }
    }

}
