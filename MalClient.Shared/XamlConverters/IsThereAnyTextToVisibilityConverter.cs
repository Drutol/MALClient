using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MALClient.XamlConverters
{
    public class IsThereAnyTextToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}