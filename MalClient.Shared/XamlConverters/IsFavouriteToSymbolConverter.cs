using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class IsFavouriteToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Symbol.UnFavorite : Symbol.Favorite;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
