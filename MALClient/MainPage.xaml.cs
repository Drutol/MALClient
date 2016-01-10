using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
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

        private Dictionary<string,Tuple<List<AnimeItem>,List<XElement>,DateTime,Dictionary<int,bool>>> _allAnimeItemsCache = new Dictionary<string, Tuple<List<AnimeItem>, List<XElement>, DateTime, Dictionary<int, bool>>>();
        private bool _onSearchPage = false;
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
                var entry = _allAnimeItemsCache[Creditentials.UserName].Item1.First(item => item.Id == id);
                if (entry != null)
                {
                    watchedEps = entry.WatchedEpisodes;
                    myStatus = entry.status;
                    myScore = entry.Score;
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
            _onSearchPage = false;
            MainMenu.IsPaneOpen = false;

            if (!Creditentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                var msg = new MessageDialog("Log in order to search.");
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
                    MainContent.Navigate(typeof(Pages.AnimeListPage));
                    break;
                case PageIndex.PageAnimeDetails:
                    HideSearchStuff();
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
            _allAnimeItemsCache[source] = new Tuple<List<AnimeItem>,List<XElement>, DateTime, Dictionary<int,bool>>(items,downItems,updateTime,loadStatus);
        }

        public void RetrieveAnimeEntries(string source,out List<AnimeItem> loadedItems,out List<XElement> downloadedItems  ,out DateTime time,out Dictionary<int,bool> loadStatus )
        {
            Tuple<List<AnimeItem>, List<XElement>, DateTime, Dictionary<int, bool>> data;
            _allAnimeItemsCache.TryGetValue(source, out data);
            time = data?.Item3 ?? DateTime.Now;
            loadStatus = data?.Item4 ?? new Dictionary<int, bool>
            {
                {1, false},
                {2, false},
                {3, false},
                {4, false},
                {6, false}
            };
            loadedItems = data?.Item1 ?? new List<AnimeItem>();
            downloadedItems = data?.Item2 ?? new List<XElement>();
        }

        public void AddAnimeEntry(string source, AnimeItem item)
        {
            if (_allAnimeItemsCache[source] != null)
            {
                _allAnimeItemsCache[source].Item1.Add(item);
            }
        }

        public void RemoveAnimeEntry(string source, AnimeItem item)
        {
            if (_allAnimeItemsCache[source] != null)
            {
                _allAnimeItemsCache[source].Item1.Remove(item);
            }
        }


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
        #endregion


    }
}
