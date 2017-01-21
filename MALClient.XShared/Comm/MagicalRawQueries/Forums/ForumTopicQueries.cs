using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.MagicalRawQueries.Forums
{
    public static class ForumTopicQueries
    {
        #region Create
        /// <summary>
        /// Creates forum or club topic.
        /// </summary>
        /// <param name="title">Topic title</param>
        /// <param name="message">OP message content</param>
        /// <param name="type">Whether standard forum or clubs</param>
        /// <param name="id">Id of board or club</param>
        /// <param name="question"></param>
        /// <param name="answers"></param>
        /// <returns></returns>
        public static async Task<bool> CreateNewTopic(string title, string message, TopicType type, int id,
            string question = null, List<string> answers = null)
        {
            return await CreateNewTopic(title, message, type == TopicType.Anime
                ? $"/forum/?action=post&anime_id={id}"
                : $"/forum/?action=post&manga_id={id}", question, answers);
        }

        public static async Task<bool> CreateNewTopic(string title, string message, ForumType type, int id,
            string question = null, List<string> answers = null)
        {
            return await CreateNewTopic(title, message, type == ForumType.Normal
                ? $"/forum/?action=post&boardid={id}"
                : $"/forum/?action=post&club_id={id}", question, answers);
        }
        private static async Task<bool> CreateNewTopic(string title, string message, string endpoint, string question = null, List<string> answers = null)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("topic_title", title),
                    new KeyValuePair<string, string>("msg_text", message),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                    new KeyValuePair<string, string>("submit", "Submit")
                };

                if (!string.IsNullOrEmpty(question) && answers != null)
                {
                    if (answers.Count > 0)
                    {
                        data.Add(new KeyValuePair<string, string>("pollQuestion", question));
                        data.AddRange(answers.Select(answer => new KeyValuePair<string, string>("pollOption[]", answer)));
                    }
                }

                var requestContent = new FormUrlEncodedContent(data);

                //var response = await client.PostAsync(endpoint, requestContent);
                var response =
                    await client.PostAsync(
                        "/forum/?action=post&club_id=73089", requestContent);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region Edit

        public static async Task<bool> EditComment(string id, string message)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("msg_text", message),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                    new KeyValuePair<string, string>("submit", "Submit")
                };

                var requestContent = new FormUrlEncodedContent(data);

                var response =
                    await client.PostAsync(
                        $"/forum/?action=message&msgid={id}", requestContent);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion

        #region DeleteComment

        public static async Task<bool> DeleteComment(string id)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("msgId", id),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                };

                var requestContent = new FormUrlEncodedContent(data);

                var response =
                    await client.PostAsync(
                        "/includes/ajax.inc.php?t=84", requestContent);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion

        #region GetTopicMessages

        private static readonly Dictionary<string, Dictionary<int, ForumTopicData>> CachedMessagesDictionary =
            new Dictionary<string, Dictionary<int, ForumTopicData >>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="page">Page starting from 1</param>
        /// <param name="lastpage"></param>
        /// <param name="force">Forces cache ignore</param>
        /// <returns></returns>
        public static async Task<ForumTopicData> GetTopicMessages(string topicId,int page,bool lastpage = false,int? messageId = null,bool force = false)
        {
            if(!force && !lastpage && messageId == null && CachedMessagesDictionary.ContainsKey(topicId))
                if (CachedMessagesDictionary[topicId].ContainsKey(page))
                    return CachedMessagesDictionary[topicId][page];

            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var response =
                    await client.GetAsync(
                        lastpage
                            ? $"/forum/?topicid={topicId}&goto=lastpost"
                            : messageId != null
                                ? $"/forum/message/{messageId}?goto=topic"
                                : $"/forum/?topicid={topicId}&show={(page - 1) * 50}");

                var doc = new HtmlDocument();
                doc.Load(await response.Content.ReadAsStreamAsync());

                var foundMembers = new Dictionary<string,MalForumUser>();
                var output = new ForumTopicData();

                try
                {
                    output.AllPages = int.Parse(
                                            doc.FirstOfDescendantsWithClass("div", "fl-r pb4")
                                                .Descendants("a")
                                                .Last()
                                                .Attributes["href"]
                                                .Value.Split('=').Last()) / 50;
                }
                catch (Exception)
                {
                    output.AllPages = 1;
                }
                if(!lastpage)
                    try
                    {
                        var pageNodes = doc.FirstOfDescendantsWithClass("div", "fl-r pb4").ChildNodes.Where(node => node.Name != "a");
                        output.CurrentPage =
                            int.Parse(
                                pageNodes.First(node => Regex.IsMatch(node.InnerText.Trim(), @"\[.*\]"))
                                    .InnerText.Trim());
                    }
                    catch (Exception e)
                    {
                        output.CurrentPage = output.AllPages;
                    }

                foreach (var row in doc.WhereOfDescendantsWithClass("div", "forum_border_around"))
                {
                    var current = new ForumMessageEntry();

                    current.Id = row.Attributes["id"].Value.Replace("forumMsg", "");

                    var divs = row.ChildNodes.Where(node => node.Name == "div").ToList();
                    var headerDivs = divs[0].Descendants("div").ToList();

                    current.CreateDate = WebUtility.HtmlDecode(headerDivs[2].InnerText.Trim());
                    current.MessageNumber = WebUtility.HtmlDecode(headerDivs[1].InnerText.Trim());

                    var tds = row.Descendants("tr").First().ChildNodes.Where(node => node.Name == "td").ToList();

                    var posterName = WebUtility.HtmlDecode(tds[0].Descendants("strong").First().InnerText.Trim());

                    if (foundMembers.ContainsKey(posterName))
                    {
                        current.Poster = foundMembers[posterName];
                    }
                    else
                    {
                        var poster = new MalForumUser();

                        poster.MalUser.Name = posterName;
                        poster.Title =  WebUtility.HtmlDecode(tds[0].FirstOrDefaultOfDescendantsWithClass("div", "custom-forum-title")?.InnerText.Trim());
                        poster.MalUser.ImgUrl =
                            tds[0].Descendants("img")
                                .FirstOrDefault(
                                    node =>
                                        node.Attributes.Contains("src") &&
                                        node.Attributes["src"].Value.Contains("useravatars"))?.Attributes["src"].Value;
                        if (tds[0].ChildNodes[1].ChildNodes.Count == 10)
                        {
                            poster.Status = tds[0].ChildNodes[1].ChildNodes[5].InnerText.Trim();
                            poster.Joined = tds[0].ChildNodes[1].ChildNodes[7].InnerText.Trim();
                            poster.Posts = tds[0].ChildNodes[1].ChildNodes[9].InnerText.Trim();

                        }
                        else
                        {
                            poster.Status = tds[0].ChildNodes[1].ChildNodes[2].InnerText.Trim();
                            poster.Joined = tds[0].ChildNodes[1].ChildNodes[4].InnerText.Trim();
                            poster.Posts = tds[0].ChildNodes[1].ChildNodes[6].InnerText.Trim();
                        }

                        try
                        {
                            poster.SignatureHtml = tds[1].FirstOfDescendantsWithClass("div", "sig").OuterHtml;
                        }
                        catch (Exception)
                        {
                            //no signature
                        }            

                        foundMembers.Add(posterName,poster);
                        current.Poster = poster;
                    }

                    current.EditDate = WebUtility.HtmlDecode(tds[1].Descendants("em").FirstOrDefault()?.InnerText.Trim());

                    current.HtmlContent =
                        tds[1].Descendants("div")
                            .First(
                                node =>
                                    node.Attributes.Contains("id") && node.Attributes["id"].Value == $"message{current.Id}")
                            .OuterHtml;

                    output.Messages.Add(current);
                }
                if (!CachedMessagesDictionary.ContainsKey(topicId))
                    CachedMessagesDictionary.Add(topicId, new Dictionary<int, ForumTopicData>
                    {
                        {page, output}
                    });
                else
                    CachedMessagesDictionary[topicId].Add(page, output);
                
                return output;
            }
            catch (Exception)
            {
                return new ForumTopicData();
            }


        }

        #endregion
        // 
    }
}
