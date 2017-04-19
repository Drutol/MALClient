using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

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
                    var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                    var resp = await client.GetAsync($"/forum/search?q={query}&u=&uloc=1&loc={scope}");

                    var doc = new HtmlDocument();
                    doc.LoadHtml(await resp.Content.ReadAsStringAsync());
                    var s = await resp.Content.ReadAsStringAsync();
                    var topicContainer =
                        doc.DocumentNode.Descendants("table")
                            .First(
                                node => node.Attributes.Contains("id") && node.Attributes["id"].Value == "forumTopics");
                    foreach (var topicRow in topicContainer.Descendants("tr").Skip(1)) //skip forum table header
                    {
                        try
                        {
                            output.ForumTopicEntries.Add(ParseTopicRow(topicRow));
                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                }
                catch (Exception e)
                {
                    //
                }
            return output;

        }

        public static async Task<ForumBoardContent> GetRecentTopics(string user)
        {
            var output = new ForumBoardContent {Pages = 0};
                try
                {
                    var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                    var resp = await client.GetAsync($"/forum/search?u={user}&q=&uloc=1&loc=-1");

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
                            output.ForumTopicEntries.Add(ParseTopicRow(topicRow));
                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }
            return output;

        }

        public static async Task<ForumBoardContent> GetWatchedTopics()
        {
            var output = new ForumBoardContent {Pages = 0};
                try
                {
                    var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                    var resp = await client.GetAsync("/forum/?action=viewstarred");

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
                            output.ForumTopicEntries.Add(ParseTopicRow(topicRow,1));
                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }
            return output;

        }



        private static ForumTopicEntry ParseTopicRow(HtmlNode topicRow,int tdOffset = 0)
        {
            var current = new ForumTopicEntry();
            var tds = topicRow.Descendants("td").ToList();

            current.Type = tds[1].ChildNodes[0].InnerText;

            var titleLinks = tds[1].Descendants("a").ToList();
            var titleLink = string.IsNullOrWhiteSpace(titleLinks[0].InnerText) ? titleLinks[1] : titleLinks[0];

            current.Title = WebUtility.HtmlDecode(titleLink.InnerText);
            var link = titleLink.Attributes["href"].Value;
            if (link.Contains("&amp;goto="))
            {
                var pos = link.IndexOf("&amp;goto=");
                link = link.Substring(0, pos);
            }

            current.Id = link.Split('=').Last();


            var spans = tds[1].Descendants("span").Where(node => !string.IsNullOrEmpty(node.InnerText)).ToList();
            try
            {
                var offset = 0;
                if (spans.Count == 3)
                    offset = 1;
                current.Op = WebUtility.HtmlDecode(spans[offset].InnerText.Trim());
                current.Created = WebUtility.HtmlDecode(spans[1+offset].InnerText.Trim());
            }
            catch (Exception e)
            {
                //html and specific indexes...
            }
            try
            {
                current.Replies = tds[2 + tdOffset].InnerText.Trim();

                current.LastPoster = WebUtility.HtmlDecode(tds[3 + tdOffset].Descendants("a").First().InnerText.Trim());
                current.LastPostDate = tds[3 + tdOffset].ChildNodes.Last().InnerText.Trim();
            }
            catch (Exception )
            {
                //html and specific indexes...
            }


            return current;
        }
    }

}
