using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Flyouts
{
    public sealed partial class ListItemStatusFlyout : MenuFlyoutPresenter
    {
        public ListItemStatusFlyout()
        {
            InitializeComponent();
        }

        public void ShowAt(FrameworkElement target)
        {
            StatusFlyout.ShowAt(target);
        }
    }
}