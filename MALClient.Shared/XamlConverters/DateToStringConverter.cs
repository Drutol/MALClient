using System;
using Windows.UI.Xaml.Data;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime) value).ToString("g");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
