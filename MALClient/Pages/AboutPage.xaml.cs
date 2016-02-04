using System;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Utils.RegisterBackNav(PageIndex.PageAnimeList, null);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Utils.DeregisterBackNav();
        }

        private async void Donate(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as MenuFlyoutItem;
                await CurrentApp.RequestProductPurchaseAsync(btn.Name, false);
                var msg = new MessageDialog("Thanks for donating!");
                await msg.ShowAsync();
            }
            catch (Exception)
            {
                // no donation
            }
            
        }

        private async void LaunchIssues(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Mordonus/MALClient/issues"));
        }

        private async void LaunchRepo(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Mordonus/MALClient"));
        }
    }
}
