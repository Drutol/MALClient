using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Pages;
using MALClient.UserControls;

namespace MALClient.ViewModels
{
    public interface IMainViewInteractions
    {
        void Navigate(Type page, object args = null);
        void NavigateOff(Type page, object args = null);
        void SearchInputFocus(FocusState state);
        void InitSplitter();
        HamburgerControl Hamburger { get; }
        Grid GridRootContent { get; }
    }

    public class MainViewModel : ViewModelBase
    {
        private bool? _searchStateBeforeNavigatingToSearch;
        private bool _wasOnDetailsFromSearch;

        public PageIndex CurrentMainPage { get; set; }
        public PageIndex CurrentOffPage { get; set; }

        internal async Task Navigate(PageIndex index, object args = null)
        {
            var wasOnSearchPage = SearchToggleLock;
            SearchToggleLock = false;
            
            await Task.Delay(1);
            if (!Credentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                var msg = new MessageDialog("Log in first in order to access this page.");
                await msg.ShowAsync();
                return;
            }
            ScrollToTopButtonVisibility = Visibility.Collapsed;
            RefreshButtonVisibility = Visibility.Collapsed;
            OffRefreshButtonVisibility = Visibility.Collapsed;

            ViewModelLocator.Hamburger.UpdateAnimeFiltersSelectedIndex();


            //prepare for some index mess
            if (index == PageIndex.PageMangaList && args == null) // navigating from startup
                args = AnimeListPageNavigationArgs.Manga;

            if (index == PageIndex.PageAbout ||
                index == PageIndex.PageSettings ||
                index == PageIndex.PageAbout)
            {
                CurrentOffPage = index;
                NavMgr.ResetBackNav();
            }

            if (index == PageIndex.PageSeasonal ||
                index == PageIndex.PageMangaList ||
                index == PageIndex.PageTopManga ||
                index == PageIndex.PageTopAnime ||
                index == PageIndex.PageAnimeList)
            {
                if(index == PageIndex.PageSeasonal || index == PageIndex.PageTopAnime || index == PageIndex.PageTopManga || index==PageIndex.PageMangaList)
                    CurrentMainPage = index; //used by hamburger's filters
                else
                    CurrentMainPage = PageIndex.PageAnimeList;
                ViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(true);
                index = PageIndex.PageAnimeList;
                
            }
            else if(index == PageIndex.PageSearch || 
                index == PageIndex.PageRecomendations ||
                index == PageIndex.PageProfile ||
                index == PageIndex.PageLogIn ||
                index == PageIndex.PageMangaSearch)
            {
                ViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(false);
                CurrentMainPage = index;
            }




            //ViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageAnimeList);

            if (index == PageIndex.PageAnimeList && _searchStateBeforeNavigatingToSearch != null)
            {
                SearchToggleStatus = (bool) _searchStateBeforeNavigatingToSearch;
                if (SearchToggleStatus)
                    ShowSearchStuff();
                else
                    HideSearchStuff();
            }
            ResetSearchFilter();
            switch (index)
            {
                case PageIndex.PageAnimeList:
                    ShowSearchStuff();
                    if ((_searchStateBeforeNavigatingToSearch == null || !_searchStateBeforeNavigatingToSearch.Value) && (wasOnSearchPage || _wasOnDetailsFromSearch))
                    {
                        CurrentSearchQuery = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }
                    View.Navigate(typeof (AnimeListPage), args);
                    break;
                case PageIndex.PageAnimeDetails:
                    OffRefreshButtonVisibility = Visibility.Visible;
                    RefreshOffDataCommand = new RelayCommand(() => ViewModelLocator.AnimeDetails.RefreshData());
                    _wasOnDetailsFromSearch = (args as AnimeDetailsPageNavigationArgs).Source == PageIndex.PageSearch;
                    //from search , details are passed instead of being downloaded once more
                    OffContentVisibility = Visibility.Visible;
                    View.NavigateOff(typeof (AnimeDetailsPage), args);
                    break;
                case PageIndex.PageSettings:
                    HideSearchStuff();
                    OffContentVisibility = Visibility.Visible;
                    View.NavigateOff(typeof (SettingsPage));
                    break;
                case PageIndex.PageSearch:
                case PageIndex.PageMangaSearch:
                    _searchStateBeforeNavigatingToSearch = SearchToggleStatus;
                    NavigateSearch(args);
                    break;
                case PageIndex.PageLogIn:
                    HideSearchStuff();
                    View.Navigate(typeof (LogInPage));
                    break;
                case PageIndex.PageProfile:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.ProfilePage.LoadProfileData(null,true));
                    View.Navigate(typeof (ProfilePage),args);
                    break;
                case PageIndex.PageRecomendations:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.Recommendations.PopulateData());
                    CurrentStatus = "Recommendations";
                    View.Navigate(typeof (RecomendationsPage), args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            RaisePropertyChanged(() => SearchToggleLock);
        }

        #region Helpers

        internal AnimeListPageNavigationArgs GetCurrentListOrderParams()
        {
            var page = ViewModelLocator.AnimeList;
            return new AnimeListPageNavigationArgs(
                page.SortOption,
                page.CurrentStatus,
                page.SortDescending,
                page.CurrentPosition,
                page.WorkMode,
                page.ListSource,
                page.CurrentSeason,
                page.DisplayMode);
        }

        #endregion

        #region PropertyPairs

        public bool SearchToggleLock { get; private set; }

        private IMainViewInteractions _view;

        public IMainViewInteractions View
        {
            get { return _view; }
            set
            {
                _view = value;
                Navigate(Credentials.Authenticated
                    ? (Settings.DefaultMenuTab == "anime" ? PageIndex.PageAnimeList : PageIndex.PageMangaList)
                    : PageIndex.PageLogIn);
                //entry point whatnot
            }
        } //entry point

        private bool _menuPaneState;

        public bool MenuPaneState
        {
            get { return _menuPaneState; }
            private set
            {
                View.Hamburger.Width = View.Hamburger.Width == 220.0 ? 0 : 220.0;
                _menuPaneState = value;              
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
                ReverseSearchInput();
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

        private string _currentOffStatus;

        public string CurrentOffStatus
        {
            get { return _currentOffStatus; }
            set
            {
                _currentOffStatus = value;
                RaisePropertyChanged(() => CurrentOffStatus);
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

                if (SearchToggleLock) return;

                ViewModelLocator.AnimeList.RefreshList(true);
            }
        }

        private ICommand _reversePaneCommand;

        public ICommand ReversePaneCommand
        {
            get
            {
                return _reversePaneCommand ??
                       (_reversePaneCommand = new RelayCommand(() => MenuPaneState = !MenuPaneState));
            }
        }

        private ICommand _toggleSearchCommand;

        public ICommand ToggleSearchCommand
        {
            get
            {
                return _toggleSearchCommand ??
                       (_toggleSearchCommand = new RelayCommand(() =>
                       {
                           if (!SearchToggleLock) return;
                           if (!string.IsNullOrWhiteSpace(CurrentSearchQuery))
                               OnSearchInputSubmit();
                       }));
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

        private ICommand _refreshOffDataCommand;

        public ICommand RefreshOffDataCommand
        {
            get { return _refreshOffDataCommand; }
            private set
            {
                _refreshOffDataCommand = value;
                RaisePropertyChanged(() => RefreshOffDataCommand);
            }
        }

        private ICommand _goTopCommand;

        public ICommand GoTopCommand
        {
            get
            {
                return _goTopCommand ??
                       (_goTopCommand = new RelayCommand(() => ViewModelLocator.AnimeList.ScrollToTop()));
            }
        }

        public ICommand _navigateBackCommand;
        public ICommand NavigateBackCommand
        {
            get
            {
                return _navigateBackCommand ??
                       (_navigateBackCommand = new RelayCommand(NavMgr.CurrentViewOnBackRequested));
            }
        }


        private ICommand _hideOffContentCommand;

        public ICommand HideOffContentCommand
        {
            get
            {
                return _hideOffContentCommand ??
                       (_hideOffContentCommand = new RelayCommand(() =>
                       {
                           OffContentVisibility = Visibility.Collapsed;
                           NavMgr.ResetBackNav();
                       }));
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

        private Visibility _offRefreshButtonVisibility;

        public Visibility OffRefreshButtonVisibility
        {
            get { return _offRefreshButtonVisibility; }
            set
            {
                _offRefreshButtonVisibility = value;
                RaisePropertyChanged(() => OffRefreshButtonVisibility);
            }
        }


        public Visibility _navigateBackButtonVisibility = Visibility.Collapsed;
        public Visibility NavigateBackButtonVisibility
        {
            get { return _navigateBackButtonVisibility; }
            set
            {
                _navigateBackButtonVisibility = value;
                RaisePropertyChanged(() => NavigateBackButtonVisibility);
            }
        }

        private Visibility _scrollToTopButtonVisibility = Visibility.Collapsed;

        public Visibility ScrollToTopButtonVisibility
        {
            get { return _scrollToTopButtonVisibility; }
            set
            {
                if (value == _scrollToTopButtonVisibility)
                    return;
                _scrollToTopButtonVisibility = value;
                RaisePropertyChanged(() => ScrollToTopButtonVisibility);
            }
        }

        private Visibility _searchFilterButtonVisibility = Visibility.Collapsed;

        public Visibility SearchFilterButtonVisibility
        {
            get { return _searchFilterButtonVisibility; }
            set
            {
                _searchFilterButtonVisibility = value;
                RaisePropertyChanged(() => SearchFilterButtonVisibility);
            }
        }

        private Visibility _offContentVisibility = Visibility.Collapsed;

        public Visibility OffContentVisibility
        {
            get { return _offContentVisibility; }
            set
            {
                _offContentVisibility = value;
                RaisePropertyChanged(() => OffContentVisibility);
                if (value == Visibility.Visible)
                {
                    MainContentColumnSpan = 1;
                    View.InitSplitter();
                }
                else
                {
                    MainContentColumnSpan = 3;
                }
                View.GridRootContent.UpdateLayout();
            }
        }

        private Brush _searchFilterButtonBrush = new SolidColorBrush(Colors.Black);

        public Brush SearchFilterButtonBrush
        {
            get { return _searchFilterButtonBrush; }
            set
            {
                _searchFilterButtonBrush = value;
                RaisePropertyChanged(() => SearchFilterButtonBrush);
            }
        }

        private double _offContentStatusBarWidth = 420;

        public double OffContentStatusBarWidth
        {
            get { return _offContentStatusBarWidth; }
            set
            {
                _offContentStatusBarWidth = value;
                RaisePropertyChanged(() => OffContentStatusBarWidth);
            }
        }



        private int _searchFilterSelectedIndex;

        public int SearchFilterSelectedIndex
        {
            get { return _searchFilterSelectedIndex; }
            set
            {
                _searchFilterSelectedIndex = value;
                OnSearchFilterSelected();
                RaisePropertyChanged(() => SearchFilterSelectedIndex);
            }
        }

        private int _mainContentColumnSpan = 1;

        public int MainContentColumnSpan
        {
            get { return _mainContentColumnSpan; }
            set
            {
                _mainContentColumnSpan = value;
                RaisePropertyChanged(() => MainContentColumnSpan);
            }
        }

        public ObservableCollection<string> SearchFilterOptions { get; } = new ObservableCollection<string>();



        #endregion

        #region Search

        private void ReverseSearchInput()
        {
            if (SearchToggleLock)
            {
                if (!string.IsNullOrWhiteSpace(CurrentSearchQuery))
                    OnSearchInputSubmit();
                return;
            }
            SearchInputVisibility = SearchToggleStatus;
            if (!SearchToggleLock)
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
            if (SearchToggleLock)
                ViewModelLocator.SearchPage.SubmitQuery(CurrentSearchQuery);
        }

        private void NavigateSearch(object args)
        {
            SearchToggleLock = true;
            ShowSearchStuff();
            ToggleSearchStuff();
            if (string.IsNullOrWhiteSpace((args as SearchPageNavigationArgs).Query))
            {
                View.SearchInputFocus(FocusState.Keyboard);
                (args as SearchPageNavigationArgs).Query = CurrentSearchQuery;
            }
            View.Navigate(typeof (AnimeSearchPage), args);
        }

        #endregion

        #region UIHelpers

        public void PopulateSearchFilters(HashSet<string> filters)
        {
            SearchFilterOptions.Clear();
            if (filters.Count <= 1)
            {
                SearchFilterButtonVisibility = Visibility.Collapsed;
                return;
            }
            SearchFilterButtonVisibility = Visibility.Visible;
            foreach (var filter in filters)
                SearchFilterOptions.Add(filter);
            SearchFilterOptions.Add("None");
            SearchFilterSelectedIndex = SearchFilterOptions.Count - 1;
        }

        private void OnSearchFilterSelected()
        {
            if (SearchFilterSelectedIndex < 0)
            {
                SearchFilterButtonVisibility = Visibility.Collapsed;
                return;
            }
            if (SearchFilterSelectedIndex == SearchFilterOptions.Count - 1)
                SearchFilterButtonBrush =
                    new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Light
                        ? Colors.Black
                        : Colors.FloralWhite);
            else
                SearchFilterButtonBrush = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;

            ViewModelLocator.SearchPage.SubmitFilter(SearchFilterOptions[SearchFilterSelectedIndex]);
        }

        private void ResetSearchFilter()
        {
            SearchFilterButtonVisibility = Visibility.Collapsed;
            SearchFilterButtonBrush =
                new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Light
                    ? Colors.Black
                    : Colors.FloralWhite);
            SearchFilterOptions.Clear();
        }

        private void ShowSearchStuff()
        {
           SearchToggleVisibility = true;
            if (SearchToggleStatus)
                SearchInputVisibility = true;
        }

        private void HideSearchStuff()
        {
            SearchToggleStatus = false;
            SearchInputVisibility = false;
            SearchToggleVisibility = false;       
        }

        private void ToggleSearchStuff()
        {
            SearchToggleStatus = true;
            SearchInputVisibility = true;
           
        }

        private void UnToggleSearchStuff()
        {
            SearchToggleStatus = false;
            SearchInputVisibility = false;         
        }

        #endregion
    }
}