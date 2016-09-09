using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.XShared.ViewModels
{
    public abstract class MainViewModelBase : ViewModelBase
    {
        public static Tuple<int, string> InitDetails;

        protected Tuple<PageIndex, object> _postponedNavigationArgs;
        protected bool? _searchStateBeforeNavigatingToSearch;

        protected bool _subscribed;
        protected bool _wasOnDetailsFromSearch;

        public virtual event OffContentPaneStateChanged OffContentPaneStateChanged;
        public virtual event NavigationRequest MainNavigationRequested;
        public virtual event NavigationRequest OffNavigationRequested;
        public virtual event SearchQuerySubmitted OnSearchQuerySubmitted;
        public virtual event SearchDelayedQuerySubmitted OnSearchDelayedQuerySubmitted;


        protected abstract void CurrentStatusStoryboardBegin();
        protected abstract void CurrentOffSubStatusStoryboardBegin();
        protected abstract void CurrentOffStatusStoryboardBegin();

        protected virtual void InitSplitter()
        {
            
        }

        public PageIndex? CurrentMainPage { get; set; }
        public PageIndex? CurrentMainPageKind { get; set; }
        public PageIndex? CurrentOffPage { get; set; }



        public abstract void Navigate(PageIndex index, object args = null);

        protected void AnimeListOnInitialized()
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
                page.DisplayMode)
            { ResetBackNav = page.ResetedNavBack };
        }

        protected void AnimeListOnInitializedLoadArgs()
        {
            Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(InitDetails.Item1, InitDetails.Item2, null, null));
            ViewModelLocator.AnimeList.Initialized -= AnimeListOnInitializedLoadArgs;
        }
        #endregion

        #region Properties

        public bool SearchToggleLock { get; protected set; }

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

        private string _currentStatus;

        public string CurrentStatus
        {
            get { return _currentStatus; }
            set
            {
                if (_currentStatus == value)
                    return;
                _currentStatus = value;
                CurrentStatusStoryboardBegin();
                RaisePropertyChanged(() => CurrentStatus);
            }
        }

        private string _currentStatusSub;

        public string CurrentStatusSub
        {
            get { return _currentStatusSub; }
            set
            {
                if (_currentStatusSub == value)
                    return;
                _currentStatusSub = value;
                CurrentOffSubStatusStoryboardBegin();
                RaisePropertyChanged(() => CurrentStatusSub);
            }
        }

        private string _currentOffStatus;

        public virtual string CurrentOffStatus
        {
            get { return _currentOffStatus; }
            set
            {
                _currentOffStatus = value;
                CurrentOffStatusStoryboardBegin();
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

                if (string.IsNullOrEmpty(value))
                    OnSearchQuerySubmitted?.Invoke(CurrentSearchQuery);
                else
                    SubmitSearchQueryWithDelayCheck();
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
            protected set
            {
                _refreshDataCommand = value;
                RaisePropertyChanged(() => RefreshDataCommand);
            }
        }

        private ICommand _refreshOffDataCommand;

        public ICommand RefreshOffDataCommand
        {
            get { return _refreshOffDataCommand; }
            protected set
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
                                                   new RelayCommand(ViewModelLocator.NavMgr.CurrentOffViewOnBackRequested));

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
                           OffContentVisibility = false;
                           ViewModelLocator.NavMgr.ResetOffBackNav();
                       }));
            }
        }

        private bool _refreshButtonVisibility;

        public bool RefreshButtonVisibility
        {
            get { return _refreshButtonVisibility; }
            set
            {
                _refreshButtonVisibility = value;
                RaisePropertyChanged(() => RefreshButtonVisibility);
            }
        }

        private bool _offRefreshButtonVisibility;

        public bool OffRefreshButtonVisibility
        {
            get { return _offRefreshButtonVisibility; }
            set
            {
                _offRefreshButtonVisibility = value;
                RaisePropertyChanged(() => OffRefreshButtonVisibility);
            }
        }


        private bool _navigateOffBackButtonVisibility = false;

        public bool NavigateOffBackButtonVisibility
        {
            get { return _navigateOffBackButtonVisibility; }
            set
            {
                _navigateOffBackButtonVisibility = value;
                RaisePropertyChanged(() => NavigateOffBackButtonVisibility);
            }
        }

        private bool _navigateMainBackButtonVisibility;

        public bool NavigateMainBackButtonVisibility
        {
            get { return _navigateMainBackButtonVisibility; }
            set
            {
                _navigateMainBackButtonVisibility = value;
                RaisePropertyChanged(() => NavigateMainBackButtonVisibility);
            }
        }

        private bool _scrollToTopButtonVisibility;

        public bool ScrollToTopButtonVisibility
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

        private bool _searchFilterButtonVisibility;

        public bool SearchFilterButtonVisibility
        {
            get { return _searchFilterButtonVisibility; }
            set
            {
                _searchFilterButtonVisibility = value;
                RaisePropertyChanged(() => SearchFilterButtonVisibility);
            }
        }

        private bool _offContentVisibility;

        public bool OffContentVisibility
        {
            get { return _offContentVisibility; }
            set
            {
                _offContentVisibility = value;
                RaisePropertyChanged(() => OffContentVisibility);
                if (value)
                {
                    MainContentColumnSpan = 1;
                    InitSplitter();
                }
                else
                {
                    OffContentPaneStateChanged?.Invoke();
                    MainContentColumnSpan = 3;
                }
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

        private bool _isSearchFilterActive; //new SolidColorBrush(Colors.Black);

        public bool IsSearchFilterActive
        {
            get { return _isSearchFilterActive; }
            set
            {
                _isSearchFilterActive = value;
                RaisePropertyChanged(() => IsSearchFilterActive);
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
            {
                OnSearchQuerySubmitted?.Invoke(CurrentSearchQuery);
            }
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

        public async void SubmitSearchQueryWithDelayCheck()
        {
            string query = CurrentSearchQuery;
            await Task.Delay(500);
            if (query == CurrentSearchQuery)
                OnSearchDelayedQuerySubmitted?.Invoke(CurrentSearchQuery);
        }

        #endregion

        #region UIHelpers

        public void PopulateSearchFilters(HashSet<string> filters)
        {
            SearchFilterOptions.Clear();
            if (filters.Count <= 1 || (CurrentMainPage.Value != PageIndex.PageSearch && CurrentMainPage.Value != PageIndex.PageMangaSearch))
            {
                SearchFilterButtonVisibility = false;
                return;
            }
            SearchFilterButtonVisibility = true;
            foreach (var filter in filters)
                SearchFilterOptions.Add(filter);
            SearchFilterOptions.Add("None");
            SearchFilterSelectedIndex = SearchFilterOptions.Count - 1;
        }

        private void OnSearchFilterSelected()
        {
            if (SearchFilterSelectedIndex < 0)
            {
                SearchFilterButtonVisibility = false;
                return;
            }
            IsSearchFilterActive = SearchFilterSelectedIndex != SearchFilterOptions.Count - 1;

            ViewModelLocator.SearchPage.SubmitFilter(SearchFilterOptions[SearchFilterSelectedIndex]);
        }

        protected void ResetSearchFilter()
        {
            SearchFilterButtonVisibility = false;
            IsSearchFilterActive = false;
            SearchFilterOptions.Clear();
        }

        protected void ShowSearchStuff()
        {
            SearchToggleVisibility = true;
            if (SearchToggleStatus)
                SearchInputVisibility = true;
        }

        protected void HideSearchStuff()
        {
            SearchToggleStatus = false;
            SearchInputVisibility = false;
            SearchToggleVisibility = false;
        }

        protected void ToggleSearchStuff()
        {
            SearchToggleStatus = true;
            SearchInputVisibility = true;
        }

        protected void UnToggleSearchStuff()
        {
            SearchToggleStatus = false;
            SearchInputVisibility = false;
        }

        #endregion
    }
}
