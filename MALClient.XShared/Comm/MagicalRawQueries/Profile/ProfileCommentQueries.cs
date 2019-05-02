using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm.MagicalRawQueries.Profile
{
    public static class ProfileCommentQueries
    {
        public static async Task<bool> SendComment(string username,string userId,string comment)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

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
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Messages");
                return false;
            }
        }

        public static async Task<bool> SendCommentReply(string userId,string comment)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

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
                //com id - token
                //id = 31985758 & csrf_token = dfsdfsd
                //client.PostAsync("/includes/ajax.inc.php?t=73")

                return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.SeeOther;
            }
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Messages");
                return false;
            }
        }

        public static async Task<bool> DeleteComment(string id)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var contentPairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", id),
                new KeyValuePair<string, string>("csrf_token", client.Token),
            };
                var content = new FormUrlEncodedContent(contentPairs);

                var response =
                    await client.PostAsync("/includes/ajax.inc.php?t=78 ", content);

                return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.SeeOther;
            }
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Messages");
                return false;
            }
        }

        public static async Task<List<MalMessageModel>> GetComToComMessages(string path)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var response =
                    await client.GetAsync($"/comtocom.php?{path}");

                var doc = new HtmlDocument();
                doc.LoadHtml(await response.Content.ReadAsStringAsync());
                var output = new List<MalMessageModel>();

                foreach (var commentBox in doc.FirstOfDescendantsWithId("div","content").ChildNodes.Where(node => node.Name == "div").Skip(1))
                {
                    
                    try
                    {
                        var current = new MalMessageModel();

                        current.Content = WebUtility.HtmlDecode(commentBox.FirstOfDescendantsWithClass("div", "spaceit").InnerText).Trim();
                        current.Date = commentBox.Descendants("small").First().InnerText.Trim(new[] { '|', ' ' });
                        current.Sender = WebUtility.HtmlDecode(commentBox.Descendants("a").Skip(1).First().InnerText.Trim());
                        foreach (var img in commentBox.Descendants("img").Skip(1))
                        {
                            if (img.Attributes.Contains("src"))
                                current.Images.Add(img.Attributes["src"].Value);
                        }
                        output.Add(current);
                    }
                    catch (Exception)
                    {
                        //html
                    }
                    
                }
                output.Reverse();
                return output;

            }
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Messages");
                return new List<MalMessageModel>();
            }
        }
    }
}
