using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Flyouts
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