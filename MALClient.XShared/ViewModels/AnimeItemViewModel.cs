using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.Models.Interfaces;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.Models.Models.Library;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Comm.Manga;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;

namespace MALClient.XShared.ViewModels
{
    public enum AnimeItemDisplayContext
    {
        Index,
        AirDay,
    }

    public class AnimeItemViewModel : ViewModelBase, IAnimeData, IAnimeListItem
    {
        static AnimeItemViewModel()
        {
            UpdateScoreFlyoutChoices();
        }

        public const string InvalidStartEndDate = "0000-00-00";
        public static double MaxWidth { get; set; }
        public static List<string> ScoreFlyoutChoices { get; set; }

        public AnimeItemAbstraction ParentAbstraction { get; }
        private bool _seasonalState;

        public int Id { get; private set; }
        public string ImgUrl { get; private set; }

        public bool AllowDetailsNavigation { private get; set; } = true; //Disabled when draggig grid item
        
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
            AllowItemManipulation = auth;
            //Assign properties
            Title = name;
            ShowMoreVisibility = false;
            //We are not seasonal so it's already on list            
            AddToListVisibility = false;
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
            AllowItemManipulation = false;
            Title = data.Title;
            MyScore = 0;
            MyStatus = AnimeStatus.AllOrAiring;
            GlobalScore = data.Score;
            int.TryParse(data.Episodes, out _allEpisodes);
            Airing = ParentAbstraction.AirDay >= 0;
            SetAuthStatus(false, true);
            AdjustIncrementButtonsVisibility();
            ShowMoreVisibility = false;
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

        public bool IsRewatching
        {
            get { return ParentAbstraction.IsRewatching; }
            set
            {
                ParentAbstraction.IsRewatching = value;
                RaisePropertyChanged(() => MyStatusBind);
                RaisePropertyChanged(() => MyStatusBindShort);
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
                    : Utils.Utilities.DayToString((DayOfWeek) (ParentAbstraction.AirDay - 1));

        private bool _airing;

        private bool? _airDayBrush;
        public bool? AirDayBrush
        {
            get
            {
                if (_airDayBrush != null)
                    return _airDayBrush.Value;

                if (ParentAbstraction.AirStartDate != null && !ParentAbstraction.AirStartDate.Contains("00"))
                {
                    var diff = DateTimeOffset.Parse(ParentAbstraction.AirStartDate).Subtract(DateTimeOffset.Now);
                    if (diff.TotalSeconds > 0)
                    {
                        _airDayBrush = true;
                        _airDayTillBind = diff.TotalDays < 1
                            ? _airDayTillBind = diff.TotalHours.ToString("N0") + "h"
                            : diff.TotalDays.ToString("N0") + "d";
                        RaisePropertyChanged(() => AirDayTillBind);
                    }
                    else
                        _airDayBrush = false;
                }
                else
                    _airDayBrush = false;

                return _airDayBrush;
            }
        }

        private AnimeItemDisplayContext _animeItemDisplayContext;

        public AnimeItemDisplayContext AnimeItemDisplayContext
        {
            get { return _animeItemDisplayContext; }
            set
            {
                if(value == _animeItemDisplayContext)
                    return;

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

        private bool? _isFavouriteVisibility;

        public bool IsFavouriteVisibility
        {
            get
            {
                return Settings.SelectedApiType != ApiType.Hummingbird && (bool)(_isFavouriteVisibility ??
                                                                                 (_isFavouriteVisibility =
                                                                                     FavouritesManager.IsFavourite(
                                                                                         ParentAbstraction.RepresentsAnime ? FavouriteType.Anime : FavouriteType.Manga,
                                                                                         Id.ToString())));
            }
            set
            {
                _isFavouriteVisibility = value;
                RaisePropertyChanged(() => IsFavouriteVisibility);
            }
        }


        public bool TitleMargin
            => string.IsNullOrEmpty(TopLeftInfoBind);


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
            (!Settings.DisplaySeasonWithType || string.IsNullOrEmpty(ParentAbstraction.AirStartDate)
                ? ""
                : Utils.Utilities.SeasonToCapitalLetterWithYear(ParentAbstraction.AirStartDate) + " ")  + 
            (ParentAbstraction.Type == 0
                ? ""
                : ParentAbstraction.RepresentsAnime
                    ? ((AnimeType) ParentAbstraction.Type).ToString()
                    : ((MangaType) ParentAbstraction.Type).ToString());

        public string PureType
            => ParentAbstraction.Type == 0
                ? ""
                : ParentAbstraction.RepresentsAnime
                    ? ((AnimeType) ParentAbstraction.Type).ToString()
                    : ((MangaType) ParentAbstraction.Type).ToString();


        public string MyStatusBind => Utils.Utilities.StatusToString((int)MyStatus, !ParentAbstraction.RepresentsAnime,ParentAbstraction.IsRewatching);
        public string MyStatusBindShort => Utils.Utilities.StatusToShortString((int)MyStatus, !ParentAbstraction.RepresentsAnime,ParentAbstraction.IsRewatching);

        public AnimeStatus MyStatus
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
                        $"{(AllEpisodesFocused == 0 ? "?" : AllEpisodesFocused.ToString())} {(ParentAbstraction.RepresentsAnime ? "Episodes" : "Volumes")}";

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

        public string GlobalScoreBind
            => ParentAbstraction.LoadedVolatile && GlobalScore != 0 ? GlobalScore == 0 ? "N/A" : GlobalScore.ToString("N2") : "";

        public float GlobalScore
        {
            get { return ParentAbstraction.GlobalScore; }
            set
            {
                if (value == 0)
                    return;
                ParentAbstraction.GlobalScore = value;
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

        public bool TagsControlVisibility => !string.IsNullOrEmpty(Notes);

        private bool _addToListVisibility;

        public bool AddToListVisibility
        {
            get { return Settings.SelectedApiType == ApiType.Mal && _addToListVisibility; }
            set
            {
                _addToListVisibility = value;
                RaisePropertyChanged(() => AddToListVisibility);
            }
        }

        private bool _incrementEpsVisibility;

        public bool IncrementEpsVisibility
        {
            get { return _incrementEpsVisibility; }
            set
            {
                _incrementEpsVisibility = value;
                RaisePropertyChanged(() => IncrementEpsVisibility);
            }
        }

        private bool _decrementEpsVisibility;

        public bool DecrementEpsVisibility
        {
            get { return _decrementEpsVisibility; }
            set
            {
                _decrementEpsVisibility = value;
                RaisePropertyChanged(() => DecrementEpsVisibility);
            }
        }

        private bool _showMoreVisiblity;

        public bool ShowMoreVisibility
        {
            get { return _showMoreVisiblity; }
            set
            {
                _showMoreVisiblity = value;
                RaisePropertyChanged(() => ShowMoreVisibility);
            }
        }

        private bool _updateButtonsVisibility = true;

        public bool UpdateButtonsVisibility
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

        private bool _watchedEpsInputNoticeVisibility = false;

        public bool WatchedEpsInputNoticeVisibility
        {
            get { return _watchedEpsInputNoticeVisibility; }
            set
            {
                _watchedEpsInputNoticeVisibility = value;
                RaisePropertyChanged(() => WatchedEpsInputNoticeVisibility);
            }
        }

        private bool _allowItemManipulation;

        public bool AllowItemManipulation
        {
            get { return _allowItemManipulation; }
            set
            {
                _allowItemManipulation = value;
                RaisePropertyChanged(() => AllowItemManipulation);
            }
        }

        private bool _loadingUpdate = false;

        public bool LoadingUpdate
        {
            get { return _loadingUpdate; }
            set
            {
                _loadingUpdate = value;
                RaisePropertyChanged(() => LoadingUpdate);
            }
        }

        private ICommand _onFlyoutEpsKeyDown;

        public ICommand OnFlyoutEpsKeyDown => _onFlyoutEpsKeyDown ?? (_onFlyoutEpsKeyDown = new RelayCommand(ChangeWatchedEps));

        private ICommand _changeStatusCommand;

        public ICommand ChangeStatusCommand => _changeStatusCommand ?? (_changeStatusCommand = new RelayCommand<string>(ChangeStatus));

        private ICommand _changeScoreCommand;

        public ICommand ChangeScoreCommand => _changeScoreCommand ?? (_changeScoreCommand = new RelayCommand<object>(ChangeScore));

        private ICommand _changeWatchedCommand;

        public ICommand ChangeWatchedCommand => _changeWatchedCommand ?? (_changeWatchedCommand = new RelayCommand(ChangeWatchedEps));

        private ICommand _incrementWatchedCommand;

        public ICommand IncrementWatchedCommand => _incrementWatchedCommand ?? (_incrementWatchedCommand = new RelayCommand(IncrementWatchedEp));

        private ICommand _decrementWatchedCommand;

        public ICommand DecrementWatchedCommand => _decrementWatchedCommand ?? (_decrementWatchedCommand = new RelayCommand(DecrementWatchedEp));

        private ICommand _addAnimeCommand;

        public ICommand AddAnimeCommand => _addAnimeCommand ?? (_addAnimeCommand = new RelayCommand(AddThisToMyList));

        private ICommand _pinTileCustomCommand;

        public ICommand PinTileCustomCommand
        {
            get
            {
                return _pinTileCustomCommand ??
                       (_pinTileCustomCommand =
                           new RelayCommand(() =>
                           {
                               try
                               {
                                   SimpleIoc.Default.GetInstance<IPinTileService>().Load(this);
                               }
                               catch (Exception)
                               {
                                   //not windows
                               }
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
                           if (Settings.SelectedApiType == ApiType.Mal)
                           {
                               ResourceLocator.ClipboardProvider.SetText(
                                   $"http://www.myanimelist.net/{(ParentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}");
                           }
                           else
                           {
                               ResourceLocator.ClipboardProvider.SetText(
                                   $"https://hummingbird.me/{(ParentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}");
                           }                          
                       }));
            }
        }

        private ICommand _copyTitleToClipboardCommand;

        public ICommand CopyTitleToClipboardCommand
        {
            get
            {
                return _copyTitleToClipboardCommand ??
                       (_copyTitleToClipboardCommand = new RelayCommand(() =>
                       {
                           ResourceLocator.ClipboardProvider.SetText(Title);
                       }));
            }
        }

        private ICommand _openInMALCommand;

        public ICommand OpenInMALCommand
        {
            get
            {
                return _openInMALCommand ??
                       (_openInMALCommand = new RelayCommand(() =>
                       {
                           if (Settings.SelectedApiType == ApiType.Mal)
                           {
                               ResourceLocator.SystemControlsLauncherService.LaunchUri(
                                       new Uri(
                                           $"https://myanimelist.net/{(ParentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}"));
                           }
                           else
                           {
                               ResourceLocator.SystemControlsLauncherService.LaunchUri(
                                   new Uri(
                                       $"https://hummingbird.me/{(ParentAbstraction.RepresentsAnime ? "anime" : "manga")}/{Id}"));
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

        public string TimeTillNextAirCache { get; set; }
        public string GetTimeTillNextAir(TimeZoneInfo zoneInfo)
        {

            if (ParentAbstraction.ExactAiringTime != null && Airing)
            {
                if (ParentAbstraction.AirStartDate == InvalidStartEndDate)
                    return "";
                
                DateTime jst = TimeZoneInfo.ConvertTime(DateTime.Now, zoneInfo);
                DateTime jstTarget = TimeZoneInfo.ConvertTime(DateTime.Today, zoneInfo);
                var time = ParentAbstraction.ExactAiringTime;
                var dayDiff = (7 +((int)time.DayOfWeek - (int)jst.DayOfWeek)) % 7;
                jstTarget = jstTarget.AddDays(dayDiff);
                jstTarget = jstTarget.Add(time.Time);

                //make sure that it's after start date
                DateTime date;
                if (!DateTime.TryParse(ParentAbstraction.AirStartDate, out date))
                    return "";
                if (jstTarget < date)
                    return "";

                var diff = jstTarget - jst;

                if (diff.TotalDays < 0) //TODO : Find Reason
                    return "";

                if (diff.TotalDays > 1)
                    return $"{diff.Days}d {diff.Hours}h {diff.Minutes}m";
                return $"{diff.Hours}h {diff.Minutes}m";
            }
            return "";

        }

        #endregion

        #region Utils/Helpers

        //Pinned with custom link

        public void SetAuthStatus(bool auth, bool eps = false)
        {
            Auth = auth;
            if (auth)
            {
                AddToListVisibility = false;
                UpdateButtonsVisibility = true;
                UpdateButtonsEnableState = true;
            }
            else
            {
                AddToListVisibility = _seasonalState && Credentials.Authenticated
                    ? true
                    : false;
                UpdateButtonsEnableState = false;

                if (eps)
                {
                    RaisePropertyChanged(() => MyEpisodesBind);
                    UpdateButtonsVisibility = false;
                }
            }
            AdjustIncrementButtonsVisibility();
        }


        public void UpdateChapterData(int allEpisodes)
        {
            if (Settings.MangaFocusVolumes)
            {
                _allVolumes = allEpisodes;
            }
            else
            {
                _allEpisodes = allEpisodes;
            }
            
        }

        public void AdjustIncrementButtonsVisibility()
        {
            if (!Auth || !Credentials.Authenticated)
            {
                IncrementEpsVisibility = false;
                DecrementEpsVisibility = false;
                return;
            }

            if (MyEpisodes == _allEpisodes && _allEpisodes != 0)
            {
                IncrementEpsVisibility = false;
                DecrementEpsVisibility = true;
            }
            else if (MyEpisodes == 0)
            {
                IncrementEpsVisibility = true;
                DecrementEpsVisibility = false;
            }
            else
            {
                IncrementEpsVisibility = true;
                DecrementEpsVisibility = true;
            }
        }

        public void UpdateVolatileDataBindings()
        {
            RaisePropertyChanged(() => TopLeftInfoBind);
            RaisePropertyChanged(() => GlobalScoreBind);
            RaisePropertyChanged(() => Type);
            RaisePropertyChanged(() => PureType);
        }

        public async void NavigateDetails(PageIndex? sourceOverride = null, object argsOverride = null)
        {
            if (!AllowDetailsNavigation || (Settings.SelectedApiType == ApiType.Hummingbird && !ParentAbstraction.RepresentsAnime) || ViewModelLocator.AnimeDetails.Id == Id)
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
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails, navArgs);
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

        private bool _incrementing;
        private bool _decrementing;
        private async void IncrementWatchedEp()
        {
            if(_incrementing || IncrementEpsVisibility == false || (AllEpisodesFocused != 0 && MyEpisodesFocused == AllEpisodesFocused))
                return;
            _incrementing = true;
            LoadingUpdate = true;
            var trigCompleted = true;
            if (MyStatus ==  AnimeStatus.PlanToWatch || MyStatus == AnimeStatus.Dropped ||
                MyStatus ==  AnimeStatus.OnHold)
            {
                if (MyEpisodesFocused+1 != AllEpisodesFocused || AllEpisodesFocused == 0) //avoid double status change when show is going to be completed immediatelly
                {
                    trigCompleted = AllEpisodes > 1;
                    PromptForStatusChange(AllEpisodes == 1 ? AnimeStatus.Completed : AnimeStatus.Watching);
                }              
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

            if (trigCompleted && MyEpisodesFocused == AllEpisodesFocused && AllEpisodesFocused != 0)
                PromptForStatusChange( AnimeStatus.Completed);

            LoadingUpdate = false;
            _incrementing = false;
        }

        private async void DecrementWatchedEp()
        {
            if (_decrementing || DecrementEpsVisibility == false || MyEpisodesFocused == 0)
                return;
            _decrementing = true;
            LoadingUpdate = true;
            MyEpisodesFocused--;
            AdjustIncrementButtonsVisibility();
            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
            {
                MyEpisodesFocused++;
                AdjustIncrementButtonsVisibility();
            }

            _decrementing = false;
            LoadingUpdate = false;
        }

        public async void ChangeWatchedEps()
        {
            int watched;
            if (!int.TryParse(WatchedEpsInput, out watched))
            {
                WatchedEpsInputNoticeVisibility = true;
                return;
            }
            if (watched >= 0 && (_allEpisodes == 0 || watched <= _allEpisodes))
            {
                LoadingUpdate = true;
                WatchedEpsInputNoticeVisibility = false;
                var prevWatched = MyEpisodesFocused;

                var trigCompleted = true;
                if (MyStatus == AnimeStatus.PlanToWatch || MyStatus == AnimeStatus.Dropped ||
                    MyStatus == AnimeStatus.OnHold)
                {
                    if (MyEpisodesFocused + watched != AllEpisodesFocused || AllEpisodesFocused == 0) //avoid double status change when show is going to be completed immediatelly
                    {
                        trigCompleted = AllEpisodes > 1;
                        PromptForStatusChange(AllEpisodes == 1 ? AnimeStatus.Completed : AnimeStatus.Watching);
                    }
                }

                MyEpisodesFocused = watched;
                var response = await GetAppropriateUpdateQuery().GetRequestResponse();
                if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                    MyEpisodesFocused = prevWatched;

                AdjustIncrementButtonsVisibility();
                ParentAbstraction.LastWatched = DateTime.Now;

                if (trigCompleted && MyEpisodesFocused == AllEpisodesFocused && AllEpisodesFocused != 0)
                    PromptForStatusChange(AnimeStatus.Completed);

                LoadingUpdate = false;
                WatchedEpsInput = "";
            }
            else
            {
                WatchedEpsInputNoticeVisibility = true;
            }
        }

        #endregion

        private void ChangeStatus(string status)
        {
            ChangeStatus((AnimeStatus)Utils.Utilities.StatusToInt(status));
        }

        public async void ChangeStatus(AnimeStatus status)
        {
            LoadingUpdate = true;
            var myPrevStatus = MyStatus;
            MyStatus = status;
            var stat =  status;
            if (Settings.SetStartDateOnWatching && stat == AnimeStatus.Watching &&
                (Settings.OverrideValidStartEndDate || ParentAbstraction.MyStartDate == AnimeItemViewModel.InvalidStartEndDate))
                StartDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
            else if (Settings.SetEndDateOnDropped && stat == AnimeStatus.Dropped &&
                     (Settings.OverrideValidStartEndDate || ParentAbstraction.MyEndDate == AnimeItemViewModel.InvalidStartEndDate))
                EndDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
            else if (Settings.SetEndDateOnCompleted &&  stat == AnimeStatus.Completed &&
                     (Settings.OverrideValidStartEndDate || ParentAbstraction.MyEndDate == AnimeItemViewModel.InvalidStartEndDate))
                EndDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");

            //in case of series having one episode
            if(AllEpisodes == 1 && myPrevStatus == AnimeStatus.PlanToWatch && stat == AnimeStatus.Completed)
                if(Settings.SetStartDateOnWatching && (Settings.OverrideValidStartEndDate || ParentAbstraction.MyStartDate == "0000-00-00"))
                    StartDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");

            if (IsRewatching)
            {
                if (AllEpisodes != 0 && MyStatus == AnimeStatus.Completed)
                    MyEpisodes = AllEpisodes;
                IsRewatching = false;
            }

            ViewModelLocator.AnimeDetails.UpdateAnimeReferenceUiBindings(Id);

            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                MyStatus = myPrevStatus;

            if (MyStatus == AnimeStatus.Completed && _allEpisodes != 0)
                 PromptForWatchedEpsChange(_allEpisodes);

            LoadingUpdate = false;
        }

        public async void ChangeScore(object score)
        {
            LoadingUpdate = true;
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

            LoadingUpdate = false;
        }

        private async void AddThisToMyList()
        {
            LoadingUpdate = true;
            var response =
                ParentAbstraction.RepresentsAnime
                    ? await new AnimeAddQuery(Id.ToString()).GetRequestResponse()
                    : await new MangaAddQuery(Id.ToString()).GetRequestResponse();
            LoadingUpdate = false;
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
                   MyStatus = Settings.DefaultStatusAfterAdding,
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
                   MyStatus = Settings.DefaultStatusAfterAdding,
                   MyEpisodes = 0,
                   MyScore = 0,
                   MyStartDate = startDate,
                   MyEndDate = AnimeItemViewModel.InvalidStartEndDate,
                   AllVolumes = AllVolumes,
                   MyVolumes = MyVolumes
               };
            ParentAbstraction.EntryData = animeItem;
            _seasonalState = false;
            
            MyScore = 0;
            MyStatus = Settings.DefaultStatusAfterAdding;
            MyEpisodes = 0;
            if (Settings.SetStartDateOnListAdd)
                ParentAbstraction.MyStartDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
            if (ParentAbstraction.RepresentsAnime)
                MyVolumes = 0;

            AllowItemManipulation = true;
            AddToListVisibility = false;
            ResourceLocator.AnimeLibraryDataStorage.AddAnimeEntry(ParentAbstraction);
            await Task.Delay(10);
            RaisePropertyChanged(() => MyStatusBindShort);
            RaisePropertyChanged(() => MyStatusBind);
            UpdateButtonsVisibility = true;
            SetAuthStatus(true);
            if (ViewModelLocator.AnimeDetails.Id == Id)
                ViewModelLocator.AnimeDetails.CurrentAnimeHasBeenAddedToList(this);
        }

        public void UpdateWithSeasonData(SeasonalAnimeData data, bool updateScore)
        {
            if (updateScore)
                GlobalScore = data.Score;
            Airing = data.AirDay >= 0;
            if (!Auth)
            {
                UpdateButtonsVisibility = false;
                _seasonalState = true;
            }
            RaisePropertyChanged(() => MyEpisodesBind);
        }
        #endregion

        #region Prompts

        public void PromptForStatusChange(AnimeStatus to)
        {
            try
            {
                if (MyStatus != AnimeStatus.Completed || !IsRewatching)
                    if (MyStatus == to)
                            return;
                if (!Settings.StatusPromptEnable)
                {
                    if(!Settings.StatusPromptProceedOnDisabled)
                        return;

                    ChangeStatus(to);
                    return;
                }
                ResourceLocator.MessageDialogProvider.ShowMessageDialogWithInput(
                        $"From : {Utils.Utilities.StatusToString((int)MyStatus, !ParentAbstraction.RepresentsAnime, IsRewatching)}\nTo : {Utils.Utilities.StatusToString((int)to,!ParentAbstraction.RepresentsAnime)}",
                        "Would you like to change current status?","Yes","No", async () =>
                        {
                            if (to == AnimeStatus.Completed && IsRewatching)
                            {
                                IsRewatching = false;
                                if (AllEpisodes != 0)
                                {
                                    MyEpisodes = AllEpisodes;
                                }
                                await GetAppropriateUpdateQuery().GetRequestResponse();
                                AdjustIncrementButtonsVisibility();
                                ViewModelLocator.AnimeDetails.UpdateAnimeReferenceUiBindings(Id);
                            }
                            else
                                ChangeStatus(to);
                        });
            }
            catch (Exception)
            {
                //TODO access denied excpetion? we can try that 
            }

        }

        public void PromptForWatchedEpsChange(int to)
        {
            try
            {
                if (MyEpisodesFocused == to)
                    return;
                Action updateAction = async () =>
                {
                    var myPrevEps = MyEpisodesFocused;
                    MyEpisodesFocused = to;
                    var response = await GetAppropriateUpdateQuery().GetRequestResponse();
                    if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                        MyEpisodesFocused = myPrevEps;

                    AdjustIncrementButtonsVisibility();
                };
                if (!Settings.WatchedEpsPromptEnable)
                {
                    if (!Settings.WatchedEpsPromptProceedOnDisabled)
                        return;

                    updateAction.Invoke();
                    return;
                }
                ResourceLocator.MessageDialogProvider.ShowMessageDialogWithInput($"From : {MyEpisodesFocused}\nTo : {to}",
                    $"Would you like to change {(ParentAbstraction.RepresentsAnime ? "watched episodes" : $"{"read " +(Settings.MangaFocusVolumes ? "volumes" : "chapters")}") } value?", "Yes", "No", updateAction);
            }
            catch (Exception)
            {
                //TODO access denied excpetion? we can try that 
            }

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

    }
}