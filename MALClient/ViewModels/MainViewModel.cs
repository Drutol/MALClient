using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Pages;
using MALClient.UserControls;

namespace MALClient.ViewModels
{
    public interface IMainViewNavigate
    {
        void Navigate(Type page, object args = null);
        object GetCurrentContent();
        void SearchInputFocus(FocusState state);
    }

    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class MainViewModel : ViewModelBase
    {
        private readonly Dictionary<string, AnimeUserCache> _allAnimeItemsCache = new Dictionary<string, AnimeUserCache>();
        private List<RecomendationData> _recomendationDataCache = new List<RecomendationData>();
        private List<AnimeItemAbstraction> _seasonalAnimeCache = new List<AnimeItemAbstraction>();
        private Tuple<DateTime, ProfileData> _profileDataCache;
        private bool? _searchStateBeforeNavigatingToSearch;        
        private bool _wasOnDetailsFromSearch;
        private bool _onSearchPage;

        public IMainViewNavigate View { get; set; }

        private bool _menuPaneState;
        public bool MenuPaneState
        {
            get { return _menuPaneState; }
            private set
            {
                _menuPaneState = value;
                RaisePropertyChanged(() => MenuPaneState);
                if(value)
                    new ViewModelLocator().Hamburger.PaneOpened();
            }
        }

        private bool _searchToggleStatus;
        public bool SearchToggleStatus
        {
            get { return _searchToggleStatus; }
            set
            {
                _searchToggleStatus = value;
                RaisePropertyChanged(() => SearchToggleStatus);
            }
        }

        private bool _searchToggleVisibility;
        public bool SearchToggleVisibility
        {
            get { return _searchToggleVisibility; }
            set
            {
                _searchToggleVisibility = value;
                RaisePropertyChanged(() => SearchToggleVisibility);
            }
        }

        private bool _searchInputVisibility;
        public bool SearchInputVisibility
        {
            get { return _searchInputVisibility; }
            set
            {
                _searchInputVisibility = value;
                RaisePropertyChanged(() => SearchInputVisibility);
            }
        }

        private string _currentStatus;
        public string CurrentStatus
        {
            get { return _currentStatus; }
            set
            {
                _currentStatus = value;
                RaisePropertyChanged(() => CurrentStatus);
            }
        }

        private string _currentSearchQuery;
        public string CurrentSearchQuery
        {
            get { return SearchToggleStatus ? _currentSearchQuery : ""; }
            set
            {
                _currentSearchQuery = value;
                RaisePropertyChanged(() => CurrentSearchQuery);

                if (_onSearchPage) return;
                var source = View.GetCurrentContent() as AnimeListPage;
                source.RefreshList(true);
            }
        }

        private ICommand _reversePaneCommand;
        public ICommand ReversePaneCommand
        {
            get {
                return _reversePaneCommand ??
                       (_reversePaneCommand = new CommandHandler(() => MenuPaneState = true, true));
            }
        }

        private ICommand _toggleSearchCommand;
        public ICommand ToggleSearchCommand
        {
            get {
                return _toggleSearchCommand ??
                       (_toggleSearchCommand = new CommandHandler(ReverseSearchInput, true));
            }
        }

        public MainViewModel()
        { 

        }

        internal async Task Navigate(PageIndex index, object args = null)
        {
            var wasOnSearchPage = _onSearchPage;
            _onSearchPage = false;
            MenuPaneState = false;

            if (!Creditentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                var msg = new MessageDialog("Log in first in order to access this page.");
                await msg.ShowAsync();
                return;
            }

            var vl = new ViewModelLocator();
            vl.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageAnimeList);

            if (index == PageIndex.PageAnimeList && _searchStateBeforeNavigatingToSearch != null)
            {
                SearchToggleStatus = (bool)_searchStateBeforeNavigatingToSearch;
                if (SearchToggleStatus)
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
                        CurrentSearchQuery = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }
                    View.Navigate(typeof(AnimeListPage), args);
                    break;
                case PageIndex.PageAnimeDetails:
                    HideSearchStuff();
                    _wasOnDetailsFromSearch = (args as AnimeDetailsPageNavigationArgs).AnimeElement != null;
                    //from search , details are passed instead of being downloaded once more
                    View.Navigate(typeof(AnimeDetailsPage), args);
                    break;
                case PageIndex.PageSettings:
                    HideSearchStuff();
                    View.Navigate(typeof(SettingsPage));
                    break;
                case PageIndex.PageSearch:
                    NavigateSearch(args != null);
                    break;
                case PageIndex.PageLogIn:
                    HideSearchStuff();
                    View.Navigate(typeof(LogInPage));
                    break;
                case PageIndex.PageProfile:
                    HideSearchStuff();
                    View.Navigate(typeof(ProfilePage), RetrieveProfileData());
                    break;
                case PageIndex.PageAbout:
                    HideSearchStuff();
                    SetStatus("About");
                    View.Navigate(typeof(AboutPage));
                    break;
                case PageIndex.PageRecomendations:
                    HideSearchStuff();
                    SetStatus("Recommendations");
                    View.Navigate(typeof(RecomendationsPage), args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }

        private void ReverseSearchInput()
        {
            if(_onSearchPage)
            {
                if (!string.IsNullOrWhiteSpace(CurrentSearchQuery))
                    OnSearchInputSubmit();
                return;
            }

            SearchToggleStatus = !SearchToggleStatus;
            SearchInputVisibility = SearchToggleStatus;
            if(!_onSearchPage)
            {
                (View.GetCurrentContent() as AnimeListPage).RefreshList();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(CurrentSearchQuery))
                    OnSearchInputSubmit();
            }                                   
        }

        public void AnimeListScrollTo(AnimeItem animeItem)
        {
            var content = View.GetCurrentContent();
            if (content is AnimeListPage)
                ((AnimeListPage)content).ScrollTo(animeItem);
        }

        internal AnimeListPageNavigationArgs GetCurrentListOrderParams(bool seasonal)
        {
            var page = View.GetCurrentContent() as AnimeListPage;
            return new AnimeListPageNavigationArgs(page.SortOption, page.CurrentStatus, page.SortDescending,
                page.CurrentPage, seasonal, page.ListSource);
        }

        public void OnSearchInputSubmit()
        {
            if(_onSearchPage)
                (View.GetCurrentContent() as AnimeSearchPage).SubmitQuery(CurrentSearchQuery);
        }

        private void NavigateSearch(bool autoSearch = false)
        {
            _onSearchPage = true;
            _searchStateBeforeNavigatingToSearch = SearchToggleStatus;
            ShowSearchStuff();
            ToggleSearchStuff();
            if (!autoSearch)
                View.SearchInputFocus(FocusState.Keyboard);
            View.Navigate(typeof(AnimeSearchPage), autoSearch ? CurrentSearchQuery : "");
        }

        public void SetStatus(string status)
        {
            CurrentStatus = status;
        }

        private void ShowSearchStuff()
        {
            SearchToggleVisibility = true;
            if (SearchToggleStatus)
                SearchInputVisibility = true;
        }

        private void HideSearchStuff()
        {
            SearchInputVisibility = false;
            SearchToggleVisibility = false;
        }

        private void ToggleSearchStuff()
        {
            SearchInputVisibility = true;
            SearchToggleStatus = true;
        }

        private void UnToggleSearchStuff()
        {
            SearchInputVisibility = false;
            SearchToggleStatus = false;
        }

        #region SmallDataCaching

        public void SaveAnimeEntries(string source, List<AnimeItemAbstraction> items, DateTime updateTime)
        {
            _allAnimeItemsCache[source.ToLower()] = new AnimeUserCache
            {
                LoadedAnime = items,
                LastUpdate = updateTime
            };
            var changedSomething = false;
            foreach (AnimeItemAbstraction animeItemAbstraction in items)
            {
                if (animeItemAbstraction.MyEpisodes == animeItemAbstraction.AllEpisodes)
                {
                    changedSomething = true;
                    DataCache.DeregisterVolatileData(animeItemAbstraction.Id);
                }
            }
            if (changedSomething)
                DataCache.SaveVolatileData();
        }

        public AnimeUserCache RetrieveLoadedAnime()
        {
            if (!Creditentials.Authenticated)
                return null;
            AnimeUserCache data;
            _allAnimeItemsCache.TryGetValue(Creditentials.UserName.ToLower(), out data);
            return data;
        }

        public bool TryRetrieveAuthenticatedAnimeItem(int id, ref IAnimeData reference)
        {
            if (!Creditentials.Authenticated)
                return false;
            try
            {
                reference =
                    _allAnimeItemsCache[Creditentials.UserName.ToLower()].LoadedAnime.First(
                        abstraction => abstraction.Id == id).AnimeItem;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void RetrieveAnimeEntries(string source, out List<AnimeItemAbstraction> loadedItems, out DateTime time)
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
            try
            {
                _allAnimeItemsCache[source.ToLower()].LoadedAnime.Clear();
            }
            catch (Exception)
            {
                //no entry here
            }
        }

        //Profile
        public void SaveProfileData(ProfileData data)
        {
            _profileDataCache = new Tuple<DateTime, ProfileData>(DateTime.Now, data);
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
        //Recommendations
        public void SaveRecommendationsData(List<RecomendationData> data)
        {
            _recomendationDataCache = data;
        }

        public List<RecomendationData> RetrieveRecommendationData()
        {
            return _recomendationDataCache;
        }

        #endregion

        #region LogInOut

        public void LogOut()
        {
            foreach (
                AnimeItemAbstraction userCach in
                    _allAnimeItemsCache.SelectMany(animeUserCach => animeUserCach.Value.LoadedAnime))
            {
                userCach.SetAuthStatus(false, true);
            }
            foreach (AnimeItemAbstraction animeItemAbstraction in _seasonalAnimeCache)
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
            catch (Exception)
            {
                /* ignored */
            }
        }
#endregion
    }
}
