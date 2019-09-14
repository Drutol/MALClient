using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm.MagicalRawQueries.Profile
{
    public static class MalFriendsQueries
    {
        public static async Task<List<MalFriendRequest>> GetFriendRequests()
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var response = await client.GetAsync("https://myanimelist.net/myfriends.php?go=pending");

                var doc = new HtmlDocument();
                doc.Load(await response.Content.ReadAsStreamAsync());


                var table = doc.DocumentNode.Descendants("table").First();
                var rows = table.ChildNodes.Where(node => node.Name == "tr").ToList();

                var output = new List<MalFriendRequest>();

                for (int i = 0; i < rows.Count; i+=2)
                {
                    var current = new MalFriendRequest();
                    var tds = rows[i + 1].Descendants("td").ToList();
                    current.User.ImgUrl = tds[0].Descendants("img").First().Attributes["src"].Value.Replace("/thumbs", "").Replace("_thumb", "");
                    current.User.Name = WebUtility.HtmlDecode(tds[1].Descendants("strong").First().InnerText.Trim());
                    current.Message = WebUtility.HtmlDecode(tds[1].FirstOrDefaultOfDescendantsWithClass("div", "spaceit")?.InnerText?.Substring(17)?.Trim());
                    current.Id = rows[i].Descendants("td").First().Id.Substring(3);
                    output.Add(current);
                }

                return output;
            }
            catch (Exception)
            {
                return new List<MalFriendRequest>();
            }
        }


        public static async Task<bool> SendFriendRequest(string id, string message)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                if (message == null)
                    message = string.Empty;

                var contentPairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("attachedMsg", message),
                    new KeyValuePair<string, string>("confirm", "Yes send friend request"),
                    new KeyValuePair<string, string>("go", "add"),
                    new KeyValuePair<string, string>("confirm", "1"),
                    new KeyValuePair<string, string>("csrf_token", client.Token)
                };

                
                var content = new FormUrlEncodedContent(contentPairs);

                var response =
                    await client.PostAsync($"https://myanimelist.net/myfriends.php?go=add&id={id}", content);

                return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.SeeOther;
            }
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Friends");
                return false;
            }
        }

        public static async Task<bool> RespondToFriendRequest(string id, bool accept)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var contentPairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("id", id),
                    new KeyValuePair<string, string>("csrf_token", client.Token)
                };

                
                var content = new FormUrlEncodedContent(contentPairs);

                var response =
                    await client.PostAsync(accept ? "https://myanimelist.net/myfriends.php?go=confirm" : "https://myanimelist.net/myfriends.php?go=deny", content);

                return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.SeeOther;
            }
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Friends");
                return false;
            }
        }

        public static async Task<bool> RemoveFriend(string id)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var response =
                    await client.GetAsync($"https://myanimelist.net/myfriends.php?go=remove&id={id}");

                return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.SeeOther;
            }
            catch (Exception)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Friends");
                return false;
            }
        }
    }
}
