using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MALClient.UWP.Shared.XamlConverters
{
    class AnimeItemIsThereAnyTextTopLeftToTitleMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.IsNullOrEmpty(value as string) ? new Thickness(5, 3, 25, 3) : new Thickness(5, 3, 70, 3);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
