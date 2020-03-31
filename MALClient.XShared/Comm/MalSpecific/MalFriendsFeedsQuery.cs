using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;

using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.MalSpecific
{
    public class MalFriendsFeedsQuery
    {
        private readonly IEnumerable<MalUser> _friends;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(2);

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
                var tasks = _friends.Select(GetUserFeed).ToList();
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
                await _semaphoreSlim.WaitAsync();
                var resp = await new FeedQuery(user.Name).GetRequestResponse();
                var output = new List<UserFeedEntryModel>();

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resp);
                var nodes = xmlDoc.GetElementsByTagName("item");
                foreach (XmlNode node in nodes)
                {
                    var current = new UserFeedEntryModel();
                    current.Date = DateTime.Parse(node.ChildNodes[4].InnerText);
                    if (DateTime.UtcNow.Subtract(current.Date).TotalDays > Settings.FeedsMaxEntryAge)
                        continue;

                    current.User = user;
                    current.Header = node.ChildNodes[0].InnerText;
                    current.Link = node.ChildNodes[1].InnerText;
                    current.Description = node.ChildNodes[3].InnerText;
                    var linkParts = current.Link.Substring(10).Split('/');
                    current.Id = int.Parse(linkParts[2]);
                    var pos = current.Header.LastIndexOf('-');
                    current.Title = current.Header.Substring(0, pos).Trim();
                    output.Add(current);
                }

                await Task.Delay(100);
                return output;
            }
            catch (Exception e) 
            {
                return null;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
