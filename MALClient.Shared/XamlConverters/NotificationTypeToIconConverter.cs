using System;
using Windows.UI.Xaml.Data;
using MALClient.Models.Enums;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class NotificationTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((MalNotificationsTypes)value)
            {
                case MalNotificationsTypes.Generic:
                    return FontAwesomeIcon.None;
                case MalNotificationsTypes.FriendRequest:
                    return FontAwesomeIcon.User;
                case MalNotificationsTypes.FriendRequestAcceptDeny:
                    return FontAwesomeIcon.User;
                case MalNotificationsTypes.ProfileComment:
                    return FontAwesomeIcon.Comment;
                case MalNotificationsTypes.BlogComment:
                    return FontAwesomeIcon.None;
                case MalNotificationsTypes.ForumQuoute:
                    return FontAwesomeIcon.CommentsOutline;
                case MalNotificationsTypes.UserMentions:
                    return FontAwesomeIcon.At;
                case MalNotificationsTypes.WatchedTopics:
                    return FontAwesomeIcon.Binoculars;
                case MalNotificationsTypes.ClubMessages:
                    return FontAwesomeIcon.Group;
                case MalNotificationsTypes.NewRelatedAnime:
                    return FontAwesomeIcon.Clone;
                case MalNotificationsTypes.NowAiring:
                    return FontAwesomeIcon.Camera;
                case MalNotificationsTypes.Payment:
                    return FontAwesomeIcon.Money;
                case MalNotificationsTypes.Messages:
                    return FontAwesomeIcon.EnvelopeOutline;
                case MalNotificationsTypes.WatchedTopic:
                    return FontAwesomeIcon.Eye;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}