using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class ComparisonResultToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = (int) value;
            if (val < 0)
                return new SolidColorBrush(Colors.Crimson);
            else if(val > 0)
                return new SolidColorBrush(Color.FromArgb(0xff, 0x98, 0xc9, 0x26));
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
