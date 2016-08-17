using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MalClient.Shared.Models.Forums;
using MalClient.Shared.Utils;

namespace MalClient.Shared.Comm.Forums
{
    public class ForumBoardIndexContentQuery : Query
    {
        public ForumBoardIndexContentQuery()
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString("https://myanimelist.net/forum/"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<ForumIndexContent> GetPeekPosts()
        {
            var output = new ForumIndexContent();
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            var list = new List<ForumBoardEntryPeekPost>(2);
            int i = 0;
            foreach (var topics in doc.WhereOfDescendantsWithClass("ul", "topics"))
            {
                try
                {
                    foreach (var post in topics.Descendants("li"))
                    {
                        i++;
                        try
                        {
                            if (i == 5 || i == 6) //skip db midifiaction board
                                continue;
                            var current = new ForumBoardEntryPeekPost();
                            current.PostTime = WebUtility.HtmlDecode(post.FirstOfDescendantsWithClass("span", "date di-ib pt4 fs10 fn-grey4").InnerText.TrimEnd('»'));
                            var titleNode = post.FirstOfDescendantsWithClass("a", "topic-title-link");
                            current.Title = WebUtility.HtmlDecode(titleNode.InnerText);
                            current.Id = titleNode.Attributes["href"].Value.Split('=').Last();
                            var img = post.Descendants("img").First();
                            current.User.ImgUrl = img.Attributes["src"].Value;
                            current.User.Name = img.Attributes["alt"].Value;
                            list.Add(current);
                            if (list.Count == 2) //assume we have 2 for each board
                            {
                                output.ForumBoardEntryPeekPosts.Add(list);
                                list = new List<ForumBoardEntryPeekPost>();
                            }
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
            }

            try
            {
                int block = 0;
                var sideBlocks = doc.WhereOfDescendantsWithClass("div", "forum-side-block").Take(4);
                foreach (var sideBlock in sideBlocks)
                {
                    var postList = new List<ForumPostEntry>();
                    foreach (var post in sideBlock.Descendants("li"))
                    {
                        var current = new ForumPostEntry();

                        var titleNode = post.FirstOfDescendantsWithClass("a", "title");
                        current.Id = titleNode.Attributes["href"].Value.Split('=').Last();
                        current.Title = WebUtility.HtmlDecode(titleNode.InnerText.Trim());

                        current.ImgUrl = post.Descendants("img").First().Attributes["src"].Value;

                        var infoSpan = post.FirstOfDescendantsWithClass("span", "information di-ib fs10 fn-grey4");

                        current.Created = infoSpan.ChildNodes[0].InnerText.Trim();
                        current.Created = current.Created.Substring(0, current.Created.Length - 3); //remove "by"

                        current.Op = infoSpan.Descendants("a").First().InnerText.Trim();

                        postList.Add(current);
                    }
                    switch (block)
                    {
                        case 0:
                            output.PopularNewTopics = postList;
                            break;
                        case 1:
                            output.RecentPosts = postList;
                            break;
                        case 2:
                            output.AnimeSeriesDisc = postList;
                            break;
                        default:
                            output.MangaSeriesDisc = postList;
                            break;
                    }
                    block++;
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
