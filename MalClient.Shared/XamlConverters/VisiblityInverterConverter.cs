using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MalClient.Shared.XamlConverters
{
    public sealed class VisiblityInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var param = (Visibility) value;
            return param == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}