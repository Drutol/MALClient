using System;
using Android.Views;
using MALClient.Models.Enums;

namespace MALClient.Android.BindingConverters
{
    public static class Converters
    {
        public static ViewStates BoolToVisibility(bool arg)
        {
            return arg ? ViewStates.Visible : ViewStates.Gone;
        }

        public static ViewStates BoolToVisibilityLight(bool arg)
        {
            return arg ? ViewStates.Visible : ViewStates.Gone;
        }

        public static ViewStates BoolToVisibilityInverted(bool arg)
        {
            return arg ? ViewStates.Gone : ViewStates.Visible;
        }

        public static ViewStates BoolToVisibilityInvertedLight(bool arg)
        {
            return arg ? ViewStates.Invisible : ViewStates.Visible;
        }

        public static ViewStates VisibilityInverterConverter(ViewStates arg)
        {
            return arg == ViewStates.Visible ? ViewStates.Gone : ViewStates.Visible;
        }

        public static ViewStates IsStringEmptyToVisibility(string arg)
        {
            return string.IsNullOrEmpty(arg) ? ViewStates.Gone : ViewStates.Visible;
        }

        public static string ToDiffString(this DateTime dateTime)
        {
            var diff = DateTime.Now.Subtract(dateTime);
            if (diff.TotalDays > 10)
                return dateTime.ToString("g");
            return
                $@"{(diff.Days > 0 ? $"{diff.Days} {(diff.Days == 1 ? "day" : "days")}" : "")}{(diff.Hours > 0
                    ? $" {diff.Hours} {(diff.Hours == 1 ? "hour" : "hours")}"
                    : "")}{(diff.TotalDays < 1 && diff.Minutes > 0
                    ? $" {diff.Minutes} {(diff.Minutes == 1 ? "minute" : "minutes")}"
                    : "")} ago";
        }

        public static int MalNotificationTypeToIconConverter(MalNotificationsTypes type)
        {
            switch (type)
            {
                case MalNotificationsTypes.Generic:
                    return Resource.String.fa_icon_bug;
                case MalNotificationsTypes.FriendRequest:
                    return Resource.String.fa_icon_user;
                case MalNotificationsTypes.FriendRequestAcceptDeny:
                    return Resource.String.fa_icon_user;
                case MalNotificationsTypes.ProfileComment:
                    return Resource.String.fa_icon_comment;
                case MalNotificationsTypes.BlogComment:
                    return Resource.String.fa_icon_bug;
                case MalNotificationsTypes.ForumQuoute:
                    return Resource.String.fa_icon_comment_o;
                case MalNotificationsTypes.UserMentions:
                    return Resource.String.fa_icon_at;
                case MalNotificationsTypes.WatchedTopics:
                    return Resource.String.fa_icon_binoculars;
                case MalNotificationsTypes.ClubMessages:
                    return Resource.String.fa_icon_group;
                case MalNotificationsTypes.NewRelatedAnime:
                    return Resource.String.fa_icon_clone;
                case MalNotificationsTypes.NowAiring:
                    return Resource.String.fa_icon_camera;
                case MalNotificationsTypes.Payment:
                    return Resource.String.fa_icon_money;
                case MalNotificationsTypes.Messages:
                    return Resource.String.fa_icon_envelope_o;
                case MalNotificationsTypes.WatchedTopic:
                    return Resource.String.fa_icon_eye;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}