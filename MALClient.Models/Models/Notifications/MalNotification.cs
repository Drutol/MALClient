using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.Models.Models.ApiResponses;

namespace MALClient.Models.Models.Notifications
{
    public class MalNotification
    {
        private MalNotification()
        {
            
        }

        public MalNotificationsTypes Type { get; protected set; } = MalNotificationsTypes.Generic;
        public string Id { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public string Header { get; set; }
        public string LuanchArgs { get; set; }
        public bool IsSupported { get; set; }

        public static MalNotification CreateFromRawData(MalScrappedNotification notification)
        {
            var output = new MalNotification();
            switch (notification.typeIdentifier)
            {
                case "friend-request":
                    output.Type = MalNotificationsTypes.FriendRequest;
                    output.Header = "New friend request";
                    output.Content = $"{notification.friendName} sent you a friend request!";
                    output.IsSupported = false;
                    break;
                case "friend-request-accept":
                    output.Type = MalNotificationsTypes.FriendRequest;
                    output.Header = "Friend request accepted";
                    output.Content = $"{notification.friendName} accepted your friend request!";
                    output.IsSupported = false;
                    break;
                case "friend-request-deny":
                    output.Type = MalNotificationsTypes.FriendRequest;
                    output.Header = "Friend request denied";
                    output.Content = $"{notification.friendName} rejected yopur friend request.";
                    output.IsSupported = false;
                    break;
                case "profile-comment":
                    output.Type = MalNotificationsTypes.ProfileComment;
                    output.Header = "New profile comment";
                    output.Content = $"{notification.commentUserName} posted a comment on your profile\n{notification.text}.";
                    output.LuanchArgs = notification.url;
                    output.IsSupported = true;
                    break;
                case "forum-quote":
                    output.Type = MalNotificationsTypes.ForumQuoute;
                    break;
                case "blog-comment":
                    output.Type = MalNotificationsTypes.BlogComment;
                    break;
                case " watched-topic-message":
                    output.Type = MalNotificationsTypes.WatchedTopics;
                    break;
                case "club-mass-message-in-forum":
                    output.Type = MalNotificationsTypes.ClubMessages;
                    break;
                case "user-mention-in-club-comment":
                    output.Type = MalNotificationsTypes.UserMentions;
                    output.Header = notification.categoryName;
                    output.Content = $"{notification.senderName} has mentioned you in club {notification.pageTitle}";
                    output.IsSupported = false;
                    break;
                case " on-air":
                    output.Type = MalNotificationsTypes.NowAiring;
                    output.Header = notification.categoryName;
                    output.Content = $"The anime you plan to watch began airing on {notification.date} {notification.animes.First().title}";
                    output.LuanchArgs = notification.animes.First().url;
                    output.IsSupported = true;
                    break;
                case " payment-stripe":
                    output.Type = MalNotificationsTypes.Payment;
                    output.Header = "Payment notification.";
                    output.Content = "(I don't know what does it mean, feel free to let me know about this on github)";
                    output.IsSupported = false;
                    break;
                case "related-anime-add":
                    output.Type = MalNotificationsTypes.NewRelatedAnime;
                    output.Header = notification.categoryName;
                    output.Content = $"{notification.anime.title} ({notification.anime.mediaType}) has just been added to MAL databse!";
                    output.LuanchArgs = notification.url;
                    output.IsSupported = true;
                    break;
                case "user-mention-in-forum-message":
                    output.Type = MalNotificationsTypes.UserMentions;
                    break;                
                default:
                    output.Type = MalNotificationsTypes.Generic;
                    break;
            }
            output.Id = notification.id;
            output.Date = notification.createdAtForDisplay;
            return output;

        }
    }
}
