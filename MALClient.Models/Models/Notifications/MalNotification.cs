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
        public string LaunchArgs { get; set; }
        public bool IsSupported { get; set; }
        public bool IsRead { get; set; }
        public string ImgUrl { get; set; }

        public static MalNotification CreateFromRawData(MalScrappedNotification notification)
        {
            var output = new MalNotification();
            output.IsRead = notification.isRead;

            if (output.IsRead)
                return output;

            switch (notification.typeIdentifier)
            {
                case "friend_request":
                    output.Type = MalNotificationsTypes.FriendRequest;
                    output.Header = "New friend request";
                    output.Content = $"{notification.friendName} sent you a friend request!";
                    output.LaunchArgs = notification.url;
                    output.IsSupported = false;
                    output.ImgUrl = notification.friendImageUrl;
                    break;
                case "friend_request_accept":
                    output.Type = MalNotificationsTypes.FriendRequestAcceptDeny;
                    output.Header = "Friend request accepted";
                    output.Content = $"{notification.friendName} accepted your friend request!";
                    output.IsSupported = false;
                    break;
                case "friend_request_deny":
                    output.Type = MalNotificationsTypes.FriendRequestAcceptDeny;
                    output.Header = "Friend request denied";
                    output.Content = $"{notification.friendName} rejected your friend request.";
                    output.IsSupported = false;
                    break;
                case "profile_comment":
                    output.Type = MalNotificationsTypes.ProfileComment;
                    output.Header = "New profile comment";
                    output.Content = $"{notification.commentUserName} posted a comment on your profile\n{notification.text}.";
                    output.LaunchArgs = notification.url;
                    output.ImgUrl = notification.commentUserImageUrl;
                    output.IsSupported = true;
                    break;
                case "forum_quote":
                    output.Type = MalNotificationsTypes.ForumQuoute;
                    output.Header = "New forum quoute!";
                    output.Content = $"{notification.quoteUserName} has quouted your post in the \"{notification.topicTitle}\" thread.";
                    output.LaunchArgs = notification.topicUrl;
                    output.IsSupported = true;
                    break;
                case "blog_comment":
                    output.Type = MalNotificationsTypes.BlogComment;
                    output.Header = "New blog comment.";
                    output.IsSupported = false;
                    break;
                case "watched_topic_message":
                    output.Type = MalNotificationsTypes.WatchedTopics;
                    output.Header = "New reply on your watched topic!";
                    output.Content = $"New reply was posted on your watched topic: \"{notification.pageTitle}\"";
                    output.LaunchArgs = notification.url;
                    output.IsSupported = true;
                    break;
                case "club_mass_message_in_forum":
                    output.Type = MalNotificationsTypes.ClubMessages;
                    output.Header = "New club message.";
                    output.IsSupported = false;
                    break;
                case "user_mention_in_club_comment":
                    output.Type = MalNotificationsTypes.UserMentions;
                    output.Header = notification.categoryName;
                    output.Content = $"{notification.senderName} has mentioned you in club {notification.pageTitle}";
                    output.LaunchArgs = notification.pageUrl;
                    output.IsSupported = false;
                    break;
                case "on_air":
                    output.Type = MalNotificationsTypes.NowAiring;
                    output.Header = notification.categoryName;
                    output.Content = $"The anime you plan to watch began airing on {notification.date} {notification.animes.First().title}";
                    output.LaunchArgs = notification.animes.First().url;
                    output.IsSupported = true;
                    break;
                case " payment_stripe":
                    output.Type = MalNotificationsTypes.Payment;
                    output.Header = "Payment notification.";
                    output.Content = "(I don't know what does it mean, feel free to let me know about this on github)";
                    output.IsSupported = false;
                    break;
                case "related_anime_add":
                    output.Type = MalNotificationsTypes.NewRelatedAnime;
                    output.Header = notification.categoryName;
                    output.Content = $"{notification.anime.title} ({notification.anime.mediaType}) has just been added to MAL databse!";
                    output.LaunchArgs = notification.url;
                    output.IsSupported = true;
                    break;
                case "user_mention_in_forum_message":
                    output.Type = MalNotificationsTypes.UserMentions;
                    output.Header = notification.categoryName;
                    output.Content = $"{notification.senderName} has mentioned you in forum message {notification.pageTitle}";
                    output.LaunchArgs = notification.pageUrl;
                    output.IsSupported = true;
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
