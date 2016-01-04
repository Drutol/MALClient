using System;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MALClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
       
        public MainPage()
        {
            this.InitializeComponent();
            Utils.Utils.CheckTiles();
            MainContent.Navigate(typeof (Pages.AnimeListPage));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(e.Parameter.ToString()))
                LaunchUri(e.Parameter.ToString());
        }

        private async void LaunchUri(string url)
        {
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        internal void ReversePane()
        {
            MainMenu.IsPaneOpen = !MainMenu.IsPaneOpen;
            if(MainMenu.IsPaneOpen)
                HamburgerControl.PaneOpened();
            else            
                HamburgerControl.PaneClosed();
            
        }

        internal void NavigateList()
        {
            MainContent.Navigate(typeof (Pages.AnimeListPage));
        }

        internal void NavigateLogin()
        {
            MainContent.Navigate(typeof(Pages.LogInPage));
        }
    }
}
