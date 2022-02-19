using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JikanDotNet;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Profile
{
    public class FriendsQuery : Query
    {
        private readonly string _userName;

        public FriendsQuery(string userName)
        {
            _userName = userName;
        }

        public async Task<List<MalFriend>> GetFriends()
        {
            var output = new List<MalFriend>();

            try
            {
                var jikan = new Jikan();
                var result = await jikan.GetUserFriendsAsync(_userName);

                foreach (var friend in result.Data)
                {
                    output.Add(new MalFriend
                    {
                        Id = friend.User.Url,
                        User = new MalUser
                        {
                            ImgUrl = friend.User.Images.JPG.ImageUrl,
                            Name = friend.User.Username,
                        },
                        FriendsSince = friend.FriendsSince?.ToString("yyyy-MM-dd") ?? "N/A",
                        LastOnline = friend.LastOnline?.ToString("yyyy-MM-dd") ?? "N/A",
                    });
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
