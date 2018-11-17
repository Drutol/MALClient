using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Main
{
    public partial class AnimeListViewModel
    {
        #region PropertyPairs

        private string _listSource;

        public string ListSource
        {
            get { return _listSource; }
            set
            {
                if (_listSource == value)
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

        private bool _appbarBtnPinTileVisibility;

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

        public bool HumApiSpecificControlsVisibility => Settings.SelectedApiType == ApiType.Mal ? false : true;

        public bool MalApiSpecificControlsVisibility => Settings.SelectedApiType == ApiType.Hummingbird ? false : true;

        private bool _appBtnGoBackToMyListVisibility = false;

        public bool AppBtnGoBackToMyListVisibility
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
                    var random = _rangomGenerator ?? (_rangomGenerator = new Random((int)DateTime.Now.Ticks));
                    var pool = _animeItemsSet.Select(abstraction => abstraction.ViewModel).Union(AnimeItems).ToList();
                    if (pool.Count == 0)
                        return;
                    if (_randomedIds.Count == pool.Count)
                        _randomedIds = new List<int>();
                    foreach (var randomedId in _randomedIds)
                    {
                        var item = pool.FirstOrDefault(model => model.Id == randomedId);
                        if (item != null)
                            pool.Remove(item);
                    }
                    var winner = pool[random.Next(0, pool.Count)];
                    if (Settings.EnsureRandomizerAlwaysSelectsWinner && !AnimeItems.Contains(winner))
                    {
                        var indexesToLoad = _animeItemsSet.IndexOf(winner.ParentAbstraction) + 10;
                        AnimeItems.AddRange(_animeItemsSet.Take(indexesToLoad).Select(abstraction => abstraction.ViewModel));
                        _animeItemsSet = _animeItemsSet.Skip(indexesToLoad).ToList();
                    }

                    winner.NavigateDetails();
                    _randomedIds.Add(winner.Id);
                    if(!ViewModelLocator.Mobile)
                        ScrollIntoViewRequested?.Invoke(winner, true);
                }));
            }
        }

        private bool _upperCommandBarVisibility = true;

        public bool UpperCommandBarVisibility
        {
            get { return _upperCommandBarVisibility; }
            set
            {
                _upperCommandBarVisibility = value;
                RaisePropertyChanged(() => UpperCommandBarVisibility);
            }
        }

        private bool _appBtnSortingVisibility = false;

        public bool AppBtnSortingVisibility
        {
            get { return _appBtnSortingVisibility; }
            set
            {
                _appBtnSortingVisibility = value;
                RaisePropertyChanged(() => AppBtnSortingVisibility);
            }
        }

        private int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                CanLoadMore = value <= 4 && WorkMode.GetAttribute<EnumUtilities.AnimeListWorkModeEnumMember>().AllowLoadingMore; ;
            }
        }

        public bool CanLoadMore
        {
            get { return _canLoadMore && !_canLoadMoreFilterLock && !WasPreviousQuery; }
            set
            {
                _canLoadMore = value;
                RaisePropertyChanged(() => CanLoadMore);
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
                _canLoadMoreFilterLock = GetDesiredStatus() != AnimeStatus.AllOrAiring;//we cannot laod more when filters are active
                RaisePropertyChanged(() => CanLoadMore);
                SetDisplayMode((AnimeStatus)GetDesiredStatus());
                if (!Initializing)
                {
                    if (Settings.HideFilterSelectionFlyout)
                        HideFiltersFlyout?.Invoke();
                  
                    RefreshList(false, true);
                }
                
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
                    HideSortingFlyout?.Invoke();
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
                    SetSortOrder((SortOptions)int.Parse(s));
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

        private AnimeListWorkModes _workMode;

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
            private set
            {
                if (_scrollHandlerAdded && CanAddScrollHandler)
                {
                    //we don't want to be subscribed to wrong srollviewer

                    RemoveScrollHandlerRequest?.Invoke();
                    _scrollHandlerAdded = false;
                }
                RemoveScrollingConatinerReferenceRequest?.Invoke();
                _displayMode = value;
                RaisePropertyChanged(() => ListItemGridWidth);
                RaisePropertyChanged(() => DisplayMode);
                RaisePropertyChanged(() => CurrentlySelectedDisplayMode);
            }
        }

        public Tuple<AnimeListDisplayModes, string> CurrentlySelectedDisplayMode
        {
            get { return DisplayModes[(int)DisplayMode]; }
            set
            {
                DisplayMode = value.Item1;
                if (Settings.LockDisplayMode)
                    _manuallySelectedViewMode = value.Item1;
                _lastOffset = 0;
                if (Settings.HideViewSelectionFlyout)
                    HideViewsFlyout?.Invoke();
                RaisePropertyChanged(() => DisplayMode);
                RefreshList(false, true);
            }
        }

        public ObservableCollection<Tuple<AnimeListDisplayModes, string>> DisplayModes { get; } = new ObservableCollection<Tuple<AnimeListDisplayModes, string>>
        {
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteList, "Detailed Grid"), new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteGrid, "Grid"), new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteCompactList, "Compact list")
        };


        private bool _sortAirDayVisibility;

        public bool SortAirDayVisibility
        {
            get { return _sortAirDayVisibility; }
            set
            {
                _sortAirDayVisibility = value;
                RaisePropertyChanged(() => SortAirDayVisibility);
            }
        }

        public bool LoadAllDetailsButtonVisiblity
            => WorkMode != AnimeListWorkModes.SeasonalAnime && WorkMode != AnimeListWorkModes.Manga &&
               WorkMode != AnimeListWorkModes.TopManga;


        private bool _goingCustomSeason;

        private int _seasonalUrlsSelectedIndex;

        public int SeasonalUrlsSelectedIndex
        {
            get { return _seasonalUrlsSelectedIndex; }
            set
            {
                if (_goingCustomSeason || value == _seasonalUrlsSelectedIndex || value < 0 || !SeasonSelection.Any())
                    return;
                if (SeasonSelection.Count == 5) //additional custom season
                    SeasonSelection.RemoveAt(4);
                _seasonalUrlsSelectedIndex = value;
                CurrentSeason = SeasonSelection[value];
                RaisePropertyChanged(() => SeasonalUrlsSelectedIndex);
                HideSeasonSelectionFlyout?.Invoke();
                FetchSeasonalData();
            }
        }

        public List<string> SeasonSeasons { get; } = new List<string>
        {
            "Winter","Spring","Summer","Fall"
        };

        public List<string> SeasonYears { get; } = new List<string>();

        public string CurrentlySelectedCustomSeasonSeason { get; set; }

        public string CurrentlySelectedCustomSeasonYear { get; set; }

        private ICommand _goToCustomSeasonCommand;

        public ICommand GoToCustomSeasonCommand
            => _goToCustomSeasonCommand ?? (_goToCustomSeasonCommand = new RelayCommand(
                () =>
                {

                    if (string.IsNullOrEmpty(CurrentlySelectedCustomSeasonSeason) || string.IsNullOrEmpty(CurrentlySelectedCustomSeasonYear))
                        return;
                    _goingCustomSeason = true;
                    if (SeasonSelection.Count == 5) //additional custom season
                        SeasonSelection.RemoveAt(4);
                    CurrentSeason = new AnimeSeason
                    {
                        Name = $"{CurrentlySelectedCustomSeasonSeason} {CurrentlySelectedCustomSeasonYear}",
                        Url = $"https://myanimelist.net/anime/season/{CurrentlySelectedCustomSeasonYear}/{CurrentlySelectedCustomSeasonSeason.ToLower()}"
                    };
                    SeasonSelection.Add(CurrentSeason);
                    _seasonalUrlsSelectedIndex = 4;
                    RaisePropertyChanged(() => SeasonalUrlsSelectedIndex);
                    _goingCustomSeason = false;
                    FetchSeasonalData();
                }));

        private SortOptions _sortOption = SortOptions.SortNothing;

        public SortOptions SortOption
        {
            get { return _sortOption; }
            set
            {
                if (!Initializing && Settings.HideSortingSelectionFlyout)
                    HideSortingFlyout?.Invoke();
                _sortOption = value;
            }
        }

        private double? _maxWidth;
        private int _currentPage;
        private bool _canLoadMore;

        public double MaxWidth => (_maxWidth ?? (_maxWidth = AnimeItemViewModel.MaxWidth)).Value;

        #endregion
    }
}
