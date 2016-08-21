using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.PlayTo;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm;
using MalClient.Shared.Delegates;
using MalClient.Shared.Models;
using MalClient.Shared.Models.MalSpecific;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Main;
using MALClient.Pages;
using MALClient.Pages.Forums;
using MALClient.Pages.Main;
using MALClient.Pages.Messages;
using MALClient.Pages.Off;

namespace MALClient.ViewModels
{
    public class MainViewModel : ViewModelBase , IMainViewModel
    {
        public static Tuple<int, string> InitDetails;

        private bool? _searchStateBeforeNavigatingToSearch;
        private bool _wasOnDetailsFromSearch;
        public PageIndex? LastIndex { get; private set; }

        public PinTileDialogViewModel PinDialogViewModel { get; } = new PinTileDialogViewModel();

        public event NavigationRequest NavigationRequested;

        public async void Navigate(PageIndex index, object args = null)
        {
            PageIndex originalIndex = index;
            var wasOnSearchPage = SearchToggleLock;
            SearchToggleLock = false;
            if(View.CurrentDisplayMode == SplitViewDisplayMode.CompactOverlay)
                MenuPaneState = false;
            CurrentStatusSub = "";
            IsCurrentStatusSelectable = false;
            if (!Credentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                var msg = new MessageDialog("Log in first in order to access this page.");
                await msg.ShowAsync();
                return;
            }
            Utilities.TelemetryTrackEvent(TelemetryTrackedEvents.Navigated, index.ToString());
            ScrollToTopButtonVisibility = Visibility.Collapsed;
            RefreshButtonVisibility = Visibility.Collapsed;

            if (index == PageIndex.PageMangaList && args == null) // navigating from startup
                args = AnimeListPageNavigationArgs.Manga;

            if (index == PageIndex.PageSeasonal ||
                index == PageIndex.PageMangaList ||
                index == PageIndex.PageTopManga ||
                index == PageIndex.PageTopAnime)
                index = PageIndex.PageAnimeList;




            MobileViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageAnimeList ||
                                                                    index == PageIndex.PageMessanging ||
                                                                    index == PageIndex.PageForumIndex);

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
                    if (MobileViewModelLocator.AnimeList.Initializing)
                    {
                        if (!_subscribed)
                        {
                            MobileViewModelLocator.AnimeList.Initialized += AnimeListOnInitialized;
                            _subscribed = true;
                        }
                        _postponedNavigationArgs = new Tuple<PageIndex, object>(originalIndex, args);
                        return;
                    }
                    MobileViewModelLocator.Hamburger.SetActiveButton(HamburgerButtons.AnimeList);
                    ShowSearchStuff();
                    if ((_searchStateBeforeNavigatingToSearch == null || !_searchStateBeforeNavigatingToSearch.Value) &&
                        (wasOnSearchPage || _wasOnDetailsFromSearch))
                    {
                        CurrentSearchQuery = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }
                    if (LastIndex == PageIndex.PageAnimeList)
                        MobileViewModelLocator.AnimeList.Init(args as AnimeListPageNavigationArgs);
                    else
                        NavigationRequested?.Invoke(typeof(AnimeListPage), args);
                    break;
                case PageIndex.PageAnimeDetails:
                    HideSearchStuff();
                    var detail = ViewModelLocator.AnimeDetails;
                    detail.DetailImage = null;
                    detail.LeftDetailsRow.Clear();
                    detail.RightDetailsRow.Clear();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.AnimeDetails.RefreshData());
                    _wasOnDetailsFromSearch = (args as AnimeDetailsPageNavigationArgs).Source == PageIndex.PageSearch;
                    //from search , details are passed instead of being downloaded once more
                    if (LastIndex == PageIndex.PageAnimeDetails)
                        ViewModelLocator.AnimeDetails.Init(args as AnimeDetailsPageNavigationArgs);
                    else
                        NavigationRequested?.Invoke(typeof(AnimeDetailsPage), args);
                    break;
                case PageIndex.PageSettings:
                    HideSearchStuff();
                    NavigationRequested?.Invoke(typeof(SettingsPage));
                    break;
                case PageIndex.PageSearch:
                case PageIndex.PageMangaSearch:
                    if(LastIndex != PageIndex.PageSearch && LastIndex != PageIndex.PageMangaSearch && LastIndex != PageIndex.PageCharacterSearch)
                        _searchStateBeforeNavigatingToSearch = SearchToggleStatus;
                    NavigateSearch(args);
                    break;
                case PageIndex.PageLogIn:
                    HideSearchStuff();
                    NavigationRequested?.Invoke(typeof(LogInPage));
                    break;
                case PageIndex.PageProfile:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Collapsed;
                    if (Settings.SelectedApiType == ApiType.Mal)
                    {
                        if (LastIndex == PageIndex.PageProfile)
                            MobileViewModelLocator.ProfilePage.LoadProfileData(args as ProfilePageNavigationArgs);
                        else
                            NavigationRequested?.Invoke(typeof(ProfilePage), args);
                    }
                    else
                        NavigationRequested?.Invoke(typeof(HummingbirdProfilePage), args);
                    break;
                case PageIndex.PageRecomendations:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.Recommendations.PopulateData());
                    CurrentStatus = "Recommendations";
                    NavigationRequested?.Invoke(typeof(RecomendationsPage), args);
                    break;
                case PageIndex.PageCalendar:
                    HideSearchStuff();
                    CurrentStatus = "Calendar";
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.CalendarPage.Init(true); });
                    NavigationRequested?.Invoke(typeof(CalendarPage), args);
                    break;
                case PageIndex.PageArticles:
                case PageIndex.PageNews:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.MalArticles.Init(null));
                    if (LastIndex == PageIndex.PageArticles)
                        ViewModelLocator.MalArticles.Init(args as MalArticlesPageNavigationArgs);
                    else
                        NavigationRequested?.Invoke(typeof(MalArticlesPage), args);
                    break;
                case PageIndex.PageMessanging:
                    HideSearchStuff();
                    CurrentStatus = $"{Credentials.UserName} - Messages";
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.MalMessaging.Init(true); });
                    NavigationRequested?.Invoke(typeof(MalMessagingPage), args);
                    break;
                case PageIndex.PageMessageDetails:
                    var msgModel = args as MalMessageDetailsNavArgs;
                    CurrentOffStatus = msgModel.WorkMode == MessageDetailsWorkMode.Message
                        ? (msgModel.Arg != null
                            ? $"{(msgModel.Arg as MalMessageModel)?.Sender} - {(msgModel.Arg as MalMessageModel)?.Subject}"
                            : "New Message")
                        : $"Comments {Credentials.UserName} - {(msgModel.Arg as MalComment)?.User.Name}";
                    NavigationRequested?.Invoke(typeof(MalMessageDetailsPage), args);
                    break;
                case PageIndex.PageForumIndex:
                    HideSearchStuff();
                    CurrentStatus = "Forums";
                    if (args == null || (args as ForumsNavigationArgs)?.Page == ForumsPageIndex.PageIndex)
                    {
                        RefreshButtonVisibility = Visibility.Visible;
                        RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.ForumsIndex.Init(true); });
                    }
                    if (LastIndex == PageIndex.PageForumIndex)
                        ViewModelLocator.ForumsMain.Init(args as ForumsNavigationArgs);
                    else
                        NavigationRequested?.Invoke(typeof(ForumsMainPage), args);
                    break;
                case PageIndex.PageHistory:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.History.Init(null, true); });
                    CurrentStatus = $"History - {(args as HistoryNavigationArgs)?.Source ?? Credentials.UserName}";
                    NavigationRequested?.Invoke(typeof(HistoryPage), args);
                    break;
                case PageIndex.PageCharacterDetails:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.CharacterDetails.RefreshData());
                    OffContentVisibility = Visibility.Visible;

                    if (CurrentOffPage == PageIndex.PageCharacterDetails)
                        ViewModelLocator.CharacterDetails.Init(args as CharacterDetailsNavigationArgs);
                    else
                        NavigationRequested?.Invoke(typeof(CharacterDetailsPage), args);
                    break;
                case PageIndex.PageStaffDetails:
                    HideSearchStuff();
                    RefreshButtonVisibility = Visibility.Visible;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.StaffDetails.RefreshData());
                    OffContentVisibility = Visibility.Visible;

                    if (CurrentOffPage == PageIndex.PageStaffDetails)
                        ViewModelLocator.StaffDetails.Init(args as StaffDetailsNaviagtionArgs);
                    else
                        NavigationRequested?.Invoke(typeof(StaffDetailsPage), args);
                    break;
                case PageIndex.PageCharacterSearch:
                    if (LastIndex != PageIndex.PageSearch && LastIndex != PageIndex.PageMangaSearch && LastIndex != PageIndex.PageCharacterSearch)
                        _searchStateBeforeNavigatingToSearch = SearchToggleStatus;
                    ShowSearchStuff();
                    ToggleSearchStuff();

                    SearchToggleLock = true;

                    NavigationRequested?.Invoke(typeof(CharacterSearchPage));
                    await Task.Delay(10);
                    View.SearchInputFocus(FocusState.Keyboard);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            LastIndex = index;
            RaisePropertyChanged(() => SearchToggleLock);


        }

        private bool _subscribed;
        private Tuple<PageIndex, object> _postponedNavigationArgs;
        private void AnimeListOnInitialized()
        {
            MobileViewModelLocator.AnimeList.Initialized += AnimeListOnInitialized;
            _subscribed = false;
            Navigate(_postponedNavigationArgs.Item1, _postponedNavigationArgs.Item2);
        }

        #region Helpers

        public AnimeListPageNavigationArgs GetCurrentListOrderParams()
        {
            var page = MobileViewModelLocator.AnimeList;
            return new AnimeListPageNavigationArgs(
                page.SortOption,
                page.CurrentStatus,
                page.SortDescending,
                page.WorkMode,
                page.ListSource,
                page.CurrentSeason,
                page.DisplayMode) {ResetBackNav = page.ResetedNavBack};
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
                if (InitDetails != null)
                    MobileViewModelLocator.AnimeList.Initialized += AnimeListOnInitializedLoadArgs;
            }
        }

        private void AnimeListOnInitializedLoadArgs()
        {
            Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(InitDetails.Item1, InitDetails.Item2, null, null));
            MobileViewModelLocator.AnimeList.Initialized -= AnimeListOnInitializedLoadArgs;
        }

        private bool _menuPaneState;

        public bool MenuPaneState
        {
            get { return _menuPaneState; }
            set
            {
                _menuPaneState = value;
                RaisePropertyChanged(() => MenuPaneState);
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

        private bool _isCurrentStatusSelectable;

        public bool IsCurrentStatusSelectable
        {
            get { return _isCurrentStatusSelectable; }
            set
            {
                _isCurrentStatusSelectable = value;
                RaisePropertyChanged(() => IsCurrentStatusSelectable);
            }
        }

        public PageIndex? CurrentOffPage { get; set; }
        public Visibility OffContentVisibility { get; set; }
        public event SearchQuerySubmitted OnSearchQuerySubmitted;
        public event SearchDelayedQuerySubmitted OnSearchDelayedQuerySubmitted;

        private string _currentStatus;

        public string CurrentStatus
        {
            get { return _currentStatus; }
            set
            {
                if(_currentStatus == value)
                    return;
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
                  if(_currentStatusSub == value)
                    return;
                  _currentStatusSub = value;
                  View.CurrentOffSubStatusStoryboard.Begin();
                  RaisePropertyChanged(() => CurrentStatusSub);
              }
        }

        private string _currentSearchQuery;

        public Visibility NavigateMainBackButtonVisibility { get; set; }

        public string CurrentSearchQuery
        {
            get { return SearchToggleStatus ? _currentSearchQuery : ""; }
            set
            {
                _currentSearchQuery = value;
                RaisePropertyChanged(() => CurrentSearchQuery);
                if(LastIndex == PageIndex.PageAnimeList)
                CurrentHintSet =
                    SearchHints.Where(s => s.StartsWith(value, StringComparison.CurrentCultureIgnoreCase))
                        .Take(3)
                        .ToList();
                if (SearchToggleLock) return;

                if (string.IsNullOrEmpty(value))
                    OnSearchQuerySubmitted?.Invoke(CurrentSearchQuery);
                else
                    SubmitSearchQueryWithDelayCheck();
            }
        }

        private async void SubmitSearchQueryWithDelayCheck()
        {
             string query = CurrentSearchQuery;
             await Task.Delay(500);
             if(query == CurrentSearchQuery)
                OnSearchDelayedQuerySubmitted?.Invoke(CurrentSearchQuery);
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

        private ICommand _changeStatusCommand;

        public ICommand ChangeStatusCommand
        {
            get
            {
                return _changeStatusCommand ??
                       (_changeStatusCommand =
                           new RelayCommand<string>(
                               s => MobileViewModelLocator.AnimeList.StatusSelectorSelectedIndex = int.Parse(s)));
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

        private ICommand _goTopCommand;

        public ICommand GoTopCommand
        {
            get
            {
                return _goTopCommand ??
                       (_goTopCommand = new RelayCommand(() => MobileViewModelLocator.AnimeList.ScrollToTop()));
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
                OnSearchQuerySubmitted?.Invoke(CurrentSearchQuery);
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
                OnSearchQuerySubmitted?.Invoke(CurrentSearchQuery);
        }

        public event OffContentPaneStateChanged OffContentPaneStateChanged;
        public ICommand HideOffContentCommand { get; }

        public string CurrentOffStatus
        {
            get { return CurrentStatus; }
            set { CurrentStatus = value; }
        }

        public Visibility NavigateOffBackButtonVisibility { get; set; }

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
            NavigationRequested?.Invoke(typeof(AnimeSearchPage), args);
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