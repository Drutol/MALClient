using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MALClient.XamlConverters
{
    public sealed class AlternateRowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ListViewItem item = (ListViewItem) value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            // Get the index of a ListViewItem
            int index = listView.IndexFromContainer(item);

            return index%2 == 0 ? "LightGray" : "Transparent";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
