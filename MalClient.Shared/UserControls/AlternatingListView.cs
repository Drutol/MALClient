using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MalClient.Shared.UserControls
{
    public class AlternatingListView : ListView
    {
        private static readonly Brush _b2 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(170, 44, 44, 44)
                : Color.FromArgb(170, 230, 230, 230));

        private static readonly Brush _b1 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(255, 11, 11, 11)
                : Colors.WhiteSmoke);

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var listViewItem = element as ListViewItem;
            if (listViewItem != null)
            {
                var index = IndexFromContainer(element);
                if ((index + 1)%2 == 0)
                {
                    listViewItem.Background = _b1;
                }
                else
                {
                    listViewItem.Background = _b2;
                }
            }
        }
    }
}