using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using MALClient.XShared.Utils.Enums;

namespace MALClient.Shared.XamlConverters
{
    public class DisplayModeToPresenterControlVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var mode = (AnimeListDisplayModes) value;
            switch (parameter as string)
            {
                case "IndefiniteList":
                    return mode == AnimeListDisplayModes.IndefiniteList ? Visibility.Visible : Visibility.Collapsed;
                case "IndefiniteGrid":
                    return mode == AnimeListDisplayModes.IndefiniteGrid ? Visibility.Visible : Visibility.Collapsed;
                case "IndefiniteCompactList":
                    return mode == AnimeListDisplayModes.IndefiniteCompactList
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}