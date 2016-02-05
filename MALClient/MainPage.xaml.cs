using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using MALClient.Pages;
using System.Xml.Linq;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.UI.Core;
using Windows.UI.Popups;
using MALClient.Comm;
using MALClient.Items;
using MALClient.UserControls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
#pragma warning disable 4014
namespace MALClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Dictionary<string,AnimeUserCache> _allAnimeItemsCache = new Dictionary<string, AnimeUserCache>();
        private List<AnimeItemAbstraction> _seasonalAnimeCache = new List<AnimeItemAbstraction>(); 
        private bool _onSearchPage = false;
        private bool _wasOnDetailsFromSearch = false;
        private bool? _searchStateBeforeNavigatingToSearch = null;
        private Tuple<DateTime, ProfileData> _profileDataCache;
        public HamburgerControl Hamburger => HamburgerControl;

        public MainPage()
        {
            this.InitializeComponent();
            Utils.CheckTiles();
            if (Creditentials.Authenticated)
            {
                Navigate(PageIndex.PageRecommendations);
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
            if (MainMenu.IsPaneOpen)
                HamburgerControl.PaneOpened();
            //else            
            //    HamburgerControl.PaneClosed();

        }

        internal void UpdateHamburger()
        {
            HamburgerControl.UpdateProfileImg();
        }

        public void AnimeListScrollTo(AnimeItem animeItem)
        {
            if(MainContent.Content is AnimeListPage)
                ((AnimeListPage) MainContent.Content).ScrollTo(animeItem);
        }

        #region Navigation
        internal async Task Navigate(PageIndex index, object args = null)
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

            HamburgerControl.ChangeBottomStackPanelMargin(index == PageIndex.PageAnimeList);

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
                    await Task.Run(async () =>
                    {
                        await
                            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                            {
                               MainContent.Navigate(typeof (Pages.AnimeListPage), args);
                            });
                    });
                    break;
                case PageIndex.PageAnimeDetails:
                    HideSearchStuff();
                    _wasOnDetailsFromSearch = (args as AnimeDetailsPageNavigationArgs).AnimeElement != null; //from search , details are passed instead of being downloaded once more
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
                    HideSearchStuff();
                    await Task.Run(async () =>
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        {
                            MainContent.Navigate(typeof (Pages.ProfilePage), RetrieveProfileData());
                        });
                    });
                    break;
                case PageIndex.PageAbout:
                    HideSearchStuff();
                    SetStatus("About");
                    MainContent.Navigate(typeof(Pages.AboutPage));
                    break;
                case PageIndex.PageRecommendations:
                    HideSearchStuff();
                    MainContent.Navigate(typeof(Pages.RecomendationsPage));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }

        internal AnimeListPageNavigationArgs GetCurrentListOrderParams(bool seasonal)
        {
            var page = MainContent.Content as AnimeListPage;
            return new AnimeListPageNavigationArgs(page.SortOption, page.CurrentStatus, page.SortDescending,page.CurrentPage,seasonal,page.ListSource);
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

            if ((e==null || e.Key == VirtualKey.Enter) && SearchInput.Text.Length >= 2)
            {
                SearchInput.IsEnabled = false; //reset input
                SearchInput.IsEnabled = true;
                var source = MainContent.Content as AnimeSearchPage;
                source.SubmitQuery(SearchInput.Text);
            }
        }

        private void ReverseSearchInput(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            if (_onSearchPage)
            {
                btn.IsChecked = true;
                SearchInput_OnKeyDown(null,null);
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

        public void SaveAnimeEntries(string source, List<AnimeItemAbstraction> items, DateTime updateTime )
        {
            _allAnimeItemsCache[source.ToLower()] = new AnimeUserCache()
            {
                LoadedAnime = items,
                LastUpdate = updateTime,
            };
            bool changedSomething = false;
            foreach (var animeItemAbstraction in items)
            {
                if (animeItemAbstraction.MyEpisodes == animeItemAbstraction.AllEpisodes)
                {
                    changedSomething = true;
                    DataCache.DeregisterVolatileData(animeItemAbstraction.Id);
                }
            }
            if(changedSomething)
                DataCache.SaveVolatileData();
        }

        public AnimeUserCache RetrieveLoadedAnime()
        {
            if (!Creditentials.Authenticated)
                return null;
            AnimeUserCache data;
            _allAnimeItemsCache.TryGetValue(Creditentials.UserName.ToLower(),out data);
            return data;
        }

        public bool TryRetrieveAuthenticatedAnimeItem(int id,ref IAnimeData reference)
        {
            if (!Creditentials.Authenticated)
                return false;
            try
            {
                reference = _allAnimeItemsCache[Creditentials.UserName.ToLower()].LoadedAnime.First(abstraction => abstraction.Id == id).AnimeItem;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void RetrieveAnimeEntries(string source,out List<AnimeItemAbstraction> loadedItems  ,out DateTime time )
        {
            AnimeUserCache data;
            _allAnimeItemsCache.TryGetValue(source.ToLower(), out data);
            time = data?.LastUpdate ?? DateTime.Now;

            loadedItems = data?.LoadedAnime ?? new List<AnimeItemAbstraction>();
        }

        public void AddAnimeEntry(string source, AnimeItemAbstraction item)
        {
            source = source.ToLower();
            if (_allAnimeItemsCache[source] != null)
            {
                _allAnimeItemsCache[source].LoadedAnime.Add(item);
            }
        }

        public void RemoveAnimeEntry(string source, AnimeItemAbstraction item)
        {
            source = source.ToLower();
            if (_allAnimeItemsCache[source] != null)
            {
                _allAnimeItemsCache[source].LoadedAnime.Remove(item);
            }
        }

        internal void PurgeUserCache(string source)
        {
            _allAnimeItemsCache[source.ToLower()].LoadedAnime.Clear();
            _allAnimeItemsCache[source.ToLower()] = null;
        }

        //Profile
        public void SaveProfileData(ProfileData data)
        {
            _profileDataCache = new Tuple<DateTime, ProfileData>(DateTime.Now,data);
        }

        private ProfileData RetrieveProfileData()
        {
            if (_profileDataCache == null)
                return null;
            TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(_profileDataCache.Item1);
            if (diff.TotalSeconds < 3600)
                return _profileDataCache.Item2;
            return null;
        }
        //Season
        public void SaveSeasonData(List<AnimeItemAbstraction> data)
        {
            _seasonalAnimeCache = data;
        }

        public List<AnimeItemAbstraction> RetrieveSeasonData()
        {
            return _seasonalAnimeCache;
        }

        public void ClearAnimeItemsForSource(string userName)
        {
            _allAnimeItemsCache[userName.ToLower()].LoadedAnime.Clear();
        }
        #endregion

        #region LogInOut
        public void LogOut()
        {
            foreach (var userCach in _allAnimeItemsCache.SelectMany(animeUserCach => animeUserCach.Value.LoadedAnime))
            {
                userCach.SetAuthStatus(false, true);
            }
            foreach (var animeItemAbstraction in _seasonalAnimeCache)
            {
                animeItemAbstraction.SetAuthStatus(false, true);
            }
        }

        public void LogIn()
        {
            _seasonalAnimeCache.Clear();
            try
            {
                _allAnimeItemsCache[Creditentials.UserName.ToLower()].LoadedAnime.Clear();
                _allAnimeItemsCache[Creditentials.UserName.ToLower()] = null;
            }
            catch (Exception) { /* ignored */ }
        }
        #endregion




    }
}
