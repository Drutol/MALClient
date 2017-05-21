using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.MalSpecific
{
    public class MalFriendsFeedsQuery
    {
        private readonly IEnumerable<MalUser> _friends;

        class FeedQuery : Query
        {
            public FeedQuery(string username)
            {
                Request =
                    WebRequest.Create(
                        Uri.EscapeUriString(
                            $"https://myanimelist.net/rss.php?type=rw&u={username}"));
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.Method = "GET";
            }
        }

        public MalFriendsFeedsQuery(IEnumerable<MalUser> friends)
        {
            _friends = friends;
        }

        public async Task<List<UserFeedEntryModel>> GetFeeds()
        {
            try
            {
                var tasks = _friends.Select(user => Task.Run(() => GetUserFeed(user))).ToList();
                await Task.WhenAll(tasks);
                return tasks.Where(task => task.Result != null).SelectMany(task => task.Result).ToList();
            }
            catch (Exception)
            {
                return new List<UserFeedEntryModel>();
            }

        }

        private async Task<List<UserFeedEntryModel>> GetUserFeed(MalUser user)
        {
            try
            {
                var resp = await new FeedQuery(user.Name).GetRequestResponse();
                var output = new List<UserFeedEntryModel>();

                var xmlDoc = XElement.Parse(resp);
                var nodes = xmlDoc.Element("channel").Elements("item").Take(Settings.FeedsMaxEntries);
                foreach (var node in nodes)
                {
                    var current = new UserFeedEntryModel();
                    current.Date = DateTime.Parse(node.Element("pubDate").Value);
                    if (DateTime.UtcNow.Subtract(current.Date).TotalDays > Settings.FeedsMaxEntryAge)
                        continue;

                    current.User = user;
                    current.Header = node.Element("title").Value;
                    current.Link = node.Element("link").Value;
                    current.Description = node.Element("description").Value;
                    var linkParts = current.Link.Substring(10).Split('/');
                    current.Id = int.Parse(linkParts[2]);
                    var pos = current.Header.LastIndexOf('-');
                    current.Title = current.Header.Substring(0, pos).Trim();
                    output.Add(current);
                }
                return output;
            }
            catch (Exception)
            {
                return null;
            }           
        }
    }
}
