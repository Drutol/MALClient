using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public interface IMainViewInteractions
    {
        void Navigate(Type page, object args = null);
        void SearchInputFocus(FocusState state);
        Tuple<int, string> InitDetails { get; }
        Storyboard PinDialogStoryboard { get; }
        Storyboard CurrentStatusStoryboard { get; }
        Storyboard HidePinDialogStoryboard { get; }
    }

    public class MainViewModel : ViewModelBase
    {        
        private bool? _searchStateBeforeNavigatingToSearch;
        private bool _wasOnDetailsFromSearch;
        public PageIndex? LastIndex { get; private set; }

        public PinTileDialogViewModel PinDialogViewModel { get; } = new PinTileDialogViewModel();

        internal async void Navigate(PageIndex index, object args = null)
        {
            PageIndex originalIndex = index;
            var wasOnSearchPage = SearchToggleLock;
            SearchToggleLock = false;
            MenuPaneState = false;

            await Task.Delay(1);
            if (!Credentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                var msg = new MessageDialog("Log in first in order to access this page.");
                await msg.ShowAsync();
                return;
            }
            ScrollToTopButtonVisibility = Visibility.Collapsed;
            RefreshButtonVisibility = Visibility.Collapsed;

            if (index == PageIndex.PageMangaList && args == null) // navigating from startup
                args = AnimeListPageNavigationArgs.Manga;

            if (index == PageIndex.PageSeasonal ||
                index == PageIndex.PageMangaList ||
                index == PageIndex.PageTopManga ||
                index == PageIndex.PageTopAnime)
                index = PageIndex.PageAnimeList;




            ViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageAnimeList);

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
                    if (!ViewModelLocator.AnimeList.Initiazlized)
                    {
                        if (!_subscribed)
                        {
                            ViewModelLocator.AnimeList.Initialized += AnimeListOnInitialized;
                            _subscribed = true;
                        }
                        _postponedNavigationArgs = new Tuple<PageIndex, object>(originalIndex, args);
                        return;
                    }
                    ShowSearchStuff();
                    if ((_searchStateBeforeNavigatingToSearch == null || !_searchStateBeforeNavigatingToSearch.Value) &&
                        (wasOnSearchPage || _wasOnDetailsFromSearch))
                    {
                        CurrentSearchQuery = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }
                    if (LastIndex == PageIndex.PageAnimeList)
                        ViewModelLocator.AnimeList.Init(args as AnimeListPageNavigationArgs);
                    else
                        View.Navigate(typeof(AnimeListPage), args);
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
                        View.Navigate(typeof(AnimeDetailsPage), args);
                    break;
                case PageIndex.PageSettings:
                    HideSearchStuff();
                    View.Navigate(typeof(SettingsPage));
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
                    RefreshButtonVisibility = Visibility.Collapsed;
                    if (Settings.SelectedApiType == ApiType.Mal)
                    {
                        if (LastIndex == PageIndex.PageProfile)
                            ViewModelLocator.ProfilePage.LoadProfileData(args as ProfilePageNavigationArgs);
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
                    CurrentStatus = "Calendar";
                    View.Navigate(typeof(CalendarPage), args);
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
            ViewModelLocator.AnimeList.Initialized += AnimeListOnInitialized;
            _subscribed = false;
            Navigate(_postponedNavigationArgs.Item1, _postponedNavigationArgs.Item2);
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

        private bool _menuPaneState;

        public bool MenuPaneState
        {
            get { return _menuPaneState; }
            private set
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

        private ICommand _changeStatusCommand;

        public ICommand ChangeStatusCommand
        {
            get
            {
                return _changeStatusCommand ??
                       (_changeStatusCommand =
                           new RelayCommand<string>(
                               s => ViewModelLocator.AnimeList.StatusSelectorSelectedIndex = int.Parse(s)));
            }
        }

        private ICommand _reversePaneCommand;

        public ICommand ReversePaneCommand
        {
            get
            {
                return _reversePaneCommand ??
                       (_reversePaneCommand = new RelayCommand(() => MenuPaneState = true));
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
                       (_goTopCommand = new RelayCommand(() => ViewModelLocator.AnimeList.ScrollToTop()));
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