using System;
using Windows.UI.Xaml.Data;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;

namespace MalClient.Shared.XamlConverters
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