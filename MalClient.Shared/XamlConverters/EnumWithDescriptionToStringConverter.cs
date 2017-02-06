using System;
using Windows.UI.Xaml.Data;
using MALClient.Models.Enums;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class EnumWithDescriptionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as Enum).GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
