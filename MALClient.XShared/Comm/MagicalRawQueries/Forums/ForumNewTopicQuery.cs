using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;

namespace MALClient.XShared.Comm.MagicalRawQueries.Forums
{
    public static class ForumNewTopicQuery
    {
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
        private static async Task<bool> CreateNewTopic(string title,string message,string endpoint,string question = null,List<string> answers = null )
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
            catch (Exception e)
            {
                return false;
            }
            
        }
    }
}
