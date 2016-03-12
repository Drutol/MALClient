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
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Pages;
using MALClient.UserControls;

namespace MALClient.ViewModels
{
    public interface IMainViewInteractions
    {
        void Navigate(Type page, object args = null);
        object GetCurrentContent();
        void SearchInputFocus(FocusState state);
    }

    public class CommandHandler : ICommand
    {
        private readonly Action _action;
        private readonly bool _canExecute;
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
        private Tuple<DateTime, ProfileData> _profileDataCache;
        private bool? _searchStateBeforeNavigatingToSearch;        
        private bool _wasOnDetailsFromSearch;
        private bool _onSearchPage;
        public PageIndex CurrentPage;
        
        #region PropertyPairs
        private IMainViewInteractions _view;
        public IMainViewInteractions View
        {
            get { return _view; }
            set
            {
                _view = value;
                Navigate(Creditentials.Authenticated ? PageIndex.PageAnimeList : PageIndex.PageLogIn);
            }
        } //entry point

        private bool _menuPaneState;
        public bool MenuPaneState
        {
            get { return _menuPaneState; }
            private set
            {
                _menuPaneState = value;
                RaisePropertyChanged(() => MenuPaneState);
                if(value)
                    ViewModelLocator.Hamburger.PaneOpened();
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

                ViewModelLocator.AnimeList.RefreshList(true);
            }
        }

        private ICommand _reversePaneCommand;
        public ICommand ReversePaneCommand
        {
            get {
                return _reversePaneCommand ??
                       (_reversePaneCommand = new RelayCommand(() => MenuPaneState = true));
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

        private ICommand _refreshDataCommand;
        public ICommand RefreshDataCommand
        {
            get { return _refreshDataCommand; }
            private set
            {
                _refreshDataCommand = value;
                RaisePropertyChanged(() => RefreshDataCommand);
            }
        }

        private Visibility _refreshButtonVisibility;
        public Visibility RefreshButtonVisibility
        {
            get { return _refreshButtonVisibility; }
            set
            {
                _refreshButtonVisibility = value;
                RaisePropertyChanged(() => RefreshButtonVisibility);
            }
        }

        #endregion

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

            RefreshButtonVisibility = Visibility.Collapsed;

            if (index == PageIndex.PageSeasonal)
                index = PageIndex.PageAnimeList;

            ViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageAnimeList);

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
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.AnimeDetails.RefreshData());
                    _wasOnDetailsFromSearch = (args as AnimeDetailsPageNavigationArgs).Source == PageIndex.PageSearch;
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
                    CurrentStatus = "About";
                    View.Navigate(typeof(AboutPage));
                    break;
                case PageIndex.PageRecomendations:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.Recommendations.PopulateData());
                    CurrentStatus = "Recommendations";
                    View.Navigate(typeof(RecomendationsPage), args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }

        #region Search
        private void ReverseSearchInput()
        {
            if (_onSearchPage)
            {
                if (!string.IsNullOrWhiteSpace(CurrentSearchQuery))
                    OnSearchInputSubmit();
                return;
            }

            SearchToggleStatus = !SearchToggleStatus;
            SearchInputVisibility = SearchToggleStatus;
            if (!_onSearchPage)
            {
                ViewModelLocator.AnimeList.RefreshList(true);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(CurrentSearchQuery))
                    OnSearchInputSubmit();
            }
        }

        public void OnSearchInputSubmit()
        {
            if (_onSearchPage)
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
        #endregion

        #region UIHelpers
        //public void AnimeListScrollTo(AnimeItem animeItem)
        //{
        //    var content = View.GetCurrentContent();
        //    if (content is AnimeListPage)
        //        ((AnimeListPage)content).ScrollTo(animeItem);
        //}

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
        #endregion

        #region Helpers
        internal AnimeListPageNavigationArgs GetCurrentListOrderParams()
        {
            var page = ViewModelLocator.AnimeList;
            return new AnimeListPageNavigationArgs(
                page.SortOption, 
                page.CurrentStatus, 
                page.SortDescending,
                page.CurrentPage, 
                page.IsSeasonal, 
                page.ListSource , 
                page.CurrentSeason);
        }
        #endregion

        #region SmallDataCaching

        

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
        #endregion


    }
}
