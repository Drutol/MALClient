using System;
using Windows.UI.Xaml.Data;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class DateToDiffStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = (DateTime) value;
            var diff = DateTime.Now.Subtract(date);
            if (diff.TotalDays > 10)
                return date.ToString("g");
            return
                $@"{(diff.Days > 0 ? $"{diff.Days} {(diff.Days == 1 ? "day" : "days")}" : "")} {(diff.Hours > 0
                    ? $"{diff.Hours} {(diff.Hours == 1 ? "hour" : "hours")}"
                    : "")} {(diff.TotalDays < 1 && diff.Minutes > 0
                    ? $"{diff.Minutes} {(diff.Hours == 1 ? "minute " : "minutes ")}"
                    : "")}ago";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
