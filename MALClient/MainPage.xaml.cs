using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.Pages;
using System.Xml.Linq;

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
            MALClient.Utils.CheckTiles();
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

        private void ReversePane()
        {
            MainMenu.IsPaneOpen = !MainMenu.IsPaneOpen;
            if(MainMenu.IsPaneOpen)
                HamburgerControl.PaneOpened();
            else            
                HamburgerControl.PaneClosed();
            
        }

        internal void NavigateList()
        {
            _onSearchPage = false;
            ShowSearchStuff();
            MainMenu.IsPaneOpen = false;
            MainContent.Navigate(typeof (Pages.AnimeListPage));
        }

        private void ShowSearchStuff()
        {
            SearchToggle.Visibility = Visibility.Visible;
            if ((bool)SearchToggle.IsChecked)
                SearchInput.Visibility = Visibility.Visible;
        }

        private void HideSearchStuff()
        {
            SearchInput.Visibility = Visibility.Collapsed;
            SearchToggle.Visibility = Visibility.Collapsed;
        }

        private void ToggleSearchStuff()
        {
            SearchInput.Visibility = Visibility.Visible;
            SearchToggle.IsChecked = true;
        }

        internal void NavigateLogin()
        {
            _onSearchPage = false;
            MainMenu.IsPaneOpen = false;
            SetStatus("Log In");
            HideSearchStuff();
            MainContent.Navigate(typeof(Pages.LogInPage));
        }

        internal void NavigateDetails(XElement item)
        {
            HideSearchStuff();
            MainMenu.IsPaneOpen = false;
            _onSearchPage = false;
            MainContent.Navigate(typeof (Pages.AnimeDetailsPage), new AnimeDetailsPageNavigationArgs(0, item));
        }

        private bool _onSearchPage = false;
        internal void NavigateSearch()
        {
            _onSearchPage = true;
            ShowSearchStuff();
            ToggleSearchStuff();
            MainMenu.IsPaneOpen = false;
            SearchInput.Focus(FocusState.Keyboard);
            MainContent.Navigate(typeof (Pages.AnimeSearchPage));
        }

        private void ReversePane(object sender, RoutedEventArgs e)
        {
            ReversePane();
        }

        public void SetStatus(string status)
        {
            CurrentStatus.Text = status;
        }

        private void ReverseSearchInput(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            if ((bool) btn.IsChecked)
            {
                SearchInput.Visibility = Visibility.Visible;
                _currSearchQuery = SearchInput.Text;
                if(!string.IsNullOrWhiteSpace(_currSearchQuery))
                    SearchQuerySubmitted(null,null);
            }
            else
            {
                SearchInput.Visibility = Visibility.Collapsed;
                _currSearchQuery = null;
                var source = MainContent.Content as AnimeListPage;
                source.RefreshList();
            }
                
        }

        private string _currSearchQuery = null;

        private void SearchQuerySubmitted(object o, TextChangedEventArgs textChangedEventArgs)
        {
            if(_onSearchPage)
                return;

            var source = MainContent.Content as AnimeListPage;
            _currSearchQuery = SearchInput.Text;
            source.RefreshList();
        }

        internal string GetSearchQuery()
        {
            return _currSearchQuery;
        }

        private void SearchInput_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(!_onSearchPage)
                return;
           
            if (e.Key == VirtualKey.Enter && SearchInput.Text.Length >= 2)
            {
                var txt = sender as TextBox;
                txt.IsEnabled = false; //reset input
                txt.IsEnabled = true; 
                var source = MainContent.Content as AnimeSearchPage;
                source.SubmitQuery(txt.Text);
            }
        }
    }
}
