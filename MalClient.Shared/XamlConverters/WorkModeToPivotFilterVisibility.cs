using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using MALClient.Models.Enums;
using MALClient.XShared.Utils.Enums;

namespace MALClient.Shared.XamlConverters
{
    public class WorkModeToPivotFilterVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var mode = (AnimeListWorkModes) value;
            switch (parameter as string)
            {
                case "SeasonalWorkMode":
                    return mode == AnimeListWorkModes.SeasonalAnime ? Visibility.Visible : Visibility.Collapsed;
                case "AnyWorkMode":
                    return mode != AnimeListWorkModes.SeasonalAnime ? Visibility.Visible : Visibility.Collapsed;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}