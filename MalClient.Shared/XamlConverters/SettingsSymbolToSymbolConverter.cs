using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using MALClient.Models.Enums;

namespace MALClient.Shared.XamlConverters
{
    public class SettingsSymbolToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((SettingsSymbolsEnum)value)
            {
                case SettingsSymbolsEnum.Setting:
                    return Symbol.Setting;
                case SettingsSymbolsEnum.SaveLocal:
                    return Symbol.SaveLocal;
                case SettingsSymbolsEnum.CalendarWeek:
                    return Symbol.CalendarWeek;
                case SettingsSymbolsEnum.PreviewLink:
                    return Symbol.PreviewLink;
                case SettingsSymbolsEnum.PostUpdate:
                    return Symbol.PostUpdate;
                case SettingsSymbolsEnum.Manage:
                    return Symbol.Manage;
                case SettingsSymbolsEnum.Contact:
                    return Symbol.Contact;
                case SettingsSymbolsEnum.Placeholder:
                    return Symbol.Placeholder;
                case SettingsSymbolsEnum.Important:
                    return Symbol.Important;
                case SettingsSymbolsEnum.SwitchApps:
                    return Symbol.SwitchApps;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
