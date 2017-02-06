using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class PinUnpinSymbolIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool) value ? Symbol.UnPin : Symbol.Pin;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
