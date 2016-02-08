using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Pages;
using MALClient.UserControls;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MALClient
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page , IMainViewNavigate
    {
        private readonly Dictionary<string, AnimeUserCache> _allAnimeItemsCache =
            new Dictionary<string, AnimeUserCache>();
        private List<RecomendationData> _recomendationDataCache = new List<RecomendationData>(); 
        private bool _onSearchPage;
        private Tuple<DateTime, ProfileData> _profileDataCache;
        private bool? _searchStateBeforeNavigatingToSearch;
        private List<AnimeItemAbstraction> _seasonalAnimeCache = new List<AnimeItemAbstraction>();
        private bool _wasOnDetailsFromSearch;
#pragma warning disable 4014
        public MainPage()
        {
            InitializeComponent();
            Utils.CheckTiles();
            var vl = new ViewModelLocator();
            vl.Main.View = this;
            if (Creditentials.Authenticated)
                vl.Main.Navigate(PageIndex.PageRecomendations);               
            else
                vl.Main.Navigate(PageIndex.PageLogIn);
        }
#pragma warning restore 4014
        public HamburgerControl Hamburger => HamburgerControl;

        private void ReversePane()
        {
            MainMenu.IsPaneOpen = !MainMenu.IsPaneOpen;
            if (MainMenu.IsPaneOpen)
                HamburgerControl.PaneOpened();
            //else            
            //    HamburgerControl.PaneClosed();
        }

        #region Search

        private string _currSearchQuery;

        private void SearchQuerySubmitted(object o, TextChangedEventArgs textChangedEventArgs)
        {
            if (_onSearchPage) // we are on anime list
                return;

            var source = MainContent.Content as AnimeListPage;
            _currSearchQuery = SearchInput.Text;
            source.RefreshList(true);
        }

        private void SearchInput_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((e == null || e.Key == VirtualKey.Enter) && SearchInput.Text.Length >= 2)
            {
                SearchInput.IsEnabled = false; //reset input
                SearchInput.IsEnabled = true;
                new ViewModelLocator().Main.OnSearchInputSubmit();
            }
        }

        private void ReverseSearchInput(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            if (_onSearchPage)
            {
                btn.IsChecked = true;
                SearchInput_OnKeyDown(null, null);
                return;
            }
            if ((bool) btn.IsChecked)
            {
                SearchInput.Visibility = Visibility.Visible;
                _currSearchQuery = SearchInput.Text;
                if (!string.IsNullOrWhiteSpace(_currSearchQuery))
                    SearchQuerySubmitted(null, null);
            }
            else
            {
                SearchInput.Visibility = Visibility.Collapsed;
                _currSearchQuery = null;
                var source = MainContent.Content as AnimeListPage;
                source.RefreshList();
            }
        }

        #endregion


        public void Navigate(Type page, object args = null)
        {
            MainContent.Navigate(page, args);
        }

        public object GetCurrentContent()
        {
            return MainContent.Content;
        }

        public void SearchInputFocus(FocusState state)
        {
            SearchInput.Focus(state);
        }
    }
}