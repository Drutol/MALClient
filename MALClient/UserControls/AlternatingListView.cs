using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MALClient.UserControls
{
    public class AlternatingListView : ListView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var listViewItem = element as ListViewItem;
            if (listViewItem != null)
            {
                var index = IndexFromContainer(element);

                if ((index + 1)%2 == 0)
                {
                    listViewItem.Background = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    listViewItem.Background = new SolidColorBrush(Color.FromArgb(170, 230, 230, 230));
                }
            }
        }
    }
}