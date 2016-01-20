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
        public readonly bool LoadSeasonal = false;
        public readonly int CurrPage;
        public AnimeListPageNavigationArgs(AnimeListPage.SortOptions sort,int status,bool desc,int page)
        {
            SortOption = sort;
            Status = status;
            Descending = desc;
            CurrPage = page;
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
        }

        private SortOptions _sortOption = SortOptions.SortNothing;

        public SortOptions SortOption => _sortOption;
        public int CurrentStatus => GetDesiredStatus();
        public bool SortDescending => _sortDescending;
        public int CurrentPage => _currentPage;
        private bool _sortDescending;
        private bool _loaded = false;
        private ObservableCollection<AnimeItem> _animeItems = new ObservableCollection<AnimeItem>(); // + Page
        private ObservableCollection<AnimeItem> _animeItemsSet = new ObservableCollection<AnimeItem>(); //All for current list
        private List<AnimeItemAbstraction> _allLoadedAnimeItems = new List<AnimeItemAbstraction>();
        private List<XElement> _allDownloadedAnimeItems = new List<XElement>();
        private DateTime _lastUpdate;
        private System.Threading.Timer _timer;
        private bool _seasonalState = false;
        private int _currentPage = 1;
        private int _allPages;
        private int _itemsPerPage = 10; //TODO : Setting for this

        private Dictionary<int,bool> _loadedDictionary = new Dictionary<int, bool>
        {
            {1,false},
            {2,false},
            {3,false},
            {4,false},
            {6,false}
        }; 

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
            EmptyNotice.Visibility = Visibility.Collapsed;
            UpdateUpperStatus();
        }

        private async void UpdateStatus()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateNotice.Text = $"Updated {GetLastUpdatedStatus()}";
            });
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            
            AnimeListPageNavigationArgs args = e.Parameter as AnimeListPageNavigationArgs;
            if (args != null)
            {
                if (args.LoadSeasonal)
                {
                    _seasonalState = true;
                    AppBtnSortSecondary.Visibility = Visibility.Collapsed;
                    AppBtnSortPrimary.Visibility = Visibility.Visible;
                    SpinnerLoading.Visibility = Visibility.Visible;
                    EmptyNotice.Visibility = Visibility.Collapsed;
                    AppbarBtnPinTile.Visibility = Visibility.Collapsed;
                    BtnOrderDescending.IsChecked = _sortDescending = false;
                    SwitchFiltersToSeasonal();
                    SwitchSortingToSeasonal();
                    SetSortOrder(SortOptions.SortWatched); //index
                    SetDesiredStatus((int)AnimeStatus.AllOrAiring);
                    _loadedDictionary = new Dictionary<int, bool>
                    {
                        {1, true},
                        {2, true},
                        {3, true},
                        {4, true},
                        {6, true}
                    };
                    
                    await Task.Run(async () =>
                    {
                        await
                            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                            {
                                FetchSeasonalData();
                            });
                    });
                    
                    return;
                }

                SetSortOrder(args?.SortOption);
                SetDesiredStatus(args?.Status);
                BtnOrderDescending.IsChecked = args.Descending;
                _sortDescending = args.Descending;
                _currentPage = args.CurrPage;                
            }
            else // default
                SetDefaults();

            if (!string.IsNullOrWhiteSpace(Creditentials.UserName))
            {
                ListSource.Text = Creditentials.UserName;
                FetchData();
            }
            else
            {
                EmptyNotice.Visibility = Visibility.Visible;
                EmptyNotice.Text += "\nList source is not set.\nLog in or set it manually.";
                Utils.GetMainPageInstance()?.SetStatus("Anime List");
            }

            if (_timer == null)
                _timer = new System.Threading.Timer(state => { UpdateStatus(); }, null, (int)TimeSpan.FromMinutes(1).TotalMilliseconds, (int)TimeSpan.FromMinutes(1).TotalMilliseconds);

            UpdateStatus();
            base.OnNavigatedTo(e);
        }

        private void SwitchSortingToSeasonal()
        {
            sort3.Text = "Index";
        }

        private void SwitchFiltersToSeasonal()
        {
            (StatusSelector.Items[5] as ListViewItem).Content = "Airing";
        }

        private void SetDefaults()
        {
            SetSortOrder(null);
            SetDesiredStatus(null);
            BtnOrderDescending.IsChecked = Utils.IsSortDescending();
            _sortDescending = Utils.IsSortDescending();
        }

        private async void FetchSeasonalData(bool force = false)
        {
            var possibleLoadedData = force ? new List<AnimeItemAbstraction>() : Utils.GetMainPageInstance().RetrieveSeasonData();
            if (possibleLoadedData.Count == 0)
            {
                var data = await new AnimeSeasonalQuery().GetSeasonalAnime();
                _allLoadedAnimeItems.Clear();
                Utils.GetMainPageInstance().SetStatus("Data downloaded , processing...");
                var loadedStuff = Utils.GetMainPageInstance().RetrieveLoadedAnime();
                Dictionary<int, XElement> downloadedItems = loadedStuff.DownloadedAnime.ToDictionary(item => int.Parse(item.Element("series_animedb_id").Value));
                Dictionary<int,AnimeItemAbstraction> loadedItems = loadedStuff.LoadedAnime.ToDictionary(item => item.Id);
                HashSet<int> loadedIds = new HashSet<int>();
                HashSet<int> downloadedIds = new HashSet<int>();
                loadedIds.UnionWith(loadedItems.Keys);
                downloadedIds.UnionWith(downloadedItems.Keys);
                foreach (SeasonalAnimeData animeData in data)
                {
                    _allLoadedAnimeItems.Add(new AnimeItemAbstraction(animeData, downloadedItems, loadedItems));
                }
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
            SpinnerLoading.Visibility = Visibility.Visible;
            EmptyNotice.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(ListSource.Text))
            {
                EmptyNotice.Visibility = Visibility.Visible;
                EmptyNotice.Text += "\nList source is not set.\nLog in or set it manually.";
            }
            else
            {
                EmptyNotice.Text = "We have come up empty...";
            }

            _allLoadedAnimeItems.Clear();
            _allDownloadedAnimeItems.Clear();
            _animeItems.Clear();

            if(!force)
                Utils.GetMainPageInstance()?.RetrieveAnimeEntries(ListSource.Text,out _allLoadedAnimeItems, out _allDownloadedAnimeItems , out _lastUpdate,out _loadedDictionary);

            if (_allLoadedAnimeItems.Count == 0 && _allDownloadedAnimeItems.Count == 0)
            {
                var possibleCachedData = force ? null : await DataCache.RetrieveDataForUser(ListSource.Text);
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
                        user = ListSource.Text
                    };
                    data = await new AnimeListQuery(args).GetRequestResponse();
                    DataCache.SaveDataForUser(ListSource.Text, data);
                    _lastUpdate = DateTime.Now;
                }
                XDocument parsedData = XDocument.Parse(data);
                var anime = parsedData.Root.Elements("anime").ToList();
                int status = GetDesiredStatus();

                if (status == 7)
                {
                    for (int i = 0; i < 4; i++)
                        _loadedDictionary[i] = true;
                    _loadedDictionary[6] = true;
                }
                else
                    _loadedDictionary[status] = true;

                foreach (var item in anime)
                {
                    if (status == 7 || Convert.ToInt32(item.Element("my_status").Value) == status) //if displaying all or element has desired MyStatus
                    {
                        _allLoadedAnimeItems.Add(new AnimeItemAbstraction(
                            (Creditentials.Authenticated && ListSource.Text == Creditentials.UserName),
                            item.Element("series_title").Value,
                            item.Element("series_image").Value,
                            Convert.ToInt32(item.Element("series_animedb_id").Value),
                            Convert.ToInt32(item.Element("my_status").Value),
                            Convert.ToInt32(item.Element("my_watched_episodes").Value),
                            Convert.ToInt32(item.Element("series_episodes").Value),
                            Convert.ToInt32(item.Element("my_score").Value)));
                    }
                    else
                        _allDownloadedAnimeItems.Add(item);
                }
                //TODO : For some unknown reason items may duplicate/triplicate etc.
                _allDownloadedAnimeItems = _allDownloadedAnimeItems.Distinct().ToList();
                _allLoadedAnimeItems = _allLoadedAnimeItems.Distinct().ToList();
                Utils.GetMainPageInstance()?.SaveAnimeEntries(ListSource.Text, _allLoadedAnimeItems ,_allDownloadedAnimeItems , _lastUpdate , _loadedDictionary);

            }


            RefreshList();
            Animes.ItemsSource = _animeItems;
            SpinnerLoading.Visibility = Visibility.Collapsed;

        }

        private int GetDesiredStatus()
        {
            int value = StatusSelector.SelectedIndex;
            value++;
            return (value == 5 || value == 6) ? value + 1 : value;
        }

        private void SetDesiredStatus(int? value)
        {
            if (value != null)
            {
                value = (value == 6 || value == 7) ? value - 1 : value;
                value--;
            }

            StatusSelector.SelectedIndex = value ?? 0; //TODO : Add setting for this
        }

        public void RefreshList(bool searchSource = false)
        {
            string query = Utils.GetMainPageInstance()?.GetSearchQuery();
            bool queryCondition = !string.IsNullOrWhiteSpace(query) && query.Length > 1;
            if (searchSource && !queryCondition) // refresh was requested from search but there's nothing to update
                return;


            EmptyNotice.Visibility = Visibility.Collapsed;            
            _animeItemsSet.Clear();
            int status = GetDesiredStatus();

            if (queryCondition)
                status = 7; //If we are gonna search we will have to load all items first.

            //Check if all items of desired MyStatus are loaded
            if (status == 7 || !_loadedDictionary[status])
            {
                //Update dictionary MyStatus
                if (status == 7)
                {
                    for (int i = 0; i < 4; i++)
                        _loadedDictionary[i] = true;
                    _loadedDictionary[6] = true;
                }
                else
                    _loadedDictionary[status] = true;
                //Load rest of items
                List<XElement> elementsToRemove = new List<XElement>();
                foreach (var item in _allDownloadedAnimeItems.Where(item => status == 7 || Convert.ToInt32(item.Element("my_status").Value) == status))
                {
                    _allLoadedAnimeItems.Add(new AnimeItemAbstraction(
                        (Creditentials.Authenticated && ListSource.Text == Creditentials.UserName),
                        item.Element("series_title").Value,
                        item.Element("series_image").Value,
                        Convert.ToInt32(item.Element("series_animedb_id").Value),
                        Convert.ToInt32(item.Element("my_status").Value),
                        Convert.ToInt32(item.Element("my_watched_episodes").Value),
                        Convert.ToInt32(item.Element("series_episodes").Value),
                        Convert.ToInt32(item.Element("my_score").Value)));
                    elementsToRemove.Add(item);

                }
                foreach (var element in elementsToRemove)
                {
                    _allDownloadedAnimeItems.Remove(element);
                }
                //Submit updated list to higher-ups
                Utils.GetMainPageInstance()?.SaveAnimeEntries(ListSource.Text, _allLoadedAnimeItems, _allDownloadedAnimeItems, _lastUpdate, _loadedDictionary);
            }

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
                default:
                    throw new ArgumentOutOfRangeException(nameof(_sortOption), _sortOption, null);
            }
            if (_sortDescending)
                items = items.Reverse();
            foreach (var item in items)
                _animeItemsSet.Add(item.AnimeItem);
            if(_animeItemsSet.Count == 0)
                EmptyNotice.Visibility = Visibility.Visible;
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
            {
                item.ItemLoaded();
                _animeItems.Add(item);
            }
            UpdatePageStatus();

        }

        private void UpdatePageStatus()
        {
            TxtPageCount.Text = $"{_currentPage}/{_allPages}";
        }

        #endregion

        private async void UpdateUpperStatus(int retries = 5)
        {
            var page = Utils.GetMainPageInstance();
            if (page != null)
                if(!_seasonalState)
                    page.SetStatus($"{ListSource.Text} - {Utils.StatusToString(GetDesiredStatus())}");
                else
                    page.SetStatus($"Airing - {Utils.StatusToString(GetDesiredStatus())}");
            else if (retries >= 0)
            {
                await Task.Delay(1000);
                UpdateUpperStatus(retries-1);
            }
        }

        private string GetLastUpdatedStatus()
        {
            string output;
            TimeSpan lastUpdateDiff = DateTime.Now.ToUniversalTime().Subtract(_lastUpdate);
            if (lastUpdateDiff.Days > 0)
                output = lastUpdateDiff.Days + "day" + (lastUpdateDiff.Days > 1 ? "s" : "") + " ago.";
            else if(lastUpdateDiff.Hours > 0)
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
            if(lastUpdateDiff.Days < 20000) //Seems like reasonable workaround
                UpdateNotice.Visibility = Visibility.Visible;
            return output;
        }

        private void ChangeListStatus(object sender, SelectionChangedEventArgs e)
        {
            if(_loaded)
                RefreshList();
        }

        private void AlternateRowColors()
        {
            for (int i = 0; i < _animeItems.Count; i++)
            {
                if ((i + 1)%2 == 0)
                    _animeItems[i].Setbackground(new SolidColorBrush(Color.FromArgb(170, 230, 230, 230)));
                else
                    _animeItems[i].Setbackground(new SolidColorBrush(Colors.Transparent));
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
            if(_seasonalState)
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
                default:
                    _sortOption = SortOptions.SortNothing;
                    break;
            }
            sort1.IsChecked = false;
            sort2.IsChecked = false;
            sort3.IsChecked = false;
            sort4.IsChecked = false;
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
            if (e.Key == VirtualKey.Enter)
            {
                var txt = sender as TextBox;
                txt.IsEnabled = false; //reset input
                txt.IsEnabled = true;
                FlyoutListSource.Hide();
                BottomCommandBar.IsOpen = false;
                FetchData();
            }
        }

    }
}
