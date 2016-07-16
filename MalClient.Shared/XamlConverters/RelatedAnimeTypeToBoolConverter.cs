using System;
using Windows.UI.Xaml.Data;
using MALClient.Comm;

namespace MALClient.XamlConverters
{
    public class RelatedAnimeTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = (RelatedItemType) value;
            return val == RelatedItemType.Anime ||
                   (val == RelatedItemType.Manga && Settings.SelectedApiType == ApiType.Mal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}