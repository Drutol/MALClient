using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
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
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.Utils.Managers;

namespace MALClient.XShared.ViewModels.Details
{
    public class AnimeDetailsPageViewModel : ViewModelBase
    {
        private readonly IClipboardProvider _clipboardProvider;
        private readonly ISystemControlsLauncherService _systemControlsLauncherService;
        //additional fields
        private int _allEpisodes;
        private int _allVolumes;
        private string _alternateImgUrl;
        private IAnimeData _animeItemReference; //our connection with everything
        public bool AnimeMode { get; set; }

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

        private bool _loadingAlternate;

        private List<FavouriteViewModel> _mangaCharacterData;

        public AnimeDetailsPageNavigationArgs _prevArgs;
        private List<string> _synonyms = new List<string>(); //used to increase ann's search reliability

        public AnimeDetailsPageViewModel(IClipboardProvider clipboardProvider,ISystemControlsLauncherService systemControlsLauncherService)
        {
            _clipboardProvider = clipboardProvider;
            _systemControlsLauncherService = systemControlsLauncherService;
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

        public ObservableCollection<DirectRecommendationData> Recommendations { get; } =
            new ObservableCollection<DirectRecommendationData>();

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

        public static List<string> ScoreFlyoutChoices { get; set; }


        private string SourceLink { get; set; }

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                if (value <= 0)
                    _prevArgs = null;
            }
        }

        public int MalId { get; set; }

        private int AllEpisodes
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

        //public event EventHandler OnInitialized; 

        public async void Init(AnimeDetailsPageNavigationArgs param)
        {
            Initialized = false;
            LoadingGlobal = true;
            //wait for UI
            await Task.Delay(5);
            ViewModelLocator.GeneralMain.IsCurrentStatusSelectable = true;

            _loadingAlternate = false;

            //details reset
            _loadedDetails = _loadedReviews = _loadedRecomm = _loadedRelated = false;

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
            PivotItemDetailsVisibility = AnimeMode ? true : false;

            if (AnimeMode)
            {
                Status1Label = "Watching";
                Status5Label = "Plan to watch";
                WatchedEpsLabel = "Watched\nepisodes";
                UpdateEpsUpperLabel = "Watched\nepisodes";
            }
            else
            {
                Status1Label = "Reading";
                Status5Label = "Plan to read";
                WatchedEpsLabel = "Read\nchapters";
                UpdateEpsUpperLabel = "Read\nchapters";
                LoadCharactersButtonVisibility = false;
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

            switch (param.Source)
            {
                case PageIndex.PageSearch:
                case PageIndex.PageMangaSearch:
                    ExtractData(param.AnimeElement);
                    if (_prevArgs != null)
                        ViewModelLocator.NavMgr.RegisterBackNav(_prevArgs);
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
                    await FetchData();
                    if (_prevArgs != null)
                        ViewModelLocator.NavMgr.RegisterBackNav(_prevArgs);
                    if(ViewModelLocator.Mobile || (!ViewModelLocator.Mobile && param.Source != PageIndex.PageProfile))
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
                    ExtractData(param.AnimeElement);
                    if (_prevArgs != null)
                        ViewModelLocator.NavMgr.RegisterBackNav(_prevArgs);
                    ViewModelLocator.NavMgr.RegisterBackNav(param.Source, param.PrevPageSetup);
                    break;
            }

            _prevArgs = param;
            _prevArgs.RegisterBackNav = false;
            _prevArgs.Source = PageIndex.PageAnimeDetails;
            Initialized = true;
            DetailsPivotSelectedIndex = param.SourceTabIndex;
            //param.SourceTab == DetailsPageTabs.General  ? 0 : _animeMode ? (int)param.SourceTab : (int)param.SourceTab - 1;
        }

        private  void OpenMalPage()
        {
            if (Settings.SelectedApiType == ApiType.Mal)
            {
                    _systemControlsLauncherService.LaunchUri(new Uri($"https://myanimelist.net/{(AnimeMode ? "anime" : "manga")}/{Id}"));
            }
            else
            {
                _systemControlsLauncherService.LaunchUri(new Uri($"https://hummingbird.me/{(AnimeMode ? "anime" : "manga")}/{Id}"));
            }
        }

        private  void OpenAnnPage()
        {
            _systemControlsLauncherService.LaunchUri(new Uri(SourceLink));
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

        #region Properties

        public string MyEpisodesBind => $"{MyEpisodes}/{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())}";

        private int MyEpisodes
        {
            get { return _animeItemReference?.MyEpisodes ?? 0; }
            set
            {
                _animeItemReference.MyEpisodes = value;
                RaisePropertyChanged(() => MyEpisodesBind);
            }
        }

        public string MyStatusBind => Utilities.StatusToString(MyStatus, !AnimeMode);

        private int MyStatus
        {
            get { return _animeItemReference?.MyStatus ?? (int) AnimeStatus.AllOrAiring; }
            set
            {
                _animeItemReference.MyStatus = value;
                RaisePropertyChanged(() => MyStatusBind);
            }
        }

        public string MyScoreBind
            =>
                MyScore == 0
                    ? "Unranked"
                    : $"{MyScore.ToString(Settings.SelectedApiType == ApiType.Mal ? "N0" : "N1")}/{(Settings.SelectedApiType == ApiType.Mal ? "10" : "5")}"
            ;

        private float MyScore
        {
            get { return _animeItemReference?.MyScore ?? 0; }
            set
            {
                _animeItemReference.MyScore = value;
                RaisePropertyChanged(() => MyScoreBind);
            }
        }

        public string MyVolumesBind => $"{MyVolumes}/{(AllVolumes == 0 ? "?" : AllVolumes.ToString())}";

        public int MyVolumes
        {
            get { return _animeItemReference?.MyVolumes ?? 0; }
            set
            {
                _animeItemReference.MyVolumes = value;
                RaisePropertyChanged(() => MyVolumesBind);
            }
        }

        private ObservableCollection<string> _myTags;

        public ObservableCollection<string> MyTags
        {
            get { return _myTags; }
            set
            {
                _myTags = value;
                RaisePropertyChanged(() => MyTags);
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

        private bool _loadingUpdate;

        public bool LoadingUpdate
        {
            get { return _loadingUpdate; }
            set
            {
                _loadingUpdate = value;
                RaisePropertyChanged(() => LoadingUpdate);
            }
        }

        private string _synopsis;

        public string Synopsis
        {
            get { return _synopsis; }
            set
            {
                _synopsis = value;
                RaisePropertyChanged(() => Synopsis);
            }
        }

        private float GlobalScore
        {
            get { return _globalScore; }
            set
            {
                if (_animeItemReference != null)
                    _animeItemReference.GlobalScore = value;
                _globalScore = value;
            }
        }

        private bool _loadingGlobal;

        public bool LoadingGlobal
        {
            get { return _loadingGlobal; }
            set
            {
                _loadingGlobal = value;
                RaisePropertyChanged(() => LoadingGlobal);
            }
        }

        public bool MalApiSpecificControlsVisibility
            => Settings.SelectedApiType == ApiType.Mal ? true : false;


        private bool _loadingDetails = false;

        public bool LoadingDetails
        {
            get { return _loadingDetails; }
            set
            {
                _loadingDetails = value;
                RaisePropertyChanged(() => LoadingDetails);
            }
        }

        private bool _loadingReviews = false;

        public bool LoadingReviews
        {
            get { return _loadingReviews; }
            set
            {
                _loadingReviews = value;
                RaisePropertyChanged(() => LoadingReviews);
            }
        }

        private bool _loadingRelated = false;

        public bool LoadingRelated
        {
            get { return _loadingRelated; }
            set
            {
                _loadingRelated = value;
                RaisePropertyChanged(() => LoadingRelated);
            }
        }

        private bool _loadingHummingbirdImage = false;

        public bool LoadingHummingbirdImage
        {
            get { return _loadingHummingbirdImage; }
            set
            {
                _loadingHummingbirdImage = value;
                RaisePropertyChanged(() => LoadingHummingbirdImage);
            }
        }

        private bool _loadingRecommendations = false;

        public bool LoadingRecommendations
        {
            get { return _loadingRecommendations; }
            set
            {
                _loadingRecommendations = value;
                RaisePropertyChanged(() => LoadingRecommendations);
            }
        }

        private bool _detailedDataVisibility;

        public bool DetailedDataVisibility
        {
            get { return _detailedDataVisibility; }
            set
            {
                _detailedDataVisibility = value;
                RaisePropertyChanged(() => DetailedDataVisibility);
            }
        }

        private bool _charactersGridVisibility = false;

        public bool CharactersGridVisibility
        {
            get { return _charactersGridVisibility; }
            set
            {
                _charactersGridVisibility = value;
                RaisePropertyChanged(() => CharactersGridVisibility);
            }
        }

        private bool _mangaCharacterGridVisibility = false;

        public bool MangaCharacterGridVisibility
        {
            get { return _mangaCharacterGridVisibility; }
            set
            {
                _mangaCharacterGridVisibility = value;
                RaisePropertyChanged(() => MangaCharacterGridVisibility);
            }
        }

        private bool _loadingCharactersVisibility = false;

        public bool LoadingCharactersVisibility
        {
            get { return _loadingCharactersVisibility; }
            set
            {
                _loadingCharactersVisibility = value;
                RaisePropertyChanged(() => LoadingCharactersVisibility);
            }
        }

        private bool _loadCharactersButtonVisibility;

        public bool LoadCharactersButtonVisibility
        {
            get { return _loadCharactersButtonVisibility; }
            set
            {
                _loadCharactersButtonVisibility = value;
                RaisePropertyChanged(() => LoadCharactersButtonVisibility);
            }
        }


        public bool ReviewsListViewVisibility
            => Settings.DetailsListReviewsView ? true : false;

        public bool RecomsListViewVisibility
            => Settings.DetailsListRecomsView ? true : false;

        private DateTimeOffset _startDateTimeOffset; //= DateTimeOffset.Parse("2015-09-10");
        public bool StartDateValid;

        public DateTimeOffset StartDateTimeOffset
        {
            get { return _startDateTimeOffset; }
            set
            {
                _startDateTimeOffset = value;
                _animeItemReference.StartDate = value.ToString("yyyy-MM-dd");
                StartDateValid = true;
                LaunchUpdate();
                RaisePropertyChanged(() => StartDateTimeOffset);
                RaisePropertyChanged(() => MyStartDate);
            }
        }

        private DateTimeOffset _endDateTimeOffset;
        public bool EndDateValid;

        public DateTimeOffset EndDateTimeOffset
        {
            get { return _endDateTimeOffset; }
            set
            {
                _endDateTimeOffset = value;
                _animeItemReference.EndDate = value.ToString("yyyy-MM-dd");
                EndDateValid = true;
                LaunchUpdate();
                RaisePropertyChanged(() => EndDateTimeOffset);
                RaisePropertyChanged(() => MyEndDate);
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

        private string _readVolumesInput;

        public string ReadVolumesInput
        {
            get { return _readVolumesInput; }
            set
            {
                _readVolumesInput = value;
                RaisePropertyChanged(() => ReadVolumesInput);
            }
        }

        private string _newTagInput;

        public string NewTagInput
        {
            get { return _newTagInput; }
            set
            {
                _newTagInput = value;
                RaisePropertyChanged(() => NewTagInput);
            }
        }

        private bool _watchedEpsInputNoticeVisibility;

        public bool WatchedEpsInputNoticeVisibility
        {
            get { return _watchedEpsInputNoticeVisibility; }
            set
            {
                _watchedEpsInputNoticeVisibility = value;
                RaisePropertyChanged(() => WatchedEpsInputNoticeVisibility);
            }
        }

        private bool _myDetailsVisibility;

        public bool MyDetailsVisibility
        {
            get { return _myDetailsVisibility; }
            set
            {
                _myDetailsVisibility = value;
                RaisePropertyChanged(() => MyDetailsVisibility);
            }
        }

        private bool _addAnimeVisibility;

        public bool AddAnimeVisibility
        {
            get { return _addAnimeVisibility; }
            set
            {
                _addAnimeVisibility = value;
                RaisePropertyChanged(() => AddAnimeVisibility);
            }
        }

        private int _currentlySelectedImagePivotIndex;

        public int CurrentlySelectedImagePivotIndex
        {
            get { return _currentlySelectedImagePivotIndex; }
            set
            {
                if (Settings.SelectedApiType == ApiType.Hummingbird)
                    return;
                _currentlySelectedImagePivotIndex = value;
                if (value == 1 && HummingbirdImage == null)
                    LoadHummingbirdCoverImageMobile();
                RaisePropertyChanged(() => CurrentlySelectedImagePivotIndex);
            }
        }

        private ICommand _saveImageCommand;

        public ICommand SaveImageCommand
        {
            get
            {
                return _saveImageCommand ??
                       (_saveImageCommand =
                           new RelayCommand<string>(
                               async opt =>
                               {
                                   if (AnimeMode || (!AnimeMode && opt != "hum"))
                                       ResourceLocator.ImageDownloaderService.DownloadImage(
                                           opt == "hum"
                                               ? (_alternateImgUrl ??
                                                  (_alternateImgUrl = await LoadHummingbirdCoverImage()))
                                               : _imgUrl, Title);
                               }));
            }
        }

        private ICommand _changeStatusCommand;

        public ICommand ChangeStatusCommand
            => _changeStatusCommand ?? (_changeStatusCommand = new RelayCommand<object>(ChangeStatus));

        private ICommand _navigateCharacterDetailsCommand;

        public ICommand NavigateCharacterDetailsCommand
            => _navigateCharacterDetailsCommand ?? (_navigateCharacterDetailsCommand = new RelayCommand<AnimeCharacter>(
                character =>
                {
                    ViewModelLocator.NavMgr.RegisterBackNav(_prevArgs);
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageCharacterDetails,new CharacterDetailsNavigationArgs {Id = int.Parse(character.Id)});
                }));

        private ICommand _navigateStaffDetailsCommand;

        public ICommand NavigateStaffDetailsCommand
            => _navigateStaffDetailsCommand ?? (_navigateStaffDetailsCommand = new RelayCommand<AnimeStaffPerson>(
                person =>
                {
                    ViewModelLocator.NavMgr.RegisterBackNav(_prevArgs);
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageStaffDetails,new StaffDetailsNaviagtionArgs {Id = int.Parse(person.Id)});
                }));


        private ICommand _navigateForumBoardCommand;

        public ICommand NavigateForumBoardCommand
            =>
                _navigateForumBoardCommand ??
                (_navigateForumBoardCommand =
                    new RelayCommand(
                        () =>
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                                new ForumsBoardNavigationArgs(Id, Title, AnimeMode))));

        private ICommand _toggleFavouriteCommand;

        public ICommand ToggleFavouriteCommand
            => _toggleFavouriteCommand ?? (_toggleFavouriteCommand = new RelayCommand(async () =>
            {
                IsFavouriteButtonEnabled = false;
                IsFavourite = !IsFavourite;
                if (IsFavourite)
                    await
                        FavouritesManager.AddFavourite(AnimeMode ? FavouriteType.Anime : FavouriteType.Manga,
                            Id.ToString());
                else
                    await
                        FavouritesManager.RemoveFavourite(AnimeMode ? FavouriteType.Anime : FavouriteType.Manga,
                            Id.ToString());
                var reference = _animeItemReference as AnimeItemViewModel;
                if (reference != null)
                    reference.IsFavouriteVisibility = IsFavourite ? true : false;
                IsFavouriteButtonEnabled = true;
            }));

        private ICommand _removeTagCommand;

        public ICommand RemoveTagCommand => _removeTagCommand ?? (_removeTagCommand = new RelayCommand<object>(o =>
        {
            MyTags.Remove(o as string);
            _animeItemReference.Notes = MyTags.Aggregate("", (s, s1) => s += s1 + ",");
            ChangeNotes();
        }));

        private ICommand _addTagCommand;

        public ICommand AddTagCommand => _addTagCommand ?? (_addTagCommand = new RelayCommand(() =>
        {
            if (!MyTags.Any(t => string.Equals(NewTagInput, t, StringComparison.CurrentCultureIgnoreCase)) &&
                MyTags.Count < 10)
            {
                MyTags.Add(NewTagInput);
                _animeItemReference.Notes += "," + NewTagInput;
                ChangeNotes();
                if (
                    !ViewModelLocator.GeneralMain.SearchHints.Any(
                        t => string.Equals(NewTagInput, t, StringComparison.CurrentCultureIgnoreCase)))
                    ViewModelLocator.GeneralMain.SearchHints.Add(NewTagInput); // add to hints
            }
            NewTagInput = "";
        }));

        private ICommand _resetStartDateCommand;

        public ICommand ResetStartDateCommand
        {
            get
            {
                return _resetStartDateCommand ?? (_resetStartDateCommand = new RelayCommand(() =>
                {
                    StartDateValid = false;
                    _animeItemReference.StartDate = AnimeItemViewModel.InvalidStartEndDate;
                    RaisePropertyChanged(() => MyStartDate);
                    LaunchUpdate();
                }));
            }
        }

        private ICommand _resetEndDateCommand;

        public ICommand ResetEndDateCommand
        {
            get
            {
                return _resetEndDateCommand ?? (_resetEndDateCommand = new RelayCommand(() =>
                {
                    EndDateValid = false;
                    _animeItemReference.EndDate = AnimeItemViewModel.InvalidStartEndDate;
                    RaisePropertyChanged(() => MyEndDate);
                    LaunchUpdate();
                }));
            }
        }

        private ICommand _navigateDetailsCommand;

        public ICommand NavigateDetailsCommand
        {
            get
            {
                return _navigateDetailsCommand ?? (_navigateDetailsCommand =
                    new RelayCommand<IDetailsPageArgs>(args => NavigateDetails(args)));
            }
        }

        private ICommand _changeScoreCommand;

        public ICommand ChangeScoreCommand
            => _changeScoreCommand ?? (_changeScoreCommand = new RelayCommand<object>(ChangeScore));

        private ICommand _changeWatchedCommand;

        public ICommand ChangeWatchedCommand
            => _changeWatchedCommand ?? (_changeWatchedCommand = new RelayCommand(ChangeWatchedEps));

        private ICommand _changeVolumesCommand;

        public ICommand ChangeVolumesCommand
            => _changeVolumesCommand ?? (_changeVolumesCommand = new RelayCommand(ChangeReadVolumes));

        private ICommand _addAnimeCommand;

        public ICommand AddAnimeCommand => _addAnimeCommand ?? (_addAnimeCommand = new RelayCommand(AddAnime));

        private ICommand _removeAnimeCommand;

        public ICommand RemoveAnimeCommand
            => _removeAnimeCommand ?? (_removeAnimeCommand = new RelayCommand(RemoveAnime));

        private ICommand _openInMalCommand;

        public ICommand OpenInMalCommand => _openInMalCommand ?? (_openInMalCommand = new RelayCommand(OpenMalPage));

        private ICommand _openInAnnCommand;

        public ICommand OpenInAnnCommand => _openInAnnCommand ?? (_openInAnnCommand = new RelayCommand(OpenAnnPage));

        private ICommand _loadCharactersCommand;

        public ICommand LoadCharactersCommand
            => _loadCharactersCommand ?? (_loadCharactersCommand = new RelayCommand(() => LoadCharacters()));


        private ICommand _copyToClipboardCommand;

        public object CopyToClipboardCommand
        {
            get
            {
                return _copyToClipboardCommand ?? (_copyToClipboardCommand = new RelayCommand(() =>
                {
                    if (Settings.SelectedApiType == ApiType.Mal)
                    {
                        _clipboardProvider.SetText($"http://www.myanimelist.net/{(AnimeMode ? "anime" : "manga")}/{Id}");
                    }
                    else
                    {
                        _clipboardProvider.SetText($"https://hummingbird.me/{(AnimeMode ? "anime" : "manga")}/{Id}");
                    }                  
                }));
            }
        }

        private string _detailImage;

        public string DetailImage
        {
            get { return _detailImage; }
            set
            {
                _detailImage = value;
                RaisePropertyChanged(() => DetailImage);
            }
        }

        private string _hummingbirdImage;

        public string HummingbirdImage
        {
            get { return _hummingbirdImage; }
            set
            {
                _hummingbirdImage = value;
                RaisePropertyChanged(() => HummingbirdImage);
            }
        }

        private bool _imageOverlayVisibility = false;

        public bool ImageOverlayVisibility
        {
            get { return _imageOverlayVisibility; }
            set
            {
                _imageOverlayVisibility = value;
                if (value == true)
                    ViewModelLocator.NavMgr.RegisterOneTimeOverride(
                        new RelayCommand(() => ImageOverlayVisibility = false));
                RaisePropertyChanged(() => ImageOverlayVisibility);
            }
        }

        private bool _noEpisodesDataVisibility;

        public bool NoEpisodesDataVisibility
        {
            get { return _noEpisodesDataVisibility; }
            set
            {
                _noEpisodesDataVisibility = value;
                RaisePropertyChanged(() => NoEpisodesDataVisibility);
            }
        }

        private bool _alternateImageUnavailableNoticeVisibility;

        public bool AlternateImageUnavailableNoticeVisibility
        {
            get { return _alternateImageUnavailableNoticeVisibility; }
            set
            {
                _alternateImageUnavailableNoticeVisibility = value;
                RaisePropertyChanged(() => AlternateImageUnavailableNoticeVisibility);
            }
        }

        private bool _noEDsDataVisibility;

        public bool NoEDsDataVisibility
        {
            get { return _noEDsDataVisibility; }
            set
            {
                _noEDsDataVisibility = value;
                RaisePropertyChanged(() => NoEDsDataVisibility);
            }
        }

        private bool _noOPsDataVisibility;

        public bool NoOPsDataVisibility
        {
            get { return _noOPsDataVisibility; }
            set
            {
                _noOPsDataVisibility = value;
                RaisePropertyChanged(() => NoOPsDataVisibility);
            }
        }

        private bool _noGenresDataVisibility;

        public bool NoGenresDataVisibility
        {
            get { return _noGenresDataVisibility; }
            set
            {
                _noGenresDataVisibility = value;
                RaisePropertyChanged(() => NoGenresDataVisibility);
            }
        }

        private bool _annSourceButtonVisibility;

        public bool AnnSourceButtonVisibility
        {
            get { return _annSourceButtonVisibility; }
            set
            {
                _annSourceButtonVisibility = value;
                RaisePropertyChanged(() => AnnSourceButtonVisibility);
            }
        }

        private bool _pivotItemDetailsVisibility = true;

        public bool PivotItemDetailsVisibility
        {
            get { return _pivotItemDetailsVisibility; }
            set
            {
                _pivotItemDetailsVisibility = value;
                RaisePropertyChanged(() => PivotItemDetailsVisibility);
            }
        }

        private int _detailsPivotSelectedIndex;

        public int DetailsPivotSelectedIndex
        {
            get { return _detailsPivotSelectedIndex; }
            set
            {
                _detailsPivotSelectedIndex = value;
                RaisePropertyChanged(() => DetailsPivotSelectedIndex);
            }
        }

        private bool _noReviewsDataNoticeVisibility = false;

        public bool NoReviewsDataNoticeVisibility
        {
            get { return _noReviewsDataNoticeVisibility; }
            set
            {
                _noReviewsDataNoticeVisibility = value;
                RaisePropertyChanged(() => NoReviewsDataNoticeVisibility);
            }
        }

        private bool _noRecommDataNoticeVisibility = false;

        public bool NoRecommDataNoticeVisibility
        {
            get { return _noRecommDataNoticeVisibility; }
            set
            {
                _noRecommDataNoticeVisibility = value;
                RaisePropertyChanged(() => NoRecommDataNoticeVisibility);
            }
        }

        private bool _noRelatedDataNoticeVisibility = false;

        public bool NoRelatedDataNoticeVisibility
        {
            get { return _noRelatedDataNoticeVisibility; }
            set
            {
                _noRelatedDataNoticeVisibility = value;
                RaisePropertyChanged(() => NoRelatedDataNoticeVisibility);
            }
        }

        private bool _detailsOpsVisibility = false;

        public bool DetailsOpsVisibility
        {
            get { return _detailsOpsVisibility; }
            set
            {
                _detailsOpsVisibility = value;
                RaisePropertyChanged(() => DetailsOpsVisibility);
            }
        }

        private bool _detailsEdsVisibility = false;

        public bool DetailsEdsVisibility
        {
            get { return _detailsEdsVisibility; }
            set
            {
                _detailsEdsVisibility = value;
                RaisePropertyChanged(() => DetailsEdsVisibility);
            }
        }

        private bool _myVolumesVisibility = false;

        public bool MyVolumesVisibility
        {
            get { return _myVolumesVisibility; }
            set
            {
                _myVolumesVisibility = value;
                RaisePropertyChanged(() => MyVolumesVisibility);
            }
        }

        private string _detailsSource;

        public string DetailsSource
        {
            get { return _detailsSource; }
            set
            {
                _detailsSource = value;
                RaisePropertyChanged(() => DetailsSource);
            }
        }

        private int _hiddenPivotItemIndex = -1;

        public int HiddenPivotItemIndex
        {
            get { return _hiddenPivotItemIndex; }
            set
            {
                _hiddenPivotItemIndex = value;
                RaisePropertyChanged(() => HiddenPivotItemIndex);
            }
        }

        private bool _addAnimeBtnEnableState = true;

        public bool AddAnimeBtnEnableState
        {
            get { return _addAnimeBtnEnableState; }
            set
            {
                _addAnimeBtnEnableState = value;
                RaisePropertyChanged(() => AddAnimeBtnEnableState);
            }
        }

        private bool _isFavouriteButtonEnabled = true;

        public bool IsFavouriteButtonEnabled
        {
            get { return _isFavouriteButtonEnabled; }
            set
            {
                _isFavouriteButtonEnabled = value;
                RaisePropertyChanged(() => IsFavouriteButtonEnabled);
            }
        }

        private bool _isFavourite;

        public bool IsFavourite
        {
            get { return _isFavourite; }
            set
            {
                _isFavourite = value;
                RaisePropertyChanged(() => IsFavourite);
            }
        }

        private bool _removeAnimeBtnEnableState = true;



        public bool RemoveAnimeBtnEnableState
        {
            get { return _removeAnimeBtnEnableState; }
            set
            {
                _removeAnimeBtnEnableState = value;
                RaisePropertyChanged(() => RemoveAnimeBtnEnableState);
            }
        }

        #endregion

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

        private Query GetAppropriateUpdateQuery()
        {
            if (AnimeMode)
                return new AnimeUpdateQuery(_animeItemReference);
            return new MangaUpdateQuery(_animeItemReference);
        }

        private async void LaunchUpdate()
        {
            LoadingUpdate = true;
            await GetAppropriateUpdateQuery().GetRequestResponse();
            LoadingUpdate = false;
        }

        private async void ChangeStatus(object status)
        {
            LoadingUpdate = true;
            var prevStatus = MyStatus;
            MyStatus = Utilities.StatusToInt(status as string);

            if (Settings.SetStartDateOnWatching && (string) status == "Watching" &&
                (Settings.OverrideValidStartEndDate || !StartDateValid))
            {
                _startDateTimeOffset = DateTimeOffset.Now;
                _animeItemReference.StartDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
                StartDateValid = true;
                RaisePropertyChanged(() => StartDateTimeOffset);
                RaisePropertyChanged(() => MyStartDate);
            }
            else if (Settings.SetEndDateOnDropped && (string) status == "Dropped" &&
                     (Settings.OverrideValidStartEndDate || !EndDateValid))
            {
                _endDateTimeOffset = DateTimeOffset.Now;
                _animeItemReference.EndDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
                EndDateValid = true;
                RaisePropertyChanged(() => EndDateTimeOffset);
                RaisePropertyChanged(() => MyEndDate);
            }
            else if (Settings.SetEndDateOnCompleted && (string) status == "Completed" &&
                     (Settings.OverrideValidStartEndDate || !EndDateValid))
            {
                _endDateTimeOffset = DateTimeOffset.Now;
                _animeItemReference.EndDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
                EndDateValid = true;
                RaisePropertyChanged(() => EndDateTimeOffset);
                RaisePropertyChanged(() => MyEndDate);
            }

            var response = await GetAppropriateUpdateQuery().GetRequestResponse();
            if (response != "Updated" && Settings.SelectedApiType == ApiType.Mal)
                MyStatus = prevStatus;

            if (_animeItemReference is AnimeItemViewModel)
                if (MyStatus == (int) AnimeStatus.Completed && MyEpisodes != AllEpisodes && AllEpisodes != 0)
                {
                    await ((AnimeItemViewModel) _animeItemReference).PromptForWatchedEpsChange(AllEpisodes);
                    RaisePropertyChanged(() => MyEpisodesBind);
                }


            LoadingUpdate = false;
        }

        private async void ChangeScore(object score)
        {
            LoadingUpdate = true;
            var prevScore = MyScore;
            if (Settings.SelectedApiType == ApiType.Hummingbird)
            {
                MyScore = (float) Convert.ToDouble(score as string)/2;
                if (MyScore == prevScore)
                    MyScore = 0;
            }
            else
            {
                MyScore = Convert.ToInt32(score as string);
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

        private async void ChangeWatchedEps()
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
                        (MyStatus == (int) AnimeStatus.PlanToWatch || MyStatus == (int) AnimeStatus.Dropped ||
                         MyStatus == (int) AnimeStatus.OnHold))
                    {                        
                        reference.PromptForStatusChange((int) AnimeStatus.Watching);
                        RaisePropertyChanged(() => MyStatusBind);
                    }
                    else if (MyEpisodes == AllEpisodes && AllEpisodes != 0)
                    {
                        
                        reference.PromptForStatusChange((int) AnimeStatus.Completed);
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
            AddAnimeBtnEnableState = false;
            var response = AnimeMode
                ? await new AnimeAddQuery(Id.ToString()).GetRequestResponse()
                : await new MangaAddQuery(Id.ToString()).GetRequestResponse();
            LoadingUpdate = false;
            AddAnimeBtnEnableState = true;
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
                    MyStatus = AnimeStatus.PlanToWatch,
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
                    MyStatus = AnimeStatus.PlanToWatch,
                    MyEpisodes = 0,
                    MyScore = 0,
                    MyStartDate = startDate,
                    MyEndDate = AnimeItemViewModel.InvalidStartEndDate,
                    AllVolumes = AllVolumes,
                    MyVolumes = MyVolumes
                });
            _animeItemReference = animeItem.ViewModel;

            MyScore = 0;
            MyStatus = 6;
            MyEpisodes = 0;
            GlobalScore = GlobalScore; //trigger setter of anime item
            if (string.Equals(Status, "Currently Airing", StringComparison.CurrentCultureIgnoreCase))
                (_animeItemReference as AnimeItemViewModel).Airing = true;
            ViewModelLocator.AnimeList.AddAnimeEntry(animeItem);
            MyDetailsVisibility = true;
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

        private async void RemoveAnime()
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
                    RemoveAnimeBtnEnableState = false;

                    var response = AnimeMode
                        ? await new AnimeRemoveQuery(Id.ToString()).GetRequestResponse()
                        : await new MangaRemoveQuery(Id.ToString()).GetRequestResponse();

                    LoadingUpdate = false;
                    RemoveAnimeBtnEnableState = true;

                    ViewModelLocator.AnimeList.RemoveAnimeEntry(
                        (_animeItemReference as AnimeItemViewModel).ParentAbstraction);

                    (_animeItemReference as AnimeItemViewModel).SetAuthStatus(false, true);
                    AddAnimeVisibility = true;
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

            LeftDetailsRow.Add(new Tuple<string, string>("Score", GlobalScore.ToString("N2")));
            LeftDetailsRow.Add(new Tuple<string, string>("Start",
                StartDate == "0000-00-00" || StartDate == "" ? "?" : StartDate));
            RightDetailsRow.Add(new Tuple<string, string>("Type", Type));
            RightDetailsRow.Add(new Tuple<string, string>("Status", Status));
            RightDetailsRow.Add(new Tuple<string, string>("End",
                EndDate == "0000-00-00" || EndDate == "" ? "?" : EndDate));

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

        private void ExtractData(AnimeGeneralDetailsData data)
        {
            Title = _animeItemReference?.Title ?? data.Title;
            Type = data.Type;
            Status = data.Status;
            Synopsis = data.Synopsis.Replace("quot;", "\"").Replace("ndash;","-");
            StartDate = data.StartDate;
            EndDate = data.EndDate;
            GlobalScore = data.GlobalScore;
            _imgUrl = data.ImgUrl;
            if (Settings.SelectedApiType == ApiType.Hummingbird)
                MalId = data.MalId;

            _synonyms = data.Synonyms;
            _synonyms = _synonyms.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            for (var i = 0; i < _synonyms.Count; i++)
                _synonyms[i] = Regex.Replace(_synonyms[i], @" ?\(.*?\)", string.Empty);
            //removes string from brackets (sthsth) lol ->  lol
            AllEpisodes = data.AllEpisodes;
            if (!AnimeMode)
                AllVolumes = data.AllVolumes;

            PopulateData();
        }

        private async Task FetchData(bool force = false)
        {
            LoadingGlobal = true;

            try
            {
                var data = await new AnimeGeneralDetailsQuery().GetAnimeDetails(force, Id.ToString(), Title, AnimeMode);
                ExtractData(data);
            }
            catch (Exception)
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
        }

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
                return;
            }
            DetailedDataVisibility = true;
            //Now we can build elements here
            var i = 1;
            foreach (var genre in data.Information.First(s => s.StartsWith("Genres:")).Substring(7).Split(','))
            {
                if (i%2 == 0)
                    LeftGenres.Add(Utilities.FirstCharToUpper(genre));
                else
                    RightGenres.Add(Utilities.FirstCharToUpper(genre));
                i++;
            }

            foreach (var info in data.Information)
            {
                var infoString = info;
                if(info.StartsWith("Genres:"))
                    continue;
                infoString = infoString.Replace(", add some", "");
                var parts = infoString.Split(':');
                Information.Add(new Tuple<string, string>(parts[0],string.Join(":",parts.Skip(1))));
            }

            foreach (var statistic in data.Statistics)
            {
                var infoString = statistic;
                var pos = infoString.IndexOf("1 indicates");
                if (pos != -1)
                    continue;
                pos = infoString.IndexOf("2 based");
                if (pos != -1)
                    infoString = infoString.Substring(0, pos-2);

                var parts = infoString.Split(':');
                Stats.Add(new Tuple<string, string>(parts[0], parts[1]));
            }

            foreach (var op in data.Openings)
                OPs.Add(op);
            foreach (var ed in data.Endings)
                EDs.Add(ed);
            RaisePropertyChanged(() => AnimeMode);
            LoadingDetails = false;
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
            NoReviewsDataNoticeVisibility = Reviews.Count > 0 ? false : true;
            LoadingReviews = false;
        }

        public async void LoadRecommendations(bool force = false)
        {
            if (LoadingRecommendations == true || (_loadedRecomm && !force && Initialized))
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
            foreach (var item in recomm)
                Recommendations.Add(item);
            NoRecommDataNoticeVisibility = Recommendations.Count > 0 ? false : true;
            LoadingRecommendations = false;
        }

        public async void LoadRelatedAnime(bool force = false)
        {
            if (LoadingRelated == true || (_loadedRelated && !force && Initialized))
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
            NoRelatedDataNoticeVisibility = RelatedAnime.Count > 0 ? false : true;
            LoadingRelated = false;
        }

        private async void LoadHummingbirdCoverImageMobile()
        {
            if (!AnimeMode)
            {
                AlternateImageUnavailableNoticeVisibility = true;
                return;
            }
            if (_loadingAlternate)
                return;
            _loadingAlternate = true;
            AlternateImageUnavailableNoticeVisibility = false;
            LoadingHummingbirdImage = true;
            AnimeDetailsData data = null;
            await Task.Run(async () => data = await new AnimeDetailsHummingbirdQuery(MalId).GetAnimeDetails());
            if (data?.AlternateCoverImgUrl != null)
            {
                _alternateImgUrl = data.AlternateCoverImgUrl;
                HummingbirdImage = data.AlternateCoverImgUrl;
            }
            LoadingHummingbirdImage = false;
            _loadingAlternate = false;
        }

        private async Task<string> LoadHummingbirdCoverImage()
        {
            if (!AnimeMode)
            {
                return null;
            }
            if (_loadingAlternate)
                return null;
            _loadingAlternate = true;
            LoadingUpdate = true;
            AnimeDetailsData data = null;
            await Task.Run(async () => data = await new AnimeDetailsHummingbirdQuery(Id).GetAnimeDetails());
            LoadingUpdate = false;
            _loadingAlternate = false;
            return data?.AlternateCoverImgUrl;
        }


        private async void LoadCharacters(bool force = false)
        {
            LoadingCharactersVisibility = true;
            LoadCharactersButtonVisibility = false;
            if (AnimeMode)
            {
                AnimeStaffData = new AnimeStaffDataViewModels(await new AnimeCharactersStaffQuery(Id, AnimeMode).GetCharStaffData(force));
                CharactersGridVisibility = true;
            }
            else //broken for now -> malformed html
            {
                //MangaCharacterData = await new AnimeCharactersStaffQuery(Id, _animeMode).GetMangaCharacters(force);
                MangaCharacterGridVisibility = true;
            }
            LoadingCharactersVisibility = false;
        }
        #endregion
    }
}