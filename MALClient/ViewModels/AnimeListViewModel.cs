using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public class AnimeListViewModel : ViewModelBase
    {
        private List<AnimeItemAbstraction> _allLoadedAnimeItems = new List<AnimeItemAbstraction>();
        private List<AnimeItemAbstraction> _allLoadedSeasonalAnimeItems = new List<AnimeItemAbstraction>();

        public readonly ObservableCollection<AnimeItem> _animeItems = new ObservableCollection<AnimeItem>(); // + Page

        private readonly ObservableCollection<AnimeItemAbstraction> _animeItemsSet =
            new ObservableCollection<AnimeItemAbstraction>(); //All for current list

        private readonly int _itemsPerPage = Utils.GetItemsPerPage();

        private readonly List<Parameter<bool>> _sortsCheckStatuses = new List<Parameter<bool>>
        {
            new Parameter<bool>(true),
            new Parameter<bool>(false),
            new Parameter<bool>(false),
            new Parameter<bool>(false),
            new Parameter<bool>(false),
        }; 
        private int _allPages;


        private DateTime _lastUpdate;
        private Timer _timer;
        private string _prevListSource;
        private bool _seasonalState;
        private bool _wasPreviousQuery;

        private SortOptions _sortOption = SortOptions.SortNothing;
        public SortOptions SortOption
        {
            get { return _sortOption; }
            set
            {
                _sortOption = value;
                RefreshList();
            }
        }

        public int CurrentStatus => GetDesiredStatus();
        public string CurrentUpdateStatus => GetLastUpdatedStatus();         
        public string CurrentPageStatus => $"{_currentPage}/{_allPages}";
        public ObservableCollection<AnimeItem> AnimeItems => _animeItems;
        public List<Parameter<bool>> SortCheckStates => _sortsCheckStatuses;
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

        private int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                RaisePropertyChanged(() => CurrentPageStatus);
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

        private bool _animesTopPageControlsVisibility;
        public bool AnimesTopPageControlsVisibility
        {
            get { return _animesTopPageControlsVisibility; }
            set
            {
                _animesTopPageControlsVisibility = value;
                RaisePropertyChanged(() => AnimesTopPageControlsVisibility);
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

        private bool _appbarBtnPinTileVisibility = true;
        public bool AppbarBtnPinTileVisibility
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

        private bool _prevPageButtonEnableState;
        public bool PrevPageButtonEnableState
        {
            get { return _prevPageButtonEnableState; }
            set
            {
                _prevPageButtonEnableState = value;
                RaisePropertyChanged(() => PrevPageButtonEnableState);
            }
        }

        private bool _nextPageButtonEnableState;
        public bool NextPageButtonEnableState
        {
            get { return _nextPageButtonEnableState; }
            set
            {
                _nextPageButtonEnableState = value;
                RaisePropertyChanged(() => NextPageButtonEnableState);
            }
        }        

        private int _statusSelectorSelectedIndex;
        public int StatusSelectorSelectedIndex
        {
            get { return _statusSelectorSelectedIndex; }
            set
            {
                if(value == _statusSelectorSelectedIndex)
                    return;

                _statusSelectorSelectedIndex = value;
                RefreshList();
                RaisePropertyChanged(() => StatusSelectorSelectedIndex);
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

        private ICommand _prevPageCommand;
        public ICommand PrevPageCommand
        {
            get
            {
                return _prevPageCommand ??
                       (_prevPageCommand = new RelayCommand(PrevPage));
            }
        }

        private ICommand _nextPageCommand;
        public ICommand NextPageCommand
        {
            get
            {
                return _nextPageCommand ??
                       (_nextPageCommand = new RelayCommand(NextPage));
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ??
                       (_refreshCommand = new RelayCommand(ReloadList));
            }
        }

        private AnimeListPage _view;
        public AnimeListPage View
        {
            get { return _view; }
            set
            {
                _view = value;
                //Init
            }
        }

        public bool IsSeasonal => _seasonalState;
#endregion
        public async void Init(AnimeListPageNavigationArgs args) // TODO : Refactor this 
        {
            if (args != null)
            {
                if (args.LoadSeasonal)
                {
                    _seasonalState = true;
                    Loading = true;
                    EmptyNoticeVisibility = false;
                    AppbarBtnPinTileVisibility = false;
                    AppBtnListSourceVisibility = false;

                    if (args.NavArgs)
                    {
                        ListSource = args.ListSource;
                        SortDescending = SortDescending = args.Descending;
                        SetSortOrder(args.SortOption); //index
                        SetDesiredStatus(args.Status);
                        CurrentPage = args.CurrPage;
                    }
                    else
                    {
                        SortDescending = false;
                        SetSortOrder(SortOptions.SortWatched); //index
                        SetDesiredStatus((int)AnimeStatus.AllOrAiring);
                    }

                    SwitchFiltersToSeasonal();
                    SwitchSortingToSeasonal();

                    await Task.Run(async () =>
                    {
                        await
                            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                                async () => { await FetchSeasonalData(); });
                    });
                    return;
                } // else we just have nav data

                ListSource = args.ListSource;
                SetSortOrder(args.SortOption);
                SetDesiredStatus(args.Status);
                SortDescending = args.Descending;
                CurrentPage = args.CurrPage;
            }
            else // default
                SetDefaults();

            _seasonalState = false;
            AppbarBtnPinTileVisibility = true;
            Sort3Label = "Watching";
            StatusAllLabel = "All";
            AppBtnListSourceVisibility = true;

            if (string.IsNullOrWhiteSpace(ListSource))
            {
                if (!string.IsNullOrWhiteSpace(Creditentials.UserName))
                    ListSource = Creditentials.UserName;
            }
            if (string.IsNullOrWhiteSpace(ListSource))
            {
                EmptyNoticeVisibility = true;
                EmptyNoticeContent = "We have come up empty...\nList source is not set.\nLog in or set it manually.";
                BtnSetSourceVisibility = true;
                UpdateUpperStatus();
            }
            else
            {
                await FetchData();
            }

            if (_timer == null)
                _timer = new Timer(state => { UpdateStatus(); }, null, (int)TimeSpan.FromMinutes(1).TotalMilliseconds,
                    (int)TimeSpan.FromMinutes(1).TotalMilliseconds);

            UpdateStatus();
        }


        #region Helpers
        private string GetLastUpdatedStatus()
        {
            var output = "Updated ";
            try
            {
                TimeSpan lastUpdateDiff = DateTime.Now.Subtract(_lastUpdate);
                if (lastUpdateDiff.Days > 0)
                    output += lastUpdateDiff.Days + "day" + (lastUpdateDiff.Days > 1 ? "s" : "") + " ago.";
                else if (lastUpdateDiff.Hours > 0)
                {
                    output += lastUpdateDiff.Hours + "hour" + (lastUpdateDiff.Hours > 1 ? "s" : "") + " ago.";
                }
                else if (lastUpdateDiff.Minutes > 0)
                {
                    output += $"{lastUpdateDiff.Minutes} minute" + (lastUpdateDiff.Minutes > 1 ? "s" : "") + " ago.";
                }
                else
                {
                    output += "just now.";
                }
                if (lastUpdateDiff.Days < 20000) //Seems like reasonable workaround
                    UpdateNoticeVisibility = true;
            }
            catch (Exception)
            {
                output = "";
            }

            return output;
        }

        private void AlternateRowColors()
        {
            for (var i = 0; i < _animeItems.Count; i++)
            {
                _animeItems[i].ViewModel.Setbackground(
                    new SolidColorBrush((i + 1) % 2 == 0 ? Color.FromArgb(170, 230, 230, 230) : Colors.Transparent));
            }
        }

        private void SetSortOrder(SortOptions? option)
        {
            switch (option ?? Utils.GetSortOrder())
            {
                case SortOptions.SortNothing:
                    SortOption = SortOptions.SortNothing;
                    _sortsCheckStatuses[3].Value = true;
                    break;
                case SortOptions.SortTitle:
                    SortOption = SortOptions.SortTitle;
                    _sortsCheckStatuses[0].Value = true;
                    break;
                case SortOptions.SortScore:
                    SortOption = SortOptions.SortScore;
                    _sortsCheckStatuses[1].Value = true;
                    break;
                case SortOptions.SortWatched:
                    SortOption = SortOptions.SortWatched;
                    _sortsCheckStatuses[2].Value = true;
                    break;
                case SortOptions.SortAirDay:
                    SortOption = SortOptions.SortAirDay;
                    _sortsCheckStatuses[4].Value = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void UpdateStatus()
        {
            await
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { RaisePropertyChanged(() => CurrentUpdateStatus); });
        }

        private void SetDefaults()
        {
            SetSortOrder(null);
            SetDesiredStatus(null);
            SortDescending = Utils.IsSortDescending();
        }

        public async void UpdateUpperStatus(int retries = 5)
        {
            while (true)
            {
                MainViewModel page = Utils.GetMainPageInstance();

                if (page != null)

                    if (!_seasonalState)
                        if (!string.IsNullOrWhiteSpace(ListSource))
                            page.CurrentStatus = $"{ListSource} - {Utils.StatusToString(GetDesiredStatus())}";
                        else
                            page.CurrentStatus = "Anime list";
                    else
                        page.CurrentStatus = $"Airing - {Utils.StatusToString(GetDesiredStatus())}";

                else if (retries >= 0)
                {
                    await Task.Delay(1000);
                    retries = retries - 1;
                    continue;
                }
                break;
            }
        }
        #endregion

        public void RefreshList(bool searchSource = false)
        {
            var query = ViewModelLocator.Main.CurrentSearchQuery;
            var queryCondition = !string.IsNullOrWhiteSpace(query) && query.Length > 1;
            if (!_wasPreviousQuery && searchSource && !queryCondition)
                // refresh was requested from search but there's nothing to update
                return;

            _wasPreviousQuery = queryCondition;
            _currentPage = 1;

            _animeItemsSet.Clear();
            var status = queryCondition ? 7 : GetDesiredStatus();

            IEnumerable<AnimeItemAbstraction> items = _seasonalState ? _allLoadedSeasonalAnimeItems : _allLoadedAnimeItems;

            items = items.Where(item => queryCondition || status == 7 || item.MyStatus == status);

            if (queryCondition)
                items = items.Where(item => item.Title.ToLower().Contains(query.ToLower()));

            switch (SortOption)
            {
                case SortOptions.SortTitle:
                    items = items.OrderBy(item => item.Title);
                    break;
                case SortOptions.SortScore:
                    if (!_seasonalState)
                        items = items.OrderBy(item => item.MyScore);
                    else
                        items = items.OrderBy(item => item.GlobalScore);
                    break;
                case SortOptions.SortWatched:
                    if (_seasonalState)
                        items = items.OrderBy(item => item.Index);
                    else
                        items = items.OrderBy(item => item.MyEpisodes);
                    break;
                case SortOptions.SortNothing:
                    break;
                case SortOptions.SortAirDay:
                    var today = (int)DateTime.Now.DayOfWeek;
                    today++;
                    IEnumerable<AnimeItemAbstraction> nonAiringItems =
                        items.Where(abstraction => abstraction.AirDay == -1);
                    IEnumerable<AnimeItemAbstraction> airingItems = items.Where(abstraction => abstraction.AirDay != -1);
                    IEnumerable<AnimeItemAbstraction> airingAfterToday =
                        airingItems.Where(abstraction => abstraction.AirDay >= today);
                    IEnumerable<AnimeItemAbstraction> airingBeforeToday =
                        airingItems.Where(abstraction => abstraction.AirDay < today);
                    if (SortDescending)
                        items = airingAfterToday.OrderByDescending(abstraction => today - abstraction.AirDay)
                            .Concat(
                                airingBeforeToday.OrderByDescending(abstraction => today - abstraction.AirDay)
                                    .Concat(nonAiringItems));
                    else
                        items = airingBeforeToday.OrderBy(abstraction => today - abstraction.AirDay)
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
            foreach (AnimeItemAbstraction item in items)
                _animeItemsSet.Add(item);
            //If we have items then we should hide EmptyNotice       
            EmptyNoticeVisibility = _animeItemsSet.Count == 0;

            //How many pages do we have?
            _allPages = (int)Math.Ceiling((double)_animeItemsSet.Count / _itemsPerPage);
            if (_allPages <= 1)
                AnimesTopPageControlsVisibility = false;
            else
            {
                AnimesTopPageControlsVisibility = true;
                if (CurrentPage <= 1)
                {
                    PrevPageButtonEnableState = false;
                    CurrentPage = 1;
                }
                else
                {
                    PrevPageButtonEnableState = true;
                }

                NextPageButtonEnableState = CurrentPage != _allPages;
            }


            ApplyCurrentPage();
            AlternateRowColors();
            UpdateUpperStatus();
            RaisePropertyChanged(() => CurrentUpdateStatus);
        }

        #region Pagination

        private void PrevPage()
        {
            CurrentPage--;
            PrevPageButtonEnableState = CurrentPage != 1;
            NextPageButtonEnableState = true;
            ApplyCurrentPage();
        }

        private void NextPage()
        {
            CurrentPage++;
            NextPageButtonEnableState = CurrentPage != _allPages;
            PrevPageButtonEnableState = true;
            ApplyCurrentPage();
        }

        private void ApplyCurrentPage()
        {
            _animeItems.Clear();
            foreach (
                AnimeItemAbstraction item in _animeItemsSet.Skip(_itemsPerPage * (CurrentPage - 1)).Take(_itemsPerPage))
                _animeItems.Add(item.AnimeItem);
            RaisePropertyChanged(() => CurrentPageStatus);
        }

        #endregion

        #region FetchAndPopulate

        private async Task FetchSeasonalData(bool force = false)
        {
                Utils.GetMainPageInstance().CurrentStatus = "Downloading data...\nThis may take a while...";
                List<SeasonalAnimeData> data = new List<SeasonalAnimeData>();
                await Task.Run(async () => data = await new AnimeSeasonalQuery().GetSeasonalAnime(force));
                if (data == null)
                {
                    RefreshList();
                    Loading = false;
                    return;
                }
                _allLoadedSeasonalAnimeItems.Clear();
                foreach (SeasonalAnimeData animeData in data)
                {
                    DataCache.RegisterVolatileData(animeData.Id, new VolatileDataCache
                    {
                        DayOfAiring = animeData.AirDay,
                        GlobalScore = animeData.Score
                    });
                    var abstraction = _allLoadedAnimeItems.FirstOrDefault(item => item.Id == animeData.Id);
                    if (abstraction == null)
                        _allLoadedSeasonalAnimeItems.Add(new AnimeItemAbstraction(animeData));
                    else
                    {
                        abstraction.AirDay = animeData.AirDay;
                        abstraction.GlobalScore = animeData.Score;
                        abstraction.ViewModel.UpdateWithSeasonData(animeData);
                        _allLoadedSeasonalAnimeItems.Add(abstraction);
                    }
                }
                DataCache.SaveVolatileData();            

            UpdateUpperStatus();           
            RefreshList();
            Loading = false;
        }

        public async Task FetchData(bool force = false)
        {
            if (_prevListSource == ListSource)
            {
                foreach (var item in _allLoadedAnimeItems.Where(abstraction => abstraction.Loaded))
                {
                    item.ViewModel.SignalBackToList();
                }
                return;
            }
            _prevListSource = ListSource;

            Loading = true;
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

            _allLoadedAnimeItems = new List<AnimeItemAbstraction>();
            _animeItems.Clear();           

            if (_allLoadedAnimeItems.Count == 0)
            {
                Tuple<string, DateTime> possibleCachedData = force
                    ? null
                    : await DataCache.RetrieveDataForUser(ListSource);
                string data = "";
                if (possibleCachedData != null)
                {
                    data = possibleCachedData.Item1;
                    _lastUpdate = possibleCachedData.Item2;
                }
                else
                {
                    var args = new AnimeListParameters
                    {
                        status = "all",
                        type = "anime",
                        user = ListSource
                    };
                    await Task.Run(async() => data = await new AnimeListQuery(args).GetRequestResponse());
                    if (string.IsNullOrEmpty(data) || data.Contains("<error>Invalid username</error>"))
                    {
                        RefreshList();
                        Loading = false;
                        return;
                    }
                    DataCache.SaveDataForUser(ListSource, data);
                    _lastUpdate = DateTime.Now;
                }
                XDocument parsedData = XDocument.Parse(data);
                List<XElement> anime = parsedData.Root.Elements("anime").ToList();
                var auth = Creditentials.Authenticated &&
                           string.Equals(ListSource, Creditentials.UserName,
                               StringComparison.CurrentCultureIgnoreCase);
                foreach (XElement item in anime)
                {
                    _allLoadedAnimeItems.Add(new AnimeItemAbstraction(
                        auth,
                        item.Element("series_title").Value,
                        item.Element("series_image").Value,
                        Convert.ToInt32(item.Element("series_animedb_id").Value),
                        Convert.ToInt32(item.Element("my_status").Value),
                        Convert.ToInt32(item.Element("my_watched_episodes").Value),
                        Convert.ToInt32(item.Element("series_episodes").Value),
                        Convert.ToInt32(item.Element("my_score").Value)));
                }

                _allLoadedAnimeItems = _allLoadedAnimeItems.Distinct().ToList();
            }


            RefreshList();
            //UpdateStatusCounterBadges();
            Loading = false;
        }

        public bool TryRetrieveAuthenticatedAnimeItem(int id, ref IAnimeData reference)
        {
            if (!Creditentials.Authenticated)
                return false;
            try
            {
                reference =
                    _allLoadedAnimeItems.First(abstraction => abstraction.Id == id).ViewModel;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region StatusRelatedStuff

        private int GetDesiredStatus()
        {
            var value = StatusSelectorSelectedIndex;
            value++;
            return value == 0 ? 1 : value == 5 || value == 6 ? value + 1 : value;
        }

        private void SetDesiredStatus(int? value)
        {
            value = value ?? Utils.GetDefaultAnimeFilter();

            value = value == 6 || value == 7 ? value - 1 : value;
            value--;

            StatusSelectorSelectedIndex = (int)value;
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
        #endregion

        private void SwitchSortingToSeasonal()
        {
            Sort3Label = "Index";
        }

        private void SwitchFiltersToSeasonal()
        {
            StatusAllLabel = "Airing";
        }

        private async void ReloadList()
        {
            if (_seasonalState)
                await FetchSeasonalData(true);
            else
                await FetchData(true);
        }

        public void AddAnimeEntry(AnimeItemAbstraction parentAbstraction)
        {
            if(string.Equals(Creditentials.UserName,ListSource,StringComparison.CurrentCultureIgnoreCase))
                _allLoadedAnimeItems.Add(parentAbstraction);
        }

        public void RemoveAnimeEntry(AnimeItemAbstraction parentAbstraction)
        {
            if (string.Equals(Creditentials.UserName, ListSource, StringComparison.CurrentCultureIgnoreCase))
                _allLoadedAnimeItems.Remove(parentAbstraction);
        }

        #region LogInOut

        public void LogOut()
        {
            foreach (AnimeItemAbstraction userCache in _allLoadedAnimeItems)
            {
                userCache.SetAuthStatus(false, true);
            }
        }

        public void LogIn()
        {
            ListSource = Creditentials.UserName;
            _prevListSource = "";
            _allLoadedSeasonalAnimeItems = new List<AnimeItemAbstraction>();
        }
        #endregion
    }
}