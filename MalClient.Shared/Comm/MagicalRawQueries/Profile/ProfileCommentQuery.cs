using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MalClient.Shared.Comm.MagicalRawQueries.Profile
{
    public static class ProfileCommentQuery
    {
        public static async Task<bool> SendComment(string username,string userId,string comment)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var contentPairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("profileMemId", userId),
                new KeyValuePair<string, string>("commentText", comment),
                new KeyValuePair<string, string>("profileUsername", username),
                new KeyValuePair<string, string>("csrf_token", client.Token),
                new KeyValuePair<string, string>("commentSubmit", "Submit Comment")
            };
                var content = new FormUrlEncodedContent(contentPairs);

                var response =
                    await client.PostAsync("/addcomment.php", content);

                return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.SeeOther;
            }
            catch (WebException)
            {
                MalHttpContextProvider.ErrorMessage("Messages");
                return false;
            }
        }

        public static async Task<bool> SendCommentReply(string userId,string comment)
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var contentPairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("profileMemId", userId),
                new KeyValuePair<string, string>("commentText", comment),
                new KeyValuePair<string, string>("area", "2"),
                new KeyValuePair<string, string>("csrf_token", client.Token),
                new KeyValuePair<string, string>("commentSubmit", "1")
            };
                var content = new FormUrlEncodedContent(contentPairs);

                var response =
                    await client.PostAsync("/addcomment.php", content);

                //edit comment function - not sure what it does
                // /includes/ajax.inc.php?t=73
                //id = 31985758 & csrf_token = dfsdfsd
                //client.PostAsync("/includes/ajax.inc.php?t=73")

                return response.IsSuccessStatusCode;
            }
            catch (WebException)
            {
                MalHttpContextProvider.ErrorMessage("Messages");
                return false;
            }
        }
    }
}
