using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{

    public class AnimeListPageNavigationArgs
    {
        public AnimeListPage.SortOptions SortOption;
        public readonly int Status;
        public readonly bool Descending;
        public readonly bool LoadSeasonal;
        public readonly int CurrPage;
        public readonly bool NavArgs;
        public readonly string ListSource;
        public AnimeListPageNavigationArgs(AnimeListPage.SortOptions sort, int status, bool desc, int page,
            bool seasonal,string source)
        {
            SortOption = sort;
            Status = status;
            Descending = desc;
            CurrPage = page;
            LoadSeasonal = seasonal;
            ListSource = source;
            NavArgs = true;
        }

        public AnimeListPageNavigationArgs()
        {
            LoadSeasonal = true;
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnimeListPage : Page
    {
        public enum SortOptions
        {
            SortNothing,
            SortTitle,
            SortScore,
            SortWatched,
            SortAirDay
        }

        private SortOptions _sortOption = SortOptions.SortNothing;

        public SortOptions SortOption => _sortOption;
        public int CurrentStatus => GetDesiredStatus();
        public bool SortDescending => _sortDescending;
        public int CurrentPage => _currentPage;
        public string ListSource => TxtListSource.Text;
        private bool _sortDescending;
        private bool _loaded;
        private string _currentSoure;
        private ObservableCollection<AnimeItem> _animeItems = new ObservableCollection<AnimeItem>(); // + Page
        private ObservableCollection<AnimeItemAbstraction> _animeItemsSet = new ObservableCollection<AnimeItemAbstraction>(); //All for current list
        private List<AnimeItemAbstraction> _allLoadedAnimeItems = new List<AnimeItemAbstraction>();
        private DateTime _lastUpdate;
        private System.Threading.Timer _timer;
        private bool _seasonalState = false;
        private int _currentPage = 1;
        private int _allPages;
        private readonly int _itemsPerPage = Utils.GetItemsPerPage();
        private bool _wasPreviousQuery = false;

        #region Init
        public AnimeListPage()
        {
            this.InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
            var scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(Animes, 0), 0) as ScrollViewer;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            UpdateUpperStatus();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            AnimeListPageNavigationArgs args = e.Parameter as AnimeListPageNavigationArgs;
            if (args != null)
            {
                if (args.LoadSeasonal)
                {
                    _seasonalState = true;
                    SpinnerLoading.Visibility = Visibility.Visible;
                    EmptyNotice.Visibility = Visibility.Collapsed;
                    AppbarBtnPinTile.Visibility = Visibility.Collapsed;
                    AppBtnListSource.Visibility = Visibility.Collapsed;

                    if (args.NavArgs)
                    {
                        TxtListSource.Text = args.ListSource;
                        _currentSoure = args.ListSource;
                        BtnOrderDescending.IsChecked = _sortDescending = args.Descending;
                        SetSortOrder(args.SortOption); //index
                        SetDesiredStatus(args.Status);
                        _currentPage = args.CurrPage;
                    }
                    else
                    {
                        BtnOrderDescending.IsChecked = _sortDescending = false;
                        SetSortOrder(SortOptions.SortWatched); //index
                        SetDesiredStatus((int)AnimeStatus.AllOrAiring);
                    }

                    SwitchFiltersToSeasonal();
                    SwitchSortingToSeasonal();

                    await Task.Run(async () =>
                    {
                        await
                            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                            {
                                FetchSeasonalData();
                            });
                    });
                    return;
                } // else we just have nav data

                TxtListSource.Text = args.ListSource;
                _currentSoure = args.ListSource;
                SetSortOrder(args.SortOption);
                SetDesiredStatus(args.Status);
                BtnOrderDescending.IsChecked = args.Descending;
                _sortDescending = args.Descending;
                _currentPage = args.CurrPage;
            }
            else // default
                SetDefaults();

            if (string.IsNullOrWhiteSpace(ListSource))
            {
                if (!string.IsNullOrWhiteSpace(Creditentials.UserName))
                    TxtListSource.Text = Creditentials.UserName;
            }
            _currentSoure = TxtListSource.Text;
            if (string.IsNullOrWhiteSpace(ListSource))
            {
                EmptyNotice.Visibility = Visibility.Visible;
                EmptyNotice.Text += "\nList source is not set.\nLog in or set it manually.";
                BtnSetSource.Visibility = Visibility.Visible;
                UpdateUpperStatus();
            }
            else
            {
                FetchData();
            }

            if (_timer == null)
                _timer = new System.Threading.Timer(state => { UpdateStatus(); }, null, (int)TimeSpan.FromMinutes(1).TotalMilliseconds, (int)TimeSpan.FromMinutes(1).TotalMilliseconds);

            UpdateStatus();
            base.OnNavigatedTo(e);
        }
        #endregion

        #region UIHelpers
        private void SwitchSortingToSeasonal()
        {
            sort3.Text = "Index";
        }

        private void SwitchFiltersToSeasonal()
        {
            (StatusSelector.Items[5] as ListViewItem).Content = "Airing"; //We are quite confiddent here
        }

        private async void UpdateStatus()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateNotice.Text = $"Updated {GetLastUpdatedStatus()}";
            });
        }

        private void SetDefaults()
        {
            SetSortOrder(null);
            SetDesiredStatus(null);
            BtnOrderDescending.IsChecked = Utils.IsSortDescending();
            _sortDescending = Utils.IsSortDescending();
        }

        internal void ScrollTo(AnimeItem animeItem)
        {
            try
            {
                var scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(Animes, 0), 0) as ScrollViewer;
                double offset = _animeItems.TakeWhile(t => animeItem != t).Sum(t => t.ActualHeight);
                scrollViewer.ScrollToVerticalOffset(offset);
            }
            catch (Exception)
            {
                // ehh
            }
        }

        private async void UpdateUpperStatus(int retries = 5)
        {
            while (true)
            {
                var page = Utils.GetMainPageInstance();

                if (page != null)

                    if (!_seasonalState)
                        if (!string.IsNullOrWhiteSpace(TxtListSource.Text))
                            page.SetStatus($"{TxtListSource.Text} - {Utils.StatusToString(GetDesiredStatus())}");
                        else
                            page.SetStatus("Anime list");
                    else
                        page.SetStatus($"Airing - {Utils.StatusToString(GetDesiredStatus())}");

                else if (retries >= 0)
                {
                    await Task.Delay(1000);
                    retries = retries - 1;
                    continue;
                }
                break;
            }
        }

        private void AlternateRowColors()
        {
            for (int i = 0; i < _animeItems.Count; i++)
            {
                if ((i + 1) % 2 == 0)
                    _animeItems[i].Setbackground(new SolidColorBrush(Color.FromArgb(170, 230, 230, 230)));
                else
                    _animeItems[i].Setbackground(new SolidColorBrush(Colors.Transparent));
            }
        }

        private string GetLastUpdatedStatus()
        {
            string output;
            TimeSpan lastUpdateDiff = DateTime.Now.Subtract(_lastUpdate);
            if (lastUpdateDiff.Days > 0)
                output = lastUpdateDiff.Days + "day" + (lastUpdateDiff.Days > 1 ? "s" : "") + " ago.";
            else if (lastUpdateDiff.Hours > 0)
            {
                output = lastUpdateDiff.Hours + "hour" + (lastUpdateDiff.Hours > 1 ? "s" : "") + " ago.";
            }
            else if (lastUpdateDiff.Minutes > 0)
            {
                output = $"{lastUpdateDiff.Minutes} minute" + (lastUpdateDiff.Minutes > 1 ? "s" : "") + " ago.";
            }
            else
            {
                output = "just now.";
            }
            if (lastUpdateDiff.Days < 20000) //Seems like reasonable workaround
                UpdateNotice.Visibility = Visibility.Visible;
            return output;
        }

        private void UpdateStatusCounterBadges()
        {
            Dictionary<int, int> counters = new Dictionary<int, int>();
            for (AnimeStatus i = AnimeStatus.Watching; i <= AnimeStatus.PlanToWatch; i++)
                counters[(int)i] = 0;
            foreach (AnimeItemAbstraction animeItemAbstraction in _allLoadedAnimeItems)
            {
                if(animeItemAbstraction.MyStatus <= 6)
                    counters[animeItemAbstraction.MyStatus]++;
            }
            AnimeStatus j = AnimeStatus.Watching;
            foreach (object item in StatusSelector.Items)
            {
                (item as ListViewItem).Content = counters[(int)j] + " - " + Utils.StatusToString((int)j);
                j++;
                if ((int)j == 5)
                    j++;
                if(j == AnimeStatus.AllOrAiring)
                    return;
            }
        }
        #endregion

        #region FetchAndPopulate
        private async void FetchSeasonalData(bool force = false)
        {
            var possibleLoadedData = force ? new List<AnimeItemAbstraction>() : Utils.GetMainPageInstance().RetrieveSeasonData();
            if (possibleLoadedData.Count == 0)
            {
                Utils.GetMainPageInstance().SetStatus("Downloading data...\nThis may take a while...");
                var data = await new AnimeSeasonalQuery().GetSeasonalAnime(force);
                _allLoadedAnimeItems.Clear();
                var loadedStuff = Utils.GetMainPageInstance().RetrieveLoadedAnime();
                Dictionary<int, AnimeItemAbstraction> loadedItems = loadedStuff?.LoadedAnime.ToDictionary(item => item.Id);
                foreach (SeasonalAnimeData animeData in data)
                {
                    _allLoadedAnimeItems.Add(new AnimeItemAbstraction(animeData, loadedItems));
                    DataCache.RegisterVolatileData(animeData.Id, new VolatileDataCache
                    {
                        DayOfAiring = animeData.AirDay,
                        GlobalScore = animeData.Score
                    });
                }
                DataCache.SaveVolatileData();
                Utils.GetMainPageInstance().SaveSeasonData(_allLoadedAnimeItems);
            }
            else
            {
                _allLoadedAnimeItems = possibleLoadedData;
            }

            UpdateUpperStatus();
            Animes.ItemsSource = _animeItems;
            RefreshList();
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void FetchData(bool force = false)
        {
            BtnSetSource.Visibility = Visibility.Collapsed;
            SpinnerLoading.Visibility = Visibility.Visible;
            EmptyNotice.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(TxtListSource.Text))
            {
                EmptyNotice.Visibility = Visibility.Visible;
                EmptyNotice.Text += "\nList source is not set.\nLog in or set it manually.";
                BtnSetSource.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyNotice.Text = "We have come up empty...";
            }

            _allLoadedAnimeItems = new List<AnimeItemAbstraction>();
            _animeItems.Clear();

            if (!force)
                Utils.GetMainPageInstance().RetrieveAnimeEntries(TxtListSource.Text, out _allLoadedAnimeItems, out _lastUpdate);

            if (_allLoadedAnimeItems.Count == 0)
            {
                var possibleCachedData = force ? null : await DataCache.RetrieveDataForUser(TxtListSource.Text);
                string data;
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
                        user = TxtListSource.Text
                    };
                    data = await new AnimeListQuery(args).GetRequestResponse();
                    if (data.Contains("<error>Invalid username</error>"))
                    {
                        RefreshList();
                        SpinnerLoading.Visibility = Visibility.Collapsed;
                        return;
                    }
                    DataCache.SaveDataForUser(TxtListSource.Text, data);
                    _lastUpdate = DateTime.Now;
                }
                XDocument parsedData = XDocument.Parse(data);
                var anime = parsedData.Root.Elements("anime").ToList();
                bool auth = Creditentials.Authenticated &&
                            string.Equals(TxtListSource.Text, Creditentials.UserName, StringComparison.CurrentCultureIgnoreCase);
                foreach (var item in anime)
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
                //TODO : For some unknown reason items may duplicate/triplicate etc. May no longer be the case
                _allLoadedAnimeItems = _allLoadedAnimeItems.Distinct().ToList();

                Utils.GetMainPageInstance().SaveAnimeEntries(TxtListSource.Text, _allLoadedAnimeItems, _lastUpdate);

            }


            RefreshList();
            Animes.ItemsSource = _animeItems;
            UpdateStatusCounterBadges();
            SpinnerLoading.Visibility = Visibility.Collapsed;

        }
        #endregion

        #region StatusRelatedStuff
        private int GetDesiredStatus()
        {
            int value = StatusSelector.SelectedIndex;
            value++;
            return value == 0 ? 1 : (value == 5 || value == 6) ? value + 1 : value;
        }

        private void SetDesiredStatus(int? value)
        {
            value = value ?? Utils.GetDefaultAnimeFilter();

            value = (value == 6 || value == 7) ? value - 1 : value;
            value--;

            StatusSelector.SelectedIndex = (int)value;
        }
        #endregion

        public void RefreshList(bool searchSource = false)
        {
            string query = Utils.GetMainPageInstance()?.GetSearchQuery();
            bool queryCondition = !string.IsNullOrWhiteSpace(query) && query.Length > 1;
            if (!_wasPreviousQuery && searchSource && !queryCondition) // refresh was requested from search but there's nothing to update
                return;

            _wasPreviousQuery = queryCondition;

         
            _animeItemsSet.Clear();
            int status = queryCondition ? 7 : GetDesiredStatus();                 

            var items = _allLoadedAnimeItems.Where(item => queryCondition || status == 7 || item.MyStatus == status);
            if (queryCondition)
                items = items.Where(item => item.Title.ToLower().Contains(query.ToLower()));
            switch (_sortOption)
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
                    if(_seasonalState)
                        items = items.OrderBy(item => item.Index);
                    else
                        items = items.OrderBy(item => item.MyEpisodes);
                    break;
                case SortOptions.SortNothing:
                    break;
                case SortOptions.SortAirDay:
                    int today = (int) DateTime.Now.DayOfWeek;
                    today++;
                    var nonAiringItems = items.Where(abstraction => abstraction.AirDay == -1);
                    var airingItems = items.Where(abstraction => abstraction.AirDay != -1);
                    var airingAfterToday = airingItems.Where(abstraction => abstraction.AirDay >= today);
                    var airingBeforeToday = airingItems.Where(abstraction => abstraction.AirDay < today);
                    if (_sortDescending)
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
                    throw new ArgumentOutOfRangeException(nameof(_sortOption), _sortOption, null);
            }
            //If we are descending then reverse order
            if (_sortDescending && _sortOption != SortOptions.SortAirDay)
                items = items.Reverse();
            //Add all abstractions to current set (spread across pages)
            foreach (var item in items)
                _animeItemsSet.Add(item);
            //If we have items then we should hide EmptyNotice       
            EmptyNotice.Visibility = _animeItemsSet.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            //How many pages do we have?
            _allPages = (int)Math.Ceiling((double)_animeItemsSet.Count/_itemsPerPage);
            if (_allPages <= 1)
                AnimesTopPageControls.Visibility = Visibility.Collapsed;
            else
            {
                AnimesTopPageControls.Visibility = Visibility.Visible;
                if (_currentPage <= 1)
                {
                    BtnPrevPage.IsEnabled = false;
                    _currentPage = 1;
                }
                else
                {
                    BtnPrevPage.IsEnabled = true;
                }

                BtnNextPage.IsEnabled = _currentPage != _allPages;
            }

                
            ApplyCurrentPage();
            AlternateRowColors();
            UpdateUpperStatus();
            UpdateNotice.Text = $"Updated {GetLastUpdatedStatus()}";
        }

        #region Pagination
        private void PrevPage(object sender, RoutedEventArgs e)
        {
            _currentPage--;
            BtnPrevPage.IsEnabled = _currentPage != 1;
            BtnNextPage.IsEnabled = true;
            ApplyCurrentPage();

        }

        private void NextPage(object sender, RoutedEventArgs e)
        {
            _currentPage++;
            BtnNextPage.IsEnabled = _currentPage != _allPages;
            BtnPrevPage.IsEnabled = true;
            ApplyCurrentPage();
        }

        private void ApplyCurrentPage()
        {
            _animeItems.Clear();
            foreach (var item in _animeItemsSet.Skip(_itemsPerPage*(_currentPage - 1)).Take(_itemsPerPage))
                _animeItems.Add(item.AnimeItem);
            UpdatePageStatus();

        }

        private void UpdatePageStatus()
        {
            TxtPageCount.Text = $"{_currentPage}/{_allPages}";
        }

        #endregion

        #region ActionHandlers
        private void ChangeListStatus(object sender, SelectionChangedEventArgs e)
        {
            if (_loaded)
            {
                _currentPage = 1;
                RefreshList();
            }
        }

        private async void PinTileMal(object sender, RoutedEventArgs e)
        {
            foreach (var item in Animes.SelectedItems)
            {
                var anime = item as AnimeItem;
                if (SecondaryTile.Exists(anime.Id.ToString()))
                {
                    var msg = new MessageDialog("Tile for this anime already exists.");
                    await msg.ShowAsync();
                    continue;
                }
                anime.PinTile($"http://www.myanimelist.net/anime/{anime.Id}");
            }
        }

        private void PinTileCustom(object sender, RoutedEventArgs e)
        {
            var item = Animes.SelectedItem as AnimeItem;
            item.OpenTileUrlInput();
        }

        private void RefreshList(object sender, RoutedEventArgs e)
        {
            if (_seasonalState)
                FetchSeasonalData(true);
            else
                FetchData(true);
        }     

        private void SelectSortMode(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            switch (btn.Text)
            {
                case "Title":
                    _sortOption = SortOptions.SortTitle;
                    break;
                case "Score":
                    _sortOption = SortOptions.SortScore;
                    break;
                case "Watched":
                    _sortOption = SortOptions.SortWatched;
                    break;
                case "Soonest airing":
                    _sortOption = SortOptions.SortAirDay;
                    break;
                default:
                    _sortOption = SortOptions.SortNothing;
                    break;
            }
            sort1.IsChecked = false;
            sort2.IsChecked = false;
            sort3.IsChecked = false;
            sort4.IsChecked = false;
            sort5.IsChecked = false;
            btn.IsChecked = true;
            RefreshList();

        }

        private void SetSortOrder(SortOptions? option)
        {
            switch (option ?? Utils.GetSortOrder())
            {
                case SortOptions.SortNothing:
                    _sortOption = SortOptions.SortNothing;
                    sort4.IsChecked = true;
                    break;
                case SortOptions.SortTitle:
                    _sortOption = SortOptions.SortTitle;
                    sort1.IsChecked = true;
                    break;
                case SortOptions.SortScore:
                    _sortOption = SortOptions.SortScore;
                    sort2.IsChecked = true;
                    break;
                case SortOptions.SortWatched:
                    _sortOption = SortOptions.SortWatched;
                    sort3.IsChecked = true;
                    break;
                case SortOptions.SortAirDay:
                    _sortOption = SortOptions.SortAirDay;
                    sort5.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeSortOrder(object sender, RoutedEventArgs e)
        {
            var chbox = sender as ToggleMenuFlyoutItem;
            _sortDescending = chbox.IsChecked;
            RefreshList();
        }

        private void ListSource_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e == null || e.Key == VirtualKey.Enter)
            {
                if(_currentSoure.ToLower() != Creditentials.UserName.ToLower())
                    Utils.GetMainPageInstance().PurgeUserCache(_currentSoure); //why would we want to keep those entries?
                _currentSoure = TxtListSource.Text;
                TxtListSource.IsEnabled = false; //reset input
                TxtListSource.IsEnabled = true;
                FlyoutListSource.Hide();
                BottomCommandBar.IsOpen = false;
                FetchData();
            }
        }

        private void ShowListSourceFlyout(object sender, RoutedEventArgs e)
        {
            FlyoutListSource.ShowAt(sender as FrameworkElement);
        }

        private void SetListSource(object sender, RoutedEventArgs e)
        {
            ListSource_OnKeyDown(null,null);
        }

        private void FlyoutListSource_OnOpened(object sender, object e)
        {
            TxtListSource.SelectAll();
        }

        private void Animes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppbarBtnPinTile.IsEnabled = true;
        }
        #endregion
    }
}
