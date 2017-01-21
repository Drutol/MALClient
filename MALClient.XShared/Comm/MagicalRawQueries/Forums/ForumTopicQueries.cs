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
using Newtonsoft.Json;

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


        /// <summary>
        /// Creates message in a topic
        /// </summary>
        /// <param name="id">Id of the topic</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<bool> CreateMessage(string id, string message)
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

                //var response =
                //    await client.PostAsync(
                //        $"/forum/?action=message&topic_id={id}", requestContent);                

                var response =
                    await client.PostAsync(
                        $"/forum/?action=message&topic_id=1586126", requestContent);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region Edit
        class MessageHtmlResponse
        {
            public string message_html { get; set; }
        }

        /// <summary>
        /// Takes bbcode and updates message.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <param name="topicId"></param>
        /// <returns>Returns html content of the message</returns>
        public static async Task<string> EditMessage(string id, string message, string topicId)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("msg_id", id),
                    new KeyValuePair<string, string>("msg", message),
                    new KeyValuePair<string, string>("topic_id", topicId),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                };

                var requestContent = new FormUrlEncodedContent(data);

                var response =
                    await client.PostAsync(
                        $"/includes/ajax.inc.php?t=86", requestContent);

                if (!response.IsSuccessStatusCode)
                    return null;


                return
                    JsonConvert.DeserializeObject<MessageHtmlResponse>(await response.Content.ReadAsStringAsync())
                        .message_html;
            }
            catch (Exception)
            {
                return null;
            }

        }

        class MessageBbcodeResponse
        {
            public string message { get; set; }
        }

        /// <summary>
        /// Turns message into editable bbcode
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<string> GetMessageBbcode(string id)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("msg_id", id),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                };

                var requestContent = new FormUrlEncodedContent(data);

                var response =
                    await client.PostAsync(
                        $"/includes/ajax.inc.php?t=85", requestContent);

                if (!response.IsSuccessStatusCode)
                    return null;

                return
                    JsonConvert.DeserializeObject<MessageBbcodeResponse>(await response.Content.ReadAsStringAsync())
                        .message;
            }
            catch (Exception)
            {
                return null;
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

        #region GetTopicData

        private static readonly Dictionary<string, Dictionary<int, ForumTopicData>> CachedMessagesDictionary =
            new Dictionary<string, Dictionary<int, ForumTopicData >>();

        /// <summary>
        /// Returns tons of data about topic.
        /// </summary>
        /// <param name="topicId">Self explanatory</param>
        /// <param name="page">Page starting from 1</param>
        /// <param name="lastpage">Override page to last page?</param>
        /// <param name="force">Forces cache ignore</param>
        /// <returns></returns>
        public static async Task<ForumTopicData> GetTopicData(string topicId,int page,bool lastpage = false,int? messageId = null,bool force = false)
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
                    var pager = doc.FirstOfDescendantsWithClass("div", "fl-r pb4");
                    var lastLinkNode = pager.Descendants("a").LastOrDefault(node => node.InnerText.Contains("Last"));
                    if (lastLinkNode != null)
                    {
                        output.AllPages = int.Parse(lastLinkNode
                                                .Attributes["href"]
                                                .Value.Split('=').Last()) / 50;
                    }
                    else
                    {
                        var nodes = pager.ChildNodes.Where(node => !string.IsNullOrWhiteSpace(node.InnerText) && node.InnerText != "&raquo;");
                        output.AllPages = int.Parse(nodes.Last().InnerText.Replace("[","").Replace("]","").Trim());
                    }                        
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
                                    .InnerText.Replace("[", "").Replace("]", "").Trim());
                    }
                    catch (Exception e)
                    {
                        output.CurrentPage = output.AllPages;
                    }
                else
                {
                    output.CurrentPage = output.AllPages;
                }

                output.Title = WebUtility.HtmlDecode(doc.FirstOfDescendantsWithClass("h1", "forum_locheader").InnerText.Trim());

                foreach (var bradcrumb in doc.FirstOfDescendantsWithClass("div", "breadcrumb").ChildNodes.Where(node => node.Name == "div"))
                {
                    output.Breadcrumbs.Add(new ForumBreadcrumb
                    {
                        Name = WebUtility.HtmlDecode(bradcrumb.InnerText.Trim()),
                        Link = bradcrumb.Descendants("a").First().Attributes["href"].Value
                    });
                }

                foreach (var row in doc.WhereOfDescendantsWithClass("div", "forum_border_around"))
                {
                    var current = new ForumMessageEntry {TopicId = topicId};

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
                        else if (tds[0].ChildNodes[1].ChildNodes.Count == 11)
                        {
                            poster.Status = tds[0].ChildNodes[1].ChildNodes[6].InnerText.Trim();
                            poster.Joined = tds[0].ChildNodes[1].ChildNodes[8].InnerText.Trim();
                            poster.Posts = tds[0].ChildNodes[1].ChildNodes[10].InnerText.Trim();
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

                    var actions = row.FirstOfDescendantsWithClass("div", "postActions");
                    current.CanEdit = actions.ChildNodes[0].ChildNodes.Any(node => node.InnerText?.Contains("Edit") ?? false);
                    current.CanDelete = actions.ChildNodes[0].ChildNodes.Any(node => node.InnerText?.Contains("Delete") ?? false);

                    output.Messages.Add(current);
                }



                if (!CachedMessagesDictionary.ContainsKey(topicId))
                    CachedMessagesDictionary.Add(topicId, new Dictionary<int, ForumTopicData>
                    {
                        {page, output}
                    });
                else
                {
                    if (CachedMessagesDictionary[topicId].ContainsKey(page))
                        CachedMessagesDictionary[topicId][page] = output;
                    else
                        CachedMessagesDictionary[topicId].Add(page, output);
                }
                    
                
                return output;
            }
            catch (Exception)
            {
                return new ForumTopicData();
            }


        }

        #endregion

        #region Watch/UnWatch

        /// <summary>
        /// Change topic watching status.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns null when failed, true when topic is being watched and false when it's not.</returns>
        public static async Task<bool?> ToggleTopicWatching(string id)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("topic_id", id),
                    new KeyValuePair<string, string>("timestamp",
                        $"{DateTime.Now.ToLocalTime():ddd MMM dd yyyy hh:mm:ss \"GMT\"K} ({TimeZoneInfo.Local.StandardName})"),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                };

                var requestContent = new FormUrlEncodedContent(data);

                var response =
                    await client.PostAsync(
                        "/includes/ajax.inc.php?t=69", requestContent);

                if (!response.IsSuccessStatusCode)
                    return null;

                return (await response.Content.ReadAsStringAsync()) == "Watching";
            }
            catch (Exception)
            {
                return null;
            }

        }
        #endregion

        #region Quoute

        /// <summary>
        /// Gets quoute string
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Quutable bbcode</returns>
        public static async Task<string> GetQuoute(string id)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("msgid", id),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                };

                var requestContent = new FormUrlEncodedContent(data);

                var response =
                    await client.PostAsync(
                        "/includes/quotetext.php", requestContent);

                if (!response.IsSuccessStatusCode)
                    return null;

                return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
            }
            catch (Exception)
            {
                return null;
            }

        }

        #endregion
        // 
    }
}
