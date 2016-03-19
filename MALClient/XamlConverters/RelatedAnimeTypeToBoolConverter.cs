using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using MALClient.Comm;

namespace MALClient.XamlConverters
{
    class RelatedAnimeTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = (RelatedItemType)value;
            return val == RelatedItemType.Anime || val == RelatedItemType.Manga;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
