using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MALClient.UWP.Shared.Managers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Off.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsArticlesPage : Page
    {
        public SettingsArticlesPage()
        {
            this.InitializeComponent();
        }

        private void ButtonPinArticlesOnClick(object sender, RoutedEventArgs e)
        {
            LiveTilesManager.PinArticlesTile();
        }

        private void ButtonPinNewsOnClick(object sender, RoutedEventArgs e)
        {
            LiveTilesManager.PinNewsTile();
        }
    }
}
