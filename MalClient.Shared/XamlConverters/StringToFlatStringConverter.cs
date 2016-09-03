using System;
using Windows.UI.Xaml.Data;

namespace MALClient.Shared.XamlConverters
{
    public class StringToFlatStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as string).Replace('\n', ' ');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}