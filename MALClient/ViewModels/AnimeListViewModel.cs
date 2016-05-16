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
using MALClient.Comm;
using MALClient.Items;
using MALClient.Models;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public class AnimeSeason
    {
        public string Url;
        public string Name { get; set; }
    }

    public enum AnimeListDisplayModes
    {
        IndefiniteList,
        IndefiniteGrid,
        IndefiniteCompactList
    }

    public delegate void AnimeItemListInitialized();

    public class AnimeListViewModel : ViewModelBase
    {
        private const int ItemPrefferedWidth = 385;

        private List<AnimeItemAbstraction> _allLoadedAuthAnimeItems = new List<AnimeItemAbstraction>();
        private List<AnimeItemAbstraction> _allLoadedAuthMangaItems = new List<AnimeItemAbstraction>();
        private List<AnimeItemAbstraction> _allLoadedSeasonalAnimeItems = new List<AnimeItemAbstraction>();
        private List<AnimeItemAbstraction> _allLoadedSeasonalMangaItems = new List<AnimeItemAbstraction>();

        private int _allPages;

        private List<AnimeItemAbstraction> _animeItemsSet =
            new List<AnimeItemAbstraction>(); //All for current list        

        private bool _initializing;

        public bool Initializing
        {
            get { return _initializing; }
            private set
            {
                _initializing = value;
                if(!value)
                    Initialized?.Invoke();
            }
        }

        private AnimeListDisplayModes? _manuallySelectedViewMode;
        private string _prevListSource;


        private AnimeListWorkModes _prevWorkMode = AnimeListWorkModes.Anime;
        private bool _scrollHandlerAdded;


        private bool _wasPreviousQuery;

        public bool CanAddScrollHandler;
        public AnimeSeason CurrentSeason;

        public List<AnimeItemAbstraction> AllLoadedAnimeItemAbstractions { get; private set; } =
            new List<AnimeItemAbstraction>();

        public List<AnimeItemAbstraction> AllLoadedMangaItemAbstractions { get; private set; } =
            new List<AnimeItemAbstraction>();

        public SmartObservableCollection<AnimeItemViewModel> AnimeItems { get; private set; } =
            new SmartObservableCollection<AnimeItemViewModel>();

        public ObservableCollection<AnimeSeason> SeasonSelection { get; } = new ObservableCollection<AnimeSeason>();


        public bool AreThereItemsWaitingForLoad => _animeItemsSet.Count != 0;
        public int CurrentStatus => GetDesiredStatus();

        public double ListItemGridWidth
        {
            get
            {
                var width = View.ActualWidth;
                var items = (int) width/ItemPrefferedWidth;
                var widthRest = width - items*ItemPrefferedWidth;
                return ItemPrefferedWidth + widthRest/items;
            }
        }

        public event AnimeItemListInitialized Initialized;


        public async Task Init(AnimeListPageNavigationArgs args)
        {
            //base
            _scrollHandlerAdded = false;
            Initializing = true;
            _manuallySelectedViewMode = null;
            //take out trash
            _animeItemsSet = new List<AnimeItemAbstraction>();
            AnimeItems = new SmartObservableCollection<AnimeItemViewModel>();
            RaisePropertyChanged(() => AnimeItems);

            //give visual feedback
            Loading = true;
            /*so the thing is:
                 * Without this delay app will have quite a bit of chance to crash due to some win32 exception.
                 * Sometimes it's access violation, sometimes it doesn't even say.
                 * I've read that image caching fails when thre's some kind of load on it.
                 * This is why this delay is here, 10ms of user's life isn't that bad,ain't it?             
                */
            await Task.Delay(20);

            //depending on args
            var gotArgs = false;
            if (args != null) //Save current mode
            {
                WorkMode = args.WorkMode;
                if (args.NavArgs) // Use args if we have any
                {
                    ListSource = args.ListSource;
                    SortDescending = SortDescending = args.Descending;
                    SetSortOrder(args.SortOption); //index
                    SetDesiredStatus(args.Status);
                    CurrentPosition = args.CurrPage;
                    CurrentSeason = args.CurrSeason;
                    DisplayMode = args.DisplayMode;
                    gotArgs = true;
                }
            }
            else //assume default AnimeList
            {
                WorkMode = AnimeListWorkModes.Anime;
            }
            ViewModelLocator.Hamburger.UpdateAnimeFiltersSelectedIndex();
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
                    if (WorkMode == AnimeListWorkModes.TopAnime || WorkMode == AnimeListWorkModes.TopManga)
                        AppbarBtnPinTileVisibility = AppBtnSortingVisibility = Visibility.Collapsed;
                    else
                        AppbarBtnPinTileVisibility = AppBtnSortingVisibility = Visibility.Visible;

                    AppBtnListSourceVisibility = false;
                    AppBtnGoBackToMyListVisibility = Visibility.Collapsed;
                    BtnSetSourceVisibility = false;

                    if (!gotArgs)
                    {
                        SortDescending = false;
                        SetSortOrder(SortOptions.SortWatched); //index
                        SetDesiredStatus(null);
                        CurrentSeason = null;
                    }
                    StatusAllLabel = WorkMode == AnimeListWorkModes.SeasonalAnime ? "Airing" : "All";

                    Sort3Label = "Index";
                    await FetchSeasonalData();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            View.InitSortOptions(SortOption, SortDescending);
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
        public async Task RefreshList(bool searchSource = false, bool fakeDelay = false)
        {
            var finished = false;
            await Task.Run(() =>
            {
                var query = ViewModelLocator.Main.CurrentSearchQuery;
                var queryCondition = !string.IsNullOrWhiteSpace(query) && query.Length > 1;
                if (!_wasPreviousQuery && searchSource && !queryCondition)
                    // refresh was requested from search but there's nothing to update
                {
                    finished = true;
                    return;
                }

                _wasPreviousQuery = queryCondition;

                _animeItemsSet.Clear();
                var status = queryCondition ? 7 : GetDesiredStatus();

                IEnumerable<AnimeItemAbstraction> items;
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

                items = items.Where(item => queryCondition || status == 7 || item.MyStatus == status);

                if (queryCondition)
                    items = items.Where(item => item.Title.ToLower().Contains(query.ToLower()));
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
                        default:
                            throw new ArgumentOutOfRangeException(nameof(SortOption), SortOption, null);
                    }
                //If we are descending then reverse order
                if (SortDescending && SortOption != SortOptions.SortAirDay)
                    items = items.Reverse();
                //Add all abstractions to current set (spread across pages)
                foreach (var item in items)
                    _animeItemsSet.Add(item);
            });
            if (finished)
                return;
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

        public void UpdateGridItemWidth()
        {
            RaisePropertyChanged(() => ListItemGridWidth);
        }

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
        private int unloadReoccurences;

        private void IndefiniteScrollViewerOnViewChanging(object sender, ScrollViewerViewChangingEventArgs args)
        {
            var offset = (int) Math.Ceiling(args.FinalView.VerticalOffset);
            CurrentPosition = offset;
            ViewModelLocator.Main.ScrollToTopButtonVisibility = CurrentPosition > 300
                ? Visibility.Visible
                : Visibility.Collapsed;
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
                        itemsCount = (int) (sender as FrameworkElement).ActualWidth/400;
                        AnimeItems.AddRange(_animeItemsSet.Take(itemsCount).Select(abstraction => abstraction.ViewModel));
                        _animeItemsSet = _animeItemsSet.Skip(itemsCount).ToList();
                        break;
                    case AnimeListDisplayModes.IndefiniteGrid:
                        itemsCount = (int) (sender as FrameworkElement).ActualWidth/200;
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
        private void AddScrollHandler()
        {
            if (!CanAddScrollHandler || _scrollHandlerAdded)
                return;
            _lastOffset = 0; //we are resseting this because we ARE on the very to of the list view when adding handler
            _scrollHandlerAdded = true;
            View.GetIndefiniteScrollViewer().Result.ViewChanging += IndefiniteScrollViewerOnViewChanging;
        }

        /// <summary>
        ///     To make it more probable that the list will scroll to right position wait a bit before srolling there.
        ///     It works more or less...
        /// </summary>
        /// <param name="delay"></param>
        private async void ScrollToWithDelay(int delay)
        {
            await Task.Delay(delay);
            View.GetIndefiniteScrollViewer().Result.ScrollToVerticalOffset(CurrentPosition);
        }

        /// <summary>
        ///     Scrolls to top of current indefinite scroll viewer.
        /// </summary>
        public void ScrollToTop()
        {
            CurrentPosition = 0;
            View.GetIndefiniteScrollViewer().Result.ScrollToVerticalOffset(0);
            ViewModelLocator.Main.ScrollToTopButtonVisibility = Visibility.Collapsed;
        }

        #endregion

        #region Pagination

        /// <summary>
        ///     This method is fully responsible for preparing the view.
        ///     Depending on display mode it distributes items to right containers.
        /// </summary>
        private async void UpdatePageSetup()
        {
            AnimeItems = new SmartObservableCollection<AnimeItemViewModel>();
            _lastOffset = 0;
            RaisePropertyChanged(() => DisplayMode);
            await Task.Delay(30);
            switch (DisplayMode)
            {
                case AnimeListDisplayModes.IndefiniteCompactList:
                    AnimeItems.AddRange(_animeItemsSet.Take(30).Select(abstraction => abstraction.ViewModel));
                    _animeItemsSet = _animeItemsSet.Skip(30).ToList();
                    View.GetIndefiniteScrollViewer().Result.UpdateLayout();
                    break;
                case AnimeListDisplayModes.IndefiniteList:
                    var itemsToLoad = GetItemsToLoad();
                    AnimeItems.AddRange(_animeItemsSet.Take(itemsToLoad).Select(abstraction => abstraction.ViewModel));
                    _animeItemsSet = _animeItemsSet.Skip(itemsToLoad).ToList();
                    View.GetIndefiniteScrollViewer().Result.UpdateLayout();
                    View.GetIndefiniteScrollViewer().Result.ScrollToVerticalOffset(CurrentPosition);
                    break;
                case AnimeListDisplayModes.IndefiniteGrid:
                    var gridItemsToLoad = GetGridItemsToLoad();
                    AnimeItems.AddRange(_animeItemsSet.Take(gridItemsToLoad)
                        .Select(abstraction => abstraction.ViewModel));
                    _animeItemsSet = _animeItemsSet.Skip(gridItemsToLoad).ToList();
                    View.GetIndefiniteScrollViewer().Result.UpdateLayout();
                    ScrollToWithDelay(500);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RaisePropertyChanged(() => AnimeItems);
            AddScrollHandler();
            ViewModelLocator.Main.ScrollToTopButtonVisibility = CurrentPosition > 300
                ? Visibility.Visible
                : Visibility.Collapsed;
            Loading = false;
        }

        private int GetItemsToLoad()
        {
            var width = View.ActualWidth;
            if (width < 300)
                width = 500;
            var height = View.ActualHeight;
            var result = (int) width/350;
            result *= (int) height/150;
            result = (int) (result*1.5);
            if (result < 10)
                return 10;
            return result;
        }

        private int GetGridItemsToLoad()
        {
            var width = View.ActualWidth;
            if (width < 300)
                width = 500;
            var height = View.ActualHeight - 200;
            var result = (int) width/200;
            result *= (int) height/230;
            result = (int) (result*1.5);
            if (result < 10)
                return 10;
            return result;
        }

        #endregion

        #region FetchAndPopulate

        /// <summary>
        ///     Fetches seasonal data and top manga/anime.
        ///     Results are saved in appropriate containers for further operations.
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        private async Task FetchSeasonalData(bool force = false)
        {
            Loading = true;
            EmptyNoticeVisibility = false;
            var setDefaultSeason = false;
            if (CurrentSeason == null)
            {
                CurrentSeason = new AnimeSeason {Name = "Airing", Url = "http://myanimelist.net/anime/season"};
                setDefaultSeason = true;
            }
            Utils.GetMainPageInstance().CurrentStatus = "Downloading data...\nThis may take a while...";
            //get top or seasonal anime
            var data = new List<ISeasonalAnimeBaseData>();
            switch (WorkMode)
            {
                case AnimeListWorkModes.SeasonalAnime:
                    var tResponse = new List<SeasonalAnimeData>();
                    await Task.Run(new Func<Task>(async () =>
                        tResponse = await new AnimeSeasonalQuery(CurrentSeason).GetSeasonalAnime()));
                    data.AddRange(tResponse);
                    break;
                case AnimeListWorkModes.TopAnime:
                case AnimeListWorkModes.TopManga:
                    var topResponse = new List<TopAnimeData>();
                    await Task.Run(new Func<Task>(async () =>
                        topResponse =
                            await new AnimeTopQuery(WorkMode == AnimeListWorkModes.TopAnime).GetTopAnimeData(force)));
                    data.AddRange(topResponse);
                    break;
            }
            //if we don't have any we cannot do anything I guess...
            if (data.Count == 0)
            {
                await RefreshList();
                Loading = false;
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
                source = _allLoadedAuthMangaItems.Count > 0
                    ? _allLoadedAuthMangaItems
                    : new List<AnimeItemAbstraction>();
            }
            else
            {
                if (AllLoadedAnimeItemAbstractions.Count == 0 && !_attemptedAnimeFetch)
                    await FetchData(false, AnimeListWorkModes.Anime);

                target = _allLoadedSeasonalAnimeItems = new List<AnimeItemAbstraction>();
                source = _allLoadedAuthAnimeItems.Count > 0
                    ? _allLoadedAuthAnimeItems
                    : new List<AnimeItemAbstraction>();
            }

            bool updateScore = Settings.SelectedApiType == ApiType.Mal;
            foreach (var animeData in data)
            {
                try
                {
                    if (WorkMode == AnimeListWorkModes.SeasonalAnime && Settings.SelectedApiType == ApiType.Mal) //seasonal anme comes with mal score, we don't want to polute hummingbird data
                        DataCache.RegisterVolatileData(animeData.Id, new VolatileDataCache
                        {
                            DayOfAiring = animeData.AirDay,
                            GlobalScore = animeData.Score,
                            Genres = animeData.Genres,
                            AirStartDate =
                                animeData.AirStartDate == AnimeItemViewModel.InvalidStartEndDate
                                    ? null
                                    : animeData.AirStartDate
                        });
                    AnimeItemAbstraction abstraction = null;
                    if (Settings.SelectedApiType == ApiType.Mal)
                        abstraction = source.FirstOrDefault(item => item.Id == animeData.Id);
                    else
                        abstraction = source.FirstOrDefault(item => item.MalId == animeData.Id);
                    if (abstraction == null)
                        target.Add(new AnimeItemAbstraction(animeData as SeasonalAnimeData,
                            WorkMode != AnimeListWorkModes.TopManga));
                    else
                    {
                        abstraction.AirDay = animeData.AirDay;
                        if(updateScore)
                            abstraction.GlobalScore = animeData.Score;
                        abstraction.Index = animeData.Index;
                        abstraction.ViewModel.UpdateWithSeasonData(animeData as SeasonalAnimeData,updateScore);
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
            await RefreshList();
        }

        /// <summary>
        ///     Forces currently loaded page to download new data.
        /// </summary>
        private async void ReloadList()
        {
            if (WorkMode == AnimeListWorkModes.SeasonalAnime || WorkMode == AnimeListWorkModes.TopAnime ||
                WorkMode == AnimeListWorkModes.TopManga)
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
        public async Task FetchData(bool force = false, AnimeListWorkModes? modeOverride = null)
        {
            AnimeListWorkModes requestedMode;
            requestedMode = modeOverride ?? WorkMode;

            if (!force && _prevListSource == ListSource && _prevWorkMode == requestedMode)
            {
                foreach (var item in AllLoadedAnimeItemAbstractions.Where(abstraction => abstraction.LoadedAnime))
                {
                    item.ViewModel.SignalBackToList();
                }
                if (_prevWorkMode != modeOverride)
                    await RefreshList();
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
                    else if (_allLoadedAuthAnimeItems.Count > 0 &&
                             string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                        AllLoadedAnimeItemAbstractions = _allLoadedAuthAnimeItems;
                    break;
                case AnimeListWorkModes.Manga:
                    _attemptedMangaFetch = true;
                    AllLoadedMangaItemAbstractions = new List<AnimeItemAbstraction>();
                    if (force)
                        _allLoadedAuthMangaItems = new List<AnimeItemAbstraction>();
                    else if (_allLoadedAuthMangaItems.Count > 0 &&
                             string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                        AllLoadedMangaItemAbstractions = _allLoadedAuthMangaItems;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            if (requestedMode == AnimeListWorkModes.Anime
                ? AllLoadedAnimeItemAbstractions.Count == 0
                : AllLoadedMangaItemAbstractions.Count == 0)
            {
                List<ILibraryData> data = null;
                await
                    Task.Run(async () => data = await new LibraryListQuery(ListSource, requestedMode).GetLibrary(force));
                if (data?.Count == 0)
                {
                    //no data?
                    await RefreshList();
                    Loading = false;
                    return;
                }

                var auth = Credentials.Authenticated &&
                           string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase);
                switch (requestedMode)
                {
                    case AnimeListWorkModes.Anime:

                        foreach (var item in data)
                            AllLoadedAnimeItemAbstractions.Add(new AnimeItemAbstraction(auth,
                                item as AnimeLibraryItemData));

                        if (string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                            _allLoadedAuthAnimeItems = AllLoadedAnimeItemAbstractions;
                        break;
                    case AnimeListWorkModes.Manga:
                        foreach (var item in data)
                            AllLoadedMangaItemAbstractions.Add(
                                new AnimeItemAbstraction(auth && Settings.SelectedApiType == ApiType.Mal,
                                    item as MangaLibraryItemData)); //read only manga for hummingbird

                        if (string.Equals(ListSource, Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                            _allLoadedAuthMangaItems = AllLoadedMangaItemAbstractions;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (WorkMode != requestedMode)
                return; // manga or anime is loaded top manga can proceed loading something else

            AppBtnGoBackToMyListVisibility = Credentials.Authenticated &&
                                             !string.Equals(ListSource, Credentials.UserName,
                                                 StringComparison.CurrentCultureIgnoreCase)
                ? Visibility.Visible
                : Visibility.Collapsed;

            await RefreshList();
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

                return anime
                    ? _allLoadedAuthAnimeItems.First(
                        abstraction => forceMal ? abstraction.MalId == id : abstraction.Id == id).ViewModel
                    : _allLoadedAuthMangaItems.First(
                        abstraction => forceMal ? abstraction.MalId == id : abstraction.Id == id).ViewModel;
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

        public int CurrentPosition { get; set; } = 1;

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

        public Visibility HumApiSpecificControlsVisibility
            => Settings.SelectedApiType == ApiType.Mal ? Visibility.Collapsed : Visibility.Visible;

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

        private Visibility _upperCommandBarVisibility = Settings.IsPivotFilterBarVisible
            ? Visibility.Visible
            : Visibility.Collapsed;

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
                ViewModelLocator.Hamburger.UpdateAnimeFiltersSelectedIndex();
                Loading = true;
                CurrentPosition = 1;
                _lastOffset = 0;
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
                View.ResetSelectionForMode(DisplayMode);
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
                return _setSortModeCommand ??
                       (_setSortModeCommand = new RelayCommand<string>(s =>
                       {
                           SetSortOrder((SortOptions) int.Parse(s));
                           RefreshList();
                       }));
            }
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(ReloadList)); }
        }

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

        public AnimeListPage View { get; set; }

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
                CurrentPosition = 1;
                if (Settings.HideViewSelectionFlyout)
                    View.FlyoutViews.Hide();
                RaisePropertyChanged(() => DisplayMode);
                RefreshList(false, true);
            }
        }

        public ObservableCollection<Tuple<AnimeListDisplayModes, string>> DisplayModes { get; } = new ObservableCollection
            <Tuple<AnimeListDisplayModes, string>>
        {
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteList, "Detailed Grid"),
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteGrid, "Grid"),
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteCompactList, "Compact list")
        };

        private Visibility _animesPivotHeaderVisibility;

        public Visibility AnimesPivotHeaderVisibility
        {
            get { return _animesPivotHeaderVisibility; }
            set
            {
                _animesPivotHeaderVisibility = value;
                PivotHeaerGridRowHeight = value == Visibility.Collapsed ? new GridLength(0) : new GridLength(40);
                RaisePropertyChanged(() => AnimesPivotHeaderVisibility);
            }
        }

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

        private int _animesPivotSelectedIndex;

        public int AnimesPivotSelectedIndex
        {
            get { return _animesPivotSelectedIndex; }
            set
            {
                _animesPivotSelectedIndex = value;
                CurrentPosition = value + 1;
                //AppbarBtnPinTileIsEnabled = false;
                RaisePropertyChanged(() => AnimesPivotSelectedIndex);
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
                CurrentPosition = 1;
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
                CurrentPosition = 1;
            }
        }

        #endregion

        #region StatusRelatedStuff

        private async void UpdateUpperStatus(int retries = 5)
        {
            while (true)
            {
                var page = Utils.GetMainPageInstance();

                if (page != null)

                    if (WorkMode != AnimeListWorkModes.SeasonalAnime)
                        if (WorkMode == AnimeListWorkModes.TopAnime)
                            page.CurrentStatus =
                                $"Top Anime - {Utils.StatusToString(GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";
                        else if (WorkMode == AnimeListWorkModes.TopManga)
                            page.CurrentStatus =
                                $"Top Manga - {Utils.StatusToString(GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";
                        else if (!string.IsNullOrWhiteSpace(ListSource))
                            page.CurrentStatus =
                                $"{ListSource} - {Utils.StatusToString(GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";
                        else
                            page.CurrentStatus =
                                $"{(WorkMode == AnimeListWorkModes.Anime ? "Anime list" : "Manga list")}";
                    else
                        page.CurrentStatus =
                            $"{CurrentSeason?.Name} - {Utils.StatusToString(GetDesiredStatus(), WorkMode == AnimeListWorkModes.Manga)}";

                else if (retries >= 0)
                {
                    await Task.Delay(1000);
                    retries = retries - 1;
                    continue;
                }
                break;
            }
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
            if (value == null &&
                (WorkMode == AnimeListWorkModes.SeasonalAnime || WorkMode == AnimeListWorkModes.TopAnime ||
                 WorkMode == AnimeListWorkModes.TopManga))
                value = (int) AnimeStatus.AllOrAiring;

            value = value ??
                    (WorkMode == AnimeListWorkModes.Manga ? Settings.DefaultMangaFilter : Settings.DefaultAnimeFilter);
            if (setDisp)
                SetDisplayMode((AnimeStatus) value);

            value = value == 6 || value == 7 ? value - 1 : value;
            value--;

            StatusSelectorSelectedIndex = (int) value;
        }

        //private void UpdateStatusCounterBadges()
        //{
        //    Dictionary<int, int> counters = new Dictionary<int, int>();
        //    for (var i = AnimeStatus.Watching; i <= AnimeStatus.PlanToWatch; i++)
        //        counters[(int)i] = 0;
        //    foreach (AnimeItemAbstraction animeItemAbstraction in _allLoadedAnimeItems)
        //    {
        //        if (animeItemAbstraction.MyStatus <= 6)
        //            counters[animeItemAbstraction.MyStatus]++;
        //    }
        //    var j = AnimeStatus.Watching;
        //    foreach (object item in StatusSelector.Items)
        //    {
        //        (item as ListViewItem).Content = counters[(int)j] + " - " + Utils.StatusToString((int)j);
        //        j++;
        //        if ((int)j == 5)
        //            j++;
        //        if (j == AnimeStatus.AllOrAiring)
        //            return;
        //    }
        //}

        //private string GetLastUpdatedStatus()
        //{
        //    if (WorkMode == AnimeListWorkModes.SeasonalAnime)
        //        return "";
        //    var output = "Updated ";
        //    try
        //    {
        //        TimeSpan lastUpdateDiff = DateTime.Now.Subtract(_lastUpdate);
        //        if (lastUpdateDiff.Days > 0)
        //            output += lastUpdateDiff.Days + "day" + (lastUpdateDiff.Days > 1 ? "s" : "") + " ago.";
        //        else if (lastUpdateDiff.Hours > 0)
        //        {
        //            output += lastUpdateDiff.Hours + "hour" + (lastUpdateDiff.Hours > 1 ? "s" : "") + " ago.";
        //        }
        //        else if (lastUpdateDiff.Minutes > 0)
        //        {
        //            output += $"{lastUpdateDiff.Minutes} minute" + (lastUpdateDiff.Minutes > 1 ? "s" : "") + " ago.";
        //        }
        //        else
        //        {
        //            output += "just now.";
        //        }
        //        if (lastUpdateDiff.Days < 20000) //Seems like reasonable workaround
        //            UpdateNoticeVisibility = true;
        //    }
        //    catch (Exception)
        //    {
        //        output = "";
        //    }

        //    return output;
        //}

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