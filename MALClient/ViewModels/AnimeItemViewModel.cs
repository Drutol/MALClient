using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public interface IAnimeItemInteractions
    {
        Flyout WatchedFlyout { get; }
        object MoreFlyout { get; }
        void MoreFlyoutHide();
    }

    public class AnimeItemViewModel : ViewModelBase, IAnimeData
    {
        public const string InvalidStartEndDate = "0000-00-00";

        private readonly string _imgUrl;
        public readonly AnimeItemAbstraction _parentAbstraction;
        private float _globalScore;
        private bool _seasonalState;
        //prop field pairs

        public IAnimeItemInteractions ViewGrid;
        public IAnimeItemInteractions ViewList;
        public IAnimeItemInteractions ViewCompact;

        static AnimeItemViewModel()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            //var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            MaxWidth = bounds.Width/2.1;
        }

        public static double MaxWidth { get; set; }


        //state fields
        public int Id { get; set; }

        public async void NavigateDetails(PageIndex? sourceOverride = null, object argsOverride = null)
        {
            await ViewModelLocator.Main
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(Id, Title, null, this,
                        argsOverride ?? Utils.GetMainPageInstance().GetCurrentListOrderParams())
                    {
                        Source =
                            sourceOverride ??
                            (_parentAbstraction.RepresentsAnime ? PageIndex.PageAnimeList : PageIndex.PageMangaList),
                        AnimeMode = _parentAbstraction.RepresentsAnime
                    });
        }



        public void UpdateWithSeasonData(SeasonalAnimeData data)
        {
            GlobalScore = data.Score;
            Airing = data.AirDay >= 0;
            if (!Auth)
            {
                UpdateButtonsVisibility = Visibility.Collapsed;
                _seasonalState = true;
            }
            RaisePropertyChanged(() => MyEpisodesBind);
        }

        public void SignalBackToList()
        {
            _seasonalState = false;
            RaisePropertyChanged(() => MyEpisodesBind);
            RaisePropertyChanged(() => AirDayBind);
        }

        private async void AddThisToMyList()
        {
            var response =
                _parentAbstraction.RepresentsAnime
                    ? await new AnimeAddQuery(Id.ToString()).GetRequestResponse()
                    : await new MangaAddQuery(Id.ToString()).GetRequestResponse();
            if (!response.Contains("Created"))
                return;
            _seasonalState = false;
            SetAuthStatus(true);
            MyScore = 0;
            MyStatus = 6;
            MyEpisodes = 0;
            if (_parentAbstraction.RepresentsAnime)
                MyVolumes = 0;


            AdjustIncrementButtonsVisibility();
            AddToListVisibility = Visibility.Collapsed;
            ViewModelLocator.AnimeList.AddAnimeEntry(_parentAbstraction);
        }

        #region Constructors

        private AnimeItemViewModel(string img, int id, AnimeItemAbstraction parent)
        {
            _parentAbstraction = parent;
            _imgUrl = img;
            Id = id;
            Image = new BitmapImage(new Uri(_imgUrl));
            AdjustIncrementButtonsOrientation();
            if (!_parentAbstraction.RepresentsAnime)
            {
                UpdateEpsUpperLabel = "Read chapters :";
                Status1Label = "Reading";
                Status5Label = "Plan to read";
            }
        }

        public AnimeItemViewModel(bool auth, string name, string img, int id, int myStatus, int myEps, int allEps,
            int myScore,string startDate,string endDate,
            AnimeItemAbstraction parent, bool setEpsAuth = false) : this(img, id, parent)
            //We are loading an item that IS on the list
        {
            //Assign fields
            Id = id;
            _allEpisodes = allEps;
            Auth = auth;
            //Assign properties
            MyScore = myScore;
            MyStatus = myStatus;
            Title = name;
            MyEpisodes = myEps;
            ShowMoreVisibility = Visibility.Collapsed;
            StartDate = startDate;
            EndDate = endDate;
            //We are not seasonal so it's already on list            
            AddToListVisibility = Visibility.Collapsed;
            SetAuthStatus(auth, setEpsAuth);
            AdjustIncrementButtonsVisibility();
            //There may be additional data available
            GlobalScore = _parentAbstraction.GlobalScore;
            Airing = _parentAbstraction.AirDay >= 0;
        }

        //manga
        public AnimeItemViewModel(bool auth, string name, string img, int id, int myStatus, int myEps, int allEps,
            int myScore,string startDate,string endDate,
            AnimeItemAbstraction parent, bool setEpsAuth, int myVolumes, int allVolumes)
            : this(auth, name, img, id, myStatus, myEps, allEps, myScore,startDate,endDate, parent, setEpsAuth)
        {
            AllVolumes = allVolumes;
        }

        public AnimeItemViewModel(SeasonalAnimeData data,
            AnimeItemAbstraction parent) : this(data.ImgUrl, data.Id, parent)
            //We are loading an item that is NOT on the list and is seasonal
        {
            _seasonalState = true;

            Title = data.Title;
            MyScore = 0;
            MyStatus = (int) AnimeStatus.AllOrAiring;
            GlobalScore = data.Score;
            int.TryParse(data.Episodes, out _allEpisodes);
            Airing = _parentAbstraction.AirDay >= 0;
            SetAuthStatus(false, true);
            AdjustIncrementButtonsVisibility();
            ShowMoreVisibility = Visibility.Collapsed;
        }

        #endregion

        #region PropertyPairs

        private readonly int _allEpisodes;
        public int AllEpisodes => _allEpisodes;
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int AllVolumes { get; }

        public string AirDayBind
            =>
                ViewModelLocator.AnimeList.WorkMode == AnimeListWorkModes.TopAnime ||
                ViewModelLocator.AnimeList.WorkMode == AnimeListWorkModes.TopManga
                    ? _parentAbstraction.Index.ToString()
                    : Utils.DayToString((DayOfWeek) (_parentAbstraction.AirDay - 1));

        private bool _airing;

        public bool Airing
        {
            get { return _airing; }
            set
            {
                if (_parentAbstraction.TryRetrieveVolatileData())
                {
                    RaisePropertyChanged(() => AirDayBind);
                    TitleMargin = new Thickness(5, 3, 50, 0);
                }
                _airing = value;
                RaisePropertyChanged(() => Airing);
            }
        }

        private Thickness _titleMargin = new Thickness(5, 3, 5, 3);

        public Thickness TitleMargin
        {
            get { return _titleMargin; }
            set
            {
                _titleMargin = value;
                RaisePropertyChanged(() => TitleMargin);
            }
        }

        public bool Auth { get; private set; }


        public string Type
            =>
                _parentAbstraction.Type == 0
                    ? ""
                    : _parentAbstraction.RepresentsAnime
                        ? ((AnimeType) _parentAbstraction.Type).ToString()
                        : ((MangaType) _parentAbstraction.Type).ToString();


        public string MyStatusBind => Utils.StatusToString(MyStatus, !_parentAbstraction.RepresentsAnime);
        public string MyStatusBindShort => Utils.StatusToShortString(MyStatus, !_parentAbstraction.RepresentsAnime);

        public int MyStatus
        {
            get { return _parentAbstraction.MyStatus; }
            set
            {
                if (_parentAbstraction.MyStatus == value)
                    return;
                _parentAbstraction.MyStatus = value;
                AdjustIncrementButtonsOrientation();
                AdjustIncrementButtonsVisibility();
                RaisePropertyChanged(() => MyStatusBind);
                RaisePropertyChanged(() => MyStatusBindShort);
            }
        }

        public string MyScoreBind => MyScore == 0 ? "Unranked" : $"{MyScore}/10";
        public string MyScoreBindShort => MyScore == 0 ? "N/A" : $"{MyScore}/10";

        public int MyScore
        {
            get { return _parentAbstraction.MyScore; }
            set
            {
                if (_parentAbstraction.MyScore == value)
                    return;
                _parentAbstraction.MyScore = value;
                AdjustIncrementButtonsOrientation();
                AdjustIncrementButtonsVisibility();
                RaisePropertyChanged(() => MyScoreBind);
                RaisePropertyChanged(() => MyScoreBindShort);
            }
        }



        public string MyEpisodesBind
        {
            get
            {
                if (_seasonalState)
                    return $"{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())} Episodes";

                return Auth || MyEpisodes != 0
                    ? $"{(_parentAbstraction.RepresentsAnime ? "Watched" : "Read")} : " +
                      $"{MyEpisodes}/{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())}"
                    : $"{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())} Episodes";
            }
        }

        public string MyEpisodesBindShort => $"{MyEpisodes}/{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())}";

        public int MyEpisodes
        {
            get { return _parentAbstraction.MyEpisodes; }
            set
            {
                if (_parentAbstraction.MyEpisodes == value)
                    return;
                _parentAbstraction.MyEpisodes = value;
                RaisePropertyChanged(() => MyEpisodesBind);
                RaisePropertyChanged(() => MyEpisodesBindShort);
            }
        }

        public string MyVolumesBind
            =>
                Auth || MyEpisodes != 0
                    ? "Read : " + $"{MyVolumes}/{(AllVolumes == 0 ? "?" : AllVolumes.ToString())}"
                    : $"{(AllVolumes == 0 ? "?" : AllVolumes.ToString())} Volumes";

        public int MyVolumes
        {
            get { return _parentAbstraction.MyVolumes; }
            set
            {
                if (_parentAbstraction.MyVolumes == value)
                    return;
                _parentAbstraction.MyVolumes = value;
                RaisePropertyChanged(() => MyVolumesBind);
            }
        }

        private string _watchedEpsLabel = "My watched\nepisodes :";

        public string WatchedEpsLabel
        {
            get { return _watchedEpsLabel; }
            set
            {
                _watchedEpsLabel = value;
                RaisePropertyChanged(() => WatchedEpsLabel);
            }
        }

        private string _updateEpsUpperLabel = "Watched eps :";

        public string UpdateEpsUpperLabel
        {
            get { return _updateEpsUpperLabel; }
            set
            {
                _updateEpsUpperLabel = value;
                RaisePropertyChanged(() => UpdateEpsUpperLabel);
            }
        }

        private string _status1Label = "Watching";

        public string Status1Label
        {
            get { return _status1Label; }
            set
            {
                _status1Label = value;
                RaisePropertyChanged(() => Status1Label);
            }
        }

        private string _status5Label = "Plan to watch";

        public string Status5Label
        {
            get { return _status5Label; }
            set
            {
                _status5Label = value;
                RaisePropertyChanged(() => Status5Label);
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public string GlobalScoreBind => GlobalScore == 0 ? "" : GlobalScore.ToString();

        public float GlobalScore
        {
            get { return _globalScore; }
            set
            {
                if (value == 0)
                    return;
                _globalScore = value;
                RaisePropertyChanged(() => GlobalScoreBind);
            }
        }

        private BitmapImage _image;

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged(() => Image);
            }
        }

        private bool _updateButtonsEnableState;

        public bool UpdateButtonsEnableState
        {
            get { return _updateButtonsEnableState; }
            set
            {
                _updateButtonsEnableState = value;
                RaisePropertyChanged(() => UpdateButtonsEnableState);
            }
        }

        private Visibility _addToListVisibility;

        public Visibility AddToListVisibility
        {
            get { return _addToListVisibility; }
            set
            {
                _addToListVisibility = value;
                RaisePropertyChanged(() => AddToListVisibility);
            }
        }

        private Visibility _incrementEpsVisibility;

        public Visibility IncrementEpsVisibility
        {
            get { return _incrementEpsVisibility; }
            set
            {
                _incrementEpsVisibility = value;
                RaisePropertyChanged(() => IncrementEpsVisibility);
            }
        }

        private Visibility _decrementEpsVisibility;

        public Visibility DecrementEpsVisibility
        {
            get { return _decrementEpsVisibility; }
            set
            {
                _decrementEpsVisibility = value;
                RaisePropertyChanged(() => DecrementEpsVisibility);
            }
        }

        private Visibility _showMoreVisiblity;

        public Visibility ShowMoreVisibility
        {
            get { return _showMoreVisiblity; }
            set
            {
                _showMoreVisiblity = value;
                RaisePropertyChanged(() => ShowMoreVisibility);
            }
        }

        private Visibility _updateButtonsVisibility;

        public Visibility UpdateButtonsVisibility
        {
            get { return _updateButtonsVisibility; }
            set
            {
                _updateButtonsVisibility = value;
                RaisePropertyChanged(() => UpdateButtonsVisibility);
            }
        }

        private Visibility _tileUrlInputVisibility = Visibility.Collapsed;

        public Visibility TileUrlInputVisibility
        {
            get { return _tileUrlInputVisibility; }
            set
            {
                _tileUrlInputVisibility = value;
                RaisePropertyChanged(() => TileUrlInputVisibility);
            }
        }

        private string _tileUrlInput;

        public string TileUrlInput
        {
            get { return _tileUrlInput; }
            set
            {
                _tileUrlInput = value;
                RaisePropertyChanged(() => TileUrlInput);
            }
        }

        private string _watchedEpsInput;

        public string WatchedEpsInput
        {
            get { return _watchedEpsInput; }
            set
            {
                _watchedEpsInput = value;
                RaisePropertyChanged(() => WatchedEpsInput);
            }
        }

        private Visibility _watchedEpsInputNoticeVisibility = Visibility.Collapsed;

        public Visibility WatchedEpsInputNoticeVisibility
        {
            get { return _watchedEpsInputNoticeVisibility; }
            set
            {
                _watchedEpsInputNoticeVisibility = value;
                RaisePropertyChanged(() => WatchedEpsInputNoticeVisibility);
            }
        }

        //private Brush _rootBrush = new SolidColorBrush(Colors.WhiteSmoke);

        //public Brush RootBrush
        //{
        //    get { return _rootBrush; }
        //    set
        //    {
        //        _rootBrush = value;
        //        RaisePropertyChanged(() => RootBrush);
        //    }
        //}

        private Visibility _loadingUpdate = Visibility.Collapsed;

        public Visibility LoadingUpdate
        {
            get { return _loadingUpdate; }
            set
            {
                _loadingUpdate = value;
                RaisePropertyChanged(() => LoadingUpdate);
            }
        }

        private ICommand _changeStatusCommand;

        public ICommand ChangeStatusCommand
        {
            get { return _changeStatusCommand ?? (_changeStatusCommand = new RelayCommand<object>(ChangeStatus)); }
        }

        private ICommand _changeScoreCommand;

        public ICommand ChangeScoreCommand
        {
            get { return _changeScoreCommand ?? (_changeScoreCommand = new RelayCommand<object>(ChangeScore)); }
        }

        private ICommand _changeWatchedCommand;

        public ICommand ChangeWatchedCommand
        {
            get { return _changeWatchedCommand ?? (_changeWatchedCommand = new RelayCommand(ChangeWatchedEps)); }
        }

        private ICommand _incrementWatchedCommand;

        public ICommand IncrementWatchedCommand
        {
            get
            {
                return _incrementWatchedCommand ?? (_incrementWatchedCommand = new RelayCommand(IncrementWatchedEp));
            }
        }

        private ICommand _decrementWatchedCommand;

        public ICommand DecrementWatchedCommand
        {
            get
            {
                return _decrementWatchedCommand ?? (_decrementWatchedCommand = new RelayCommand(DecrementWatchedEp));
            }
        }

        private ICommand _addAnimeCommand;

        public ICommand AddAnimeCommand
        {
            get { return _addAnimeCommand ?? (_addAnimeCommand = new RelayCommand(AddThisToMyList)); }
        }

        private ICommand _pinTileCommand;

        public ICommand PinTileCommand
        {
            get { return _pinTileCommand ?? (_pinTileCommand = new RelayCommand(() => PinTile())); }
        }

        private ICommand _pinTileCustomCommand;

        public ICommand PinTileCustomCommand
        {
            get
            {
                return _pinTileCustomCommand ??
                       (_pinTileCustomCommand =
                           new RelayCommand(() =>
                           {
                               ViewList?.MoreFlyoutHide();
                               ViewGrid?.MoreFlyoutHide();
                               ViewCompact?.MoreFlyoutHide();
                               TileUrlInputVisibility = Visibility.Visible;
                           }));
            }
        }

        private ICommand _copyLinkToClipboardCommand;

        public ICommand CopyLinkToClipboardCommand
        {
            get
            {
                return _copyLinkToClipboardCommand ??
                       (_copyLinkToClipboardCommand = new RelayCommand(() =>
                       {
                           var dp = new DataPackage();
                           dp.SetText(
                               $"http://www.myanimelist.net/{(_parentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}");
                           Clipboard.SetContent(dp);
                           ViewList?.MoreFlyoutHide();
                           ViewGrid?.MoreFlyoutHide();
                           ViewCompact?.MoreFlyoutHide();
                           Utils.GiveStatusBarFeedback("Copied to clipboard...");
                       }));
            }
        }

        private ICommand _openInMALCommand;

        public ICommand OpenInMALCommand
        {
            get
            {
                return _openInMALCommand ??
                       (_openInMALCommand = new RelayCommand(async () =>
                       {
                           ViewList?.MoreFlyoutHide();
                           ViewGrid?.MoreFlyoutHide();
                           ViewCompact?.MoreFlyoutHide();
                           await
                               Launcher.LaunchUriAsync(
                                   new Uri(
                                       $"http://myanimelist.net/{(_parentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}"));
                       }));
            }
        }

        private ICommand _pinTileMALCommand;

        public ICommand PinTileMALCommand
        {
            get
            {
                return _pinTileMALCommand ?? (_pinTileMALCommand = new RelayCommand(async () =>
                {
                    ViewList?.MoreFlyoutHide();
                    ViewGrid?.MoreFlyoutHide();
                    ViewCompact?.MoreFlyoutHide();
                    if (SecondaryTile.Exists(Id.ToString()))
                    {
                        var msg = new MessageDialog("Tile for this anime already exists.");
                        await msg.ShowAsync();
                        return;
                    }
                    PinTile(
                        $"http://www.myanimelist.net/{(_parentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}");
                }));
            }
        }

        private ICommand _navigateDetailsCommand;

        public ICommand NavigateDetailsCommand
        {
            get
            {
                return _navigateDetailsCommand ?? (_navigateDetailsCommand = new RelayCommand(() => NavigateDetails()));
            }
        }


        #endregion

        #region Utils/Helpers

        //Pinned with custom link.
        public void PinTile(string url = null)
        {
            Utils.PinTile(url ?? TileUrlInput, Id, _imgUrl, Title);
            TileUrlInputVisibility = Visibility.Collapsed;
        }

        public void SetAuthStatus(bool auth, bool eps = false)
        {
            Auth = auth;
            if (auth)
            {
                AddToListVisibility = Visibility.Collapsed;
                UpdateButtonsVisibility = Visibility.Visible;
                UpdateButtonsEnableState = true;
            }
            else
            {
                AddToListVisibility = _seasonalState && Credentials.Authenticated
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                UpdateButtonsEnableState = false;

                if (eps)
                {
                    RaisePropertyChanged(() => MyEpisodesBind);
                    UpdateButtonsVisibility = Visibility.Collapsed;
                }
            }
            AdjustIncrementButtonsVisibility();
        }

        private void AdjustIncrementButtonsVisibility()
        {
            if (!Auth || !Credentials.Authenticated)
            {
                IncrementEpsVisibility = Visibility.Collapsed;
                DecrementEpsVisibility = Visibility.Collapsed;
                return;
            }

            if (MyEpisodes == _allEpisodes && _allEpisodes != 0)
            {
                IncrementEpsVisibility = Visibility.Collapsed;
                DecrementEpsVisibility = Visibility.Visible;
            }
            else if (MyEpisodes == 0)
            {
                IncrementEpsVisibility = Visibility.Visible;
                DecrementEpsVisibility = Visibility.Collapsed;
            }
            else
            {
                IncrementEpsVisibility = Visibility.Visible;
                DecrementEpsVisibility = Visibility.Visible;
            }
        }

        private void AdjustIncrementButtonsOrientation()
        {
            //Too wide update buttons
            //if (MyScore != 0)
            //{
            //    IncrementButtonsOrientation = Orientation.Horizontal;
            //    return;
            //}
            //if (MyStatus == (int) AnimeStatus.Dropped ||
            //    MyStatus == (int) AnimeStatus.OnHold ||
            //    MyStatus == (int) AnimeStatus.Completed ||
            //    MyStatus == (int) AnimeStatus.Watching)
            //    IncrementButtonsOrientation = Orientation.Vertical;
        }

        #endregion

        #region AnimeUpdate

        private Query GetAppropriateUpdateQuery()
        {
            if (_parentAbstraction.RepresentsAnime)
                return new AnimeUpdateQuery(this);
            return new MangaUpdateQuery(this);
        }

        #region Watched

        private async void IncrementWatchedEp()
        {
            LoadingUpdate = Visibility.Visible;

            if (MyStatus == (int) AnimeStatus.PlanToWatch || MyStatus == (int) AnimeStatus.Dropped ||
                MyStatus == (int) AnimeStatus.OnHold)
                await PromptForStatusChange((int) AnimeStatus.Watching);

            MyEpisodes++;
            AdjustIncrementButtonsVisibility();
            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated")
            {
                MyEpisodes--; // Shouldn't occur really , but hey shouldn't and MAL api goes along very well.
                AdjustIncrementButtonsVisibility();
            }

            if (MyEpisodes == _allEpisodes && _allEpisodes != 0)
                await PromptForStatusChange((int) AnimeStatus.Completed);

            LoadingUpdate = Visibility.Collapsed;
        }

        private async void DecrementWatchedEp()
        {
            LoadingUpdate = Visibility.Visible;
            MyEpisodes--;
            AdjustIncrementButtonsVisibility();
            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated")
            {
                MyEpisodes++;
                AdjustIncrementButtonsVisibility();
            }


            LoadingUpdate = Visibility.Collapsed;
        }

        public async void ChangeWatchedEps()
        {
            int watched;
            if (!int.TryParse(WatchedEpsInput, out watched))
            {
                WatchedEpsInputNoticeVisibility = Visibility.Visible;
                return;
            }
            if (watched >= 0 && (_allEpisodes == 0 || watched <= _allEpisodes))
            {
                ViewList.WatchedFlyout.Hide();
                LoadingUpdate = Visibility.Visible;
                WatchedEpsInputNoticeVisibility = Visibility.Collapsed;
                var prevWatched = MyEpisodes;
                MyEpisodes = watched;
                var response = await GetAppropriateUpdateQuery().GetRequestResponse();
                if (response != "Updated")
                    MyEpisodes = prevWatched;

                if (MyEpisodes == _allEpisodes && _allEpisodes != 0)
                    await PromptForStatusChange((int) AnimeStatus.Completed);

                AdjustIncrementButtonsVisibility();


                LoadingUpdate = Visibility.Collapsed;
                WatchedEpsInput = "";
            }
            else
            {
                WatchedEpsInputNoticeVisibility = Visibility.Visible;
            }
        }

        #endregion

        private async void ChangeStatus(object status)
        {
            LoadingUpdate = Visibility.Visible;
            var myPrevStatus = MyStatus;
            MyStatus = Utils.StatusToInt(status as string);
            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated")
                MyStatus = myPrevStatus;

            if (MyStatus == (int) AnimeStatus.Completed && _allEpisodes != 0)
                await PromptForWatchedEpsChange(_allEpisodes);

            LoadingUpdate = Visibility.Collapsed;
        }

        private async void ChangeScore(object score)
        {
            LoadingUpdate = Visibility.Visible;
            var myPrevScore = MyScore;
            MyScore = Convert.ToInt32(score);
            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated")
                MyScore = myPrevScore;

            LoadingUpdate = Visibility.Collapsed;
        }

        #endregion

        #region Prompts

        public async Task PromptForStatusChange(int to)
        {
            if (MyStatus == to)
                return;
            var msg =
                new MessageDialog(
                    $"From : {Utils.StatusToString(MyStatus, !_parentAbstraction.RepresentsAnime)}\nTo : {Utils.StatusToString(to)}",
                    "Would you like to change current status?");
            var confirmation = false;
            msg.Commands.Add(new UICommand("Yes", command => confirmation = true));
            msg.Commands.Add(new UICommand("No"));
            await msg.ShowAsync();
            if (confirmation)
            {
                var myPrevStatus = MyStatus;
                MyStatus = to;
                var response = await GetAppropriateUpdateQuery().GetRequestResponse();
                if (response != "Updated")
                    MyStatus = myPrevStatus;
            }
        }

        public async Task PromptForWatchedEpsChange(int to)
        {
            if (MyEpisodes == to)
                return;
            var msg = new MessageDialog($"From : {MyEpisodes}\nTo : {to}",
                "Would you like to change watched episodes value?");
            var confirmation = false;
            msg.Commands.Add(new UICommand("Yes", command => confirmation = true));
            msg.Commands.Add(new UICommand("No"));
            await msg.ShowAsync();
            if (confirmation)
            {
                var myPrevEps = MyEpisodes;
                MyEpisodes = to;
                var response = await GetAppropriateUpdateQuery().GetRequestResponse();
                if (response != "Updated")
                    MyStatus = myPrevEps;

                AdjustIncrementButtonsVisibility();
            }
        }

        #endregion
    }
}