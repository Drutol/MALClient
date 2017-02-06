using System;
using Windows.UI.Xaml.Data;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class DummyFontAwesomeToRealFontAwesomeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (FontAwesome.UWP.FontAwesomeIcon) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
