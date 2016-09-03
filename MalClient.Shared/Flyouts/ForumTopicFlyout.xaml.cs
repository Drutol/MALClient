using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Shared.Flyouts
{
    public sealed partial class ForumTopicFlyout : FlyoutPresenter
    {
        public ForumTopicFlyout()
        {
            InitializeComponent();
        }

        public void ShowAt(FrameworkElement target)
        {
            FlyoutTopic.ShowAt(target);
        }

        private void FlyoutButtonPressed(object sender, RoutedEventArgs e)
        {
            FlyoutTopic.Hide();
        }
    }
}