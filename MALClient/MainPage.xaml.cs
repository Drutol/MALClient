using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using MALClient.Pages;
using System.Xml.Linq;
using Windows.UI.Popups;
using MALClient.Comm;
using MALClient.Items;
using MALClient.UserControls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MALClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Dictionary<string,AnimeUserCache> _allAnimeItemsCache = new Dictionary<string, AnimeUserCache>();
        private List<SeasonalAnimeData> _seasonalAnimeCache = new List<SeasonalAnimeData>(); 
        private bool _onSearchPage = false;
        private bool _wasOnDetailsFromSearch = false;
        private bool? _searchStateBeforeNavigatingToSearch = null;
        private Tuple<DateTime, ProfileData> _profileDataCache;

        public MainPage()
        {
            this.InitializeComponent();
            Utils.CheckTiles();
            if (Creditentials.Authenticated)
            {
                Navigate(PageIndex.PageAnimeList);
                HamburgerControl.SetActiveButton(HamburgerButtons.AnimeList);
            }
            else
            {
                SetStatus("Log In");
                Navigate(PageIndex.PageLogIn);
                HamburgerControl.SetActiveButton(HamburgerButtons.LogIn);
            }

            
        }

        private void ReversePane()
        {
            MainMenu.IsPaneOpen = !MainMenu.IsPaneOpen;
            //if(MainMenu.IsPaneOpen)
            //    HamburgerControl.PaneOpened();
            //else            
            //    HamburgerControl.PaneClosed();
            
        }

        internal void UpdateHamburger()
        {
            HamburgerControl.UpdateProfileImg();
        }

        internal bool TryRetrieveListItem(int id, ref int watchedEps, ref int myStatus, ref int myScore, ref AnimeItem reference)
        {
            if (_allAnimeItemsCache[Creditentials.UserName] == null) return false;
            try
            {
                var entry = _allAnimeItemsCache[Creditentials.UserName].LoadedAnime.First(item => item.Id == id);
                if (entry != null)
                {
                    watchedEps = entry.MyEpisodes;
                    myStatus = entry.MyStatus;
                    myScore = (int)entry.MyScore; //it's float only when we are doing seasonal
                    reference = entry;
                    return true;
                }
            }
            catch (Exception) // no item in sequence
            {
                return false;
            }
            return false;
        }

        #region Navigation
        internal async void Navigate(PageIndex index, object args = null)
        {
            bool wasOnSearchPage = _onSearchPage;
            _onSearchPage = false;
            MainMenu.IsPaneOpen = false;

            if (!Creditentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                var msg = new MessageDialog("Log in first in order to access this page.");
                await msg.ShowAsync();
                return;
            }

            if (index == PageIndex.PageAnimeList && _searchStateBeforeNavigatingToSearch != null)
            {
                SearchToggle.IsChecked = _searchStateBeforeNavigatingToSearch;
                if(SearchToggle.IsChecked.Value)
                    ShowSearchStuff();
                else
                {
                    HideSearchStuff();
                }
            }
            switch (index)
            {
                case PageIndex.PageAnimeList:
                    ShowSearchStuff();
                    if (wasOnSearchPage || _wasOnDetailsFromSearch)
                    {
                        _currSearchQuery = "";
                        SearchInput.Text = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }
                    MainContent.Navigate(typeof(Pages.AnimeListPage),args);
                    break;
                case PageIndex.PageAnimeDetails:
                    HideSearchStuff();
                    _wasOnDetailsFromSearch = (args as AnimeDetailsPageNavigationArgs).AnimeElement != null; //from search details are passed instead of downloaded once more
                    MainContent.Navigate(typeof(Pages.AnimeDetailsPage), args);
                    break;
                case PageIndex.PageSettings:
                    HideSearchStuff();
                    MainContent.Navigate(typeof(Pages.SettingsPage));
                    break;
                case PageIndex.PageSearch:
                    NavigateSearch(args != null);
                    break;
                case PageIndex.PageLogIn:
                    HideSearchStuff();
                    MainContent.Navigate(typeof(Pages.LogInPage));
                    break;
                case PageIndex.PageProfile:
                    ShowSearchStuff();
                    MainContent.Navigate(typeof(Pages.ProfilePage),RetrieveProfileData());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }

        internal AnimeListPageNavigationArgs GetCurrentListOrderParams()
        {
            var page = MainContent.Content as AnimeListPage;
            return new AnimeListPageNavigationArgs(page.SortOption,page.CurrentStatus,page.SortDescending);
        }


        private void NavigateSearch(bool autoSearch = false)
        {
            _onSearchPage = true;
            _currSearchQuery = SearchInput.Text;
            _searchStateBeforeNavigatingToSearch = SearchToggle.IsChecked ?? false;
            ShowSearchStuff();
            ToggleSearchStuff();
            if (!autoSearch)
                SearchInput.Focus(FocusState.Keyboard);
            MainContent.Navigate(typeof (Pages.AnimeSearchPage), autoSearch ? GetSearchQuery() : "");
        }

        #endregion

        #region UIUtils

        private void ReversePane(object sender, RoutedEventArgs e)
        {
            ReversePane();
        }

        public void SetStatus(string status)
        {
            CurrentStatus.Text = status;
        }

        private void ShowSearchStuff()
        {
            SearchToggle.Visibility = Visibility.Visible;
            if ((bool) SearchToggle.IsChecked)
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

        private void UnToggleSearchStuff()
        {
            SearchInput.Visibility = Visibility.Collapsed;
            SearchToggle.IsChecked = false;
        }

        #endregion

        #region Search

        private string _currSearchQuery = null;

        private void SearchQuerySubmitted(object o, TextChangedEventArgs textChangedEventArgs)
        {
            if (_onSearchPage) // we are on anime list
                return;

            var source = MainContent.Content as AnimeListPage;
            _currSearchQuery = SearchInput.Text;
            source.RefreshList(true);
        }

        internal string GetSearchQuery()
        {
            return _currSearchQuery;
        }

        private void SearchInput_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!_onSearchPage)
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

        private void ReverseSearchInput(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            if (_onSearchPage)
            {
                btn.IsChecked = true;
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

        #region SmallDataCaching

        public void SaveAnimeEntries(string source, List<AnimeItem> items, List<XElement> downItems  , DateTime updateTime , Dictionary<int,bool> loadStatus )
        {
            _allAnimeItemsCache[source.ToLower()] = new AnimeUserCache()
            {
                LoadedAnime = items,
                DownloadedAnime = downItems,
                LastUpdate = updateTime,
                LoadedStatus = loadStatus
            };
        }

        public AnimeUserCache RetrieveLoadedAnime()
        {
            AnimeUserCache data;
            _allAnimeItemsCache.TryGetValue(Creditentials.UserName.ToLower(),out data);
            return data;
        }

        public void RetrieveAnimeEntries(string source,out List<AnimeItem> loadedItems,out List<XElement> downloadedItems  ,out DateTime time,out Dictionary<int,bool> loadStatus )
        {
            AnimeUserCache data;
            _allAnimeItemsCache.TryGetValue(source.ToLower(), out data);
            time = data?.LastUpdate ?? DateTime.Now;
            loadStatus = data?.LoadedStatus ?? new Dictionary<int, bool>
            {
                {1, false},
                {2, false},
                {3, false},
                {4, false},
                {6, false}
            };
            loadedItems = data?.LoadedAnime ?? new List<AnimeItem>();
            downloadedItems = data?.DownloadedAnime ?? new List<XElement>();
        }

        public void AddAnimeEntry(string source, AnimeItem item)
        {
            source = source.ToLower();
            if (_allAnimeItemsCache[source] != null)
            {
                _allAnimeItemsCache[source].AnimeItemLoaded(item);
            }
        }

        public void RemoveAnimeEntry(string source, AnimeItem item)
        {
            source = source.ToLower();
            if (_allAnimeItemsCache[source] != null)
            {
                _allAnimeItemsCache[source].LoadedAnime.Remove(item);
            }
        }

        //Profile
        public void SaveProfileData(ProfileData data)
        {
            _profileDataCache = new Tuple<DateTime, ProfileData>(DateTime.Now,data);
        }

        public ProfileData RetrieveProfileData()
        {
            if (_profileDataCache == null)
                return null;
            TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(_profileDataCache.Item1);
            if (diff.TotalSeconds < 3600)
                return _profileDataCache.Item2;
            return null;
        }
        //Season
        public void SaveSeasonData(List<SeasonalAnimeData> data)
        {
            _seasonalAnimeCache = data;
        }

        public List<SeasonalAnimeData> RetrieveSeasonData()
        {
            return _seasonalAnimeCache;
        }
        #endregion


    }
}
