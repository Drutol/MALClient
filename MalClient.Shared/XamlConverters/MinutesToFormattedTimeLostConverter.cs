using System;
using Windows.UI.Xaml.Data;

namespace MALClient.Shared.XamlConverters
{
    /// <summary>
    ///     Lost as in lost watching anime .... xd
    /// </summary>
    public class MinutesToFormattedTimeLostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var timeSpan = TimeSpan.FromMinutes((int) value);
            return $"{timeSpan.Days} days, {timeSpan.Hours} hours, {timeSpan.Minutes} minutes.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}