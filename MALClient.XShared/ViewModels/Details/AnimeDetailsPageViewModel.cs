using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.Models.Interfaces;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.Models.Models.Favourites;
using MALClient.Models.Models.Library;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Comm.Manga;
using MALClient.XShared.Delegates;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;
using VideoLibrary;

namespace MALClient.XShared.ViewModels.Details
{
    public partial class AnimeDetailsPageViewModel : ViewModelBase
    {
        private readonly IClipboardProvider _clipboardProvider;
        private readonly ISystemControlsLauncherService _systemControlsLauncherService;
        private readonly IAnimeLibraryDataStorage _animeLibraryDataStorage;

        private readonly IAiringNotificationsAdapter _airingNotificationsAdapter;

        //additional fields
        private int _allEpisodes;
        private int _allVolumes;
        private string _alternateImgUrl;
        private IAnimeData _animeItemReference; //our connection with everything
        public IAnimeData AnimeItemReference => _animeItemReference;

        public bool AnimeMode
        {
            get { return _animeMode; }
            set
            {
                _animeMode = value;
                RaisePropertyChanged(() => RewatchedLabel);
                RaisePropertyChanged(() => RewatchingLabel);
                RaisePropertyChanged(() => AnimeMode);
            }
        }

        private AnimeStaffDataViewModels _animeStaffData;
        private float _globalScore;

        private int _id;
        //crucial fields
        private string _imgUrl;
        public bool _initialized;


        //loaded fields
        private bool _loadedDetails;
        private bool _loadedRecomm;
        private bool _loadedRelated;
        private bool _loadedReviews;
        private bool _loadedVideos;

        private bool _loadingAlternate;

        private List<FavouriteViewModel> _mangaCharacterData;

        public AnimeDetailsPageNavigationArgs PrevArgs { get; private set; }
        private List<string> _synonyms = new List<string>(); //used to increase ann's search reliability
        private bool _animeMode;

        public AnimeDetailsPageViewModel(IClipboardProvider clipboardProvider,
            ISystemControlsLauncherService systemControlsLauncherService, IAnimeLibraryDataStorage animeLibraryDataStorage, IAiringNotificationsAdapter airingNotificationsAdapter)
        {
            _clipboardProvider = clipboardProvider;
            _systemControlsLauncherService = systemControlsLauncherService;
            _animeLibraryDataStorage = animeLibraryDataStorage;
            _airingNotificationsAdapter = airingNotificationsAdapter;
            UpdateScoreFlyoutChoices();
        }

        public bool Initialized
        {
            get { return _initialized; }
            private set
            {
                _initialized = value;
                //OnInitialized?.Invoke(null, null);
            }
        }

        public string Title { get; set; }
        private string Type { get; set; }
        private string Status { get; set; }
        //Dates when show starts or ends airing
        private string StartDate { get; set; }
        private string EndDate { get; set; }
        //Dates set by the user
        public string MyStartDate
            =>
            (_animeItemReference?.StartDate ?? "0000-00-00") == "0000-00-00"
                ? "Not set"
                : _animeItemReference?.StartDate;

        public string MyEndDate
            => (_animeItemReference?.EndDate ?? "0000-00-00") == "0000-00-00" ? "Not set" : _animeItemReference?.EndDate
            ;

        public AnimeStaffDataViewModels AnimeStaffData
        {
            get { return _animeStaffData; }
            set
            {
                _animeStaffData = value;
                RaisePropertyChanged(() => AnimeStaffData);
            }
        }

        /// <summary>
        /// A bit of magic... wrapping magic
        /// </summary>
        public class AnimeStaffDataViewModels
        {
            public List<AnimeCharacterStaffModelViewModel> AnimeCharacterPairs { get; set; }
            public List<FavouriteViewModel> AnimeStaff { get; set; }

            public class AnimeCharacterStaffModelViewModel
            {
                public FavouriteViewModel AnimeCharacter { get; set; }
                public FavouriteViewModel AnimeStaffPerson { get; set; }

                public AnimeCharacterStaffModelViewModel(AnimeCharacterStaffModel data)
                {
                    AnimeCharacter = new FavouriteViewModel(data.AnimeCharacter);
                    AnimeStaffPerson = new FavouriteViewModel(data.AnimeStaffPerson);
                }
            }

            public AnimeStaffDataViewModels(AnimeStaffData data)
            {
                AnimeCharacterPairs =
                    data.AnimeCharacterPairs.Select(pair => new AnimeCharacterStaffModelViewModel(pair)).ToList();
                AnimeStaff = data.AnimeStaff.Select(person => new FavouriteViewModel(person)).ToList();
            }

        }

        public List<FavouriteViewModel> MangaCharacterData
        {
            get { return _mangaCharacterData; }
            set
            {
                _mangaCharacterData = value;
                RaisePropertyChanged(() => MangaCharacterData);
            }
        }

        public ObservableCollection<AnimeReviewData> Reviews { get; } = new ObservableCollection<AnimeReviewData>();

        public SmartObservableCollection<DirectRecommendationData> Recommendations { get; } =
            new SmartObservableCollection<DirectRecommendationData>();

        public ObservableCollection<RelatedAnimeData> RelatedAnime { get; } =
            new ObservableCollection<RelatedAnimeData>();

        public List<Tuple<string, string>> LeftDetailsRow { get; set; } =
            new List<Tuple<string, string>>();

        public List<Tuple<string, string>> RightDetailsRow { get; set; } =
            new List<Tuple<string, string>>();

        public ObservableCollection<string> LeftGenres { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> RightGenres { get; } = new ObservableCollection<string>();

        public ObservableCollection<Tuple<string, string>> Information { get; } =
            new ObservableCollection<Tuple<string, string>>();

        public ObservableCollection<Tuple<string, string>> Stats { get; } =
            new ObservableCollection<Tuple<string, string>>();

        public ObservableCollection<string> OPs { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> EDs { get; } = new ObservableCollection<string>();

        public ObservableCollection<AnimeVideoData> AvailableVideos { get;  } = new ObservableCollection<AnimeVideoData>();


        public static List<string> ScoreFlyoutChoices { get; set; }


        private string SourceLink { get; set; }

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                if (value <= 0)
                    PrevArgs = null;
            }
        }

        public int MalId { get; set; }

        public int AllEpisodes
        {
            get { return _animeItemReference?.AllEpisodes ?? _allEpisodes; }
            set { _allEpisodes = value; }
        }

        private int AllVolumes
        {
            get { return _animeItemReference?.AllVolumes ?? _allVolumes; }
            set { _allVolumes = value; }
        }

        public DirectRecommendationData CurrentRecommendationsSelectedItem { get; set; }

        public async void Init(AnimeDetailsPageNavigationArgs param,bool fakeDelay = true)
        {
            Initialized = false;
            LoadingGlobal = false;
            //wait for UI
            if(fakeDelay)
                await Task.Delay(5);
            ViewModelLocator.GeneralMain.IsCurrentStatusSelectable = true;

            _loadingAlternate = false;

            //details reset
            _loadedDetails = _loadedReviews = _loadedRecomm = _loadedRelated = _loadedVideos = false;

            //basic init assignment
            _animeItemReference = param.AnimeItem;
            AnimeMode = param.AnimeMode;
            Id = param.Id;
            Title = param.Title;
            if (Settings.SelectedApiType == ApiType.Mal)
                MalId = Id;
            else
                MalId = -1; //we will find this thing later

            //is manga stuff visibile
            if (AnimeMode)
            {
                MyVolumesVisibility = false;
                HiddenPivotItemIndex = -1;
            }
            else
            {
                MyVolumesVisibility = true;
                HiddenPivotItemIndex = 1;
            }
            //Add/Rem
            IsRemoveAnimeButtonEnabled = false;
            IsAddAnimeButtonEnabled = false;
            //favs
            IsFavourite = FavouritesManager.IsFavourite(AnimeMode ? FavouriteType.Anime : FavouriteType.Manga,
                Id.ToString());
            //staff
            CharactersGridVisibility = MangaCharacterGridVisibility = false;
            LoadCharactersButtonVisibility = true;
            AnimeStaffData = null;
            MangaCharacterData = null;
            //so there will be no floting start/end dates
            MyDetailsVisibility = false;
            StartDateValid = false;
            EndDateValid = false;
            _alternateImgUrl = null;

            if (AnimeMode)
            {
                Status1Label = "Watching";
                Status5Label = "Plan to watch";
                WatchedEpsLabel = "Watched\nepisodes";
                UpdateEpsUpperLabel = "Watched\nepisodes";
                if (_animeItemReference is AnimeItemViewModel vm)
                {
                    if (!vm.Auth || !vm.Airing || vm.AllEpisodes <= 0)
                    {
                        AiringNotificationsButtonVisibility = AreAirNotificationsEnabled = false;
                    }
                    else
                    {
                        AiringNotificationsButtonVisibility = true;
                        AreAirNotificationsEnabled = _airingNotificationsAdapter.AreNotificationRegistered(Id.ToString());
                    }
                }
                else
                    AiringNotificationsButtonVisibility = AreAirNotificationsEnabled = false;
            }
            else
            {
                Status1Label = "Reading";
                Status5Label = "Plan to read";
                WatchedEpsLabel = "Read\nchapters";
                UpdateEpsUpperLabel = "Read\nchapters";
                LoadCharactersButtonVisibility = false;
                AiringNotificationsButtonVisibility = false;
            }

            if (_animeItemReference == null || _animeItemReference is AnimeSearchItemViewModel ||
                (_animeItemReference is AnimeItemViewModel && !(_animeItemReference as AnimeItemViewModel).Auth))
                //if we are from search or from unauthenticated item let's look for proper abstraction
            {
                var possibleRef =
                    await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(param.Id, AnimeMode);
                if (possibleRef == null) // else we don't have this item
                {
                    //we may only prepare for its creation
                    AddAnimeVisibility = true;
                    MyDetailsVisibility = false;
                }
                else
                    _animeItemReference = possibleRef;
            } // else we already have it

            if ((_animeItemReference as AnimeItemViewModel)?.Auth ?? false)
            {
                //we have item on the list , so there's valid data here
                MyDetailsVisibility = true;
                AddAnimeVisibility = false;
                IsRemoveAnimeButtonEnabled = true;
                IsAddAnimeButtonEnabled = false;
                PopulateStartEndDates();
                //tags
                if (Settings.SelectedApiType == ApiType.Mal)
                {
                    var tags = string.IsNullOrEmpty(_animeItemReference.Notes)
                        ? new List<string>()
                        : _animeItemReference.Notes.Contains(",")
                            ? _animeItemReference.Notes.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                .ToList()
                            : new List<string> {_animeItemReference.Notes};
                    var collection = new ObservableCollection<string>(tags);
                    MyTags = collection;
                }
            }
            else
            {
                IsRemoveAnimeButtonEnabled = false;
                IsAddAnimeButtonEnabled = true;
                MyTags = new ObservableCollection<string>();
            }

            switch (param.Source)
            {
                case PageIndex.PageSearch:
                case PageIndex.PageMangaSearch:
                    ExtractData(param.AnimeElement);
                    if (PrevArgs != null)
                        ViewModelLocator.NavMgr.RegisterBackNav(PrevArgs);
                    ViewModelLocator.NavMgr.RegisterBackNav(param.Source, param.PrevPageSetup);
                    break;
                case PageIndex.PageAnimeList:
                case PageIndex.PageMangaList:
                case PageIndex.PageProfile:
                case PageIndex.PageHistory:
                case PageIndex.PageArticles:
                case PageIndex.PageForumIndex:
                case PageIndex.PageStaffDetails:
                case PageIndex.PageCharacterDetails:
                case PageIndex.PageCalendar:
                case PageIndex.PagePopularVideos:
                case PageIndex.PageListComparison:
                case PageIndex.PageClubDetails:
                    await FetchData(false, param.Source);
                    if (PrevArgs != null)
                        ViewModelLocator.NavMgr.RegisterBackNav(PrevArgs);
                    if (ViewModelLocator.Mobile || (!ViewModelLocator.Mobile && param.Source != PageIndex.PageProfile && param.Source != PageIndex.PageClubDetails))
                        ViewModelLocator.NavMgr.RegisterBackNav(param.Source, param.PrevPageSetup);
                    break;
                case PageIndex.PageAnimeDetails:
                    await FetchData();
                    if (param.RegisterBackNav) //we are already going back
                    {
                        ViewModelLocator.NavMgr.RegisterBackNav(param.PrevPageSetup as AnimeDetailsPageNavigationArgs);
                    }
                    break;
                case PageIndex.PageRecomendations:
                    if (param.AnimeElement != null)
                    {
                        ExtractData(param.AnimeElement);
                    }
                    else
                    {
                        await FetchData();
                    }

                    if (PrevArgs != null)
                        ViewModelLocator.NavMgr.RegisterBackNav(PrevArgs);
                    ViewModelLocator.NavMgr.RegisterBackNav(param.Source, param.PrevPageSetup);
                    break;
                case PageIndex.PageNotificationHub:
                case PageIndex.PageFeeds:
                    if (PrevArgs != null)
                        ViewModelLocator.NavMgr.RegisterBackNav(PrevArgs);
                    await FetchData(false, param.Source);
                    break;
            }

            PrevArgs = param;
            PrevArgs.RegisterBackNav = false;
            PrevArgs.Source = PageIndex.PageAnimeDetails;
            Initialized = true;
            DetailsPivotSelectedIndex = param.SourceTabIndex;
        }

        private void OpenMalPage()
        {
            if (Settings.SelectedApiType == ApiType.Mal)
            {
                _systemControlsLauncherService.LaunchUri(
                    new Uri($"https://myanimelist.net/{(AnimeMode ? "anime" : "manga")}/{Id}"));
            }
            else
            {
                _systemControlsLauncherService.LaunchUri(
                    new Uri($"https://hummingbird.me/{(AnimeMode ? "anime" : "manga")}/{Id}"));
            }
        }

        private async void NavigateDetails(IDetailsPageArgs args)
        {
            if (Settings.SelectedApiType == ApiType.Hummingbird)
                //recoms and review have mal id so we have to walk around thid
            {
                args.Id = await new AnimeDetailsHummingbirdQuery(args.Id).GetHummingbirdId();
            }
            ViewModelLocator.GeneralMain
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(args.Id, args.Title, null, null,
                            new AnimeDetailsPageNavigationArgs(Id, Title, null, _animeItemReference)
                            {
                                Source = PageIndex.PageAnimeDetails,
                                RegisterBackNav = false,
                                AnimeMode = AnimeMode,
                                SourceTabIndex = DetailsPivotSelectedIndex
                            })
                        {Source = PageIndex.PageAnimeDetails, AnimeMode = args.Type == RelatedItemType.Anime});
        }

        /// <summary>
        ///     Launches update of all UI bound variables.
        /// </summary>
        /// <param name="callerId">Anime item id that calls this thing.</param>
        public void UpdateAnimeReferenceUiBindings(int callerId)
        {
            if (callerId != Id)
                return;

            RaisePropertyChanged(() => StartDateTimeOffset);
            RaisePropertyChanged(() => EndDateTimeOffset);
            RaisePropertyChanged(() => MyEpisodesBind);
            RaisePropertyChanged(() => MyVolumesBind);
            RaisePropertyChanged(() => MyStatusBind);
            RaisePropertyChanged(() => MyScoreBind);
            RaisePropertyChanged(() => MyStartDate);
            RaisePropertyChanged(() => MyEndDate);
            RaisePropertyChanged(() => IncrementEpsCommand);
            RaisePropertyChanged(() => DecrementEpsCommand);
            RaisePropertyChanged(() => IsIncrementButtonEnabled);
            RaisePropertyChanged(() => IsDecrementButtonEnabled);
            RaisePropertyChanged(() => IsRewatching);
            RaisePropertyChanged(() => IsRewatchingButtonVisibility);
        }


        public void UpdateScoreFlyoutChoices()
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
                    "1 - Appalling"
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
                    "0.5 - Appalling"
                };
        }



        #region ChangeStuff

        #region IncrementDecrementRelay

        public bool IsIncrementButtonEnabled
            => (_animeItemReference as AnimeItemViewModel)?.IncrementEpsVisibility == true;

        public bool IsDecrementButtonEnabled
            => (_animeItemReference as AnimeItemViewModel)?.DecrementEpsVisibility == true;

        public ICommand IncrementEpsCommand => new RelayCommand(() =>
        {
            (_animeItemReference as AnimeItemViewModel)?.IncrementWatchedCommand.Execute(null);
            RaisePropertyChanged(() => IsIncrementButtonEnabled);
            RaisePropertyChanged(() => IsDecrementButtonEnabled);
        });

        public ICommand DecrementEpsCommand => new RelayCommand(() =>
        {
            (_animeItemReference as AnimeItemViewModel)?.DecrementWatchedCommand.Execute(null);
            RaisePropertyChanged(() => IsIncrementButtonEnabled);
            RaisePropertyChanged(() => IsDecrementButtonEnabled);
        });

        #endregion

        private Query GetAppropriateUpdateQuery(int? rewatchCount = null)
        {
            if (rewatchCount == null)
            {
                if (AnimeMode)
                    return new AnimeUpdateQuery(_animeItemReference);
                return new MangaUpdateQuery(_animeItemReference);
            }
            else
            {
                if (AnimeMode)
                    return new AnimeUpdateQuery(_animeItemReference, rewatchCount.Value);
                return new MangaUpdateQuery(_animeItemReference, rewatchCount.Value);
            }
        }

        private async void LaunchUpdate()
        {
            LoadingUpdate = true;
            await GetAppropriateUpdateQuery().GetRequestResponse();
            LoadingUpdate = false;
        }

        public async void ChangeStatus(AnimeStatus status)
        {
            LoadingUpdate = true;
            var prevStatus = MyStatus;
            MyStatus = status;

            if (Settings.SetStartDateOnWatching && MyStatus == AnimeStatus.Watching &&
                (Settings.OverrideValidStartEndDate || !StartDateValid))
            {
                _startDateTimeOffset = DateTimeOffset.Now;
                _animeItemReference.StartDate = DateTimeOffset.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                StartDateValid = true;
                RaisePropertyChanged(() => StartDateTimeOffset);
                RaisePropertyChanged(() => MyStartDate);
            }
            else if (Settings.SetEndDateOnDropped && MyStatus == AnimeStatus.Dropped &&
                     (Settings.OverrideValidStartEndDate || !EndDateValid))
            {
                _endDateTimeOffset = DateTimeOffset.Now;
                _animeItemReference.EndDate = DateTimeOffset.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                EndDateValid = true;
                RaisePropertyChanged(() => EndDateTimeOffset);
                RaisePropertyChanged(() => MyEndDate);
            }
            else if (Settings.SetEndDateOnCompleted && MyStatus == AnimeStatus.Completed &&
                     (Settings.OverrideValidStartEndDate || !EndDateValid))
            {
                if (prevStatus == AnimeStatus.PlanToWatch) //we have just insta completed the series
                {
                    _startDateTimeOffset = DateTimeOffset.Now;
                    _animeItemReference.StartDate =
                        DateTimeOffset.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    StartDateValid = true;
                    RaisePropertyChanged(() => StartDateTimeOffset);
                    RaisePropertyChanged(() => MyStartDate);
                }
                _endDateTimeOffset = DateTimeOffset.Now;
                _animeItemReference.EndDate = DateTimeOffset.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                EndDateValid = true;
                RaisePropertyChanged(() => EndDateTimeOffset);
                RaisePropertyChanged(() => MyEndDate);
            }

            //in case of series having one episode
            if (AllEpisodes == 1 && prevStatus == AnimeStatus.PlanToWatch && MyStatus == AnimeStatus.Completed)
                if (Settings.SetStartDateOnWatching &&
                    (Settings.OverrideValidStartEndDate || _animeItemReference.StartDate == "0000-00-00"))
                {
                    _startDateTimeOffset = DateTimeOffset.Now;
                    _animeItemReference.StartDate =
                        DateTimeOffset.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    StartDateValid = true;
                    RaisePropertyChanged(() => StartDateTimeOffset);
                    RaisePropertyChanged(() => MyStartDate);
                }

            if (_animeItemReference.IsRewatching)
            {
                if (_animeItemReference.AllEpisodes != 0)
                    MyEpisodes = _animeItemReference.AllEpisodes;
                _animeItemReference.IsRewatching = false;
                RaisePropertyChanged(() => IsRewatching);
                RaisePropertyChanged(() => MyStatusBind);
            }

            if (_animeItemReference.IsRewatching)
            {
                if (_animeItemReference.MyStatus == AnimeStatus.Completed && _animeItemReference.AllEpisodes != 0)
                    _animeItemReference.MyEpisodes = _animeItemReference.AllEpisodes;
            }

            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                MyStatus = prevStatus;

            if (_animeItemReference is AnimeItemViewModel vm)
            {
                if (MyStatus == AnimeStatus.Completed && MyEpisodes != AllEpisodes && AllEpisodes != 0)
                {
                    vm.PromptForWatchedEpsChange(AllEpisodes);
                    RaisePropertyChanged(() => MyEpisodesBind);
                }

                if (MyStatus == AnimeStatus.Completed && MyScore == 0 && Settings.DisplayScoreDialogAfterCompletion)
                {
                    vm.PromptForScoreChange();
                }
            }
            LoadingUpdate = false;
        }

        private async void ChangeScore(float score)
        {
            LoadingUpdate = true;
            var prevScore = MyScore;
            if (Settings.SelectedApiType == ApiType.Hummingbird)
            {
                MyScore = score/2;
                if (MyScore == prevScore)
                    MyScore = 0;
            }
            else
            {
                MyScore = score;
            }

            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                MyScore = prevScore;
            LoadingUpdate = false;
        }

        private async void ChangeNotes()
        {
            LoadingUpdate = true;
            await GetAppropriateUpdateQuery().GetRequestResponse();
            LoadingUpdate = false;
        }

        private async void ChangeRewatching(bool state)
        {
            LoadingUpdate = true;
            IsRewatchingButtonEnabled = false;

            if (state)
            {
                _animeItemReference.MyEpisodes = 0;
            }
            else if (_animeItemReference.AllEpisodes != 0)
            {
                _animeItemReference.MyEpisodes = _animeItemReference.AllEpisodes;
            }
            await GetAppropriateUpdateQuery().GetRequestResponse();
            (_animeItemReference as AnimeItemViewModel)?.AdjustIncrementButtonsVisibility();
            RaisePropertyChanged(() => IsIncrementButtonEnabled);
            RaisePropertyChanged(() => IsDecrementButtonEnabled);
            
            IsRewatchingButtonEnabled = true;
            LoadingUpdate = false;
        }

        private async void ChangeRewatchingCount(int count)
        {
            LoadingUpdate = true;
            await GetAppropriateUpdateQuery(count).GetRequestResponse();
            LoadingUpdate = false;
        }

        private async void ChangeWatchedEps() //change from input
        {
            LoadingUpdate = true;
            int eps;
            if (!int.TryParse(WatchedEpsInput, out eps))
            {
                WatchedEpsInputNoticeVisibility = true;
                LoadingUpdate = false;
                return;
            }
            if (eps >= 0 && (AllEpisodes == 0 || eps <= AllEpisodes))
            {
                WatchedEpsInputNoticeVisibility = false;
                var prevEps = MyEpisodes;
                MyEpisodes = eps;
                var response = await GetAppropriateUpdateQuery().GetRequestResponse();
                if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                    MyEpisodes = prevEps;

                var reference = _animeItemReference as AnimeItemViewModel; //avoid multiple casts
                if (reference != null)
                {
                    if (prevEps == 0 && AllEpisodes > 1 && MyEpisodes != AllEpisodes &&
                        (MyStatus == AnimeStatus.PlanToWatch || MyStatus == AnimeStatus.Dropped ||
                         MyStatus == AnimeStatus.OnHold))
                    {
                        reference.PromptForStatusChange(AnimeStatus.Watching);
                        RaisePropertyChanged(() => MyStatusBind);
                    }
                    else if (MyEpisodes == AllEpisodes && AllEpisodes != 0)
                    {

                        reference.PromptForStatusChange(AnimeStatus.Completed);
                        RaisePropertyChanged(() => MyStatusBind);
                    }
                    if (Settings.SelectedApiType == ApiType.Hummingbird)
                        reference.ParentAbstraction.LastWatched = DateTime.Now;
                }
                WatchedEpsInput = "";
            }
            else
            {
                WatchedEpsInputNoticeVisibility = true;
            }

            RaisePropertyChanged(() => IsIncrementButtonEnabled);
            RaisePropertyChanged(() => IsDecrementButtonEnabled);

            LoadingUpdate = false;
        }

        private async void ChangeReadVolumes()
        {
            LoadingUpdate = true;
            int vol;
            if (!int.TryParse(ReadVolumesInput, out vol))
            {
                WatchedEpsInputNoticeVisibility = true;
                LoadingUpdate = false;
                return;
            }
            if (vol >= 0 && (AllVolumes == 0 || vol <= AllVolumes))
            {
                WatchedEpsInputNoticeVisibility = false;
                var prevVol = MyVolumes;
                MyVolumes = vol;
                var response = await GetAppropriateUpdateQuery().GetRequestResponse();
                if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                    MyVolumes = prevVol;

                WatchedEpsInput = "";
            }
            else
            {
                WatchedEpsInputNoticeVisibility = true;
            }
            LoadingUpdate = false;
        }

        #endregion

        #region Add/Remove

        private async void AddAnime()
        {
            LoadingUpdate = true;
            IsAddAnimeButtonEnabled = false;
            var response = AnimeMode
                ? await new AnimeAddQuery(Id.ToString()).GetRequestResponse()
                : await new MangaAddQuery(Id.ToString()).GetRequestResponse();
            LoadingUpdate = false;
            IsAddAnimeButtonEnabled = true;
            if (Settings.SelectedApiType == ApiType.Mal && !response.Contains("Created") && AnimeMode)
                return;
            AddAnimeVisibility = false;
            AnimeType typeA;
            MangaType typeM;
            var type = 0;
            try
            {
                if (AnimeMode)
                {
                    Enum.TryParse(Type, out typeA);
                    type = (int) typeA;
                }
                else
                {
                    Enum.TryParse(Type.Replace("-", ""), out typeM);
                    type = (int) typeM;
                }
            }
            catch (Exception)
            {
                //who knows what MAL has thrown at us...
            }


            var startDate = "0000-00-00";
            if (Settings.SetStartDateOnListAdd)
            {
                startDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
                _startDateTimeOffset = DateTimeOffset.Now; //update without mal-update
                RaisePropertyChanged(() => StartDateTimeOffset);
            }
            var animeItem = AnimeMode
                ? new AnimeItemAbstraction(true, new AnimeLibraryItemData
                {
                    Title = Title,
                    ImgUrl = _imgUrl,
                    Type = type,
                    Id = Id,
                    AllEpisodes = AllEpisodes,
                    MalId = MalId,
                    MyStatus = Settings.DefaultStatusAfterAdding,
                    MyEpisodes = 0,
                    MyScore = 0,
                    MyStartDate = startDate,
                    MyEndDate = AnimeItemViewModel.InvalidStartEndDate
                })
                : new AnimeItemAbstraction(true, new MangaLibraryItemData
                {
                    Title = Title,
                    ImgUrl = _imgUrl,
                    Type = type,
                    Id = Id,
                    AllEpisodes = AllEpisodes,
                    MalId = MalId,
                    MyStatus = Settings.DefaultStatusAfterAdding,
                    MyEpisodes = 0,
                    MyScore = 0,
                    MyStartDate = startDate,
                    MyEndDate = AnimeItemViewModel.InvalidStartEndDate,
                    AllVolumes = AllVolumes,
                    MyVolumes = MyVolumes
                });
            _animeItemReference = animeItem.ViewModel;

            MyScore = 0;
            MyStatus = Settings.DefaultStatusAfterAdding;
            MyEpisodes = 0;
            RaisePropertyChanged(() => GlobalScore); //trigger setter of anime item
            if (string.Equals(Status, "Currently Airing", StringComparison.CurrentCultureIgnoreCase))
                (_animeItemReference as AnimeItemViewModel).Airing = true;
            ResourceLocator.AnimeLibraryDataStorage.AddAnimeEntry(animeItem);
            MyDetailsVisibility = true;
            PopulateStartEndDates();
            RaisePropertyChanged(() => StartDateTimeOffset);
            RaisePropertyChanged(() => EndDateTimeOffset);
            RaisePropertyChanged(() => IsIncrementButtonEnabled);
            RaisePropertyChanged(() => IncrementEpsCommand);
            RaisePropertyChanged(() => DecrementEpsCommand);
        }

        public void CurrentAnimeHasBeenAddedToList(IAnimeData reference)
        {
            _animeItemReference = reference;
            MyDetailsVisibility = true;
            AddAnimeVisibility = false;
            RaisePropertyChanged(() => IsIncrementButtonEnabled);
            RaisePropertyChanged(() => IncrementEpsCommand);
            RaisePropertyChanged(() => DecrementEpsCommand);
        }

        private void RemoveAnime()
        {
            if (_animeItemReference == null)
                return;
            var uSure = false;
            ResourceLocator.MessageDialogProvider.ShowMessageDialogWithInput(
                "Are you sure about deleting this entry from your list?", "You are about to remove this entry!",
                "I'm sure", "Cancel",
                async () =>
                {
                    LoadingUpdate = true;
                    IsRemoveAnimeButtonEnabled = false;

                    var response = AnimeMode
                        ? await new AnimeRemoveQuery(Id.ToString()).GetRequestResponse()
                        : await new MangaRemoveQuery(Id.ToString()).GetRequestResponse();

                    LoadingUpdate = false;
                    IsRemoveAnimeButtonEnabled = true;

                   _animeLibraryDataStorage.RemoveAnimeEntry(
                        (_animeItemReference as AnimeItemViewModel).ParentAbstraction);

                    (_animeItemReference as AnimeItemViewModel).SetAuthStatus(false, true);
                    AddAnimeVisibility = true;
                    IsAddAnimeButtonEnabled = true;
                    MyDetailsVisibility = false;
                });
        }

        #endregion

        #region FetchAndPopulate

        private void PopulateData()
        {
            var model = _animeItemReference as AnimeItemViewModel;
            if (model != null && AnimeMode)
            {
                var day = -1;
                try
                {
                    day = StartDate != AnimeItemViewModel.InvalidStartEndDate &&
                          (string.Equals(Status, "Currently Airing", StringComparison.CurrentCultureIgnoreCase) ||
                           string.Equals(Status, "Not yet aired", StringComparison.CurrentCultureIgnoreCase))
                        ? (int) DateTime.Parse(StartDate).DayOfWeek + 1
                        : -1;
                }
                catch (Exception)
                {
                    day = -1;
                }

                DataCache.RegisterVolatileData(Id, new VolatileDataCache
                {
                    DayOfAiring = day,
                    GlobalScore = GlobalScore,
                    AirStartDate = StartDate == AnimeItemViewModel.InvalidStartEndDate ? null : StartDate
                });
                model.Airing = day != -1;
                if (model.ParentAbstraction.TryRetrieveVolatileData())
                    model.UpdateVolatileDataBindings();
            }

            LeftDetailsRow = new List<Tuple<string, string>>();
            RightDetailsRow = new List<Tuple<string, string>>();
            var item = _animeItemReference as AnimeItemViewModel;
            if (AnimeMode || item == null)
            {
                LeftDetailsRow.Add(new Tuple<string, string>(AnimeMode ? "Episodes" : "Chapters",
                    AllEpisodes == 0 ? "?" : AllEpisodes.ToString()));
            }
            else
            {
                LeftDetailsRow.Add(new Tuple<string, string>(Settings.MangaFocusVolumes ? "Volumes" : "Chapters",
                    item.AllEpisodesFocused == 0 ? "?" : item.AllEpisodesFocused.ToString()));
            }

            LeftDetailsRow.Add(new Tuple<string, string>("Score", GlobalScore == 0 ? "N/A" : GlobalScore.ToString("N2")));
            LeftDetailsRow.Add(new Tuple<string, string>("Start",
                StartDate == "0000-00-00" || string.IsNullOrEmpty(StartDate)
                    ? "?"
                    : StartDate.Contains("-00-00")
                        ? StartDate.Substring(0, 4)
                        : StartDate));
            RightDetailsRow.Add(new Tuple<string, string>("Type", Type));
            if (string.Equals(Status, "Currently Airing", StringComparison.CurrentCultureIgnoreCase) && ResourceLocator.AiringInfoProvider.TryGetCurrentEpisode(Id,out int ep))
            {
                RightDetailsRow.Add(new Tuple<string, string>("Status", $"{Status}\nCurrent ep. {ep}"));
            }
            else
                RightDetailsRow.Add(new Tuple<string, string>("Status", Status));
            RightDetailsRow.Add(new Tuple<string, string>("End",
                EndDate == "0000-00-00" || string.IsNullOrEmpty(EndDate)
                    ? "?"
                    : EndDate.Contains("-00-00")
                        ? EndDate.Substring(0, 4)
                        : EndDate));

            RaisePropertyChanged(() => LeftDetailsRow);
            RaisePropertyChanged(() => RightDetailsRow);
            ViewModelLocator.GeneralMain.CurrentOffStatus = Title;

            DetailImage = _imgUrl;
            LoadingGlobal = false;

            if (Settings.DetailsAutoLoadDetails)
                LoadDetails();
            if (Settings.DetailsAutoLoadReviews)
                LoadReviews();
            if (Settings.DetailsAutoLoadRecomms)
                LoadRecommendations();
            if (Settings.DetailsAutoLoadRelated)
                LoadRelatedAnime();

            //Launch UI updates without triggering inner update logic -> nothng to update
            UpdateAnimeReferenceUiBindings(Id);
        }

        private void PopulateStartEndDates()
        {
            try
            {
                _startDateTimeOffset = DateTimeOffset.Parse(_animeItemReference.StartDate);
                StartDateValid = true;
            }
            catch (Exception)
            {
                _startDateTimeOffset = DateTimeOffset.Now;
                StartDateValid = false;
            }
            try
            {
                _endDateTimeOffset = DateTimeOffset.Parse(_animeItemReference.EndDate);
                EndDateValid = true;
            }
            catch (Exception)
            {
                _endDateTimeOffset = DateTimeOffset.Now;
                EndDateValid = false;
            }
        }

        private void ExtractData(AnimeGeneralDetailsData data)
        {
            Title = _animeItemReference?.Title ?? data.Title;
            Type = data.Type;
            Status = data.Status;
            Synopsis = data.Synopsis;
            StartDate = data.StartDate;
            EndDate = data.EndDate;
            GlobalScore = data.GlobalScore;
            _imgUrl = (_animeItemReference as AnimeItemViewModel)?.ImgUrl ?? data.ImgUrl;
            if (Settings.SelectedApiType == ApiType.Hummingbird)
                MalId = data.MalId;

            _synonyms = data.Synonyms;
            _synonyms = _synonyms.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            for (var i = 0; i < _synonyms.Count; i++)
                _synonyms[i] = Regex.Replace(_synonyms[i], @" ?\(.*?\)", string.Empty);
            //removes string from brackets (sthsth) lol ->  lol
            AllEpisodes = data.AllEpisodes;
            if (!AnimeMode)
            {
                AllVolumes = data.AllVolumes;
                var vm = _animeItemReference as AnimeItemViewModel;
                if (vm != null)
                {
                    vm.UpdateChapterData(data.AllEpisodes);
                }
            }



            PopulateData();
        }

        private async void UpdateLoadingWithDelay()
        {
            await Task.Delay(100);
            RaisePropertyChanged(() => LoadingGlobal);
        }

        private async Task FetchData(bool force = false, PageIndex? sourcePage = null)
        {
#if !ANDROID
            _loadingGlobal = true;
            UpdateLoadingWithDelay();
#else
            LoadingGlobal = true;
#endif
            try
            {
                var data =
                    await
                        new AnimeGeneralDetailsQuery().GetAnimeDetails(force, Id.ToString(), Title, AnimeMode,
                            sourcePage != null
                                ? sourcePage == PageIndex.PageCharacterDetails ||
                                  sourcePage == PageIndex.PageStaffDetails
                                    ? (ApiType?) ApiType.Mal
                                    : null
                                : null);
                ExtractData(data);
            }
            catch (Exception e)
            {

                LoadingGlobal = false;
                // no internet?              
            }
        }

        public async void RefreshData()
        {
            await FetchData(true);
            if (_loadedDetails)
                LoadDetails(true);
            if (_loadedReviews)
                LoadReviews(true);
            if (_loadedRecomm)
                LoadRecommendations(true);
            if(_loadedRelated)
                LoadRelatedAnime(true);
        }

        public event EmptyEventHander OnDetailsLoaded;
        public async void LoadDetails(bool force = false)
        {
            if (LoadingDetails || (_loadedDetails && !force && Initialized))
                return;
            _loadedDetails = true;
            LoadingDetails = true;
            LeftGenres.Clear();
            RightGenres.Clear();
            Information.Clear();
            Stats.Clear();
            OPs.Clear();
            EDs.Clear();
            var data = await new AnimeDetailsMalQuery(MalId, AnimeMode).GetDetails(force);
            if (data == null)
            {
                DetailedDataVisibility = false;
                LoadingDetails = false;
                return;
            }
            DetailedDataVisibility = true;
            //Now we can build elements here
            var i = 1;
            foreach (var genre in data.Information.FirstOrDefault(s => s.StartsWith("Genres:")).Substring(7).Split(',') ?? Enumerable.Empty<string>())
            {
                if (i%2 == 0)
                    LeftGenres.Add(Utils.Utilities.FirstCharToUpper(genre));
                else
                    RightGenres.Add(Utils.Utilities.FirstCharToUpper(genre));
                i++;
            }

            //Umm... K-ON is NOT music anime
            if (Id == 5680 || Id == 7791 || Id == 9617)
            {
                bool truthHadBeenTold = false;
                for (int j = 0; j < LeftGenres.Count; j++)
                {
                    if (LeftGenres[j].Trim() == "Music")
                    {
                        LeftGenres[j] = "Certainly NOT Music Anime...";
                        truthHadBeenTold = true;
                        break;
                    }
                }
                if (!truthHadBeenTold)
                {
                    for (int j = 0; j < RightGenres.Count; j++)
                    {
                        if (RightGenres[j].Trim() == "Music")
                        {
                            RightGenres[j] = "Certainly NOT Music Anime...";
                            break;
                        }
                    }
                }
            }

            foreach (var info in data.Information)
            {
                var infoString = info;
                if (info.StartsWith("Genres:"))
                    continue;
                infoString = infoString.Replace(", add some", "");
                var parts = infoString.Split(':');

                if (parts[0] == "Broadcast" && parts[1] != "Unknown")
                {
                    var vm = _animeItemReference as AnimeItemViewModel;
                    if (vm != null)
                    {
                        if (vm.ParentAbstraction.LoadedVolatile)
                        {
                            var time = data.ExtractAiringTime();
                            if (time != null)
                            {
                                DataCache.UpdateVolatileDataWithExactDate(Id, time);
                                vm.ParentAbstraction.ExactAiringTime = time;
                            }
                            else
                                DataCache.RegisterVolatileDataAiringTimeFetchFailure(Id);
                        }
                    }
                }
                Information.Add(new Tuple<string, string>(parts[0], string.Join(":", parts.Skip(1))));
            }
            if(_animeItemReference?.AlaternateTitle != null)
                Information.Add(new Tuple<string, string>("Alt. Title",_animeItemReference.AlaternateTitle));

            foreach (var statistic in data.Statistics)
            {
                var infoString = statistic;
                var pos = infoString.IndexOf("1 indicates");
                if (pos != -1)
                    continue;
                pos = infoString.IndexOf("2 based");
                if (pos != -1)
                    infoString = infoString.Substring(0, pos - 2);
                pos = infoString.IndexOf("(scored");
                if (pos != -1)
                    infoString = infoString.Substring(0, pos - 2);

                var parts = infoString.Split(':');
                Stats.Add(new Tuple<string, string>(parts[0], parts[1]));
            }

            foreach (var op in data.Openings)
                OPs.Add(op);
            foreach (var ed in data.Endings)
                EDs.Add(ed);
            RaisePropertyChanged(() => AnimeMode);
            LoadingDetails = false;
            OnDetailsLoaded?.Invoke();
        }

        public async void LoadReviews(bool force = false)
        {
            if (LoadingReviews == true || (_loadedReviews && !force && Initialized))
                return;
            LoadingReviews = true;
            _loadedReviews = true;
            Reviews.Clear();
            var revs = new List<AnimeReviewData>();
            await Task.Run(async () => revs = await new AnimeReviewsQuery(MalId, AnimeMode).GetAnimeReviews(force));
            if (revs == null)
            {
                LoadingReviews = false;
                NoReviewsDataNoticeVisibility = true;
                return;
            }
            foreach (var rev in revs)
                Reviews.Add(rev);
            NoReviewsDataNoticeVisibility = Reviews.Count <= 0;
            LoadingReviews = false;
        }

        public async void LoadRecommendations(bool force = false)
        {
            if (LoadingRecommendations || (_loadedRecomm && !force && Initialized))
                return;
            LoadingRecommendations = true;
            _loadedRecomm = true;
            Recommendations.Clear();
            var recomm = new List<DirectRecommendationData>();
            await
                Task.Run(
                    async () =>
                        recomm =
                            await new AnimeDirectRecommendationsQuery(MalId, AnimeMode).GetDirectRecommendations(force));
            if (recomm == null)
            {
                LoadingRecommendations = false;
                NoRecommDataNoticeVisibility = true;
                return;
            }
            Recommendations.AddRange(recomm);
            NoRecommDataNoticeVisibility = Recommendations.Count <= 0;
            LoadingRecommendations = false;
        }

        public async void LoadRelatedAnime(bool force = false)
        {
            if (LoadingRelated || (_loadedRelated && !force && Initialized))
                return;
            LoadingRelated = true;
            _loadedRelated = true;
            RelatedAnime.Clear();
            var related = new List<RelatedAnimeData>();
            await Task.Run(async () => related = await new AnimeRelatedQuery(MalId, AnimeMode).GetRelatedAnime(force));
            if (related == null)
            {
                LoadingRelated = false;
                NoRelatedDataNoticeVisibility = true;
                return;
            }
            foreach (var item in related)
                RelatedAnime.Add(item);
            NoRelatedDataNoticeVisibility = RelatedAnime.Count <= 0;
            LoadingRelated = false;
        }
   

        public async void LoadCharacters(bool force = false)
        {
            if(!LoadCharactersButtonVisibility)
                return;

            LoadingCharactersVisibility = true;
            LoadCharactersButtonVisibility = false;
            try
            {
                if (AnimeMode)
                {
                    AnimeStaffData =
                        new AnimeStaffDataViewModels(
                            await new AnimeCharactersStaffQuery(MalId, AnimeMode).GetCharStaffData(force));
                    CharactersGridVisibility = true;
                }
                else //broken for now -> malformed html
                {
                    //MangaCharacterData = await new AnimeCharactersStaffQuery(Id, _animeMode).GetMangaCharacters(force);
                    MangaCharacterGridVisibility = true;
                }
                LoadingCharactersVisibility = false;
            }
            catch (Exception)
            {
                //no iternet most probably
                LoadingCharactersVisibility = false;
            }
        }


        private async void LoadVideos(bool force = false)
        {
            if (LoadingVideosVisibility || (_loadedVideos && !force))
                return;
            AvailableVideos.Clear();
            LoadingVideosVisibility = true;
            _loadedVideos = true;

            foreach (var animeVideoData in await new AnimeVideosQuery(Id).GetVideos(force))
            {
                AvailableVideos.Add(animeVideoData);
            }

            NoVideosNoticeVisibility = !AvailableVideos.Any();
            LoadingVideosVisibility = false;
        }

        #endregion

        public static async Task OpenVideo(AnimeVideoData data)
        {
            try
            {
                var youTube = YouTube.Default;
                var video = youTube.GetVideo(data.YtLink);
                var uri =  await video.GetUriAsync();
                ViewModelLocator.GeneralMain.MediaElementSource = uri;
                ViewModelLocator.GeneralMain.MediaElementVisibility = true;
                ViewModelLocator.GeneralMain.MediaElementIndirectSource = data.YtLink;
            }
            catch (Exception e)
            {
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Something went wrong with loading this video, probably google has messed again with their api again... yay!","Unable to load youtube video!");
            }
        }
    }
}