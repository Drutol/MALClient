using System;
using Windows.UI.Xaml.Data;

namespace MALClient.XamlConverters
{
    internal class DivideValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double) value/2.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}