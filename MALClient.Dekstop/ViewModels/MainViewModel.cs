using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm;
using MalClient.Shared.Delegates;
using MalClient.Shared.Models.MalSpecific;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Main;
using MALClient.Pages;
using MALClient.Pages.Messages;
using MALClient.UserControls;
using MALClient.Utils.Managers;
using AnimeDetailsPage = MALClient.Pages.Off.AnimeDetailsPage;
using AnimeListPage = MALClient.Pages.Main.AnimeListPage;
using AnimeSearchPage = MALClient.Pages.Main.AnimeSearchPage;
using CalendarPage = MALClient.Pages.Main.CalendarPage;
using HamburgerControl = MALClient.UserControls.New.HamburgerControl;
using HummingbirdProfilePage = MALClient.Pages.Main.HummingbirdProfilePage;
using LogInPage = MALClient.Pages.Main.LogInPage;
using MalArticlesPage = MALClient.Pages.Main.MalArticlesPage;
using ProfilePage = MALClient.Pages.Main.ProfilePage;
using RecomendationsPage = MALClient.Pages.Main.RecomendationsPage;
using SettingsPage = MALClient.Pages.Off.SettingsPage;

namespace MALClient.ViewModels
{
    

    public interface IMainViewInteractions
    {
        HamburgerControl Hamburger { get; }
        Grid GridRootContent { get; }
        Image Logo { get; }
        Tuple<int, string> InitDetails { get; }
        Storyboard PinDialogStoryboard { get; }
        Storyboard CurrentStatusStoryboard { get; }
        Storyboard CurrentOffStatusStoryboard { get; }
        Storyboard HidePinDialogStoryboard { get; }
        Storyboard CurrentOffSubStatusStoryboard { get; }
        void Navigate(Type page, object args = null);
        void NavigateOff(Type page, object args = null);
        void SearchInputFocus(FocusState state);
        void InitSplitter();
    }

    public class MainViewModel : ViewModelBase , IMainViewModel
    {
        private Tuple<PageIndex, object> _postponedNavigationArgs;
        private bool? _searchStateBeforeNavigatingToSearch;

        private bool _subscribed;
        private bool _wasOnDetailsFromSearch;
        public PinTileDialogViewModel PinDialogViewModel { get; } = new PinTileDialogViewModel();

        public PageIndex? CurrentMainPage { get; set; }
        public PageIndex? CurrentMainPageKind { get; set; }
        public PageIndex? CurrentOffPage { get; set; }

        public event OffContentPaneStateChanged OffContentPaneStateChanged;

        public async void Navigate(PageIndex index, object args = null)
        {
            PageIndex? currPage = null;
            PageIndex? currOffPage = null;
            var mainPage = true;
            var originalIndex = index;
            var wasOnSearchPage = SearchToggleLock;
            if (!Credentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                var msg = new MessageDialog("Log in first in order to access this page.");
                await msg.ShowAsync();
                return;
            }
            ScrollToTopButtonVisibility = Visibility.Collapsed;

            DesktopViewModelLocator.Hamburger.UpdateAnimeFiltersSelectedIndex();

            //prepare for some index mess
            if (index == PageIndex.PageMangaList && args == null) // navigating from startup
                args = AnimeListPageNavigationArgs.Manga;

            if (index == PageIndex.PageAbout ||
                index == PageIndex.PageSettings ||
                index == PageIndex.PageAbout ||
                index == PageIndex.PageAnimeDetails ||
                index == PageIndex.PageMessageDetails)
            {
                OffRefreshButtonVisibility = Visibility.Collapsed;
                mainPage = false;
                currOffPage = index;
                if (index != PageIndex.PageAnimeDetails)
                    ViewModelLocator.NavMgr.ResetBackNav();
            }
            else
            {
                RefreshButtonVisibility = Visibility.Collapsed;
                ResetSearchFilter();
                SearchToggleLock = false;
                CurrentStatusSub = "";
                CurrentHintSet = null;
            }

            if (index == PageIndex.PageSeasonal ||
                index == PageIndex.PageMangaList ||
                index == PageIndex.PageTopManga ||
                index == PageIndex.PageTopAnime ||
                index == PageIndex.PageAnimeList)
            {
                if (index == PageIndex.PageSeasonal || index == PageIndex.PageTopAnime ||
                    index == PageIndex.PageTopManga || index == PageIndex.PageMangaList)
                    currPage = index; //used by hamburger's filters
                else
                    currPage = PageIndex.PageAnimeList;
                DesktopViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(true);
                index = PageIndex.PageAnimeList;
            }
            else if (index == PageIndex.PageSearch ||
                     index == PageIndex.PageRecomendations ||
                     index == PageIndex.PageProfile ||
                     index == PageIndex.PageLogIn ||
                     index == PageIndex.PageMangaSearch ||
                     index == PageIndex.PageCalendar ||
                     index == PageIndex.PageArticles ||
                     index == PageIndex.PageNews ||
                     index == PageIndex.PageMessanging)
            {
                DesktopViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageMessanging);
                currPage = index;
            }


            if (index == PageIndex.PageAnimeList && _searchStateBeforeNavigatingToSearch != null)
            {
                SearchToggleStatus = (bool) _searchStateBeforeNavigatingToSearch;
                if (SearchToggleStatus)
                    ShowSearchStuff();
                else
                    HideSearchStuff();
            }
            switch (index)
            {
                case PageIndex.PageAnimeList:
                    if (ViewModelLocator.AnimeList.Initializing)
                    {
                        if (!_subscribed)
                        {
                            ViewModelLocator.AnimeList.Initialized += AnimeListOnInitialized;
                            _subscribed = true;
                        }
                        _postponedNavigationArgs = new Tuple<PageIndex, object>(originalIndex, args);
                        return;
                    }
                    _postponedNavigationArgs = null;
                    ShowSearchStuff();
                    if ((_searchStateBeforeNavigatingToSearch == null || !_searchStateBeforeNavigatingToSearch.Value) &&
                        (wasOnSearchPage || _wasOnDetailsFromSearch))
                    {
                        CurrentSearchQuery = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }
                    if (CurrentMainPageKind == PageIndex.PageAnimeList)
                        ViewModelLocator.AnimeList.Init(args as AnimeListPageNavigationArgs);
                    else
                        View.Navigate(typeof(AnimeListPage), args);
                    break;
                case PageIndex.PageAnimeDetails:
                    var detail = ViewModelLocator.AnimeDetails;
                    detail.DetailImage = null;
                    detail.LeftDetailsRow.Clear();
                    detail.RightDetailsRow.Clear();
                    OffRefreshButtonVisibility = Visibility.Visible;
                    RefreshOffDataCommand = new RelayCommand(() => ViewModelLocator.AnimeDetails.RefreshData());
                    _wasOnDetailsFromSearch = (args as AnimeDetailsPageNavigationArgs).Source == PageIndex.PageSearch;
                    //from search , details are passed instead of being downloaded once more
                    OffContentVisibility = Visibility.Visible;

                    if (CurrentOffPage == PageIndex.PageAnimeDetails)
                        ViewModelLocator.AnimeDetails.Init(args as AnimeDetailsPageNavigationArgs);
                    else
                        View.NavigateOff(typeof(AnimeDetailsPage), args);
                    break;
                case PageIndex.PageSettings:
                    HideSearchStuff();
                    OffContentVisibility = Visibility.Visible;
                    View.NavigateOff(typeof(SettingsPage));
                    break;
                case PageIndex.PageSearch:
                case PageIndex.PageMangaSearch:
                    _searchStateBeforeNavigatingToSearch = SearchToggleStatus;
                    NavigateSearch(args);
                    break;
                case PageIndex.PageLogIn:
                    HideSearchStuff();
                    View.Navigate(typeof(LogInPage));
                    break;
                case PageIndex.PageProfile:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    if (Settings.SelectedApiType == ApiType.Mal)
                        RefreshDataCommand =
                            new RelayCommand(() => DesktopViewModelLocator.ProfilePage.LoadProfileData(null, true));
                    else
                        RefreshDataCommand = new RelayCommand(() => ViewModelLocator.HumProfilePage.Init(true));
                    if (Settings.SelectedApiType == ApiType.Mal)
                    {
                        if (CurrentMainPage == PageIndex.PageProfile)
                            DesktopViewModelLocator.ProfilePage.LoadProfileData(args as ProfilePageNavigationArgs);
                        else
                            View.Navigate(typeof(ProfilePage), args);
                    }

                    else
                        View.Navigate(typeof(HummingbirdProfilePage), args);
                    break;
                case PageIndex.PageRecomendations:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.Recommendations.PopulateData());
                    CurrentStatus = "Recommendations";
                    View.Navigate(typeof(RecomendationsPage), args);
                    break;
                case PageIndex.PageCalendar:
                    HideSearchStuff();
                    //RefreshButtonVisibility = Visibility.Visible;
                    //RefreshDataCommand = new RelayCommand(() => ViewModelLocator.CalendarPage.Init(true));
                    CurrentStatus = "Calendar";
                    View.Navigate(typeof(CalendarPage), args);
                    break;
                case PageIndex.PageArticles:
                case PageIndex.PageNews:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.MalArticles.Init(null); });
                    View.Navigate(typeof(MalArticlesPage), args);
                    break;
                case PageIndex.PageMessanging:
                    HideSearchStuff();
                    CurrentStatus = $"{Credentials.UserName} - Messages";
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.MalMessaging.Init(true); });
                    View.Navigate(typeof(MalMessagingPage), args);
                    break;
                case PageIndex.PageMessageDetails:
                    var msgModel = args as MalMessageModel;
                    CurrentOffStatus = msgModel != null ? $"{msgModel.Sender} - {msgModel.Subject}" : "New Message";
                    OffContentVisibility = Visibility.Visible;
                    View.NavigateOff(typeof(MalMessageDetailsPage), args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (currPage != null)
                CurrentMainPage = currPage;
            if (mainPage)
                CurrentMainPageKind = index;
            if (currOffPage != null)
                CurrentOffPage = currOffPage;
            RaisePropertyChanged(() => SearchToggleLock);
        }

        private void AnimeListOnInitialized()
        {
            ViewModelLocator.AnimeList.Initialized += AnimeListOnInitialized;
            _subscribed = false;
            if (_postponedNavigationArgs != null)
                Navigate(_postponedNavigationArgs.Item1, _postponedNavigationArgs.Item2);
        }

        #region Helpers

        public AnimeListPageNavigationArgs GetCurrentListOrderParams()
        {
            var page = ViewModelLocator.AnimeList;
            return new AnimeListPageNavigationArgs(
                page.SortOption,
                page.CurrentStatus,
                page.SortDescending,
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
                if (Settings.HamburgerMenuDefaultPaneState &&
                    ApplicationView.GetForCurrentView().VisibleBounds.Width > 500)
                {
                    View.Hamburger.Width = 250.0;
                    View.Logo.Visibility = Visibility.Visible;
                }
                else
                {
                    View.Hamburger.Width = 60.0;
                    View.Logo.Visibility = Visibility.Collapsed;
                }


                Navigate(Credentials.Authenticated
                    ? (Settings.DefaultMenuTab == "anime" ? PageIndex.PageAnimeList : PageIndex.PageMangaList)
                    : PageIndex.PageLogIn); //entry point whatnot
                if (value.InitDetails != null)
                    ViewModelLocator.AnimeList.Initialized += AnimeListOnInitializedLoadArgs;
            }
        }

        private void AnimeListOnInitializedLoadArgs()
        {
            Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(View.InitDetails.Item1, View.InitDetails.Item2, null, null));
            ViewModelLocator.AnimeList.Initialized -= AnimeListOnInitializedLoadArgs;
        }

//entry point

        private bool _menuPaneState;

        public bool MenuPaneState
        {
            get { return _menuPaneState; }
            private set
            {
                View.Hamburger.Width = View.Hamburger.Width == 250.0 ? 60 : 250.0;
                View.Logo.Visibility = View.Hamburger.Width == 250.0 ? Visibility.Visible : Visibility.Collapsed;
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
                View.CurrentStatusStoryboard.Begin();
                RaisePropertyChanged(() => CurrentStatus);
            }
        }

        private string _currentStatusSub;

        public string CurrentStatusSub
        {
            get { return _currentStatusSub; }
            set
            {
                _currentStatusSub = value;
                View.CurrentOffSubStatusStoryboard.Begin();
                RaisePropertyChanged(() => CurrentStatusSub);
            }
        }

        private string _currentOffStatus;

        public string CurrentOffStatus
        {
            get { return _currentOffStatus; }
            set
            {
                _currentOffStatus = value;
                View.CurrentOffStatusStoryboard.Begin();
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
                SetSearchHints();
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
                           SetSearchHints();
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

        private ICommand _navigateBackCommand;

        public ICommand NavigateBackCommand => _navigateBackCommand ??
                                               (_navigateBackCommand =
                                                   new RelayCommand(ViewModelLocator.NavMgr.CurrentViewOnBackRequested));

        private ICommand _navigateMainBackCommand;

        public ICommand NavigateMainBackCommand => _navigateMainBackCommand ??
                                                   (_navigateMainBackCommand =
                                                       new RelayCommand(ViewModelLocator.NavMgr.CurrentMainViewOnBackRequested));


        private ICommand _hideOffContentCommand;

        public ICommand HideOffContentCommand
        {
            get
            {
                return _hideOffContentCommand ??
                       (_hideOffContentCommand = new RelayCommand(() =>
                       {
                           ViewModelLocator.AnimeDetails.Id = -1;
                           OffContentVisibility = Visibility.Collapsed;
                           ViewModelLocator.NavMgr.ResetBackNav();
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


        private Visibility _navigateBackButtonVisibility = Visibility.Collapsed;

        public Visibility NavigateBackButtonVisibility
        {
            get { return _navigateBackButtonVisibility; }
            set
            {
                _navigateBackButtonVisibility = value;
                RaisePropertyChanged(() => NavigateBackButtonVisibility);
            }
        }

        private Visibility _navigateMainBackButtonVisibility = Visibility.Collapsed;

        public Visibility NavigateMainBackButtonVisibility
        {
            get { return _navigateMainBackButtonVisibility; }
            set
            {
                _navigateMainBackButtonVisibility = value;
                RaisePropertyChanged(() => NavigateMainBackButtonVisibility);
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
                    OffContentPaneStateChanged?.Invoke();
                    MainContentColumnSpan = 3;
                }
                View.GridRootContent.UpdateLayout();
            }
        }

        public List<string> SearchHints { get; set; }

        private List<string> _currentHintSet;

        public List<string> CurrentHintSet
        {
            get { return _currentHintSet; }
            set
            {
                _currentHintSet = value;
                RaisePropertyChanged(() => CurrentHintSet);
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
            View.Navigate(typeof(AnimeSearchPage), args);
        }

        private void SetSearchHints()
        {
            if (CurrentMainPageKind == PageIndex.PageAnimeList)
                CurrentHintSet =
                    SearchHints.Where(
                        s => s.StartsWith(CurrentSearchQuery ?? "", StringComparison.CurrentCultureIgnoreCase))
                        .Take(4)
                        .ToList();
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