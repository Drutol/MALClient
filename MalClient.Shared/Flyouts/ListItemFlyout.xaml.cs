using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UWP.Shared.Flyouts
{
    public sealed partial class ListItemFlyout : FlyoutPresenter
    {
        public ListItemFlyout()
        {
            InitializeComponent();
        }

        public void ShowAt(FrameworkElement target)
        {
            FlyoutMore.ShowAt(target);
        }

        private void FlyoutButtonPressed(object sender, RoutedEventArgs e)
        {
            FlyoutMore.Hide();
        }
    }
}