using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.Models.Models.Library;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Delegates;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels.Interfaces;

namespace MALClient.XShared.ViewModels.Main
{
    public partial class AnimeListViewModel : ViewModelBase
    {
        private readonly IAnimeLibraryDataStorage _animeLibraryDataStorage;
        private const int ItemPrefferedWidth = 385;
        private const int LastIndexPositionOnRefresh = -10; //just a constant
        private const int ItemsPerPage = 50; //just a constant

        private SmartObservableCollection<AnimeItemViewModel> _animeItems =
            new SmartObservableCollection<AnimeItemViewModel>();

        private List<AnimeItemAbstraction> _animeItemsSet =
            new List<AnimeItemAbstraction>(); //All for current list        

        private bool _initializing;
        private bool _queryHandler;

        public bool ResetedNavBack { get; set; } = true;

        private AnimeListDisplayModes? _manuallySelectedViewMode;
        private string _prevListSource;

        private string _prevQuery = "";
        private AnimeStatus _prevAnimeStatus;
        private bool _invalidatePreviousSearchResults;


        private AnimeListWorkModes _prevWorkMode = AnimeListWorkModes.Anime;
        private bool _scrollHandlerAdded;

        private bool _canLoadMoreFilterLock;

        public bool CanAddScrollHandler;
        public AnimeSeason CurrentSeason;

        public AnimeGenres Genre { get; set; }
        public AnimeStudios Studio { get; set; }

        public IDimensionsProvider DimensionsProvider { get; set; }

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

        public SmartObservableCollection<AnimeItemViewModel> AnimeItems
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

        public int CurrentStatus
        {
            get { return (int)GetDesiredStatus(); }
            set
            {
                SetDesiredStatus(value);


            }
        }

        public double ListItemGridWidth
        {
            get
            {
                var width = DimensionsProvider?.ActualWidth ?? 1000;
                var items = (int) width/ItemPrefferedWidth;
                items = items == 0 ? 1 : items;
                var widthRest = width - items*ItemPrefferedWidth;
                var sum = ItemPrefferedWidth + widthRest/items;
                return sum < ItemPrefferedWidth ? ItemPrefferedWidth : sum;
            }
        }

        public AnimeListViewModel(IAnimeLibraryDataStorage animeLibraryDataStorage)
        {
            _animeLibraryDataStorage = animeLibraryDataStorage;

            _animeLibraryDataStorage.AnimeRemoved += OnAnimeEntryRemoved;

            for (int i = 2000; i < DateTime.Now.Year + 2; i++)
            {
                SeasonYears.Add(i.ToString());
            }
        }

        /// <summary>
        /// Set -10 to scroll to last item
        /// </summary>
        public int CurrentIndexPosition { get; set; }

        public event AnimeItemListInitialized Initialized;
        public event ScrollIntoViewRequest ScrollIntoViewRequested;
        public event SortingSettingChange SortingSettingChanged;
        public event SelectionResetRequest SelectionResetRequested;
        public event EmptyEventHander HideSeasonSelectionFlyout;
        public event EmptyEventHander HideFiltersFlyout;
        public event EmptyEventHander HideSortingFlyout;
        public event EmptyEventHander HideViewsFlyout;
        public event EmptyEventHander ScrollToTopRequest;
        public event EmptyEventHander AddScrollHandlerRequest;
        public event EmptyEventHander RemoveScrollHandlerRequest;
        public event EmptyEventHander RemoveScrollingConatinerReferenceRequest;

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

            if (!_queryHandler)
            {
                ViewModelLocator.GeneralMain.OnSearchDelayedQuerySubmitted += OnOnSearchDelayedQuerySubmitted;
                ViewModelLocator.GeneralMain.OnSearchQuerySubmitted += OnOnSearchDelayedQuerySubmitted;
            }
            _queryHandler = true;

            //give visual feedback
            Loading = true;
            CanLoadMore = false;
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
                else if (WorkMode == AnimeListWorkModes.AnimeByGenre)
                {
                    Genre = args.Genre;
                }
                else if(WorkMode == AnimeListWorkModes.AnimeByStudio)
                {
                    Studio = args.Studio;
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
                    AppbarBtnPinTileVisibility = false;
                    AppBtnSortingVisibility = true;
                    AnimeItemsDisplayContext = AnimeItemDisplayContext.AirDay;
                    if (WorkMode == AnimeListWorkModes.Anime)
                    {
                        SortAirDayVisibility = true;
                        Sort3Label = "Watched";
                        StatusAllLabel = "All";
                        Filter1Label = "Watching";
                        Filter5Label = "Plan to watch";
                    }
                    else // manga
                    {
                        SortAirDayVisibility = false;
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
                case AnimeListWorkModes.AnimeByGenre:
                case AnimeListWorkModes.AnimeByStudio:
                    Loading = true;
                    EmptyNoticeVisibility = false;

                    AppBtnListSourceVisibility = false;
                    AppBtnGoBackToMyListVisibility = false;
                    BtnSetSourceVisibility = false;

                    ViewModelLocator.NavMgr.DeregisterBackNav();
                    ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);


                    if (!gotArgs)
                    {
                        SortDescending = false;
                        SetSortOrder(SortOptions.SortWatched); //index
                        SetDesiredStatus(null);
                        CurrentSeason = null;
                        SeasonSelection.Clear();
                    }
                    
                    //StatusAllLabel = WorkMode == AnimeListWorkModes.SeasonalAnime ? "Airing" : "All";

                    Sort3Label = "Index";
                    await FetchSeasonalData();
                    if (WorkMode == AnimeListWorkModes.TopAnime || WorkMode == AnimeListWorkModes.TopManga)
                    {
                        AppbarBtnPinTileVisibility = AppBtnSortingVisibility = false;
                        AnimeItemsDisplayContext = AnimeItemDisplayContext.Index;
                    }
                    else
                    {
                        if (WorkMode == AnimeListWorkModes.AnimeByGenre || WorkMode == AnimeListWorkModes.AnimeByStudio)
                        {
                            AppbarBtnPinTileVisibility = false;
                            AppBtnSortingVisibility = true;
                        }
                        else
                            AppbarBtnPinTileVisibility = AppBtnSortingVisibility = true;

                        AnimeItemsDisplayContext = AnimeItemDisplayContext.AirDay;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RaisePropertyChanged(() => LoadAllDetailsButtonVisiblity);
            SortingSettingChanged?.Invoke(SortOption, SortDescending);
            Initializing = false;
            UpdateUpperStatus();
        }

        public void OnNavigatedFrom()
        {
            ViewModelLocator.GeneralMain.OnSearchDelayedQuerySubmitted -= OnOnSearchDelayedQuerySubmitted;
            ViewModelLocator.GeneralMain.OnSearchQuerySubmitted -= OnOnSearchDelayedQuerySubmitted;
            _queryHandler = false;
        }

        private void OnOnSearchDelayedQuerySubmitted(string query)
        {
            RefreshList(true);
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
            if (!WasPreviousQuery && searchSource && !queryCondition)
                // refresh was requested from search but there's nothing to update
            {
                return;
            }
            if (!queryCondition)
            {
                _prevQuery = null;
                _invalidatePreviousSearchResults = false;
            }
            else
            {
                query = query.Trim();
            }

            if(queryCondition && !WasPreviousQuery)
                SetDesiredStatus((int)AnimeStatus.AllOrAiring);
            else if(!queryCondition && WasPreviousQuery)
                SetDesiredStatus((int)_prevAnimeStatus);

            WasPreviousQuery = queryCondition;


            var status = GetDesiredStatus();

            IEnumerable<AnimeItemAbstraction> items;
            if (queryCondition && !_invalidatePreviousSearchResults &&
                WasPreviousQuery &&
                !string.IsNullOrEmpty(_prevQuery) &&
                query.Length > _prevQuery.Length &&
                query.Substring(0, _prevQuery.Length-1) == _prevQuery) //use previous results if query is more detailed
                items = _animeItemsSet.Union(AnimeItems.Select(model => model.ParentAbstraction));
            else
                switch (WorkMode)
                {
                    case AnimeListWorkModes.Anime:
                        items = _animeLibraryDataStorage.AllLoadedAnimeItemAbstractions;
                        break;
                    case AnimeListWorkModes.SeasonalAnime:
                    case AnimeListWorkModes.TopAnime:
                    case AnimeListWorkModes.AnimeByGenre:
                    case AnimeListWorkModes.AnimeByStudio:
                        items = _animeLibraryDataStorage.AllLoadedSeasonalAnimeItems;
                        break;
                    case AnimeListWorkModes.Manga:
                        items = _animeLibraryDataStorage.AllLoadedMangaItemAbstractions;
                        break;
                    case AnimeListWorkModes.TopManga:
                        items = _animeLibraryDataStorage.AllLoadedSeasonalMangaItems;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            if(queryCondition)
                _prevQuery = query;
            _animeItemsSet.Clear();

            items = items.Where(item => status == AnimeStatus.AllOrAiring || item.MyStatus == status || (item.IsRewatching && status == AnimeStatus.Watching));

            if(!queryCondition)
                _prevAnimeStatus = status;

            if (queryCondition)
            {
                query = query.ToLower();
                bool alreadyFiltered = false;
                if (MainViewModelBase.AnimeMediaTypeHints.Contains(query))
                {
                    var type = 0;
                    if (query == "tv")
                        type = (int)AnimeType.TV;
                    else if (query == "movie")
                        type = (int)AnimeType.Movie;
                    else if (query == "special")
                        type = (int)AnimeType.Special;
                    else if (query == "ova")
                        type = (int)AnimeType.OVA;
                    items = items.Where(item => item.Type == type);
                    alreadyFiltered = true;
                }
                else if (MainViewModelBase.MangaMediaTypeHints.Contains(query))
                {
                    var type = 0;
                    if (query == "manga")
                        type = (int)MangaType.Manga;
                    else if (query == "novel")
                        type = (int)MangaType.Novel;
                    items = items.Where(item => item.Type == type);
                    alreadyFiltered = true;
                }

                _invalidatePreviousSearchResults = alreadyFiltered; //mangaa will not yield anything more manga
                if (!alreadyFiltered)
                {
                    try
                    {
                        if (ViewModelLocator.GeneralMain.SearchHints.Count > 0) //if there are any tags to begin with
                            items = items.Where(item => item.Title.ToLower().Contains(query) || item.Tags.Contains(query));
                        else
                            items = items.Where(item => item.Title.ToLower().Contains(query));
                    }
                    catch (Exception e)
                    {
                        ResourceLocator.TelemetryProvider.TrackException(e);
                    }

                }

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
                            items = items.OrderBy(item => item.MyScore).ThenByDescending(item => item.Title);
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
                    case SortOptions.SortSeason:
                        var itemsWithStartDate = new List<AnimeItemAbstraction>();
                        var itemsWithoutStartDate = new List<AnimeItemAbstraction>();
                        foreach (var item in items)
                        {
                            if (string.IsNullOrEmpty(item.AirStartDate))
                            {
                                itemsWithoutStartDate.Add(item);
                            }
                            else
                            {
                                itemsWithStartDate.Add(item);
                            }
                        }
                        if (SortDescending)
                        {
                            itemsWithStartDate =
                                itemsWithStartDate.OrderByDescending(
                                        item => int.Parse(item.AirStartDate.Substring(0, 4)))
                                    .ThenByDescending(item => Utils.Utilities.DateToSeason(item.AirStartDate))
                                    .ThenBy(item => item.Title)
                                    .ToList();
                            items = itemsWithStartDate.Concat(itemsWithoutStartDate.OrderBy(item => item.Title));
                        }
                        else
                        {
                            itemsWithStartDate =
                                itemsWithStartDate.OrderBy(
                                        item => int.Parse(item.AirStartDate.Substring(0, 4)))
                                    .ThenBy(item => Utils.Utilities.DateToSeason(item.AirStartDate))
                                    .ThenBy(item => item.Title)
                                    .ToList();
                            items = itemsWithStartDate.Concat(itemsWithoutStartDate.OrderBy(item => item.Title));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(SortOption), SortOption, null);
                }
            //If we are descending then reverse order
            if (SortDescending && SortOption != SortOptions.SortAirDay && SortOption != SortOptions.SortSeason)
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
        public void SetSortOrder(SortOptions? option)
        {
            option = option ??
             (WorkMode == AnimeListWorkModes.Manga ? Settings.MangaSortOrder : Settings.AnimeSortOrder);
            if (Settings.AutoDescendingSorting && option != null)
            {
                switch (option)
                {
                    case SortOptions.SortTitle:
                        _sortDescending = false;
                        break;
                    case SortOptions.SortScore:
                        _sortDescending = true;
                        break;
                    case SortOptions.SortWatched:
                        _sortDescending = WorkMode == AnimeListWorkModes.Anime || WorkMode == AnimeListWorkModes.Manga;
                        break;
                    case SortOptions.SortAirDay:
                        _sortDescending = true;
                        break;
                    case SortOptions.SortLastWatched:
                        _sortDescending = true;
                        break;
                    case SortOptions.SortStartDate:
                        _sortDescending = false;
                        break;
                    case SortOptions.SortEndDate:
                        _sortDescending = true;
                        break;
                    case SortOptions.SortNothing:
                        break;
                    case SortOptions.SortSeason:
                        _sortDescending = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                SortingSettingChanged?.Invoke(option.Value,_sortDescending);
            }
            SortOption = option.Value;
        }

        private void SetDefaults(int? statusOverride = null)
        {
            SetSortOrder(null);
            if (statusOverride == null)
                SetDesiredStatus(null);
            else
                StatusSelectorSelectedIndex = statusOverride.Value;
            if (!Settings.AutoDescendingSorting)
                SortDescending = WorkMode == AnimeListWorkModes.Manga
                    ? Settings.IsMangaSortDescending
                    : Settings.IsSortDescending;
        }

        private async void LoadMore()
        {
            if (CurrentPage > 4 || !CanLoadMore)
            {
                CanLoadMore = false;
                return; //we have reached max 
            }
            var prevCount = AnimeItems.Count + _animeItemsSet.Count;

            CurrentPage++;
            CurrentIndexPosition = prevCount - 2;
            await FetchSeasonalData(true, CurrentPage);
            if (prevCount == AnimeItems.Count + _animeItemsSet.Count)
            {
                CanLoadMore = false; // no items were added
            }
            else
            {
                CanLoadMore = CurrentPage <= 4 && WorkMode.GetAttribute<EnumUtilities.AnimeListWorkModeEnumMember>().AllowLoadingMore;

            }

            
        }

        public void UpdateGridItemWidth(Tuple<Tuple<double,double>, Tuple<double, double>> args)
        {
            //prevwirdth - curr width || prevHeight - currHeight
            if(args.Item1.Item1 - args.Item1.Item2 < -600 || args.Item2.Item1 - args.Item2.Item2 < -350)
                if(ViewModelLocator.AnimeList.AreThereItemsWaitingForLoad)
                    ViewModelLocator.AnimeList.RefreshList();
            if (DisplayMode == AnimeListDisplayModes.IndefiniteList)
                RaisePropertyChanged(() => ListItemGridWidth);
        }

        #region Pagination

        private bool WasPreviousQuery
        {
            get { return _wasPreviousQuery; }
            set
            {
                _wasPreviousQuery = value;
                RaisePropertyChanged(() => CanLoadMore);
            }
        }

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
            if (CurrentIndexPosition == LastIndexPositionOnRefresh)
                CurrentIndexPosition = _animeItemsSet.Count + AnimeItems.Count - 1;
            minItems = minItems < 10 ? 10 : minItems;
            var minimumIndex = CurrentIndexPosition == -1
                ? minItems
                : CurrentIndexPosition + 1 <= minItems ? minItems : CurrentIndexPosition + 1;
            var allItems = _animeItemsSet.Count;
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
            ViewModelLocator.GeneralMain.ScrollToTopButtonVisibility = CurrentIndexPosition > minItems;
            CurrentPage = (int)Math.Ceiling((double)allItems / ItemsPerPage);
            Loading = false;
            _randomedIds = new List<int>();
        }


        private int GetGridItemsToLoad()
        {
            if (DimensionsProvider?.ActualHeight < 0 && DimensionsProvider?.ActualWidth < 0)
                return int.MaxValue; //load all
            var width = DimensionsProvider?.ActualWidth ?? 1920;
            var height = DimensionsProvider?.ActualHeight ?? 1080;
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

        public void OnAnimeEntryRemoved(AnimeItemAbstraction parentAbstraction)
        {
            if (AnimeItems.Contains(parentAbstraction.ViewModel))
                AnimeItems.Remove(parentAbstraction.ViewModel);
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
        public void IndefiniteScrollViewerOnViewChanging(double finalVerticalOffset)
        {
            var offset = (int)finalVerticalOffset;
            ViewModelLocator.GeneralMain.ScrollToTopButtonVisibility = offset > 300 ? true : false;
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
                        itemsCount = (int)DimensionsProvider.ActualWidth/200;
                        AnimeItems.AddRange(_animeItemsSet.Take(itemsCount).Select(abstraction => abstraction.ViewModel));
                        _animeItemsSet = _animeItemsSet.Skip(itemsCount).ToList();
                        break;
                    case AnimeListDisplayModes.IndefiniteGrid:
                        itemsCount = (int)DimensionsProvider.ActualWidth / 160;
                        AnimeItems.AddRange(_animeItemsSet.Take(itemsCount).Select(abstraction => abstraction.ViewModel));
                        _animeItemsSet = _animeItemsSet.Skip(itemsCount).ToList();
                        break;
                    case AnimeListDisplayModes.IndefiniteCompactList:
                        itemsCount = (int)DimensionsProvider.ActualHeight / 50;
                        AnimeItems.AddRange(_animeItemsSet.Take(itemsCount).Select(abstraction => abstraction.ViewModel));
                        _animeItemsSet = _animeItemsSet.Skip(itemsCount).ToList();
                        break;
                }
            }
        }

        /// <summary>
        ///     Adds handler to scroll viewer provided by view.
        /// </summary>
        private void AddScrollHandler()
        {
            if (!CanAddScrollHandler || _scrollHandlerAdded)
                return;
            _lastOffset = 0; //we are resseting this because we ARE on the very to of the list view when adding handler
            _scrollHandlerAdded = true;
            try
            {
                AddScrollHandlerRequest?.Invoke();
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
        public  void ScrollToTop()
        { 
            ScrollToTopRequest?.Invoke();
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
            //get top or seasonal anime
            var data = new List<ISeasonalAnimeBaseData>();
            page = page == 0 ? 1 : page;
            switch (WorkMode)
            {
                case AnimeListWorkModes.SeasonalAnime:
                    var tResponse = new List<SeasonalAnimeData>();
                    await
                        Task.Run(
                            new Func<Task>(
                                async () =>
                                        tResponse = await new AnimeSeasonalQuery(CurrentSeason).GetSeasonalAnime(force)));
                    data.AddRange(tResponse ?? new List<SeasonalAnimeData>());
                    break; 
                case AnimeListWorkModes.TopAnime:
                case AnimeListWorkModes.TopManga:
                    var topResponse = new List<TopAnimeData>();
                    await
                        Task.Run(
                            new Func<Task>(
                                async () =>
                                    topResponse =
                                        await
                                            new AnimeTopQuery(
                                                WorkMode == AnimeListWorkModes.TopAnime
                                                    ? TopAnimeWorkMode
                                                    : TopAnimeType.Manga, page - 1).GetTopAnimeData(force)));
                    data.AddRange(topResponse ?? new List<TopAnimeData>());
                    break;
                case AnimeListWorkModes.AnimeByGenre:
                case AnimeListWorkModes.AnimeByStudio:
                    var sResponse = new List<SeasonalAnimeData>();
                    var query = WorkMode == AnimeListWorkModes.AnimeByStudio
                        ? new AnimeGenreStudioQuery(Studio,page)
                        : new AnimeGenreStudioQuery(Genre,page);
                    await Task.Run(new Func<Task>(async () => sResponse = await query.GetAnime()));
                    data.AddRange(sResponse ?? new List<SeasonalAnimeData>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
                if (_animeLibraryDataStorage.AllLoadedMangaItemAbstractions.Count == 0 && !_attemptedMangaFetch)
                    await FetchData(false, AnimeListWorkModes.Manga);

                target = _animeLibraryDataStorage.AllLoadedSeasonalMangaItems = new List<AnimeItemAbstraction>();
                source = _animeLibraryDataStorage.AllLoadedAuthMangaItems.Count > 0 ? _animeLibraryDataStorage.AllLoadedAuthMangaItems : new List<AnimeItemAbstraction>();
            }
            else
            {
                if (_animeLibraryDataStorage.AllLoadedAnimeItemAbstractions.Count == 0 && !_attemptedAnimeFetch)
                    await FetchData(false, AnimeListWorkModes.Anime);
                if ((WorkMode == AnimeListWorkModes.AnimeByGenre || WorkMode == AnimeListWorkModes.AnimeByStudio) && page > 1)
                {
                    target = _animeLibraryDataStorage.AllLoadedSeasonalAnimeItems;
                }
                else
                {
                    target = _animeLibraryDataStorage.AllLoadedSeasonalAnimeItems = new List<AnimeItemAbstraction>();                   
                }
                source = _animeLibraryDataStorage.AllLoadedAuthAnimeItems.Count > 0 ? _animeLibraryDataStorage.AllLoadedAuthAnimeItems : new List<AnimeItemAbstraction>();

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
                        if(animeData.AirDay != -1)
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
            if (WorkMode == AnimeListWorkModes.SeasonalAnime || WorkMode == AnimeListWorkModes.TopAnime ||
                WorkMode == AnimeListWorkModes.TopManga || WorkMode == AnimeListWorkModes.AnimeByGenre ||
                WorkMode == AnimeListWorkModes.AnimeByStudio)
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
                    _animeLibraryDataStorage.AllLoadedAnimeItemAbstractions = new List<AnimeItemAbstraction>();
                    if (force)
                        _animeLibraryDataStorage.AllLoadedAuthAnimeItems = new List<AnimeItemAbstraction>();
                    else if (_animeLibraryDataStorage.AllLoadedAuthAnimeItems.Count > 0 && string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                        _animeLibraryDataStorage.AllLoadedAnimeItemAbstractions = _animeLibraryDataStorage.AllLoadedAuthAnimeItems;
                    break;
                case AnimeListWorkModes.Manga:
                    _attemptedMangaFetch = true;
                    _animeLibraryDataStorage.AllLoadedMangaItemAbstractions = new List<AnimeItemAbstraction>();
                    if (force)
                        _animeLibraryDataStorage.AllLoadedAuthMangaItems = new List<AnimeItemAbstraction>();
                    else if (_animeLibraryDataStorage.AllLoadedAuthMangaItems.Count > 0 && string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                        _animeLibraryDataStorage.AllLoadedMangaItemAbstractions = _animeLibraryDataStorage.AllLoadedAuthMangaItems;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            if (requestedMode == AnimeListWorkModes.Anime ? _animeLibraryDataStorage.AllLoadedAnimeItemAbstractions.Count == 0 : _animeLibraryDataStorage.AllLoadedMangaItemAbstractions.Count == 0)
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
                            _animeLibraryDataStorage.AllLoadedAnimeItemAbstractions.Add(new AnimeItemAbstraction(auth, item as AnimeLibraryItemData));

                        if (string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                            _animeLibraryDataStorage.AllLoadedAuthAnimeItems = _animeLibraryDataStorage.AllLoadedAnimeItemAbstractions;
                        break;
                    case AnimeListWorkModes.Manga:
                        foreach (var item in data)
                            _animeLibraryDataStorage.AllLoadedMangaItemAbstractions.Add(new AnimeItemAbstraction(auth && Settings.SelectedApiType == ApiType.Mal, item as MangaLibraryItemData)); //read only manga for hummingbird

                        if (string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                            _animeLibraryDataStorage.AllLoadedAuthMangaItems = _animeLibraryDataStorage.AllLoadedMangaItemAbstractions;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _fetching = false;
            if (WorkMode != requestedMode)
                return; // manga or anime is loaded top manga can proceed loading something else

            AppBtnGoBackToMyListVisibility = Credentials.Authenticated && !string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase) ? true : false;
            //load tags
            ViewModelLocator.GeneralMain.SearchHints = _animeLibraryDataStorage.AllLoadedAuthAnimeItems.Concat(_animeLibraryDataStorage.AllLoadedAuthMangaItems).SelectMany(abs => abs.Tags).Distinct().ToList();
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
                    if (_animeLibraryDataStorage.AllLoadedAnimeItemAbstractions.Count == 0 && !_attemptedAnimeFetch)
                        await FetchData(false, AnimeListWorkModes.Anime);
                }
                else if (_animeLibraryDataStorage.AllLoadedMangaItemAbstractions.Count == 0 && !_attemptedMangaFetch)
                    await FetchData(false, AnimeListWorkModes.Manga);

                return anime ? _animeLibraryDataStorage.AllLoadedAuthAnimeItems.First(abstraction => forceMal ? abstraction.MalId == id : abstraction.Id == id).ViewModel : _animeLibraryDataStorage.AllLoadedAuthMangaItems.First(abstraction => forceMal ? abstraction.MalId == id : abstraction.Id == id).ViewModel;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region StatusRelatedStuff

        private void UpdateUpperStatus()
        {
            var page = ViewModelLocator.GeneralMain;

            if(page.CurrentMainPageKind != PageIndex.PageAnimeList) //we are in async void domain so we somethimes have to skip this
                return;

            if (WorkMode != AnimeListWorkModes.SeasonalAnime)
                if (WorkMode == AnimeListWorkModes.TopAnime)
                    page.CurrentStatus = $"Top {TopAnimeWorkMode} - {Utils.Utilities.StatusToString((int)GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";
                else if (WorkMode == AnimeListWorkModes.TopManga)
                    page.CurrentStatus = $"Top Manga - {Utils.Utilities.StatusToString((int)GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";
                else if (WorkMode == AnimeListWorkModes.AnimeByStudio)
                    page.CurrentStatus = $"Studio - {Studio.GetDescription()} Page 1 - {CurrentPage}";
                else if (WorkMode == AnimeListWorkModes.AnimeByGenre)
                    page.CurrentStatus = $"Genre - {Genre.GetDescription()} Page 1 - {CurrentPage}";
                else if (!string.IsNullOrWhiteSpace(ListSource))
                    page.CurrentStatus = $"{ListSource} - {Utils.Utilities.StatusToString((int)GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";
                else
                    page.CurrentStatus = $"{(WorkMode == AnimeListWorkModes.Anime ? "Anime list" : "Manga list")}";
            else
                page.CurrentStatus = $"{CurrentSeason?.Name} - {Utils.Utilities.StatusToString((int)GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";


            if (WorkMode == AnimeListWorkModes.Anime || WorkMode == AnimeListWorkModes.Manga || WorkMode == AnimeListWorkModes.SeasonalAnime || WorkMode == AnimeListWorkModes.AnimeByGenre || WorkMode == AnimeListWorkModes.AnimeByStudio)
                page.CurrentStatusSub = SortOption != SortOptions.SortWatched ? SortOption.GetDescription() : Sort3Label;
            else
                page.CurrentStatusSub = "";
        }

        public AnimeStatus GetDesiredStatus()
        {
            var value = StatusSelectorSelectedIndex;
            value++;
            return (AnimeStatus)(value == 0 ? 1 : value == 5 || value == 6 ? value + 1 : value);
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
            if (value == null &&
                (WorkMode == AnimeListWorkModes.SeasonalAnime || WorkMode == AnimeListWorkModes.AnimeByGenre ||
                 WorkMode == AnimeListWorkModes.AnimeByStudio || WorkMode == AnimeListWorkModes.TopAnime ||
                 WorkMode == AnimeListWorkModes.TopManga))
                value = (int) AnimeStatus.AllOrAiring;

            value = value ??
                    (WorkMode == AnimeListWorkModes.Manga ? Settings.DefaultMangaFilter : Settings.DefaultAnimeFilter);
            if (setDisp)
                SetDisplayMode((AnimeStatus) value);

            value = value == 6 || value == 7 ? value - 1 : value;
            value--;
            _initializing = true;
            StatusSelectorSelectedIndex = (int) value;
            _initializing = false;
        }

        #endregion

        #region AllItemLoading

        private bool _loadingAllDetailsVisibility;
        private int _allItemsToLoad;
        private int _itemsLoaded;
        private ICommand _cancelLoadingAllItemsCommand;
        private ICommand _loadAllItemsDetailsCommand;

        public bool LoadingAllDetailsVisibility
        {
            get { return _loadingAllDetailsVisibility; }
            set
            {
                _loadingAllDetailsVisibility = value;
                RaisePropertyChanged(() => LoadingAllDetailsVisibility);
            }
        }

        public int AllItemsToLoad
        {
            get { return _allItemsToLoad; }
            set
            {
                _allItemsToLoad = value;
                RaisePropertyChanged(() => AllItemsToLoad);
            }
        }

        public int ItemsLoaded
        {
            get { return _itemsLoaded; }
            set
            {
                _itemsLoaded = value;
                RaisePropertyChanged(() => ItemsLoaded);
                RaisePropertyChanged(() => LoadingItemsStatus);
            }
        }

        public string LoadingItemsStatus => $"{ItemsLoaded}/{AllItemsToLoad}";

        public ICommand CancelLoadingAllItemsCommand
            => _cancelLoadingAllItemsCommand ?? (_cancelLoadingAllItemsCommand = new RelayCommand(() => _cancelLoadingAllItems = true));

        public ICommand LoadAllItemsDetailsCommand
            => _loadAllItemsDetailsCommand ?? (_loadAllItemsDetailsCommand = new RelayCommand(LoadAllItemsDetails));

        public bool IsMangaWorkMode => WorkMode == AnimeListWorkModes.Manga || WorkMode == AnimeListWorkModes.TopManga;

        private bool _cancelLoadingAllItems;
        private bool _wasPreviousQuery;

        private async void LoadAllItemsDetails()
        {
            var idsToFetch = new List<AnimeItemAbstraction>();
            foreach (var animeItemViewModel in _animeItemsSet.Concat(AnimeItems.Select(model => model.ParentAbstraction)))
            {
                if (!animeItemViewModel.LoadedVolatile)
                    idsToFetch.Add(animeItemViewModel);
            }


            if (idsToFetch.Count > 0)
            {
                ItemsLoaded = 0;
                LoadingAllDetailsVisibility = true;
                AllItemsToLoad = idsToFetch.Count;
                foreach (var abstraction in idsToFetch)
                {
                    if (_cancelLoadingAllItems)
                    {
                        _cancelLoadingAllItems = false;
                        break;
                    }

                    try
                    {
                        var data =
                            await
                                new AnimeGeneralDetailsQuery().GetAnimeDetails(false, abstraction.Id.ToString(),
                                    abstraction.Title, true);
                        int day;
                        try
                        {
                            day = data.StartDate != AnimeItemViewModel.InvalidStartEndDate &&
                                  (string.Equals(data.Status, "Currently Airing",
                                      StringComparison.CurrentCultureIgnoreCase) ||
                                   string.Equals(data.Status, "Not yet aired", StringComparison.CurrentCultureIgnoreCase))
                                ? (int)DateTime.Parse(data.StartDate).DayOfWeek + 1
                                : -1;
                            if (day == -1)
                                abstraction.AirDay = -1;
                        }
                        catch (Exception)
                        {
                            day = -1;
                        }

                        DataCache.RegisterVolatileData(abstraction.Id, new VolatileDataCache
                        {
                            DayOfAiring = day,
                            GlobalScore = data.GlobalScore,
                            AirStartDate =
                                data.StartDate == AnimeItemViewModel.InvalidStartEndDate ? null : data.StartDate
                        });
                        if (!abstraction.LoadedVolatile)
                        {
                            if (abstraction.TryRetrieveVolatileData())
                            {
                                if(abstraction.LoadedModel)
                                    abstraction.ViewModel.UpdateVolatileDataBindings();
                            }
                        }
                        ItemsLoaded++;
                    }
                    catch (Exception e)
                    {
                        //searching for crash source
                    }
                }
                LoadingAllDetailsVisibility = false;
            }
        }

        #endregion


    }
}