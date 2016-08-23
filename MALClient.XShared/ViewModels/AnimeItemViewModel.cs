using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.Models.Models.Library;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Comm.Manga;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.Utils.Managers;

namespace MALClient.XShared.ViewModels
{
    public enum AnimeItemDisplayContext
    {
        Index,
        AirDay,
    }

    public class AnimeItemViewModel : ViewModelBase, IAnimeData
    {
        public const string InvalidStartEndDate = "0000-00-00";
        public readonly AnimeItemAbstraction ParentAbstraction;
        private float _globalScore;
        private bool _seasonalState;

        static AnimeItemViewModel()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            //var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            MaxWidth = bounds.Width/2.05;
            if (MaxWidth > 200)
                MaxWidth = 200;
            UpdateScoreFlyoutChoices();
        }

        //
        public string ImgUrl { get; set; }
        //prop field pairs

        public static double MaxWidth { get; set; }

        public static List<string> ScoreFlyoutChoices { get; set; }


        //state fields
        public int Id { get; set; }

        public async void NavigateDetails(PageIndex? sourceOverride = null, object argsOverride = null)
        {
            if (Settings.SelectedApiType == ApiType.Hummingbird && !ParentAbstraction.RepresentsAnime || ViewModelLocator.AnimeDetails.Id == Id)
                return;
            var id = Id;
            if (_seasonalState && Settings.SelectedApiType == ApiType.Hummingbird) //id switch
            {
                id = await new AnimeDetailsHummingbirdQuery(id).GetHummingbirdId();
            }
            var navArgs = new AnimeDetailsPageNavigationArgs(id, Title, null, this,
                argsOverride ?? ViewModelLocator.GeneralMain.GetCurrentListOrderParams())
            {
                Source =
                    sourceOverride ??
                    (ParentAbstraction.RepresentsAnime ? PageIndex.PageAnimeList : PageIndex.PageMangaList),
                AnimeMode = ParentAbstraction.RepresentsAnime
            };
            if (sourceOverride != null)
                navArgs.Source = sourceOverride.Value;
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,navArgs);
        }

        public void UpdateWithSeasonData(SeasonalAnimeData data, bool updateScore)
        {
            if(updateScore)
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
            RaisePropertyChanged(() => TopLeftInfoBind);
        }

        private async void AddThisToMyList()
        {
            LoadingUpdate = Visibility.Visible;
            var response =
                ParentAbstraction.RepresentsAnime
                    ? await new AnimeAddQuery(Id.ToString()).GetRequestResponse()
                    : await new MangaAddQuery(Id.ToString()).GetRequestResponse();
            LoadingUpdate = Visibility.Collapsed;
            if (Settings.SelectedApiType == ApiType.Mal && !response.Contains("Created"))
                return;
            var startDate = "0000-00-00";
            if (Settings.SetStartDateOnListAdd)
                startDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
            var animeItem = ParentAbstraction.RepresentsAnime
               ? new AnimeLibraryItemData
               {
                   Title = Title,
                   ImgUrl = ImgUrl,
                   Type = ParentAbstraction.Type,
                   Id = Id,
                   AllEpisodes = AllEpisodes,
                   MalId = ParentAbstraction.MalId,
                   MyStatus = AnimeStatus.PlanToWatch,
                   MyEpisodes = 0,
                   MyScore = 0,
                   MyStartDate = startDate,
                   MyEndDate = AnimeItemViewModel.InvalidStartEndDate
               }
               : new MangaLibraryItemData
               {
                   Title = Title,
                   ImgUrl = ImgUrl,
                   Type = ParentAbstraction.Type,
                   Id = Id,
                   AllEpisodes = AllEpisodes,
                   MalId = ParentAbstraction.MalId,
                   MyStatus = AnimeStatus.PlanToWatch,
                   MyEpisodes = 0,
                   MyScore = 0,
                   MyStartDate = startDate,
                   MyEndDate = AnimeItemViewModel.InvalidStartEndDate,
                   AllVolumes = AllVolumes,
                   MyVolumes = MyVolumes
               };
            ParentAbstraction.EntryData = animeItem;
            _seasonalState = false;
            SetAuthStatus(true);
            MyScore = 0;
            MyStatus = 6;
            MyEpisodes = 0;
            if (Settings.SetStartDateOnListAdd)
                ParentAbstraction.MyStartDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
            if (ParentAbstraction.RepresentsAnime)
                MyVolumes = 0;

            ItemManipulationMode = ManipulationModes.All;
            AddToListVisibility = Visibility.Collapsed;
            ViewModelLocator.AnimeList.AddAnimeEntry(ParentAbstraction);
            await Task.Delay(10);
            RaisePropertyChanged(() => MyStatusBindShort);
            RaisePropertyChanged(() => MyStatusBind);
            if (ViewModelLocator.AnimeDetails.Id == Id)
                ViewModelLocator.AnimeDetails.CurrentAnimeHasBeenAddedToList(this);
        }

        public static void UpdateScoreFlyoutChoices()
        {
            ScoreFlyoutChoices = Settings.SelectedApiType == ApiType.Mal
                ? new List<string>
                {
                    "10 - Masterpiece",
                    "9 - Great",
                    "8 - Very Good",
                    "7 - Good",
                    "6 - Fine",
                    "5 - Average",
                    "4 - Bad",
                    "3 - Very Bad",
                    "2 - Horrible",
                    "1 - Appaling"
                }
                : new List<string>
                {
                    "5 - Masterpiece",
                    "4.5 - Great",
                    "4 - Very Good",
                    "3.5 - Good",
                    "3 - Fine",
                    "2.5 - Average",
                    "2 - Bad",
                    "1.5 - Very Bad",
                    "1 - Horrible",
                    "0.5 - Appaling"
                };
        }

        #region Constructors

        private AnimeItemViewModel(string img, int id, AnimeItemAbstraction parent)
        {
            ParentAbstraction = parent;
            ImgUrl = img;
            Id = id;
            if (!ParentAbstraction.RepresentsAnime)
            {
                UpdateEpsUpperLabel = Settings.MangaFocusVolumes ? "Read volumes" : "Read chapters";
                Status1Label = "Reading";
                Status5Label = "Plan to read";
            }
        }

        public AnimeItemViewModel(bool auth, string name, string img, int id, int allEps,AnimeItemAbstraction parent, bool setEpsAuth = false) : this(img, id, parent)
            //We are loading an item that IS on the list
        {
            //Assign fields
            Id = id;
            _allEpisodes = allEps;
            Auth = auth;
            ItemManipulationMode = auth ? ManipulationModes.All : ManipulationModes.None;
            //Assign properties
            Title = name;
            ShowMoreVisibility = Visibility.Collapsed;
            //We are not seasonal so it's already on list            
            AddToListVisibility = Visibility.Collapsed;
            SetAuthStatus(auth, setEpsAuth);
            AdjustIncrementButtonsVisibility();
            //There may be additional data available
            GlobalScore = ParentAbstraction.GlobalScore;
            Airing = ParentAbstraction.AirDay >= 0;
        }

        //manga
        public AnimeItemViewModel(bool auth, string name, string img, int id, int allEps,
            AnimeItemAbstraction parent, bool setEpsAuth, int allVolumes)
            : this(auth, name, img, id, allEps, parent, setEpsAuth)
        {
            if (Settings.MangaFocusVolumes)
            {
                _allEpisodes = allVolumes; //invert this
                _allVolumes = allEps;
            }
            else
            {
                _allVolumes = allVolumes; //else standard
            }
        }

        public AnimeItemViewModel(SeasonalAnimeData data,
            AnimeItemAbstraction parent) : this(data.ImgUrl, data.Id, parent)
            //We are loading an item that is NOT on the list and is seasonal
        {
            _seasonalState = true;
            ItemManipulationMode = ManipulationModes.None;
            Title = data.Title;
            MyScore = 0;
            MyStatus = (int) AnimeStatus.AllOrAiring;
            GlobalScore = data.Score;
            int.TryParse(data.Episodes, out _allEpisodes);
            Airing = ParentAbstraction.AirDay >= 0;
            SetAuthStatus(false, true);
            AdjustIncrementButtonsVisibility();
            ShowMoreVisibility = Visibility.Collapsed;
        }

        #endregion

        #region PropertyPairs

         private int _allEpisodes;
         private int _allVolumes;
         public int AllEpisodes => ParentAbstraction.AllEpisodes;
         public int AllVolumes => ParentAbstraction.AllVolumes;
         public int AllEpisodesFocused => _allEpisodes;
         public int AllVolumesFocused => _allVolumes;

        public string Notes
         {
             get { return ParentAbstraction.Notes; }
             set
             {
                 ParentAbstraction.Notes = value.Trim(',');
                 RaisePropertyChanged(() => Notes);
                 RaisePropertyChanged(() => TagsControlVisibility);
             }
         }

        public string EndDate
        {
            get { return ParentAbstraction.MyEndDate; }
            set { ParentAbstraction.MyEndDate = value; }
        }

        public string StartDate
        {
            get { return ParentAbstraction.MyStartDate; }
            set { ParentAbstraction.MyStartDate = value; }
        }


        public string TopLeftInfoBind
            =>
                AnimeItemDisplayContext == AnimeItemDisplayContext.Index
                    ? ParentAbstraction?.Index.ToString()
                    : Utilities.DayToString((DayOfWeek) (ParentAbstraction.AirDay - 1));

        private bool _airing;

        private Brush _airDayBrush;
        public Brush AirDayBrush
        {
            get
            {
                if (_airDayBrush != null)
                    return _airDayBrush;
                if (ParentAbstraction.AirStartDate != null)
                {
                    var diff = DateTimeOffset.Parse(ParentAbstraction.AirStartDate).Subtract(DateTimeOffset.Now);
                    if (diff.TotalSeconds > 0)
                    {
                        _airDayBrush = new SolidColorBrush(Settings.SelectedTheme == ApplicationTheme.Dark ? Colors.Gray : Colors.LightGray);
                        _airDayTillBind = diff.TotalDays < 1 ? _airDayTillBind = diff.TotalHours.ToString("N0") + "h" : diff.TotalDays.ToString("N0") + "d";
                        RaisePropertyChanged(() => AirDayTillBind);
                    }
                    else
                        _airDayBrush = new SolidColorBrush(Colors.White);
                }
                else
                    _airDayBrush = new SolidColorBrush(Colors.White);

                return _airDayBrush;
            }
        }

        public AnimeItemDisplayContext _animeItemDisplayContext;

        public AnimeItemDisplayContext AnimeItemDisplayContext
        {
            get { return _animeItemDisplayContext; }
            set
            {
                _animeItemDisplayContext = value;
                RaisePropertyChanged(() => TopLeftInfoBind);
            }
        }


        private string _airDayTillBind;

        public string AirDayTillBind => _airDayTillBind;

        public bool Airing
        {
            get { return _airing; }
            set
            {
                if (ParentAbstraction.TryRetrieveVolatileData())
                {
                    RaisePropertyChanged(() => TopLeftInfoBind);
                }
                _airing = value;
                RaisePropertyChanged(() => Airing);
            }
        }

        private Visibility? _isFavouriteVisibility;

        public Visibility IsFavouriteVisibility
        {
            get
            {
                return Settings.SelectedApiType == ApiType.Hummingbird ? Visibility.Collapsed : (Visibility)(_isFavouriteVisibility ??
                       (_isFavouriteVisibility =
                           FavouritesManager.IsFavourite(
                               ParentAbstraction.RepresentsAnime ? FavouriteType.Anime : FavouriteType.Manga,
                               Id.ToString())
                               ? Visibility.Visible
                               : Visibility.Collapsed));
            }
            set
            {
                _isFavouriteVisibility = value;
                RaisePropertyChanged(() => IsFavouriteVisibility);
            }
        }


        public Thickness TitleMargin
            => string.IsNullOrEmpty(TopLeftInfoBind) ? new Thickness(5, 3, 25, 3) : new Thickness(5, 3, 70, 3);


        private bool _auth;

        public bool Auth
        {
            get { return _auth; }
            private set
            {
                _auth = value;
                RaisePropertyChanged(() => Auth);
            }
        }

        public string Type
            =>
                ParentAbstraction.Type == 0
                    ? ""
                    : ParentAbstraction.RepresentsAnime
                        ? ((AnimeType) ParentAbstraction.Type).ToString()
                        : ((MangaType) ParentAbstraction.Type).ToString();


        public string MyStatusBind => Utilities.StatusToString(MyStatus, !ParentAbstraction.RepresentsAnime);
        public string MyStatusBindShort => Utilities.StatusToShortString(MyStatus, !ParentAbstraction.RepresentsAnime);

        public int MyStatus
        {
            get { return ParentAbstraction.MyStatus; }
            set
            {
                if (ParentAbstraction.MyStatus == value)
                    return;
                ParentAbstraction.MyStatus = value;
                AdjustIncrementButtonsVisibility();
                RaisePropertyChanged(() => MyStatusBind);
                RaisePropertyChanged(() => MyStatusBindShort);
                RaisePropertyChanged(() => MyStatus);
            }
        }

        public string MyScoreBind
            => MyScore == 0 ? "Unranked" : $"{MyScore}/{(Settings.SelectedApiType == ApiType.Mal ? "10" : "5")}";

        public string MyScoreBindShort
            => MyScore == 0 ? "N/A" : $"{MyScore}/{(Settings.SelectedApiType == ApiType.Mal ? "10" : "5")}";

        public float MyScore
        {
            get { return ParentAbstraction.MyScore; }
            set
            {
                if (ParentAbstraction.MyScore == value)
                    return;
                ParentAbstraction.MyScore = value;
                AdjustIncrementButtonsVisibility();
                RaisePropertyChanged(() => MyScoreBind);
                RaisePropertyChanged(() => MyScoreBindShort);
                RaisePropertyChanged(() => MyScore);
            }
        }

        public string MyEpisodesBind
        {
            get
            {
                if (_seasonalState)
                    return
                        $"{(AllEpisodesFocused == 0 ? "?" : AllEpisodesFocused.ToString())} {(ParentAbstraction.RepresentsAnime ? "Episodes" : $"{(Settings.MangaFocusVolumes ? "Volumes" : "Chapters")}")}";

                return Auth || MyEpisodes != 0
                    ? $"{(ParentAbstraction.RepresentsAnime ? "Watched" : "Read")} : " +
                      $"{MyEpisodesFocused}/{(AllEpisodesFocused == 0 ? "?" : AllEpisodesFocused.ToString())}"
                    : $"{(AllEpisodesFocused == 0 ? "?" : AllEpisodesFocused.ToString())} {(ParentAbstraction.RepresentsAnime ? "Episodes" : $"{(Settings.MangaFocusVolumes ? "Volumes" : "Chapters")}")}";
            }
        }

        public string MyEpisodesBindShort => $"{MyEpisodesFocused}/{(AllEpisodesFocused == 0 ? "?" : AllEpisodesFocused.ToString())}";

        public int MyEpisodes
        {
            get { return ParentAbstraction.MyEpisodes; }
            set
            {
                if (ParentAbstraction.MyEpisodes == value)
                    return;
                ParentAbstraction.MyEpisodes = value;
                RaisePropertyChanged(() => MyEpisodesBind);
                RaisePropertyChanged(() => MyEpisodesBindShort);
                ViewModelLocator.AnimeDetails.UpdateAnimeReferenceUiBindings(Id);
            }
        }

        /// <summary>
        /// Features inverted values (chapter/vols) which reflects focus setting.
        /// </summary>
        public int MyEpisodesFocused
        {
            get { return !ParentAbstraction.RepresentsAnime && Settings.MangaFocusVolumes ? ParentAbstraction.MyVolumes : ParentAbstraction.MyEpisodes; }
            set
            {
                if (!ParentAbstraction.RepresentsAnime && Settings.MangaFocusVolumes)
                {
                    if (ParentAbstraction.MyVolumes == value)
                        return;
                    ParentAbstraction.MyVolumes = value;
                }
                else
                {
                    if (ParentAbstraction.MyEpisodes == value)
                        return;
                    ParentAbstraction.MyEpisodes = value;
                }
                RaisePropertyChanged(() => MyEpisodesBind);
                RaisePropertyChanged(() => MyEpisodesBindShort);
                AdjustIncrementButtonsVisibility();
                ViewModelLocator.AnimeDetails.UpdateAnimeReferenceUiBindings(Id);
            }
        }

        public string MyVolumesBind
            =>
                Auth || MyEpisodes != 0
                    ? "Read : " + $"{MyVolumes}/{(AllVolumes == 0 ? "?" : AllVolumes.ToString())}"
                    : $"{(AllVolumes == 0 ? "?" : AllVolumes.ToString())} Volumes";

        public int MyVolumes
        {
            get { return ParentAbstraction.MyVolumes; }
            set
            {
                if (ParentAbstraction.MyVolumes == value)
                    return;
                ParentAbstraction.MyVolumes = value;
                RaisePropertyChanged(() => MyVolumesBind);
            }
        }

        private string _watchedEpsLabel = "Watched episodes";

        public string WatchedEpsLabel
        {
            get { return _watchedEpsLabel; }
            set
            {
                _watchedEpsLabel = value;
                RaisePropertyChanged(() => WatchedEpsLabel);
            }
        }

        private string _updateEpsUpperLabel = "Watched episodes";

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

        public string GlobalScoreBind => GlobalScore == 0 ? "" : GlobalScore.ToString("N2");

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

        public Visibility TagsControlVisibility
             => string.IsNullOrEmpty(Notes) ? Visibility.Collapsed : Visibility.Visible;

        private Visibility _addToListVisibility;

        public Visibility AddToListVisibility
        {
            get { return Settings.SelectedApiType == ApiType.Mal ? _addToListVisibility : Visibility.Collapsed; }
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

        private ManipulationModes _itemManipulationMode;

        public ManipulationModes ItemManipulationMode
        {
            get { return _itemManipulationMode; }
            set
            {
                if (Settings.EnableSwipeToIncDec)
                    switch (value)
                    {
                        case ManipulationModes.All:
                            _itemManipulationMode = ManipulationModes.TranslateRailsX | ManipulationModes.TranslateX |
                                                    ManipulationModes.System | ManipulationModes.TranslateInertia;
                            break;
                        case ManipulationModes.None:
                            _itemManipulationMode = ManipulationModes.System;
                            break;
                    }
                else
                    _itemManipulationMode = ManipulationModes.System;
                RaisePropertyChanged(() => ItemManipulationMode);
            }
        }

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

        private ICommand _onFlyoutEpsKeyDown;

        public ICommand OnFlyoutEpsKeyDown
        {
            get { return _onFlyoutEpsKeyDown ?? (_onFlyoutEpsKeyDown = new RelayCommand(ChangeWatchedEps)); }
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

        private ICommand _pinTileCustomCommand;

        public ICommand PinTileCustomCommand
        {
            get
            {
                return _pinTileCustomCommand ??
                       (_pinTileCustomCommand =
                           new RelayCommand(() => { ViewModelLocator.GeneralMain.PinDialogViewModel.Load(this); }));
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
                           if (Settings.SelectedApiType == ApiType.Mal)
                           {
                               dp.SetText(
                                   $"http://www.myanimelist.net/{(ParentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}");
                           }
                           else
                           {
                               dp.SetText($"https://hummingbird.me/{(ParentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}");
                           }
                           Clipboard.SetContent(dp);
                           Utilities.GiveStatusBarFeedback("Copied to clipboard...");
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
                           if (Settings.SelectedApiType == ApiType.Mal)
                           {
                               await
                                   Launcher.LaunchUriAsync(
                                       new Uri(
                                           $"http://myanimelist.net/{(ParentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}"));
                           }
                           else
                           {
                               await Launcher.LaunchUriAsync(new Uri($"https://hummingbird.me/{(ParentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}"));
                           }
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

        //Pinned with custom link

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

        public void UpdateVolatileData()
        {
            RaisePropertyChanged(() => TopLeftInfoBind);
            RaisePropertyChanged(() => GlobalScoreBind);
        }

        #endregion

        #region AnimeUpdate

        private Query GetAppropriateUpdateQuery()
        {
            if (ParentAbstraction.RepresentsAnime)
                return new AnimeUpdateQuery(this);
            return new MangaUpdateQuery(this);
        }

        #region Watched

        private async void IncrementWatchedEp()
        {
            if(IncrementEpsVisibility == Visibility.Collapsed || (AllEpisodesFocused != 0 && MyEpisodesFocused == AllEpisodesFocused))
                return;
            LoadingUpdate = Visibility.Visible;
            var trigCompleted = true;
            if (MyStatus == (int) AnimeStatus.PlanToWatch || MyStatus == (int) AnimeStatus.Dropped ||
                MyStatus == (int) AnimeStatus.OnHold)
            {
                trigCompleted = AllEpisodes > 1;
                await PromptForStatusChange(AllEpisodes == 1 ? (int) AnimeStatus.Completed : (int) AnimeStatus.Watching);
            }

            MyEpisodesFocused++;
            AdjustIncrementButtonsVisibility();
            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
            {
                MyEpisodes--; // Shouldn't occur really , but hey shouldn't and MAL api goes along very well.
                AdjustIncrementButtonsVisibility();
            }

            ParentAbstraction.LastWatched = DateTime.Now;

            if (trigCompleted && MyEpisodes == AllEpisodesFocused && AllEpisodesFocused != 0)
                await PromptForStatusChange((int) AnimeStatus.Completed);

            LoadingUpdate = Visibility.Collapsed;
        }

        private async void DecrementWatchedEp()
        {
            if (DecrementEpsVisibility == Visibility.Collapsed || MyEpisodesFocused == 0)
                return;
            LoadingUpdate = Visibility.Visible;
            MyEpisodes--;
            AdjustIncrementButtonsVisibility();
            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
            {
                MyEpisodesFocused++;
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
                LoadingUpdate = Visibility.Visible;
                WatchedEpsInputNoticeVisibility = Visibility.Collapsed;
                var prevWatched = MyEpisodesFocused;
                MyEpisodesFocused = watched;
                var response = await GetAppropriateUpdateQuery().GetRequestResponse();
                if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                    MyEpisodesFocused = prevWatched;

                if (MyEpisodesFocused == _allEpisodes && _allEpisodes != 0)
                    await PromptForStatusChange((int) AnimeStatus.Completed);

                AdjustIncrementButtonsVisibility();
                ParentAbstraction.LastWatched = DateTime.Now;

                LoadingUpdate = Visibility.Collapsed;
                WatchedEpsInput = "";
            }
            else
            {
                WatchedEpsInputNoticeVisibility = Visibility.Visible;
            }
        }

        #endregion

        private void ChangeStatus(object status)
        {
            ChangeStatus(Utilities.StatusToInt(status as string));
        }

        private async void ChangeStatus(int status)
        {
            LoadingUpdate = Visibility.Visible;
            var myPrevStatus = MyStatus;
            MyStatus = status;
            AnimeStatus stat = (AnimeStatus) status;
            if (Settings.SetStartDateOnWatching && stat == AnimeStatus.Watching&&
                (Settings.OverrideValidStartEndDate || ParentAbstraction.MyStartDate == "0000-00-00"))
                StartDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
            else if (Settings.SetEndDateOnDropped && stat == AnimeStatus.Dropped &&
                     (Settings.OverrideValidStartEndDate || ParentAbstraction.MyEndDate == "0000-00-00"))
                EndDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
            else if (Settings.SetEndDateOnCompleted &&  stat == AnimeStatus.Completed &&
                     (Settings.OverrideValidStartEndDate || ParentAbstraction.MyEndDate == "0000-00-00"))
                EndDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");

            ViewModelLocator.AnimeDetails.UpdateAnimeReferenceUiBindings(Id);

            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                MyStatus = myPrevStatus;

            if (MyStatus == (int) AnimeStatus.Completed && _allEpisodes != 0)
                await PromptForWatchedEpsChange(_allEpisodes);

            LoadingUpdate = Visibility.Collapsed;
        }

        private async void ChangeScore(object score)
        {
            LoadingUpdate = Visibility.Visible;
            var myPrevScore = MyScore;
            if (Settings.SelectedApiType == ApiType.Hummingbird)
            {
                MyScore = (float) Convert.ToDouble(score as string)/2;
                if (MyScore == myPrevScore)
                    MyScore = 0;
            }
            else
            {
                MyScore = Convert.ToInt32(score as string);
            }
            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                MyScore = myPrevScore;

            LoadingUpdate = Visibility.Collapsed;
        }

        #endregion

        public void MangaFocusChanged(bool focusManga)
        {
            if (focusManga)
            {
                _allEpisodes = ParentAbstraction.AllVolumes; //invert this
                _allVolumes = ParentAbstraction.AllEpisodes;
            }
            else
            {
                _allEpisodes = ParentAbstraction.AllEpisodes; //else standard
                _allVolumes = ParentAbstraction.AllVolumes;
            }
            RaisePropertyChanged(() => MyEpisodesBind);
            RaisePropertyChanged(() => MyEpisodesBindShort);
            UpdateEpsUpperLabel = focusManga ? "Read volumes" : "Read chapters";
        }

        #region Prompts

        public async Task PromptForStatusChange(int to)
        {
            try
            {
                if (MyStatus == to)
                    return;
                var msg =
                    new MessageDialog(
                        $"From : {Utilities.StatusToString(MyStatus, !ParentAbstraction.RepresentsAnime)}\nTo : {Utilities.StatusToString(to)}",
                        "Would you like to change current status?");
                var confirmation = false;
                msg.Commands.Add(new UICommand("Yes", command => confirmation = true));
                msg.Commands.Add(new UICommand("No"));
                await msg.ShowAsync();
                if (confirmation)
                {
                    ChangeStatus(to);
                }
            }
            catch (Exception)
            {
                //TODO access denied excpetion? we can try that 
            }

        }

        public async Task PromptForWatchedEpsChange(int to)
        {
            try
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
                    if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                        MyStatus = myPrevEps;

                    AdjustIncrementButtonsVisibility();
                }
            }
            catch (Exception)
            {
                //TODO access denied excpetion? we can try that 
            }

        }

        #endregion
    }
}