using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;

namespace MALClient.Shared.XamlConverters
{
    public class MalNotificationTypeToIsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = (MalNotificationsTypes) value;
            return (Settings.EnabledNotificationTypes & val) == val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
