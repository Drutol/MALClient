using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using MALClient.Models.Models.MalSpecific;

namespace MALClient.Shared.XamlConverters
{
    public class MessageModelToSymbolIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var model = value as MalMessageModel;
            if (model.IsMine)
                return Symbol.MailForward;
            return model.IsRead ? Symbol.Read : Symbol.Mail;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
