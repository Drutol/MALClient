using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Profile
{
    public class FriendsQuery : Query
    {
        public FriendsQuery(string userName)
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/profile/{userName}/friends"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<MalFriend>> GetFriends()
        {
            var output = new List<MalFriend>();

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            try
            {
                foreach (var htmlNode in doc.WhereOfDescendantsWithPartialClass("div", "friendHolder"))
                {
                    var current = new MalFriend();

                    try
                    {
                        current.User.ImgUrl = htmlNode.Descendants("img").First().Attributes["src"].Value
                            .Replace("/thumbs", "").Replace("_thumb", "");
                    }
                    catch (Exception)
                    {
                        //I've a feeling thast it'll change soon
                    }
                    current.User.Name = WebUtility.HtmlDecode(htmlNode.Descendants("strong").First().InnerText.Trim());
                    var divs = htmlNode.ChildNodes[1].ChildNodes.Where(node => node.Name == "div").ToList();
                    current.LastOnline = WebUtility.HtmlDecode(divs[2].InnerText.Trim());
                    if(divs.Count == 4)
                        current.FriendsSince = WebUtility.HtmlDecode(divs[3].InnerText.Trim());

                    output.Add(current);
                }
            }
            catch (Exception e)
            {
                //HTML as always
            }
  

            return output;
        }
    }
}
