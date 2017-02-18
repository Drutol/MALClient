using System;
using Android.Views;

namespace MALClient.Android.BindingConverters
{
    public static class Converters
    {
        public static ViewStates BoolToVisibility(bool arg)
        {
            return arg ? ViewStates.Visible : ViewStates.Gone;
        }

        public static ViewStates BoolToVisibilityInverted(bool arg)
        {
            return arg ? ViewStates.Gone : ViewStates.Visible;
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
                $@"{(diff.Days > 0 ? $"{diff.Days} {(diff.Days == 1 ? "day" : "days")}" : "")} {(diff.Hours > 0
                    ? $"{diff.Hours} {(diff.Hours == 1 ? "hour" : "hours")}"
                    : "")} {(diff.TotalDays < 1 && diff.Minutes > 0
                    ? $"{diff.Minutes} {(diff.Hours == 1 ? "minute " : "minutes ")}"
                    : "")}ago";
        }
    }
}