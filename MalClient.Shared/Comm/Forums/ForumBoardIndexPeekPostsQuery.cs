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
    public class ForumBoardIndexPeekPostsQuery : Query
    {
        public ForumBoardIndexPeekPostsQuery()
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString("http://myanimelist.net/forum/"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<List<ForumBoardEntryPeekPost>>> GetPeekPosts()
        {
            var output = new List<List<ForumBoardEntryPeekPost>>();
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
                            current.Title = WebUtility.HtmlDecode(post.FirstOfDescendantsWithClass("a", "topic-title-link").InnerText);
                            var img = post.Descendants("img").First();
                            current.User.ImgUrl = img.Attributes["src"].Value;
                            current.User.Name = img.Attributes["alt"].Value;
                            list.Add(current);
                            if (list.Count == 2) //assume we have 2 for each board
                            {
                                output.Add(list);
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

            return output;
        }
    }
}
