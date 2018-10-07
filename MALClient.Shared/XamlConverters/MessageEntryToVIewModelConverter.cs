using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Utils;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class MessageEntryToViewModelConverter : IValueConverter
    {
        public class MessageEntry
        {
            public MessageEntry(MalMessageModel msg)
            {
                Msg = msg;
                if (Msg.Sender.Equals(Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                {
                    HorizontalAlignment = HorizontalAlignment.Right;
                    Margin = new Thickness(20, 0, 0, 0);
                    CornerRadius = new CornerRadius(10, 10, 0, 10);
                    Background = Application.Current.Resources["SystemControlHighlightAltListAccentLowBrush"] as Brush;
                }
                else
                {
                    HorizontalAlignment = HorizontalAlignment.Left;
                    Margin = new Thickness(0, 0, 20, 0);
                    CornerRadius = new CornerRadius(10, 10, 10, 0);
                    Background = Application.Current.Resources["SystemControlHighlightListAccentLowBrush"] as Brush;
                    Background.Opacity = .5;
                }
            }

            public MalMessageModel Msg { get; }
            public HorizontalAlignment HorizontalAlignment { get; }
            public Thickness Margin { get; }
            public CornerRadius CornerRadius { get; }
            public Brush Background { get; }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as IEnumerable<MalMessageModel>).Select(model => new MessageEntry(model));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
