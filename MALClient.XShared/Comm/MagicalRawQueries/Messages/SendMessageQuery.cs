using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm.MagicalRawQueries.Messages
{
    public class SendMessageQuery
    {
        /// <summary>
        ///     Send new message.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SendMessage(string subject, string message, string targetUser)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var contentPairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("csrf_token", client.Token),
                new KeyValuePair<string, string>("sendmessage", "Send Message")
            };
                var content = new FormUrlEncodedContent(contentPairs);

                var response =
                    await client.PostAsync($"https://myanimelist.net/mymessages.php?go=send&toname={targetUser}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Messages");
                return false;
            }

        }

        /// <summary>
        ///     Send reply message.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SendMessage(string subject, string message, string targetUser, string threadId,
            string replyId)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var contentPairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("message", message),
                    new KeyValuePair<string, string>("csrf_token", client.Token),
                    new KeyValuePair<string, string>("sendmessage", "Send Message")
                };
                var content = new FormUrlEncodedContent(contentPairs);

                var response =
                    await
                        client.PostAsync(
                            $"https://myanimelist.net/mymessages.php?go=send&replyid={replyId}&threadid={threadId}&toname={targetUser}",
                            content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Messages");
                return false;
            }

        }
    }
}