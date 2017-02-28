using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Interfaces;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.Models.Models.Favourites;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;

namespace MALClient.XShared.ViewModels.Details
{
    public partial class AnimeDetailsPageViewModel
    {
        #region Properties

        public string MyEpisodesBind => $"{MyEpisodes}/{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())}";

        public int MyEpisodes
        {
            get { return _animeItemReference?.MyEpisodes ?? 0; }
            set
            {
                _animeItemReference.MyEpisodes = value;
                RaisePropertyChanged(() => MyEpisodesBind);
            }
        }

        public string MyStatusBind => Utils.Utilities.StatusToString((int)MyStatus, !AnimeMode, IsRewatching);

        private AnimeStatus MyStatus
        {
            get { return _animeItemReference?.MyStatus ?? AnimeStatus.AllOrAiring; }
            set
            {
                _animeItemReference.MyStatus = value;
                RaisePropertyChanged(() => MyStatusBind);
                RaisePropertyChanged(() => IsRewatchingButtonVisibility);
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


        private bool _loadingVideosVisibility = false;

        public bool LoadingVideosVisibility
        {
            get { return _loadingVideosVisibility; }
            set
            {
                _loadingVideosVisibility = value;
                RaisePropertyChanged(() => LoadingVideosVisibility);
            }
        }

        private bool _noVideosNoticeVisibility ;

        public bool NoVideosNoticeVisibility
        {
            get { return _noVideosNoticeVisibility; }
            set
            {
                _noVideosNoticeVisibility = value;
                RaisePropertyChanged(() => NoVideosNoticeVisibility);
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

        private ICommand _saveImageCommand;

        public ICommand SaveImageCommand
        {
            get
            {
                return _saveImageCommand ??
                       (_saveImageCommand =
                           new RelayCommand<string>(
                               opt =>
                               {
                                   ResourceLocator.ImageDownloaderService.DownloadImage(_imgUrl, Title,true);
                               }));
            }
        }

        private ICommand _changeStatusCommand;

        public ICommand ChangeStatusCommand
            => _changeStatusCommand ?? (_changeStatusCommand = new RelayCommand<string>(s => ChangeStatus((AnimeStatus)Utils.Utilities.StatusToInt(s))));

        private ICommand _navigateCharacterDetailsCommand;

        public ICommand NavigateCharacterDetailsCommand
            => _navigateCharacterDetailsCommand ?? (_navigateCharacterDetailsCommand = new RelayCommand<AnimeCharacter>(
                character =>
                {
                    ViewModelLocator.NavMgr.RegisterBackNav(PrevArgs);
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageCharacterDetails, new CharacterDetailsNavigationArgs { Id = int.Parse(character.Id) });
                }));

        private ICommand _navigateStaffDetailsCommand;

        public ICommand NavigateStaffDetailsCommand
            => _navigateStaffDetailsCommand ?? (_navigateStaffDetailsCommand = new RelayCommand<AnimeStaffPerson>(
                person =>
                {
                    if(person.IsUnknown)
                        return;
                    ViewModelLocator.NavMgr.RegisterBackNav(PrevArgs);
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageStaffDetails, new StaffDetailsNaviagtionArgs { Id = int.Parse(person.Id) });
                }));


        private ICommand _navigateForumBoardCommand;

        public ICommand NavigateForumBoardCommand
            =>
                _navigateForumBoardCommand ??
                (_navigateForumBoardCommand =
                    new RelayCommand(
                        () =>
                        {
                            (PageIndex index, object arg) backNavArgs = default((PageIndex index, object arg));                            if (ViewModelLocator.Mobile)
                                ViewModelLocator.NavMgr.RegisterBackNav(PrevArgs);
                            else
                                backNavArgs = 
                                (index: ViewModelLocator.GeneralMain.CurrentMainPageKind.Value,
                                 arg: ViewModelLocator.GeneralMain.CurrentMainPageKind.Value == PageIndex.PageAnimeList
                                    ? ViewModelLocator.GeneralMain.GetCurrentListOrderParams()
                                    : ViewModelLocator.GeneralMain.LastNavArgs);
                                                     
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                                new ForumsBoardNavigationArgs(Id, Title, AnimeMode));
                            if (!ViewModelLocator.Mobile)
                            {
                                //Register back nav to whetever place we are currently in
                                ViewModelLocator.NavMgr.RegisterUnmonitoredMainBackNav(backNavArgs.Item1, backNavArgs.Item2);
                            }
                        }));

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

        public ICommand NavigateDetailsCommand => _navigateDetailsCommand ?? (_navigateDetailsCommand =
                                                      new RelayCommand<IDetailsPageArgs>(NavigateDetails));

        private ICommand _changeScoreCommand;

        public ICommand ChangeScoreCommand
            => _changeScoreCommand ?? (_changeScoreCommand = new RelayCommand<string>(str => ChangeScore(int.Parse(str))));

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

        private ICommand _loadCharactersCommand;

        public ICommand LoadCharactersCommand
            => _loadCharactersCommand ?? (_loadCharactersCommand = new RelayCommand(() => LoadCharacters()));

        private ICommand _loadVideosCommand;

        public ICommand LoadVideosCommand
            => _loadVideosCommand ?? (_loadVideosCommand = new RelayCommand(() => LoadVideos()));

        private ICommand _openVideoCommand;

        public ICommand OpenVideoCommand
            => _openVideoCommand ?? (_openVideoCommand = new RelayCommand<AnimeVideoData>(async data =>
               {
                   LoadingVideosVisibility = true;
                   await Task.Delay(1);
                   await OpenVideo(data);
                   LoadingVideosVisibility = false;
               }));

        private ICommand _navigateWallpapersPage;

        public ICommand NavigateWallpapersPage
            => _navigateWallpapersPage ?? (_navigateWallpapersPage = new RelayCommand(() =>
               {
                   ViewModelLocator.GeneralMain.Navigate(PageIndex.PageWallpapers,new WallpaperPageNavigationArgs {Query = _animeItemReference.Title});
               }));

        private ICommand _setRewatchingCountCommand;

        public ICommand SetRewatchingCountCommand
            => _setRewatchingCountCommand ?? (_setRewatchingCountCommand = new RelayCommand<int>(ChangeRewatchingCount));

        private ICommand _copyToClipboardCommand;

        public ICommand CopyToClipboardCommand
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

        public bool IsRewatching
        {
            get { return _animeItemReference?.IsRewatching ?? false; }
            set
            {
                if(value == _animeItemReference.IsRewatching)
                    return;
                _animeItemReference.IsRewatching = value;
                RaisePropertyChanged(() => IsRewatching);

                ChangeRewatching(value);
            }
        }

        public bool IsRewatchingButtonVisibility => _animeItemReference?.MyStatus == AnimeStatus.Completed;

        private bool _isRewatchingButtonEnabled = true;

        public bool IsRewatchingButtonEnabled
        {
            get { return _isRewatchingButtonEnabled; }
            set
            {
                _isRewatchingButtonEnabled = value;
                RaisePropertyChanged(() => IsRewatchingButtonEnabled);
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

        private bool _isAddAnimeButtonEnabled = true;

        public bool IsAddAnimeButtonEnabled
        {
            get { return _isAddAnimeButtonEnabled; }
            set
            {
                _isAddAnimeButtonEnabled = value;
                RaisePropertyChanged(() => IsAddAnimeButtonEnabled);
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

        private bool _isRemoveAnimeButtonEnabled = true;
        
        public bool IsRemoveAnimeButtonEnabled
        {
            get { return _isRemoveAnimeButtonEnabled; }
            set
            {
                _isRemoveAnimeButtonEnabled = value;
                RaisePropertyChanged(() => IsRemoveAnimeButtonEnabled);
            }
        }

        public List<int> RewatchedOptions { get; } = new List<int> {0,1,2,3,4,5,6,7,8,9};

        public string RewatchingLabel => AnimeMode ? "Rewatching" : "Rereading";

        public string RewatchedLabel => AnimeMode ? "rewatched" : "reread";


        #endregion
    }
}
