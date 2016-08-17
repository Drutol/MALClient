using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm;
using MalClient.Shared.Comm.Anime;
using MalClient.Shared.Delegates;
using MalClient.Shared.Models;
using MalClient.Shared.Models.Anime;
using MalClient.Shared.Models.AnimeScrapped;
using MalClient.Shared.Models.Library;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Main
{
    public interface IAnimeListViewInteractions
    {
        double ActualWidth { get;  }
        double ActualHeight { get; }
        Flyout FlyoutFilters { get;  }
        MenuFlyout FlyoutSorting { get; }
        ScrollViewer IndefiniteScrollViewer { set; }
        Flyout FlyoutViews { get; }
        Task<ScrollViewer> GetIndefiniteScrollViewer();
        void FlyoutSeasonSelectionHide();
    }


    public class AnimeListViewModel : ViewModelBase
    {
        private const int ItemPrefferedWidth = 385;

        private List<AnimeItemAbstraction> _allLoadedAuthAnimeItems = new List<AnimeItemAbstraction>();
        private List<AnimeItemAbstraction> _allLoadedAuthMangaItems = new List<AnimeItemAbstraction>();
        private List<AnimeItemAbstraction> _allLoadedSeasonalAnimeItems = new List<AnimeItemAbstraction>();
        private List<AnimeItemAbstraction> _allLoadedSeasonalMangaItems = new List<AnimeItemAbstraction>();

        private int _allPages;

        private SmartObservableCollection<AnimeItemViewModel> _animeItems =
            new SmartObservableCollection<AnimeItemViewModel>();

        private List<AnimeItemAbstraction> _animeItemsSet =
            new List<AnimeItemAbstraction>(); //All for current list        

        private bool _initializing;

        public bool ResetedNavBack { get; set; } = true;

        private AnimeListDisplayModes? _manuallySelectedViewMode;
        private string _prevListSource;

        private string _prevQuery = "";
        private int _prevAnimeStatus;


        private AnimeListWorkModes _prevWorkMode = AnimeListWorkModes.Anime;
        private bool _scrollHandlerAdded;


        private bool _wasPreviousQuery;

        public bool CanAddScrollHandler;
        public AnimeSeason CurrentSeason;

        public bool Initializing
        {
            get { return _initializing; }
            private set
            {
                _initializing = value;
                if (!value)
                    Initialized?.Invoke();
            }
        }

        public List<AnimeItemAbstraction> AllLoadedAnimeItemAbstractions { get; private set; } =
            new List<AnimeItemAbstraction>();

        public List<AnimeItemAbstraction> AllLoadedMangaItemAbstractions { get; private set; } =
            new List<AnimeItemAbstraction>();

        private SmartObservableCollection<AnimeItemViewModel> AnimeItems
        {
            get { return _animeItems; }
            set
            {
                _animeItems = value;
                RaisePropertyChanged(() => AnimeListItems);
                RaisePropertyChanged(() => AnimeCompactItems);
                RaisePropertyChanged(() => AnimeGridItems);
            }
        }

        public SmartObservableCollection<AnimeItemViewModel> AnimeListItems
            => DisplayMode == AnimeListDisplayModes.IndefiniteList ? AnimeItems : null;

        public SmartObservableCollection<AnimeItemViewModel> AnimeGridItems
            => DisplayMode == AnimeListDisplayModes.IndefiniteGrid ? AnimeItems : null;

        public SmartObservableCollection<AnimeItemViewModel> AnimeCompactItems
            => DisplayMode == AnimeListDisplayModes.IndefiniteCompactList ? AnimeItems : null;

        public ObservableCollection<AnimeSeason> SeasonSelection { get; } = new ObservableCollection<AnimeSeason>();


        public bool AreThereItemsWaitingForLoad => _animeItemsSet.Count != 0;
        public int CurrentStatus => GetDesiredStatus();

        public double ListItemGridWidth
        {
            get
            {
                var width = View?.ActualWidth ?? 1000;
                var items = (int) width/ItemPrefferedWidth;
                var widthRest = width - items*ItemPrefferedWidth;
                return ItemPrefferedWidth + widthRest/items;
            }
        }

        public int CurrentIndexPosition { get; set; }

        public event AnimeItemListInitialized Initialized;
        public event ScrollIntoViewRequest ScrollIntoViewRequested;
        public event SortingSettingChange SortingSettingChanged;
        public event SelectionResetRequest SelectionResetRequested;

        public async void Init(AnimeListPageNavigationArgs args)
        {
            //base
            _scrollHandlerAdded = false;
            Initializing = true;
            _manuallySelectedViewMode = null;
            //take out trash
            _animeItemsSet = new List<AnimeItemAbstraction>();
            AnimeItems = new SmartObservableCollection<AnimeItemViewModel>();
            RaisePropertyChanged(() => AnimeItems);
            _randomedIds = new List<int>();
            _fetching = _fetchingSeasonal = false;

            if (args == null || args.ResetBackNav)
                ViewModelLocator.NavMgr.ResetMainBackNav();

            //give visual feedback
            Loading = true;
            LoadMoreFooterVisibility = Visibility.Collapsed;
            await Task.Delay(10);

            //depending on args
            var gotArgs = false;
            if (args != null) //Save current mode
            {
                ResetedNavBack = args.ResetBackNav;
                WorkMode = args.WorkMode;
                if (WorkMode == AnimeListWorkModes.TopAnime)
                {
                    TopAnimeWorkMode = args.TopWorkMode;
                    ViewModelLocator.GeneralHamburger.SetActiveButton(args.TopWorkMode);//we have to have it
                }

                if (!string.IsNullOrEmpty(args.ListSource))
                    ListSource = args.ListSource;
                else
                    ListSource = Credentials.UserName;
                if (args.NavArgs) // Use args if we have any
                {
                    SortDescending = SortDescending = args.Descending;
                    SetSortOrder(args.SortOption); //index
                    SetDesiredStatus(args.Status);
                    CurrentIndexPosition = args.SelectedItemIndex;
                    CurrentSeason = args.CurrSeason;
                    DisplayMode = args.DisplayMode;
                    gotArgs = true;
                }
            }
            else //assume default AnimeList
            {
                WorkMode = AnimeListWorkModes.Anime;
                ListSource = Credentials.UserName;
            }
            ViewModelLocator.GeneralHamburger.UpdateAnimeFiltersSelectedIndex();
            RaisePropertyChanged(() => CurrentlySelectedDisplayMode);
            switch (WorkMode)
            {
                case AnimeListWorkModes.Manga:
                case AnimeListWorkModes.Anime:
                    if (!gotArgs)
                        SetDefaults(args?.StatusIndex);

                    AppBtnListSourceVisibility = true;
                    AppbarBtnPinTileVisibility = Visibility.Collapsed;
                    AppBtnSortingVisibility = Visibility.Visible;
                    AnimeItemsDisplayContext = AnimeItemDisplayContext.AirDay;
                    if (WorkMode == AnimeListWorkModes.Anime)
                    {
                        SortAirDayVisibility = Visibility.Visible;
                        Sort3Label = "Watched";
                        StatusAllLabel = "All";
                        Filter1Label = "Watching";
                        Filter5Label = "Plan to watch";
                    }
                    else // manga
                    {
                        SortAirDayVisibility = Visibility.Collapsed;
                        Sort3Label = "Read";
                        StatusAllLabel = "All";
                        Filter1Label = "Reading";
                        Filter5Label = "Plan to read";
                    }

                    //try to set list source - display notice on fail
                    if (string.IsNullOrWhiteSpace(ListSource))
                    {
                        if (!string.IsNullOrWhiteSpace(Credentials.UserName))
                            ListSource = Credentials.UserName;
                    }
                    if (string.IsNullOrWhiteSpace(ListSource))
                    {
                        EmptyNoticeVisibility = true;
                        EmptyNoticeContent =
                            "We have come up empty...\nList source is not set.\nLog in or set it manually.";
                        BtnSetSourceVisibility = true;
                        Loading = false;
                    }
                    else
                        await FetchData(); //we have source we can fetch

                    break;
                case AnimeListWorkModes.SeasonalAnime:
                case AnimeListWorkModes.TopAnime:
                case AnimeListWorkModes.TopManga:
                    Loading = true;
                    EmptyNoticeVisibility = false;

                    AppBtnListSourceVisibility = false;
                    AppBtnGoBackToMyListVisibility = Visibility.Collapsed;
                    BtnSetSourceVisibility = false;

                    ViewModelLocator.NavMgr.DeregisterBackNav();
                    ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);

                    if (!gotArgs)
                    {
                        SortDescending = false;
                        SetSortOrder(SortOptions.SortWatched); //index
                        SetDesiredStatus(null);
                        CurrentSeason = null;
                        if (SeasonSelection.Count != 0)
                            SeasonalUrlsSelectedIndex = 1;
                    }
                    StatusAllLabel = WorkMode == AnimeListWorkModes.SeasonalAnime ? "Airing" : "All";

                    Sort3Label = "Index";
                    await FetchSeasonalData();
                    if (WorkMode == AnimeListWorkModes.TopAnime || WorkMode == AnimeListWorkModes.TopManga)
                    {
                        AppbarBtnPinTileVisibility = AppBtnSortingVisibility = Visibility.Collapsed;
                        if (AnimeItems.Count + _animeItemsSet.Count <= 150)
                            LoadMoreFooterVisibility = Visibility.Visible;
                        AnimeItemsDisplayContext = AnimeItemDisplayContext.Index;
                    }
                    else
                    {
                        AppbarBtnPinTileVisibility = AppBtnSortingVisibility = Visibility.Visible;
                        AnimeItemsDisplayContext = AnimeItemDisplayContext.AirDay;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SortingSettingChanged?.Invoke(SortOption, SortDescending);
            Initializing = false;
            UpdateUpperStatus();
        }

        /// <summary>
        ///     Main refresh function
        /// </summary>
        /// <param name="searchSource">
        ///     If it's from search -> check if there's anuthing to update before refreshing.
        /// </param>
        /// <param name="fakeDelay">
        ///     To make app more responsive micro delays are good to trigger spinners and such.
        /// </param>
        /// <returns></returns>
        public async void RefreshList(bool searchSource = false, bool fakeDelay = false)
        {
            //await Task.Run(() =>
            //{
            var query = ViewModelLocator.GeneralMain.CurrentSearchQuery;

            var queryCondition = !string.IsNullOrWhiteSpace(query) && query.Length > 1;
            if (!_wasPreviousQuery && searchSource && !queryCondition)
                // refresh was requested from search but there's nothing to update
            {
                return;
            }
            if (!queryCondition)
                _prevQuery = null;

            if(queryCondition && !_wasPreviousQuery)
                SetDesiredStatus((int)AnimeStatus.AllOrAiring);
            else if(!queryCondition && _wasPreviousQuery)
                SetDesiredStatus(_prevAnimeStatus);

            _wasPreviousQuery = queryCondition;


            var status = GetDesiredStatus();

            IEnumerable<AnimeItemAbstraction> items;
            if (queryCondition &&
                _wasPreviousQuery &&
                !string.IsNullOrEmpty(_prevQuery) &&
                query.Length > _prevQuery.Length &&
                query.Substring(0, _prevQuery.Length-1) == _prevQuery) //use previous results if query is more detailed
                items = _animeItemsSet.Union(AnimeItems.Select(model => model.ParentAbstraction));
            else
                switch (WorkMode)
                {
                    case AnimeListWorkModes.Anime:
                        items = AllLoadedAnimeItemAbstractions;
                        break;
                    case AnimeListWorkModes.SeasonalAnime:
                    case AnimeListWorkModes.TopAnime:
                        items = _allLoadedSeasonalAnimeItems;
                        break;
                    case AnimeListWorkModes.Manga:
                        items = AllLoadedMangaItemAbstractions;
                        break;
                    case AnimeListWorkModes.TopManga:
                        items = _allLoadedSeasonalMangaItems;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            if(queryCondition)
                _prevQuery = query;
            _animeItemsSet.Clear();

            items = items.Where(item => status == 7 || item.MyStatus == status);

            if(!queryCondition)
                _prevAnimeStatus = status;

            if (queryCondition)
            {
                query = query.ToLower();
                if (ViewModelLocator.GeneralMain.SearchHints.Count > 0) //if there are any tags to begin with
                    items = items.Where(item => item.Title.ToLower().Contains(query) || item.Tags.Contains(query));
                else
                    items = items.Where(item => item.Title.ToLower().Contains(query));
            }
            if (WorkMode == AnimeListWorkModes.TopAnime || WorkMode == AnimeListWorkModes.TopManga)
                items = items.OrderBy(item => item.Index);
            else
                switch (SortOption)
                {
                    case SortOptions.SortTitle:
                        items = items.OrderBy(item => item.Title);
                        break;
                    case SortOptions.SortScore:
                        if (WorkMode != AnimeListWorkModes.SeasonalAnime)
                            items = items.OrderBy(item => item.MyScore);
                        else
                            items = items.OrderBy(item => item.GlobalScore);
                        break;
                    case SortOptions.SortWatched:
                        if (WorkMode == AnimeListWorkModes.SeasonalAnime)
                            items = items.OrderBy(item => item.Index);
                        else
                            items = items.OrderBy(item => item.MyEpisodes);
                        break;
                    case SortOptions.SortLastWatched:
                        items = items.OrderBy(abstraction => abstraction.LastWatched);
                        break;
                    case SortOptions.SortNothing:
                        break;
                    case SortOptions.SortAirDay:
                        var today = (int) DateTime.Now.DayOfWeek;
                        today++;
                        var nonAiringItems = items.Where(abstraction => abstraction.AirDay == -1);
                        var airingItems = items.Where(abstraction => abstraction.AirDay != -1);
                        var airingAfterToday = airingItems.Where(abstraction => abstraction.AirDay >= today);
                        var airingBeforeToday = airingItems.Where(abstraction => abstraction.AirDay < today);
                        if (SortDescending)
                            items =
                                airingAfterToday.OrderByDescending(abstraction => today - abstraction.AirDay)
                                    .Concat(
                                        airingBeforeToday.OrderByDescending(
                                            abstraction => today - abstraction.AirDay)
                                            .Concat(nonAiringItems));
                        else
                            items =
                                airingBeforeToday.OrderBy(abstraction => today - abstraction.AirDay)
                                    .Concat(
                                        airingAfterToday.OrderBy(abstraction => today - abstraction.AirDay)
                                            .Concat(nonAiringItems));

                        break;
                    case SortOptions.SortStartDate:
                        items = items.OrderBy(abstraction => abstraction.MyStartDate);
                        break;
                    case SortOptions.SortEndDate:
                        items = items.OrderBy(abstraction => abstraction.MyEndDate);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(SortOption), SortOption, null);
                }
            //If we are descending then reverse order
            if (SortDescending && SortOption != SortOptions.SortAirDay)
                items = items.Reverse();
            //Add all abstractions to current set (spread across pages)
            _animeItemsSet.AddRange(items);
            //});
            //If we have items then we should hide EmptyNotice       
            EmptyNoticeVisibility = _animeItemsSet.Count == 0;

            //How many pages do we have?
            if (fakeDelay)
                await Task.Delay(10);
            UpdatePageSetup();
            UpdateUpperStatus();
        }

        /// <summary>
        ///     Sets provided sort mode or takes one from settings
        /// </summary>
        /// <param name="option"></param>
        private void SetSortOrder(SortOptions? option)
        {
            SortOption = option ??
                         (WorkMode == AnimeListWorkModes.Manga ? Settings.MangaSortOrder : Settings.AnimeSortOrder);
        }

        private void SetDefaults(int? statusOverride = null)
        {
            SetSortOrder(null);
            if (statusOverride == null)
                SetDesiredStatus(null);
            else
                StatusSelectorSelectedIndex = statusOverride.Value;
            SortDescending = WorkMode == AnimeListWorkModes.Manga
                ? Settings.IsMangaSortDescending
                : Settings.IsSortDescending;
        }

        private async void LoadMore()
        {
            LoadMoreFooterVisibility = Visibility.Collapsed;
            if ((AnimeItems.Count + _animeItemsSet.Count)%50 != 0)
                return; //we have reached max 
            var page = (int) Math.Floor((AnimeItems.Count + _animeItemsSet.Count)/50.0);
            CurrentIndexPosition = page*50 - 1;
            await FetchSeasonalData(true, page);
            if (page <= 3)
                LoadMoreFooterVisibility = Visibility.Visible;
            else
                LoadMoreFooterVisibility = Visibility.Collapsed;
        }

        public void UpdateGridItemWidth(SizeChangedEventArgs args)
        {
            if(args.PreviousSize.Width - args.NewSize.Width < -600 || args.PreviousSize.Height - args.NewSize.Height < -350)
                if(ViewModelLocator.AnimeList.AreThereItemsWaitingForLoad)
                    ViewModelLocator.AnimeList.RefreshList();
            if (DisplayMode == AnimeListDisplayModes.IndefiniteList)
                RaisePropertyChanged(() => ListItemGridWidth);
        }

        #region Pagination

        /// <summary>
        ///     This method is fully responsible for preparing the view.
        ///     Depending on display mode it distributes items to right containers.
        /// </summary>
        private void UpdatePageSetup()
        {
            AnimeItems = new SmartObservableCollection<AnimeItemViewModel>();
            _lastOffset = 0;
            RaisePropertyChanged(() => DisplayMode);
            var minItems = GetGridItemsToLoad();
            minItems = minItems < 10 ? 10 : minItems;
            var minimumIndex = CurrentIndexPosition == -1
                ? minItems
                : CurrentIndexPosition + 1 <= minItems ? minItems : CurrentIndexPosition + 1;
            switch (DisplayMode)
            {
                case AnimeListDisplayModes.IndefiniteCompactList:
                    AnimeItems.AddRange(_animeItemsSet.Take(minimumIndex).Select(abstraction => abstraction.ViewModel));
                    _animeItemsSet = _animeItemsSet.Skip(minimumIndex).ToList();
                    break;
                case AnimeListDisplayModes.IndefiniteList:
                    AnimeItems.AddRange(_animeItemsSet.Take(minimumIndex).Select(abstraction => abstraction.ViewModel));
                    _animeItemsSet = _animeItemsSet.Skip(minimumIndex).ToList();
                    break;
                case AnimeListDisplayModes.IndefiniteGrid:
                    AnimeItems.AddRange(_animeItemsSet.Take(minimumIndex)
                        .Select(abstraction => abstraction.ViewModel));
                    _animeItemsSet = _animeItemsSet.Skip(minimumIndex).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RaisePropertyChanged(() => AnimeItems);
            AddScrollHandler();
            if (CurrentIndexPosition != -1)
            {
                try
                {
                    ScrollIntoViewRequested?.Invoke(AnimeItems[CurrentIndexPosition]);
                }
                catch (Exception)
                {
                    //no index
                }
                CurrentIndexPosition = -1;
            }
            ViewModelLocator.GeneralMain.ScrollToTopButtonVisibility = CurrentIndexPosition > minItems
                ? Visibility.Visible
                : Visibility.Collapsed;
            Loading = false;
            _randomedIds = new List<int>();
        }


        private int GetGridItemsToLoad()
        {
            var width = View?.ActualWidth ?? 1920;
            var height = View?.ActualHeight ?? 1080;
            if (width == 0 || height == 0)
            {
                width = 1920;
                height = 1080; //because
            }
            switch (DisplayMode)
            {
                case AnimeListDisplayModes.IndefiniteCompactList:
                    return (int)Math.Ceiling(height / 50) + 2;
                case AnimeListDisplayModes.IndefiniteList:
                    return (int)Math.Ceiling(width / ListItemGridWidth * height / 170) + 2;
                case AnimeListDisplayModes.IndefiniteGrid:
                    return (int)Math.Ceiling(width / 200 * height / 300) + 2; //2 for good measure
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        #endregion

        #region CacheManip

        public void AddAnimeEntry(AnimeItemAbstraction parentAbstraction)
        {
            if (_allLoadedAuthAnimeItems.Count > 0)
            {
                if (parentAbstraction.RepresentsAnime)
                    _allLoadedAuthAnimeItems.Add(parentAbstraction);
                else
                    _allLoadedAuthMangaItems.Add(parentAbstraction);
            }
        }

        public void RemoveAnimeEntry(AnimeItemAbstraction parentAbstraction)
        {
            try
            {
                AnimeItems.Remove(parentAbstraction.ViewModel);
            }
            catch (Exception)
            {
                //
            }


            if (_allLoadedAuthAnimeItems.Count > 0)
            {
                if (parentAbstraction.RepresentsAnime)
                    _allLoadedAuthAnimeItems.Remove(parentAbstraction);
                else
                    _allLoadedAuthMangaItems.Remove(parentAbstraction);
            }
        }

        #endregion

        #region IndefiniteScrollerino

        private int _lastOffset;

        /// <summary>
        ///     Event handler for event fired by one of two scroll viewrs in List and Grid view mode.
        ///     It loads more items as user is scroling further.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void IndefiniteScrollViewerOnViewChanging(object sender, ScrollViewerViewChangingEventArgs args)
        {
            var offset = (int) Math.Ceiling(args.FinalView.VerticalOffset);
            ViewModelLocator.GeneralMain.ScrollToTopButtonVisibility = offset > 300 ? Visibility.Visible : Visibility.Collapsed;
            if (_animeItemsSet.Count == 0)
                return;
            //Depending on display mode we load more or less items.
            //This is the place where offset thresholds are defined
            if (offset - _lastOffset >
                (DisplayMode == AnimeListDisplayModes.IndefiniteList
                    ? 75
                    : (DisplayMode == AnimeListDisplayModes.IndefiniteCompactList ? 50 : 100)) ||
                (DisplayMode == AnimeListDisplayModes.IndefiniteList && _animeItemsSet.Count == 1) ||
                (DisplayMode == AnimeListDisplayModes.IndefiniteGrid && _animeItemsSet.Count <= 2))
            {
                _lastOffset = offset;
                int itemsCount;
                switch (DisplayMode)
                {
                    case AnimeListDisplayModes.IndefiniteList:
                        itemsCount = (int) (sender as FrameworkElement).ActualWidth/200;
                        AnimeItems.AddRange(_animeItemsSet.Take(itemsCount).Select(abstraction => abstraction.ViewModel));
                        _animeItemsSet = _animeItemsSet.Skip(itemsCount).ToList();
                        break;
                    case AnimeListDisplayModes.IndefiniteGrid:
                        itemsCount = (int) (sender as FrameworkElement).ActualWidth/160;
                        AnimeItems.AddRange(_animeItemsSet.Take(itemsCount).Select(abstraction => abstraction.ViewModel));
                        _animeItemsSet = _animeItemsSet.Skip(itemsCount).ToList();
                        break;
                    case AnimeListDisplayModes.IndefiniteCompactList:
                        itemsCount = (int) (sender as FrameworkElement).ActualHeight/50;
                        AnimeItems.AddRange(_animeItemsSet.Take(itemsCount).Select(abstraction => abstraction.ViewModel));
                        _animeItemsSet = _animeItemsSet.Skip(itemsCount).ToList();
                        break;
                }
            }
        }

        /// <summary>
        ///     Adds handler to scroll viewer provided by view.
        /// </summary>
        private async void AddScrollHandler()
        {
            if (!CanAddScrollHandler || _scrollHandlerAdded)
                return;
            _lastOffset = 0; //we are resseting this because we ARE on the very to of the list view when adding handler
            _scrollHandlerAdded = true;
            try
            {
                (await View.GetIndefiniteScrollViewer()).ViewChanging += IndefiniteScrollViewerOnViewChanging;
            }
            catch (Exception)
            {
                //we didn't get scroll handler -> add all items
                AnimeItems.AddRange(_animeItemsSet.Select(abstraction => abstraction.ViewModel));
                _animeItemsSet.Clear();
            }
        }

        /// <summary>
        ///     Scrolls to top of current indefinite scroll viewer.
        /// </summary>
        public async void ScrollToTop()
        {
            (await View.GetIndefiniteScrollViewer()).ChangeView(null,0,null);
            ViewModelLocator.GeneralMain.ScrollToTopButtonVisibility = Visibility.Collapsed;
        }

        #endregion

        #region FetchAndPopulate

        /// <summary>
        ///     Fetches seasonal data and top manga/anime.
        ///     Results are saved in appropriate containers for further operations.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        private bool _fetchingSeasonal;
        private async Task FetchSeasonalData(bool force = false, int page = 0)
        {
            if(_fetchingSeasonal)
                return;
            _fetchingSeasonal = true;


            Loading = true;
            EmptyNoticeVisibility = false;
            var setDefaultSeason = false;
            if (CurrentSeason == null)
            {
                CurrentSeason = new AnimeSeason {Name = "Airing", Url = "https://myanimelist.net/anime/season"};
                setDefaultSeason = true;
            }
            ViewModelLocator.GeneralMain.CurrentStatus = "Downloading data...\nThis may take a while...";
            //get top or seasonal anime
            var data = new List<ISeasonalAnimeBaseData>();
            switch (WorkMode)
            {
                case AnimeListWorkModes.SeasonalAnime:
                    var tResponse = new List<SeasonalAnimeData>();
                    await Task.Run(new Func<Task>(async () => tResponse = await new AnimeSeasonalQuery(CurrentSeason).GetSeasonalAnime(force)));
                    data.AddRange(tResponse);
                    break;
                case AnimeListWorkModes.TopAnime:
                case AnimeListWorkModes.TopManga:
                    var topResponse = new List<TopAnimeData>();
                    await Task.Run(new Func<Task>(async () => topResponse = await new AnimeTopQuery(WorkMode == AnimeListWorkModes.TopAnime ? TopAnimeWorkMode : TopAnimeType.Manga, page).GetTopAnimeData(force)));
                    data.AddRange(topResponse);
                    break;
            }
            //if we don't have any we cannot do anything I guess...
            if (data.Count == 0)
            {
                _fetchingSeasonal = false;
                RefreshList();
                return;
            }
            List<AnimeItemAbstraction> source;
            List<AnimeItemAbstraction> target;
            if (WorkMode == AnimeListWorkModes.TopManga)
            {
                //We have to load base mnga item first if not loaded before.
                if (AllLoadedMangaItemAbstractions.Count == 0 && !_attemptedMangaFetch)
                    await FetchData(false, AnimeListWorkModes.Manga);

                target = _allLoadedSeasonalMangaItems = new List<AnimeItemAbstraction>();
                source = _allLoadedAuthMangaItems.Count > 0 ? _allLoadedAuthMangaItems : new List<AnimeItemAbstraction>();
            }
            else
            {
                if (AllLoadedAnimeItemAbstractions.Count == 0 && !_attemptedAnimeFetch)
                    await FetchData(false, AnimeListWorkModes.Anime);

                target = _allLoadedSeasonalAnimeItems = new List<AnimeItemAbstraction>();
                source = _allLoadedAuthAnimeItems.Count > 0 ? _allLoadedAuthAnimeItems : new List<AnimeItemAbstraction>();
            }

            var updateScore = Settings.SelectedApiType == ApiType.Mal;
            foreach (var animeData in data)
            {
                try
                {
                    if (WorkMode == AnimeListWorkModes.SeasonalAnime && Settings.SelectedApiType == ApiType.Mal)
                        //seasonal anme comes with mal score, we don't want to polute hummingbird data
                        DataCache.RegisterVolatileData(animeData.Id, new VolatileDataCache
                        {
                            DayOfAiring = animeData.AirDay, GlobalScore = animeData.Score, Genres = animeData.Genres, AirStartDate = animeData.AirStartDate == AnimeItemViewModel.InvalidStartEndDate ? null : animeData.AirStartDate
                        });
                    AnimeItemAbstraction abstraction = null;
                    if (Settings.SelectedApiType == ApiType.Mal)
                        abstraction = source.FirstOrDefault(item => item.Id == animeData.Id);
                    else
                        abstraction = source.FirstOrDefault(item => item.MalId == animeData.Id);
                    if (abstraction == null)
                        target.Add(new AnimeItemAbstraction(animeData as SeasonalAnimeData, WorkMode != AnimeListWorkModes.TopManga));
                    else
                    {
                        abstraction.AirDay = animeData.AirDay;
                        if (updateScore)
                            abstraction.GlobalScore = animeData.Score;
                        abstraction.Index = animeData.Index;
                        abstraction.ViewModel.UpdateWithSeasonData(animeData as SeasonalAnimeData, updateScore);
                        target.Add(abstraction);
                    }
                }
                catch (Exception e)
                {
                    // wat
                }
            }
            if (WorkMode == AnimeListWorkModes.SeasonalAnime && SeasonSelection.Count == 0)
            {
                SeasonSelection.Clear();
                var i = 0;
                var currSeasonIndex = -1;
                foreach (var seasonalUrl in DataCache.SeasonalUrls)
                {
                    if (seasonalUrl.Key != "current")
                    {
                        SeasonSelection.Add(new AnimeSeason {Name = seasonalUrl.Key, Url = seasonalUrl.Value});
                        i++;
                    }
                    else
                        currSeasonIndex = Convert.ToInt32(seasonalUrl.Value) - 1;
                    if (seasonalUrl.Key == CurrentSeason.Name)
                    {
                        _seasonalUrlsSelectedIndex = i - 1;
                        RaisePropertyChanged(() => SeasonalUrlsSelectedIndex);
                    }
                }
                //we have set artificial default one because we did not know what lays ahead of us
                if (setDefaultSeason && currSeasonIndex != -1)
                {
                    CurrentSeason = SeasonSelection[currSeasonIndex];
                    _seasonalUrlsSelectedIndex = currSeasonIndex;
                    RaisePropertyChanged(() => SeasonalUrlsSelectedIndex);
                }
            }
            _fetchingSeasonal = false;
            RefreshList();
        }

        /// <summary>
        ///     Forces currently loaded page to download new data.
        /// </summary>
        private async void ReloadList()
        {
            if (WorkMode == AnimeListWorkModes.SeasonalAnime || WorkMode == AnimeListWorkModes.TopAnime || WorkMode == AnimeListWorkModes.TopManga)
                await FetchSeasonalData(true);
            else
                await FetchData(true);
        }

        private bool _attemptedMangaFetch;
        private bool _attemptedAnimeFetch;

        /// <summary>
        ///     Feteches manga and anime data for currstnt ListSource.
        /// </summary>
        /// <param name="force">Forces downloading new data from MAL.</param>
        /// <param name="modeOverride">
        ///     When we are accessing deatils or top anime/manga without having it pulled we can use this
        ///     override to fetch this data and do nothing else with it.That way we will avoid situation where item is on user's
        ///     list
        ///     but it wasn't downloaded by the application.
        /// </param>
        /// <returns></returns>
        private bool _fetching;
        public async Task FetchData(bool force = false, AnimeListWorkModes? modeOverride = null)
        {
            if(_fetching)
                return;
            _fetching = true;

            var requestedMode = modeOverride ?? WorkMode;

            if (!force && _prevListSource == ListSource && _prevWorkMode == requestedMode)
            {
                if (_prevWorkMode != modeOverride)
                    RefreshList();
                _fetching = false;
                return;
            }
            if (WorkMode == requestedMode)
                _prevWorkMode = WorkMode;
            _prevListSource = ListSource;

            Loading = modeOverride == null;
            BtnSetSourceVisibility = false;
            EmptyNoticeVisibility = false;

            if (string.IsNullOrWhiteSpace(ListSource))
            {
                EmptyNoticeVisibility = true;
                EmptyNoticeContent = "We have come up empty...\nList source is not set.\nLog in or set it manually.";
                BtnSetSourceVisibility = true;
            }
            else
            {
                EmptyNoticeContent = "We have come up empty...";
            }

            switch (requestedMode)
            {
                case AnimeListWorkModes.Anime:
                    _attemptedAnimeFetch = true;
                    AllLoadedAnimeItemAbstractions = new List<AnimeItemAbstraction>();
                    if (force)
                        _allLoadedAuthAnimeItems = new List<AnimeItemAbstraction>();
                    else if (_allLoadedAuthAnimeItems.Count > 0 && string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                        AllLoadedAnimeItemAbstractions = _allLoadedAuthAnimeItems;
                    break;
                case AnimeListWorkModes.Manga:
                    _attemptedMangaFetch = true;
                    AllLoadedMangaItemAbstractions = new List<AnimeItemAbstraction>();
                    if (force)
                        _allLoadedAuthMangaItems = new List<AnimeItemAbstraction>();
                    else if (_allLoadedAuthMangaItems.Count > 0 && string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                        AllLoadedMangaItemAbstractions = _allLoadedAuthMangaItems;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            if (requestedMode == AnimeListWorkModes.Anime ? AllLoadedAnimeItemAbstractions.Count == 0 : AllLoadedMangaItemAbstractions.Count == 0)
            {
                List<ILibraryData> data = null;
                await Task.Run(async () => data = await new LibraryListQuery(ListSource, requestedMode).GetLibrary(force));
                if (data?.Count == 0)
                {
                    //no data?
                    RefreshList();
                    _fetching = false;
                    return;
                }

                var auth = Credentials.Authenticated && string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase);
                switch (requestedMode)
                {
                    case AnimeListWorkModes.Anime:

                        foreach (var item in data)
                            AllLoadedAnimeItemAbstractions.Add(new AnimeItemAbstraction(auth, item as AnimeLibraryItemData));

                        if (string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                            _allLoadedAuthAnimeItems = AllLoadedAnimeItemAbstractions;
                        break;
                    case AnimeListWorkModes.Manga:
                        foreach (var item in data)
                            AllLoadedMangaItemAbstractions.Add(new AnimeItemAbstraction(auth && Settings.SelectedApiType == ApiType.Mal, item as MangaLibraryItemData)); //read only manga for hummingbird

                        if (string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                            _allLoadedAuthMangaItems = AllLoadedMangaItemAbstractions;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _fetching = false;
            if (WorkMode != requestedMode)
                return; // manga or anime is loaded top manga can proceed loading something else

            AppBtnGoBackToMyListVisibility = Credentials.Authenticated && !string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
            //load tags
            ViewModelLocator.GeneralMain.SearchHints = _allLoadedAuthAnimeItems.Concat(_allLoadedAuthMangaItems).SelectMany(abs => abs.Tags).Distinct().ToList();
            RefreshList();
        }

        /// <summary>
        ///     Method used by details page to associate itself with authenticated item in order to allow for list updates.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="anime"></param>
        /// <returns></returns>
        public async Task<IAnimeData> TryRetrieveAuthenticatedAnimeItem(int id, bool anime = true, bool forceMal = false)
        {
            if (!Credentials.Authenticated)
                return null;
            try
            {
                if (anime)
                {
                    if (AllLoadedAnimeItemAbstractions.Count == 0 && !_attemptedAnimeFetch)
                        await FetchData(false, AnimeListWorkModes.Anime);
                }
                else if (AllLoadedMangaItemAbstractions.Count == 0 && !_attemptedMangaFetch)
                    await FetchData(false, AnimeListWorkModes.Manga);

                return anime ? _allLoadedAuthAnimeItems.First(abstraction => forceMal ? abstraction.MalId == id : abstraction.Id == id).ViewModel : _allLoadedAuthMangaItems.First(abstraction => forceMal ? abstraction.MalId == id : abstraction.Id == id).ViewModel;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region PropertyPairs

        private string _listSource;

        public string ListSource
        {
            get { return _listSource; }
            set
            {
                if(_listSource == value)
                    return;
                _listSource = value;
                RaisePropertyChanged(() => ListSource);
            }
        }

        private string _emptyNoticeContent;

        public string EmptyNoticeContent
        {
            get { return _emptyNoticeContent; }
            set
            {
                _emptyNoticeContent = value;
                RaisePropertyChanged(() => EmptyNoticeContent);
            }
        }

        private bool _emptyNoticeVisibility;

        public bool EmptyNoticeVisibility
        {
            get { return _emptyNoticeVisibility; }
            set
            {
                _emptyNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyNoticeVisibility);
            }
        }

        private bool _updateNoticeVisibility;

        public bool UpdateNoticeVisibility
        {
            get { return _updateNoticeVisibility; }
            set
            {
                _updateNoticeVisibility = value;
                RaisePropertyChanged(() => UpdateNoticeVisibility);
            }
        }

        private bool _btnSetSourceVisibility;

        public bool BtnSetSourceVisibility
        {
            get { return _btnSetSourceVisibility; }
            set
            {
                _btnSetSourceVisibility = value;
                RaisePropertyChanged(() => BtnSetSourceVisibility);
            }
        }

        private Visibility _appbarBtnPinTileVisibility;

        public Visibility AppbarBtnPinTileVisibility
        {
            get { return _appbarBtnPinTileVisibility; }
            set
            {
                _appbarBtnPinTileVisibility = value;
                RaisePropertyChanged(() => AppbarBtnPinTileVisibility);
            }
        }

        private bool _appBtnListSourceVisibility = true;

        public bool AppBtnListSourceVisibility
        {
            get { return _appBtnListSourceVisibility; }
            set
            {
                _appBtnListSourceVisibility = value;
                RaisePropertyChanged(() => AppBtnListSourceVisibility);
            }
        }

        public Visibility HumApiSpecificControlsVisibility => Settings.SelectedApiType == ApiType.Mal ? Visibility.Collapsed : Visibility.Visible;

        public Visibility MalApiSpecificControlsVisibility => Settings.SelectedApiType == ApiType.Hummingbird ? Visibility.Collapsed : Visibility.Visible;

        private Visibility _appBtnGoBackToMyListVisibility = Visibility.Collapsed;

        public Visibility AppBtnGoBackToMyListVisibility
        {
            get { return _appBtnGoBackToMyListVisibility; }
            set
            {
                _appBtnGoBackToMyListVisibility = value;
                RaisePropertyChanged(() => AppBtnGoBackToMyListVisibility);
            }
        }

        private ICommand _selectAtRandomCommand;
        private Random _rangomGenerator;
        private List<int> _randomedIds = new List<int>();

        public ICommand SelectAtRandomCommand
        {
            get
            {
                return _selectAtRandomCommand ?? (_selectAtRandomCommand = new RelayCommand(() =>
                {
                    if (Settings.SelectedApiType == ApiType.Hummingbird && WorkMode == AnimeListWorkModes.TopManga)
                        return;
                    var random = _rangomGenerator ?? (_rangomGenerator = new Random((int) DateTime.Now.Ticks));
                    var pool = _animeItemsSet.Select(abstraction => abstraction.ViewModel).Union(AnimeItems).ToList();
                    if(pool.Count == 0)
                        return;
                    if(_randomedIds.Count == pool.Count)
                        _randomedIds = new List<int>();
                    _randomedIds.ForEach(id =>
                    {
                        var item = pool.FirstOrDefault(model => model.Id == id);
                        if (item != null)
                            pool.Remove(item);
                    });                  
                    var winner = pool[random.Next(0, pool.Count)];
                    if (Settings.EnsureRandomizerAlwaysSelectsWinner && !AnimeItems.Contains(winner))
                    {
                        var indexesToLoad = _animeItemsSet.IndexOf(winner.ParentAbstraction) + 10;
                        AnimeItems.AddRange(_animeItemsSet.Take(indexesToLoad).Select(abstraction => abstraction.ViewModel));
                        _animeItemsSet = _animeItemsSet.Skip(indexesToLoad).ToList();
                    }

                    winner.NavigateDetails();
                    _randomedIds.Add(winner.Id);
                    ScrollIntoViewRequested?.Invoke(winner,true);
                }));
            }
        }

        private Visibility _upperCommandBarVisibility = Visibility.Visible;

        public Visibility UpperCommandBarVisibility
        {
            get { return _upperCommandBarVisibility; }
            set
            {
                _upperCommandBarVisibility = value;
                RaisePropertyChanged(() => UpperCommandBarVisibility);
            }
        }

        private Visibility _appBtnSortingVisibility = Visibility.Collapsed;

        public Visibility AppBtnSortingVisibility
        {
            get { return _appBtnSortingVisibility; }
            set
            {
                _appBtnSortingVisibility = value;
                RaisePropertyChanged(() => AppBtnSortingVisibility);
            }
        }

        private Visibility _loadMoreFooterVisibility = Visibility.Collapsed;

        public Visibility LoadMoreFooterVisibility
        {
            get { return _loadMoreFooterVisibility; }
            private set
            {
                _loadMoreFooterVisibility = value;
                RaisePropertyChanged(() => LoadMoreFooterVisibility);
            }
        }

        private AnimeItemDisplayContext _animeItemsDisplayContext;

        public AnimeItemDisplayContext AnimeItemsDisplayContext
        {
            get { return _animeItemsDisplayContext; }
            set
            {
                _animeItemsDisplayContext = value;
                RaisePropertyChanged(() => AnimeItemsDisplayContext);
            }
        }

        private int _statusSelectorSelectedIndex;

        public int StatusSelectorSelectedIndex
        {
            get { return _statusSelectorSelectedIndex; }
            set
            {
                if (value == _statusSelectorSelectedIndex)
                    return;
                _statusSelectorSelectedIndex = value;
                RaisePropertyChanged(() => StatusSelectorSelectedIndex);
                ViewModelLocator.GeneralHamburger.UpdateAnimeFiltersSelectedIndex();
                if (GetDesiredStatus() != (int) AnimeStatus.AllOrAiring)
                    LoadMoreFooterVisibility = Visibility.Collapsed;
                else if (WorkMode == AnimeListWorkModes.TopAnime || WorkMode == AnimeListWorkModes.TopManga)
                {
                    if (!Initializing && AnimeItems.Count + _animeItemsSet.Count <= 150)
                        LoadMoreFooterVisibility = Visibility.Visible;
                    else
                        LoadMoreFooterVisibility = Visibility.Collapsed;
                }
                if (!Initializing)
                {
                    if (Settings.HideFilterSelectionFlyout)
                        View.FlyoutFilters.Hide();

                    SetDisplayMode((AnimeStatus) GetDesiredStatus());
                    RefreshList(false, true);
                }
            }
        }

        //For hiding/showing header bar - XamlResources/DictionaryAnimeList.xml
        private GridLength _pivotHeaerGridRowHeight = new GridLength(0);

        public GridLength PivotHeaerGridRowHeight
        {
            get { return _pivotHeaerGridRowHeight; }
            set
            {
                _pivotHeaerGridRowHeight = value;
                RaisePropertyChanged(() => PivotHeaerGridRowHeight);
            }
        }

        public AnimeItemViewModel TemporarilySelectedAnimeItem
        {
            get { return null; }
            set
            {
                if (value != null && ViewModelLocator.AnimeDetails.Id != value.Id)
                    value.NavigateDetails();
                RaisePropertyChanged(() => TemporarilySelectedAnimeItem);
                SelectionResetRequested?.Invoke(DisplayMode);
            }
        }

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        private bool _sortDescending;

        public bool SortDescending
        {
            get { return _sortDescending; }
            set
            {
                if (Initializing && Settings.HideSortingSelectionFlyout)
                    View.FlyoutSorting.Hide();
                _sortDescending = value;
                RaisePropertyChanged(() => SortDescending);
            }
        }

        private string _sort3Label = "Watched";

        public string Sort3Label
        {
            get { return _sort3Label; }
            set
            {
                _sort3Label = value;
                RaisePropertyChanged(() => Sort3Label);
            }
        }

        private string _filter1Label = "Watching";

        public string Filter1Label
        {
            get { return _filter1Label; }
            set
            {
                _filter1Label = value;
                RaisePropertyChanged(() => Filter1Label);
            }
        }

        private string _filter5Label = "Plan to watch";

        public string Filter5Label
        {
            get { return _filter5Label; }
            set
            {
                _filter5Label = value;
                RaisePropertyChanged(() => Filter5Label);
            }
        }

        private string _statusAllLabel = "All";

        public string StatusAllLabel
        {
            get { return _statusAllLabel; }
            set
            {
                _statusAllLabel = value;
                RaisePropertyChanged(() => StatusAllLabel);
            }
        }

        private ICommand _setSortModeCommand;

        public ICommand SetSortModeCommand
        {
            get
            {
                return _setSortModeCommand ?? (_setSortModeCommand = new RelayCommand<string>(s =>
                {
                    SetSortOrder((SortOptions) int.Parse(s));
                    RefreshList();
                }));
            }
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new RelayCommand(ReloadList));

        private ICommand _loadMoreCommand;

        public ICommand LoadMoreCommand => _loadMoreCommand ?? (_loadMoreCommand = new RelayCommand(LoadMore));

        private ICommand _goBackToMyListCommand;

        public ICommand GoBackToMyListCommand
        {
            get
            {
                return _goBackToMyListCommand ?? (_goBackToMyListCommand = new RelayCommand(() =>
                {
                    ListSource = Credentials.UserName;
                    FetchData();
                }));
            }
        }

        /// <summary>
        ///     I know that this is dirt and it shouldn't be here... I'll get rid of it someday
        /// </summary>
        public IAnimeListViewInteractions View { get; set; }

        public AnimeListWorkModes _workMode;

        public AnimeListWorkModes WorkMode
        {
            get { return _workMode; }
            set
            {
                _workMode = value;
                RaisePropertyChanged(() => WorkMode);
            }
        }

        public TopAnimeType TopAnimeWorkMode { get; set; }

        private AnimeListDisplayModes _displayMode;

        public AnimeListDisplayModes DisplayMode
        {
            get { return _displayMode; }
            set
            {
                if (_scrollHandlerAdded && CanAddScrollHandler)
                {
                    //we don't want to be subscribed to wrong srollviewer
                    View.GetIndefiniteScrollViewer().Result.ViewChanging -= IndefiniteScrollViewerOnViewChanging;
                    _scrollHandlerAdded = false;
                }
                View.IndefiniteScrollViewer = null;
                _displayMode = value;
                RaisePropertyChanged(() => ListItemGridWidth);
                RaisePropertyChanged(() => DisplayMode);
                RaisePropertyChanged(() => CurrentlySelectedDisplayMode);
            }
        }

        public Tuple<AnimeListDisplayModes, string> CurrentlySelectedDisplayMode
        {
            get { return DisplayModes[(int) DisplayMode]; }
            set
            {
                DisplayMode = value.Item1;
                if (Settings.LockDisplayMode)
                    _manuallySelectedViewMode = value.Item1;
                _lastOffset = 0;
                if (Settings.HideViewSelectionFlyout)
                    View.FlyoutViews.Hide();
                RaisePropertyChanged(() => DisplayMode);
                RefreshList(false, true);
            }
        }

        public ObservableCollection<Tuple<AnimeListDisplayModes, string>> DisplayModes { get; } = new ObservableCollection<Tuple<AnimeListDisplayModes, string>>
        {
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteList, "Detailed Grid"), new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteGrid, "Grid"), new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteCompactList, "Compact list")
        };


        private Visibility _sortAirDayVisibility;

        public Visibility SortAirDayVisibility
        {
            get { return _sortAirDayVisibility; }
            set
            {
                _sortAirDayVisibility = value;
                RaisePropertyChanged(() => SortAirDayVisibility);
            }
        }

        private int _seasonalUrlsSelectedIndex;

        public int SeasonalUrlsSelectedIndex
        {
            get { return _seasonalUrlsSelectedIndex; }
            set
            {
                if (value == _seasonalUrlsSelectedIndex || value < 0)
                    return;
                _seasonalUrlsSelectedIndex = value;
                CurrentSeason = SeasonSelection[value];
                RaisePropertyChanged(() => SeasonalUrlsSelectedIndex);
                View.FlyoutSeasonSelectionHide();
                FetchSeasonalData();
            }
        }

        private SortOptions _sortOption = SortOptions.SortNothing;

        public SortOptions SortOption
        {
            get { return _sortOption; }
            set
            {
                if (!Initializing && Settings.HideSortingSelectionFlyout)
                    View.FlyoutSorting.Hide();
                _sortOption = value;
            }
        }

        private double? _maxWidth;

        public double MaxWidth => (_maxWidth ?? (_maxWidth = AnimeItemViewModel.MaxWidth)).Value;

        #endregion

        #region StatusRelatedStuff

        private void UpdateUpperStatus()
        {
            var page = ViewModelLocator.GeneralMain;

            if (WorkMode != AnimeListWorkModes.SeasonalAnime)
                if (WorkMode == AnimeListWorkModes.TopAnime)
                    page.CurrentStatus = $"Top {TopAnimeWorkMode} - {Utilities.StatusToString(GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";
                else if (WorkMode == AnimeListWorkModes.TopManga)
                    page.CurrentStatus = $"Top Manga - {Utilities.StatusToString(GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";
                else if (!string.IsNullOrWhiteSpace(ListSource))
                    page.CurrentStatus = $"{ListSource} - {Utilities.StatusToString(GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";
                else
                    page.CurrentStatus = $"{(WorkMode == AnimeListWorkModes.Anime ? "Anime list" : "Manga list")}";
            else
                page.CurrentStatus = $"{CurrentSeason?.Name} - {Utilities.StatusToString(GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";

            page.CurrentStatusSub = SortOption != SortOptions.SortWatched ? SortOption.GetDescription() : Sort3Label;
        }

        public int GetDesiredStatus()
        {
            var value = StatusSelectorSelectedIndex;
            value++;
            return value == 0 ? 1 : value == 5 || value == 6 ? value + 1 : value;
        }

        private void SetDisplayMode(AnimeStatus val)
        {
            if (_manuallySelectedViewMode == null)
            {
                switch (val)
                {
                    case AnimeStatus.Watching:
                        DisplayMode = Settings.WatchingDisplayMode;
                        break;
                    case AnimeStatus.Completed:
                        DisplayMode = Settings.CompletedDisplayMode;
                        break;
                    case AnimeStatus.OnHold:
                        DisplayMode = Settings.OnHoldDisplayMode;
                        break;
                    case AnimeStatus.Dropped:
                        DisplayMode = Settings.DroppedDisplayMode;
                        break;
                    case AnimeStatus.PlanToWatch:
                        DisplayMode = Settings.PlannedDisplayMode;
                        break;
                    case AnimeStatus.AllOrAiring:
                        DisplayMode = Settings.AllDisplayMode;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(val), val, null);
                }
                RaisePropertyChanged(() => DisplayMode);
            }
        }

        private void SetDesiredStatus(int? value)
        {
            var setDisp = value == null;
            if (value == null && (WorkMode == AnimeListWorkModes.SeasonalAnime || WorkMode == AnimeListWorkModes.TopAnime || WorkMode == AnimeListWorkModes.TopManga))
                value = (int) AnimeStatus.AllOrAiring;

            value = value ?? (WorkMode == AnimeListWorkModes.Manga ? Settings.DefaultMangaFilter : Settings.DefaultAnimeFilter);
            if (setDisp)
                SetDisplayMode((AnimeStatus) value);

            value = value == 6 || value == 7 ? value - 1 : value;
            value--;
            Initializing = true;
            StatusSelectorSelectedIndex = (int) value;
            Initializing = false;
        }

        #endregion

        #region LogInOut

        //TODO : Refactor
        public void LogOut()
        {
            _animeItemsSet.Clear();
            AnimeItems = new SmartObservableCollection<AnimeItemViewModel>();
            RaisePropertyChanged(() => AnimeItems);
            AllLoadedAnimeItemAbstractions = new List<AnimeItemAbstraction>();
            _allLoadedAuthAnimeItems = new List<AnimeItemAbstraction>();
            AllLoadedMangaItemAbstractions = new List<AnimeItemAbstraction>();
            _allLoadedAuthMangaItems = new List<AnimeItemAbstraction>();
            _allLoadedSeasonalAnimeItems = new List<AnimeItemAbstraction>();

            ListSource = string.Empty;
            _prevListSource = "";
        }

        public void LogIn()
        {
            _animeItemsSet.Clear();
            AnimeItems = new SmartObservableCollection<AnimeItemViewModel>();
            RaisePropertyChanged(() => AnimeItems);
            AllLoadedAnimeItemAbstractions = new List<AnimeItemAbstraction>();
            _allLoadedAuthAnimeItems = new List<AnimeItemAbstraction>();
            AllLoadedMangaItemAbstractions = new List<AnimeItemAbstraction>();
            _allLoadedAuthMangaItems = new List<AnimeItemAbstraction>();
            _allLoadedSeasonalAnimeItems = new List<AnimeItemAbstraction>();
            ListSource = Credentials.UserName;
            _prevListSource = "";
        }

        #endregion
    }
}