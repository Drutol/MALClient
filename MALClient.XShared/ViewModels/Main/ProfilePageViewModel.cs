using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.Favourites;
using MALClient.Models.Models.Library;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.MagicalRawQueries.Profile;
using MALClient.XShared.Comm.Profile;
using MALClient.XShared.Delegates;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Main
{
    public sealed class ProfilePageViewModel : ViewModelBase
    {
        private readonly IAnimeLibraryDataStorage _animeLibraryDataStorage;

        private List<int> _animeChartValues = new List<int>();

        private string _currUser;

        private List<FavouriteViewModel> _favouriteCharacters;
        private List<FavouriteViewModel> _favouriteStaff;
        private ObservableCollection<MalComment> _malComments = new ObservableCollection<MalComment>();
        public ProfilePageNavigationArgs PrevArgs;

        public ProfilePageViewModel(IAnimeLibraryDataStorage animeLibraryDataStorage)
        {
            _animeLibraryDataStorage = animeLibraryDataStorage;
            MaxWidth = AnimeItemViewModel.MaxWidth;
        }

        public List<MalUser> MyFriends { get; set; }

        public ObservableCollection<MalComment> MalComments
        {
            get => _malComments;
            set
            {
                _malComments = value;
                RaisePropertyChanged(() => MalComments);
            }
        }

        public List<FavouriteViewModel> FavouriteCharacters
        {
            get => _favouriteCharacters;
            set
            {
                _favouriteCharacters = value;
                RaisePropertyChanged(() => FavouriteCharacters);
            }
        }

        public List<FavouriteViewModel> FavouriteStaff
        {
            get => _favouriteStaff;
            set
            {
                _favouriteStaff = value;
                RaisePropertyChanged(() => FavouriteStaff);
            }
        }

        public event EmptyEventHander OnInitialized;


        public async Task LoadProfileData(ProfilePageNavigationArgs args, bool force = false)
        {
            try
            {
                if (args == null)
                    args = PrevArgs;
                else
                    PrevArgs = args;

                if (args == null)
                    return;

                AboutMeHtmlContent = null;
                AboutMeWebViewVisibility = false;
                CurrentPivotIndex = args.DesiredPivotIndex;
                RaisePropertyChanged(() => CurrentPivotIndex);


                if (args.TargetUser == Credentials.UserName && args.AllowBackNavReset)
                {
                    ViewModelLocator.NavMgr.ResetMainBackNav();
                    if (ViewModelLocator.Mobile)
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
                }

                if (_currUser == null || _currUser != args.TargetUser || force)
                {
                    LoadingVisibility = true;
                    await
                        Task.Run(
                            async () =>
                                CurrentData =
                                    await new ProfileQuery(false, args?.TargetUser ?? "").GetProfileData(force));
                    _currUser = args.TargetUser ?? Credentials.UserName;
                }
                FavAnime = new List<AnimeItemViewModel>();
                FavManga = new List<AnimeItemViewModel>();
                RecentManga = new List<AnimeItemViewModel>();
                RecentAnime = new List<AnimeItemViewModel>();
                ViewModelLocator.GeneralMain.CurrentStatus = $"{_currUser} - Profile";
                var authenticatedUser = args.TargetUser.Equals(Credentials.UserName,
                    StringComparison.CurrentCultureIgnoreCase);
                IsMyProfile = authenticatedUser;
                RaisePropertyChanged(() => CurrentData);
                LoadingVisibility = false;
                RaisePropertyChanged(() => PinProfileVisibility);
                RaisePropertyChanged(() => IsPinned);
                MalComments = new ObservableCollection<MalComment>(CurrentData.Comments);
                FavouriteCharacters =
                    new List<FavouriteViewModel>(
                        CurrentData.FavouriteCharacters.Select(character => new FavouriteViewModel(character)));
                FavouriteStaff =
                    new List<FavouriteViewModel>(
                        CurrentData.FavouritePeople.Select(staff => new FavouriteViewModel(staff)));

                CommentInputBoxVisibility = !string.IsNullOrEmpty(CurrentData.ProfileMemId); //posting restricted
                LoadingAboutMeVisibility = AboutMeWebViewVisibility = false;
                if (authenticatedUser)
                {
                    _initialized = true;
                    var list = new List<AnimeItemViewModel>();
                    foreach (var id in CurrentData.FavouriteAnime)
                    {
                        var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);

                        if (data != null)
                            list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                    }
                    FavAnime = list;
                    list = new List<AnimeItemViewModel>();
                    foreach (var id in CurrentData.FavouriteManga)
                    {
                        var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                        if (data != null)
                            list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                    }
                    FavManga = list;
                    list = new List<AnimeItemViewModel>();
                    foreach (var id in CurrentData.RecentAnime)
                    {
                        var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);
                        if (data != null)
                            list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                    }
                    RecentAnime = list;
                    list = new List<AnimeItemViewModel>();
                    foreach (var id in CurrentData.RecentManga)
                    {
                        var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                        if (data != null)
                            list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                    }
                    RecentManga = list;
                    MyFriends = CurrentData.Friends;

                    CountTime(ResourceLocator.AnimeLibraryDataStorage.AllLoadedAuthAnimeItems);
                }
                else
                {
                    if (!_animeLibraryDataStorage.OthersAbstractions.ContainsKey(args.TargetUser ?? ""))
                    {
                        LoadingOhersLibrariesProgressVisiblity = true;
                        var data = new List<ILibraryData>();
                        await
                            Task.Run(
                                async () =>
                                    data =
                                        await
                                            new LibraryListQuery(args.TargetUser, AnimeListWorkModes.Anime)
                                                .GetLibrary(false));

                        var abstractions = new List<AnimeItemAbstraction>();
                        foreach (var libraryData in data)
                            abstractions.Add(new AnimeItemAbstraction(false, libraryData as AnimeLibraryItemData));

                        await
                            Task.Run(
                                async () =>
                                    data =
                                        await
                                            new LibraryListQuery(args.TargetUser, AnimeListWorkModes.Manga)
                                                .GetLibrary(false));

                        var mangaAbstractions = new List<AnimeItemAbstraction>();
                        foreach (
                            var libraryData in data)
                            mangaAbstractions.Add(new AnimeItemAbstraction(false, libraryData as MangaLibraryItemData));

                        lock (_animeLibraryDataStorage.OthersAbstractions)
                        {
                            if (!_animeLibraryDataStorage.OthersAbstractions.ContainsKey(args.TargetUser))
                                _animeLibraryDataStorage.OthersAbstractions.Add(args.TargetUser,
                                    new Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>(abstractions,
                                        mangaAbstractions));
                        }

                        LoadingOhersLibrariesProgressVisiblity = false;
                    }

                    var source = _animeLibraryDataStorage.OthersAbstractions[args.TargetUser];
                    var list = new List<AnimeItemViewModel>();
                    foreach (var id in CurrentData.FavouriteAnime)
                    {
                        var data = source.Item1.FirstOrDefault(abs => abs.Id == id);

                        if (data != null)
                            list.Add(data.ViewModel);
                    }
                    FavAnime = list;
                    list = new List<AnimeItemViewModel>();
                    foreach (var id in CurrentData.FavouriteManga)
                    {
                        var data = source.Item2.FirstOrDefault(abs => abs.Id == id);

                        if (data != null)
                            list.Add(data.ViewModel);
                    }
                    FavManga = list;
                    list = new List<AnimeItemViewModel>();
                    foreach (var id in CurrentData.RecentAnime)
                    {
                        var data = source.Item1.FirstOrDefault(abs => abs.Id == id);

                        if (data != null)
                            list.Add(data.ViewModel);
                    }
                    RecentAnime = list;
                    list = new List<AnimeItemViewModel>();
                    foreach (var id in CurrentData.RecentManga)
                    {
                        var data = source.Item2.FirstOrDefault(abs => abs.Id == id);

                        if (data != null)
                            list.Add(data.ViewModel);
                    }
                    RecentManga = list;

                    CountTime(source.Item1);
                }
                AnimeChartValues = new List<int>
                {
                    CurrentData.AnimeWatching,
                    CurrentData.AnimeCompleted,
                    CurrentData.AnimeOnHold,
                    CurrentData.AnimeDropped,
                    CurrentData.AnimePlanned
                };
                MangaChartValues = new List<int>
                {
                    CurrentData.MangaReading,
                    CurrentData.MangaCompleted,
                    CurrentData.MangaOnHold,
                    CurrentData.MangaDropped,
                    CurrentData.MangaPlanned
                };

                EmptyRecentAnimeNoticeVisibility = RecentAnime.Count == 0;
                EmptyRecentMangaNoticeVisibility = RecentManga.Count == 0;
                EmptyFavCharactersNoticeVisibility = CurrentData.FavouriteCharacters.Count == 0;
                EmptyFavAnimeNoticeVisibility = FavAnime.Count == 0;
                EmptyFavMangaNoticeVisibility = FavManga.Count == 0;
                EmptyFavPeopleNoticeVisibility = CurrentData.FavouritePeople.Count == 0;
                EmptyCommentsNoticeVisibility = CurrentData.Comments.Count == 0;
                OnInitialized?.Invoke();

                if(CurrentData.Cached)
                    RefreshCommentsCommand.Execute(null);
            }
            catch (Exception e)
            {
                ResourceLocator.TelemetryProvider.LogEvent($"Profile Crash: {args.TargetUser}, {e} , {e.StackTrace}");
                ResourceLocator.MessageDialogProvider.ShowMessageDialog(
                    "Hmm, you have encountered bug that'm hunting. I've just sent report to myself. If everything goes well it should be gone in next release :). Sorry for inconvenience!",
                    "Ooopsies!");
            }

            void CountTime(List<AnimeItemAbstraction> source)
            {
                double tvs = 0;
                double movies = 0;
                foreach (var animeItemAbstraction in source)
                {
                    if (animeItemAbstraction.MyEpisodes <= 0)
                        continue;
                    if (animeItemAbstraction.Type == (int) AnimeType.TV ||
                        animeItemAbstraction.Type == (int) AnimeType.OVA)
                        tvs += 23.67 * animeItemAbstraction.MyEpisodes;
                    else if (animeItemAbstraction.Type == (int) AnimeType.Movie &&
                             animeItemAbstraction.MyStatus == AnimeStatus.Completed)
                        movies += 95.92;
                }
                var timeAnime = TimeSpan.FromMinutes(tvs);
                var timeBoth = TimeSpan.FromMinutes(tvs + movies);
                var timeMovies = TimeSpan.FromMinutes(movies);


                ApproxTimeSpentOnAnime = Format(timeAnime);
                ApproxTimeSpentOnMovies = Format(timeMovies);
                ApproxTimeSpentOnAnimeAndMovies = Format(timeBoth);

                string Format(TimeSpan time)
                {
                    var str = "";
                    if (time.Days > 30)
                    {
                        var m = time.Days / 30;
                        str = $"{m}mo ";
                        time = time.Subtract(TimeSpan.FromDays(m * 30));
                    }

                    if (time.Days > 0)
                        str += $"{time.Days}d ";
                    str += $"{time.Hours}h ";
                    str += $"{time.Minutes}m ";
                    str += $"{time.Seconds}s ";
                    return str;
                }
            }
        }

        private void NavigateDetails(AnimeCharacter character)
        {
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(int.Parse(character.ShowId), character.Notes, null,
                    null)
                {
                    Source = PageIndex.PageProfile,
                    AnimeMode = character.FromAnime
                });
        }

        private async void NavigateCharacterWebPage(AnimeCharacter character)
        {
            ResourceLocator.SystemControlsLauncherService.LaunchUri(
                new Uri($"https://myanimelist.net/character/{character.Id}"));
        }

        private async void NavigatePersonWebPage(AnimeStaffPerson person)
        {
            ResourceLocator.SystemControlsLauncherService.LaunchUri(
                new Uri($"https://myanimelist.net/people/{person.Id}"));
        }

        #region Props

        private bool _emptyFavAnimeNoticeVisibility;

        private bool _emptyFavCharactersNoticeVisibility;

        private bool _emptyFavMangaNoticeVisibility;

        private bool _emptyFavPeopleNoticeVisibility;

        private bool _emptyRecentAnimeNoticeVisibility;

        private bool _emptyRecentMangaNoticeVisibility;
        private List<AnimeItemViewModel> _favAnime;
        private List<AnimeItemViewModel> _favManga;

        private bool _initialized;


        private bool _loadingVisibility;

        private List<int> _mangaChartValues = new List<int>();

        private ICommand _navigateCharPageCommand;

        private ICommand _navigateDetailsCommand;

        private ICommand _navigatePersonPageCommand;

        private ICommand _navAnimeListCommand;

        private ICommand _navMangaListCommand;

        private ICommand _navigateHistoryCommand;

        public ICommand NavigateHistoryCommand
            =>
                _navigateHistoryCommand ??
                (_navigateHistoryCommand =
                    new RelayCommand(
                        () =>
                        {
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageHistory,
                                new HistoryNavigationArgs {Source = CurrentData.User.Name});
                            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile,
                                new ProfilePageNavigationArgs {TargetUser = CurrentData.User.Name});
                        }));

        public ICommand NavigateClubsCommand
            =>
                new RelayCommand(() =>
                {

                    ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile,
                        new ProfilePageNavigationArgs { TargetUser = CurrentData.User.Name });
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageClubIndex);
                });

        private ICommand _sendCommentCommand;

        public ICommand SendCommentCommand => _sendCommentCommand ?? (_sendCommentCommand = new RelayCommand(async () =>
        {
            if (string.IsNullOrEmpty(CommentText))
                return;
            IsSendCommentButtonEnabled = false;
            if (await
                ProfileCommentQueries.SendComment(CurrentData.User?.Name ?? Credentials.UserName,
                    CurrentData.ProfileMemId,
                    CommentText))
            {
                CommentText = "";
                await CurrentData.UpdateComments();
                MalComments = new ObservableCollection<MalComment>(CurrentData.Comments);
            }
            IsSendCommentButtonEnabled = true;
        }));

        private ICommand _deleteCommentCommand;

        public ICommand DeleteCommentCommand
            => _deleteCommentCommand ?? (_deleteCommentCommand = new RelayCommand<MalComment>(async comment =>
            {
                if (await ProfileCommentQueries.DeleteComment(comment.Id))
                {
                    MalComments.Remove(comment);
                    var data = CurrentData;
                    data.Comments = MalComments.ToList();
                    DataCache.SaveProfileData(_currUser, data);
                }
            }));

        private ICommand _naviagateComparisonCommand;

        public ICommand NavigateComparisonCommand
            => _naviagateComparisonCommand ?? (_naviagateComparisonCommand = new RelayCommand(() =>
            {
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile,
                    new ProfilePageNavigationArgs {TargetUser = CurrentData.User.Name});
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageListComparison,
                    new ListComparisonPageNavigationArgs {CompareWith = CurrentData.User});
            }));

        private ICommand _navigateConversationCommand;

        public ICommand NavigateConversationCommand
            => _navigateConversationCommand ?? (_navigateConversationCommand = new RelayCommand<MalComment>(comment =>
            {
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMessageDetails,
                    new MalMessageDetailsNavArgs {WorkMode = MessageDetailsWorkMode.ProfileComments, Arg = comment});
            }));

        private bool _refreshingComments;
        private ICommand _refreshCommentsCommand;

        public ICommand RefreshCommentsCommand
            => _refreshCommentsCommand ?? (_refreshCommentsCommand = new RelayCommand(async () =>
            {
                if (_refreshingComments)
                    return;
                LoadingCommentsVisiblity = true;
                _refreshingComments = true;
                await CurrentData.UpdateComments();
                MalComments = new ObservableCollection<MalComment>(CurrentData.Comments);
                _refreshingComments = false;
                LoadingCommentsVisiblity = false;
            }));

        private List<AnimeItemViewModel> _recentAnime;
        private List<AnimeItemViewModel> _recentManga;

        public ProfileData CurrentData { get; set; } = new ProfileData();

        public List<AnimeItemViewModel> RecentAnime
        {
            get => _recentAnime;
            private set
            {
                _recentAnime = value;
                RaisePropertyChanged(() => RecentAnime);
            }
        }

        public List<AnimeItemViewModel> RecentManga
        {
            get => _recentManga;
            private set
            {
                _recentManga = value;
                RaisePropertyChanged(() => RecentManga);
            }
        }

        public List<AnimeItemViewModel> FavAnime
        {
            get => _favAnime;
            private set
            {
                _favAnime = value;
                RaisePropertyChanged(() => FavAnime);
            }
        }

        public List<AnimeItemViewModel> FavManga
        {
            get => _favManga;
            private set
            {
                _favManga = value;
                RaisePropertyChanged(() => FavManga);
            }
        }

        public AnimeItemViewModel TemporarilySelectedAnimeItem
        {
            get => null;
            set
            {
                var args = PrevArgs;
                args.DesiredPivotIndex = CurrentPivotIndex;
                value?.NavigateDetails(PageIndex.PageProfile, args);
            }
        }

        public bool LoadingVisibility
        {
            get => _loadingVisibility;
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        private bool _authenticatedControlsVisibility;

        public bool AuthenticatedControlsVisibility
        {
            get => _authenticatedControlsVisibility;
            set
            {
                _authenticatedControlsVisibility = value;
                RaisePropertyChanged(() => AuthenticatedControlsVisibility);
            }
        }

        private bool _commentInputBoxVisibility;

        public bool CommentInputBoxVisibility
        {
            get => _commentInputBoxVisibility;
            set
            {
                _commentInputBoxVisibility = value;
                RaisePropertyChanged(() => CommentInputBoxVisibility);
            }
        }

        public List<int> AnimeChartValues
        {
            get => _animeChartValues;
            set
            {
                _animeChartValues = value;
                RaisePropertyChanged(() => AnimeChartValues);
            }
        }

        public List<int> MangaChartValues
        {
            get => _mangaChartValues;
            set
            {
                _mangaChartValues = value;
                RaisePropertyChanged(() => MangaChartValues);
            }
        }

        public bool IsPinned
        {
            get
            {
                return ResourceLocator.HandyDataStorage.PinnedUsers.StoredItems.Any(
                    user => user.Name.Equals(CurrentData.User.Name, StringComparison.CurrentCultureIgnoreCase));
            }
            set
            {
                if (value)
                    ResourceLocator.HandyDataStorage.PinnedUsers.StoredItems.Add(CurrentData.User);
                else
                    ResourceLocator.HandyDataStorage.PinnedUsers.StoredItems.Remove(
                        ResourceLocator.HandyDataStorage.PinnedUsers.StoredItems.First(
                            user => user.Name.Equals(CurrentData.User.Name,
                                StringComparison.CurrentCultureIgnoreCase)));
                ViewModelLocator.GeneralHamburger.UpdatePinnedProfiles();
                RaisePropertyChanged(() => IsPinned);
            }
        }

        public static double MaxWidth { get; set; }

        public ICommand NavigateDetailsCommand
            => _navigateDetailsCommand ?? (_navigateDetailsCommand = new RelayCommand<AnimeCharacter>(NavigateDetails));

        public ICommand NavigateCharPageCommand
            =>
                _navigateCharPageCommand ??
                (_navigateCharPageCommand = new RelayCommand<AnimeCharacter>(NavigateCharacterWebPage));

        public ICommand NavigatePersonPageCommand
            =>
                _navigatePersonPageCommand ??
                (_navigatePersonPageCommand = new RelayCommand<AnimeStaffPerson>(NavigatePersonWebPage));

        public ICommand NavigateAnimeListCommand
            =>
                _navAnimeListCommand ??
                (_navAnimeListCommand = new RelayCommand(() =>
                {
                    ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile,
                        new ProfilePageNavigationArgs {TargetUser = CurrentData.User.Name});
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList,
                        new AnimeListPageNavigationArgs(0, AnimeListWorkModes.Anime)
                        {
                            ListSource = _currUser,
                            ResetBackNav = false
                        });
                }));

        public ICommand NavigateMangaListCommand
            =>
                _navMangaListCommand ??
                (_navMangaListCommand =
                    new RelayCommand(
                        () =>
                        {
                            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile,
                                new ProfilePageNavigationArgs {TargetUser = CurrentData.User.Name});
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList,
                                new AnimeListPageNavigationArgs(0, AnimeListWorkModes.Manga)
                                {
                                    ListSource = _currUser,
                                    ResetBackNav = false
                                });
                        }));


        public bool PinProfileVisibility
            =>
                CurrentData.User.Name != null &&
                !Credentials.UserName.Equals(CurrentData.User.Name, StringComparison.CurrentCultureIgnoreCase);


        public bool EmptyFavAnimeNoticeVisibility
        {
            get => _emptyFavAnimeNoticeVisibility;
            set
            {
                _emptyFavAnimeNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavAnimeNoticeVisibility);
            }
        }

        public bool EmptyFavCharactersNoticeVisibility
        {
            get => _emptyFavCharactersNoticeVisibility;
            set
            {
                _emptyFavCharactersNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavCharactersNoticeVisibility);
            }
        }

        public bool EmptyFavMangaNoticeVisibility
        {
            get => _emptyFavMangaNoticeVisibility;
            set
            {
                _emptyFavMangaNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavMangaNoticeVisibility);
            }
        }

        public bool EmptyCommentsNoticeVisibility
        {
            get => _emptyCommentsNoticeVisibility;
            set
            {
                _emptyCommentsNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyCommentsNoticeVisibility);
            }
        }

        public bool IsMyProfile
        {
            get => _isMyProfile;
            set
            {
                _isMyProfile = value;
                RaisePropertyChanged();
            }
        }

        public bool EmptyRecentMangaNoticeVisibility
        {
            get => _emptyRecentMangaNoticeVisibility;
            set
            {
                _emptyRecentMangaNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyRecentMangaNoticeVisibility);
            }
        }

        public bool EmptyRecentAnimeNoticeVisibility
        {
            get => _emptyRecentAnimeNoticeVisibility;
            set
            {
                _emptyRecentAnimeNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyRecentAnimeNoticeVisibility);
            }
        }

        public bool EmptyFavPeopleNoticeVisibility
        {
            get => _emptyFavPeopleNoticeVisibility;
            set
            {
                _emptyFavPeopleNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavPeopleNoticeVisibility);
            }
        }

        private bool _loadingOhersLibrariesProgressVisiblity;

        public bool LoadingOhersLibrariesProgressVisiblity
        {
            get => _loadingOhersLibrariesProgressVisiblity;
            set
            {
                _loadingOhersLibrariesProgressVisiblity = value;
                RaisePropertyChanged(() => LoadingOhersLibrariesProgressVisiblity);
            }
        }

        private bool _loadingCommentsVisiblity;

        public bool LoadingCommentsVisiblity
        {
            get => _loadingCommentsVisiblity;
            set
            {
                _loadingCommentsVisiblity = value;
                RaisePropertyChanged(() => LoadingCommentsVisiblity);
            }
        }

        private bool _loadingAboutMeVisibility;

        public bool LoadingAboutMeVisibility
        {
            get => _loadingAboutMeVisibility;
            set
            {
                _loadingAboutMeVisibility = value;
                RaisePropertyChanged(() => LoadingAboutMeVisibility);
            }
        }

        private bool _aboutMeWebViewVisibility;

        public bool AboutMeWebViewVisibility
        {
            get => _aboutMeWebViewVisibility;
            set
            {
                _aboutMeWebViewVisibility = value;
                RaisePropertyChanged(() => AboutMeWebViewVisibility);
            }
        }

        private string _commentText;

        public string CommentText
        {
            get => _commentText;
            set
            {
                _commentText = value;
                RaisePropertyChanged(() => CommentText);
            }
        }

        private string _approxTimeSpentOnAnime;

        public string ApproxTimeSpentOnAnime
        {
            get => _approxTimeSpentOnAnime;
            set
            {
                _approxTimeSpentOnAnime = value;
                RaisePropertyChanged(() => ApproxTimeSpentOnAnime);
            }
        }

        private string _approxTimeSpentOnMovies;

        public string ApproxTimeSpentOnMovies
        {
            get => _approxTimeSpentOnMovies;
            set
            {
                _approxTimeSpentOnMovies = value;
                RaisePropertyChanged(() => ApproxTimeSpentOnMovies);
            }
        }

        private string _approxTimeSpentOnAnimeAndMovies;

        public string ApproxTimeSpentOnAnimeAndMovies
        {
            get => _approxTimeSpentOnAnimeAndMovies;
            set
            {
                _approxTimeSpentOnAnimeAndMovies = value;
                RaisePropertyChanged(() => ApproxTimeSpentOnAnimeAndMovies);
            }
        }

        private bool _isSendCommentButtonEnabled = true;

        private ICommand _navigateCharacterDetailsCommand;
        private ICommand _navigateStaffDetailsCommand;
        private bool _emptyCommentsNoticeVisibility;
        private bool _areFavsExpanded;
        private ICommand _toggleFavsCommand;
        private ICommand _toggleAboutCommand;


        public bool IsSendCommentButtonEnabled
        {
            get => _isSendCommentButtonEnabled;
            set
            {
                _isSendCommentButtonEnabled = value;
                RaisePropertyChanged(() => IsSendCommentButtonEnabled);
            }
        }

        public ICommand NavigateCharacterDetailsCommand
            =>
                _navigateCharacterDetailsCommand ??
                (_navigateCharacterDetailsCommand =
                    new RelayCommand<AnimeCharacter>(
                        entry =>
                        {
                            if (ViewModelLocator.Mobile)
                            {
                                PrevArgs.DesiredPivotIndex = CurrentPivotIndex;
                                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile, PrevArgs);
                            }
                            else if (ViewModelLocator.GeneralMain.OffContentVisibility)
                            {
                                if (ViewModelLocator.GeneralMain.CurrentOffPage == PageIndex.PageStaffDetails)
                                    ViewModelLocator.StaffDetails.RegisterSelfBackNav(int.Parse(entry.Id));
                                else if (ViewModelLocator.GeneralMain.CurrentOffPage == PageIndex.PageCharacterDetails)
                                    ViewModelLocator.CharacterDetails.RegisterSelfBackNav(int.Parse(entry.Id));
                            }
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageCharacterDetails,
                                new CharacterDetailsNavigationArgs {Id = int.Parse(entry.Id)});
                        }));

        public ICommand NavigateStaffDetailsCommand
            =>
                _navigateStaffDetailsCommand ??
                (_navigateStaffDetailsCommand =
                    new RelayCommand<FavouriteBase>(
                        entry =>
                        {
                            if (ViewModelLocator.Mobile)
                            {
                                PrevArgs.DesiredPivotIndex = CurrentPivotIndex;
                                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile, PrevArgs);
                            }
                            else if (ViewModelLocator.GeneralMain.OffContentVisibility)
                            {
                                if (ViewModelLocator.GeneralMain.CurrentOffPage == PageIndex.PageStaffDetails)
                                    ViewModelLocator.StaffDetails.RegisterSelfBackNav(int.Parse(entry.Id));
                                else if (ViewModelLocator.GeneralMain.CurrentOffPage == PageIndex.PageCharacterDetails)
                                    ViewModelLocator.CharacterDetails.RegisterSelfBackNav(int.Parse(entry.Id));
                            }
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageStaffDetails,
                                new StaffDetailsNaviagtionArgs {Id = int.Parse(entry.Id)});
                        }));

        public ICommand ToggleFavsCommand
            => _toggleFavsCommand ?? (_toggleFavsCommand = new RelayCommand(() => AreFavsExpanded = !AreFavsExpanded));

        public ICommand ToggleAboutCommand
            => _toggleAboutCommand ?? (_toggleAboutCommand = new RelayCommand(() =>
            {
                AboutMeWebViewVisibility = !AboutMeWebViewVisibility;
                if (AboutMeWebViewVisibility)
                    AboutMeHtmlContent = CurrentData.HtmlContent;
            }));

        public ICommand NavigateMessagingCommand
            => new RelayCommand(
                () =>
                {
                    if (ViewModelLocator.Mobile)
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile, PrevArgs);
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMessageDetails,
                        new MalMessageDetailsNavArgs
                        {
                            WorkMode = MessageDetailsWorkMode.Message,
                            NewMessageTarget = CurrentData.User.Name
                        });
                });

        public ICommand NavigateProfileCommand
            => _navigateProfileCommand ?? (_navigateProfileCommand = new RelayCommand<MalUser>(
                   user =>
                   {
                       ViewModelLocator.NavMgr.RegisterBackNav(PrevArgs);
                       ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile,
                           new ProfilePageNavigationArgs {TargetUser = user.Name});
                   }));

        public ICommand NavigateFriendsCommand
            => _navigateFriendsCommand ?? (_navigateFriendsCommand = new RelayCommand(
                   () =>
                   {
                       ViewModelLocator.NavMgr.RegisterBackNav(PrevArgs);
                       ViewModelLocator.GeneralMain.Navigate(PageIndex.PageFriends,
                           new FriendsPageNavArgs {TargetUser = CurrentData.User});
                   }));


        private string _aboutMeHtmlContent;

        public string AboutMeHtmlContent
        {
            get => _aboutMeHtmlContent;
            set
            {
                _aboutMeHtmlContent = value;
                RaisePropertyChanged(() => AboutMeHtmlContent);
            }
        }

        private double _computedHtmlHeight = -1;
        private ICommand _navigateProfileCommand;
        private ICommand _navigateFriendsCommand;
        private bool _isMyProfile;


        public double ComputedHtmlHeight
        {
            get => _computedHtmlHeight == -1 ? double.NaN : _computedHtmlHeight;
            set
            {
                _computedHtmlHeight = value;
                RaisePropertyChanged(() => ComputedHtmlHeight);
            }
        }

        public bool AreFavsExpanded
        {
            get => _areFavsExpanded;
            set
            {
                _areFavsExpanded = value;
                RaisePropertyChanged(() => AreFavsExpanded);
            }
        }

        /// <summary>
        ///     Used to restore pivot after navigation
        /// </summary>
        public int CurrentPivotIndex { get; set; }

        #endregion
    }
}