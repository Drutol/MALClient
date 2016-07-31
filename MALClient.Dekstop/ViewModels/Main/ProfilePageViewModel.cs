using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm;
using MalClient.Shared.Comm.MagicalRawQueries.Profile;
using MalClient.Shared.Comm.Profile;
using MalClient.Shared.Models;
using MalClient.Shared.Models.Favourites;
using MalClient.Shared.Models.Library;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MALClient.Utils.Managers;

namespace MALClient.ViewModels.Main
{
    public sealed class ProfilePageViewModel : ViewModelBase , IProfileViewModel
    {
        //anime -<>- manga
        private readonly Dictionary<string, Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>>
            _othersAbstractions =
                new Dictionary<string, Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>>();

        public Dictionary<string, Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>> OthersAbstractions
            => _othersAbstractions;

        private ObservableCollection<MalComment> _malComments = new ObservableCollection<MalComment>();

        public ObservableCollection<MalComment> MalComments
        {
            get { return _malComments; }
            set
            {
                _malComments = value;
                RaisePropertyChanged(() => MalComments);
            }
        }

        private List<int> _animeChartValues = new List<int>();

        private string _currUser;
        public ProfilePageNavigationArgs PrevArgs;

        public async void LoadProfileData(ProfilePageNavigationArgs args, bool force = false)
        {
            if (args == null)
                args = PrevArgs;
            else
                PrevArgs = args;

            if (args == null)
                return;
            if (_currUser == null || _currUser != args.TargetUser || force)
            {
                LoadingVisibility = Visibility.Visible;
                await
                    Task.Run(
                        async () =>
                            CurrentData = await new ProfileQuery(false, args?.TargetUser ?? "").GetProfileData(force));
                _currUser = args?.TargetUser ?? Credentials.UserName;
            }
            FavAnime = new List<AnimeItemViewModel>();
            FavManga = new List<AnimeItemViewModel>();
            RecentManga = new List<AnimeItemViewModel>();
            RecentAnime = new List<AnimeItemViewModel>();
            DesktopViewModelLocator.Main.CurrentStatus = $"{_currUser} - Profile";
            var authenticatedUser = args.TargetUser == Credentials.UserName;
            RaisePropertyChanged(() => CurrentData);
            LoadingVisibility = Visibility.Collapsed;
            RaisePropertyChanged(() => IsPinned);
            RaisePropertyChanged(() => PinProfileVisibility);
            MalComments = new ObservableCollection<MalComment>(CurrentData.Comments);
            if (authenticatedUser)
            {
                _initialized = true;
                var list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.FavouriteAnime)
                {
                    var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);

                    if (data != null)
                    {
                        list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                    }
                }
                FavAnime = list;
                list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.FavouriteManga)
                {
                    var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                    if (data != null)
                    {
                        list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                    }
                }
                FavManga = list;
                list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.RecentAnime)
                {
                    var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);
                    if (data != null)
                    {
                        list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                    }
                }
                RecentAnime = list;
                list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.RecentManga)
                {
                    var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                    if (data != null)
                    {
                        list.Add((data as AnimeItemViewModel).ParentAbstraction.ViewModel);
                    }
                }
                RecentManga = list;
            }
            else
            {
                if (!_othersAbstractions.ContainsKey(args.TargetUser ?? ""))
                {
                    LoadingOhersLibrariesProgressVisiblity = Visibility.Visible;
                    var data = new List<ILibraryData>();
                    await
                        Task.Run(
                            async () =>
                                data =
                                    await
                                        new LibraryListQuery(args.TargetUser, AnimeListWorkModes.Anime).GetLibrary(false));

                    var abstractions = new List<AnimeItemAbstraction>();
                    foreach (var libraryData in data)
                        abstractions.Add(new AnimeItemAbstraction(false, libraryData as AnimeLibraryItemData));

                    await
                        Task.Run(
                            async () =>
                                data =
                                    await
                                        new LibraryListQuery(args.TargetUser, AnimeListWorkModes.Manga).GetLibrary(false));

                    var mangaAbstractions = new List<AnimeItemAbstraction>();
                    foreach (
                        var libraryData in data)
                        mangaAbstractions.Add(new AnimeItemAbstraction(false, libraryData as MangaLibraryItemData));

                    _othersAbstractions.Add(args.TargetUser,
                        new Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>(abstractions,
                            mangaAbstractions));

                    LoadingOhersLibrariesProgressVisiblity = Visibility.Collapsed;
                }

                var source = _othersAbstractions[args.TargetUser];
                var list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.FavouriteAnime)
                {
                    var data = source.Item1.FirstOrDefault(abs => abs.Id == id);

                    if (data != null)
                    {
                        list.Add(data.ViewModel);
                    }
                }
                FavAnime = list;
                list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.FavouriteManga)
                {
                    var data = source.Item2.FirstOrDefault(abs => abs.Id == id);

                    if (data != null)
                    {
                        list.Add(data.ViewModel);
                    }
                }
                FavManga = list;
                list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.RecentAnime)
                {
                    var data = source.Item1.FirstOrDefault(abs => abs.Id == id);

                    if (data != null)
                    {
                        list.Add(data.ViewModel);
                    }
                }
                RecentAnime = list;
                list = new List<AnimeItemViewModel>();
                foreach (var id in CurrentData.RecentManga)
                {
                    var data = source.Item2.FirstOrDefault(abs => abs.Id == id);

                    if (data != null)
                    {
                        list.Add(data.ViewModel);
                    }
                }
                RecentManga = list;
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

            EmptyRecentAnimeNoticeVisibility = RecentAnime.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyRecentMangaNoticeVisibility = RecentManga.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyFavCharactersNoticeVisibility = CurrentData.FavouriteCharacters.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyFavAnimeNoticeVisibility = FavAnime.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyFavMangaNoticeVisibility = FavManga.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyFavPeopleNoticeVisibility = CurrentData.FavouritePeople.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void NavigateDetails(AnimeCharacter character)
        {
            DesktopViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(int.Parse(character.ShowId), character.Notes, null,
                    null)
                {
                    Source = PageIndex.PageProfile,
                    AnimeMode = character.FromAnime
                });
        }

        private async void NavigateCharacterWebPage(AnimeCharacter character)
        {
            await Launcher.LaunchUriAsync(new Uri($"http://myanimelist.net/character/{character.Id}"));
        }

        private async void NavigatePersonWebPage(AnimeStaffPerson person)
        {
            await Launcher.LaunchUriAsync(new Uri($"http://myanimelist.net/people/{person.Id}"));
        }

        #region Props

        private Visibility _emptyFavAnimeNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavCharactersNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavMangaNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavPeopleNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyRecentAnimeNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyRecentMangaNoticeVisibility = Visibility.Collapsed;
        private List<AnimeItemViewModel> _favAnime;
        private List<AnimeItemViewModel> _favManga;

        private bool _initialized;


        private Visibility _loadingVisibility = Visibility.Collapsed;

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
                            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(
                                new RelayCommand(
                                    () =>
                                    {
                                        DesktopViewModelLocator.Main.Navigate(PageIndex.PageProfile,
                                            new ProfilePageNavigationArgs {TargetUser = CurrentData.User.Name});
                                    }));

                        }));

        private ICommand _sendCommentCommand;

        public ICommand SendCommentCommand => _sendCommentCommand ?? (_sendCommentCommand = new RelayCommand(async () =>
        {
            if(string.IsNullOrEmpty(CommentText))
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

        public ICommand DeleteCommentCommand => _deleteCommentCommand ?? (_deleteCommentCommand = new RelayCommand<MalComment>(async comment =>
        {
            if (await ProfileCommentQueries.DeleteComment(comment.Id))
                MalComments.Remove(comment);
        }));

        private ICommand _navigateConversationCommand;

        public ICommand NavigateConversationCommand => _navigateConversationCommand ?? (_navigateConversationCommand = new RelayCommand<MalComment>(comment =>
        {
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMessageDetails,
                new MalMessageDetailsNavArgs {WorkMode = MessageDetailsWorkMode.ProfileComments, Arg = comment});
        }));

        private bool _refreshingComments;
        private ICommand _refreshCommentsCommand;

        public ICommand RefreshCommentsCommand => _refreshCommentsCommand ?? (_refreshCommentsCommand = new RelayCommand(async () =>
        {
            if(_refreshingComments)
                return;
            _refreshingComments = true;
            await CurrentData.UpdateComments();
            MalComments = new ObservableCollection<MalComment>(CurrentData.Comments);
            _refreshingComments = false;
        }));

        private List<AnimeItemViewModel> _recentAnime;
        private List<AnimeItemViewModel> _recentManga;

        public ProfilePageViewModel()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            MaxWidth = bounds.Width/2.2;
        }

        public ProfileData CurrentData { get; set; } = new ProfileData();

        public List<AnimeItemViewModel> RecentAnime
        {
            get { return _recentAnime; }
            private set
            {
                _recentAnime = value;
                RaisePropertyChanged(() => RecentAnime);
            }
        }

        public List<AnimeItemViewModel> RecentManga
        {
            get { return _recentManga; }
            private set
            {
                _recentManga = value;
                RaisePropertyChanged(() => RecentManga);
            }
        }

        public List<AnimeItemViewModel> FavAnime
        {
            get { return _favAnime; }
            private set
            {
                _favAnime = value;
                RaisePropertyChanged(() => FavAnime);
            }
        }

        public List<AnimeItemViewModel> FavManga
        {
            get { return _favManga; }
            private set
            {
                _favManga = value;
                RaisePropertyChanged(() => FavManga);
            }
        }

        public AnimeItemViewModel TemporarilySelectedAnimeItem
        {
            get { return null; }
            set { value?.NavigateDetails(PageIndex.PageProfile); }
        }

        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        private Visibility _authenticatedControlsVisibility;

        public Visibility AuthenticatedControlsVisibility
        {
            get { return _authenticatedControlsVisibility; }
            set
            {
                _authenticatedControlsVisibility = value;
                RaisePropertyChanged(() => AuthenticatedControlsVisibility);
            }
        }

        public List<int> AnimeChartValues
        {
            get { return _animeChartValues; }
            set
            {
                _animeChartValues = value;
                RaisePropertyChanged(() => AnimeChartValues);
            }
        }

        public List<int> MangaChartValues
        {
            get { return _mangaChartValues; }
            set
            {
                _mangaChartValues = value;
                RaisePropertyChanged(() => MangaChartValues);
            }
        }

        public bool IsPinned
        {
            get { return CurrentData.User.Name != null && Settings.PinnedProfiles.Contains(CurrentData.User.Name); }
            set
            {
                if (value)
                    Settings.PinnedProfiles += ";" + CurrentData.User.Name;
                else
                {
                    var pinned = Settings.PinnedProfiles.Split(';').ToList();
                    pinned.Remove(CurrentData.User.Name);
                    Settings.PinnedProfiles = string.Join(";",pinned);
                }
                DesktopViewModelLocator.Hamburger.UpdatePinnedProfiles();
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
                    ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(
                        new RelayCommand(
                            () =>
                            {
                                DesktopViewModelLocator.Main.Navigate(PageIndex.PageProfile,
                                    new ProfilePageNavigationArgs{TargetUser = CurrentData.User.Name});
                            }));
                    DesktopViewModelLocator.Main.Navigate(PageIndex.PageAnimeList,
                        new AnimeListPageNavigationArgs(0, AnimeListWorkModes.Anime) {ListSource = _currUser});
                }));

        public ICommand NavigateMangaListCommand
            =>
                _navMangaListCommand ??
                (_navMangaListCommand =
                    new RelayCommand(
                        () =>
                            DesktopViewModelLocator.Main.Navigate(PageIndex.PageAnimeList,
                                new AnimeListPageNavigationArgs(0, AnimeListWorkModes.Manga) {ListSource = _currUser})))
            ;

        public Visibility PinProfileVisibility
            => CurrentData.User.Name == null || Credentials.UserName == CurrentData.User.Name ? Visibility.Collapsed : Visibility.Visible;


        public Visibility EmptyFavAnimeNoticeVisibility
        {
            get { return _emptyFavAnimeNoticeVisibility; }
            set
            {
                _emptyFavAnimeNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavAnimeNoticeVisibility);
            }
        }

        public Visibility EmptyFavCharactersNoticeVisibility
        {
            get { return _emptyFavCharactersNoticeVisibility; }
            set
            {
                _emptyFavCharactersNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavCharactersNoticeVisibility);
            }
        }

        public Visibility EmptyFavMangaNoticeVisibility
        {
            get { return _emptyFavMangaNoticeVisibility; }
            set
            {
                _emptyFavMangaNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavMangaNoticeVisibility);
            }
        }

        public Visibility EmptyRecentMangaNoticeVisibility
        {
            get { return _emptyRecentMangaNoticeVisibility; }
            set
            {
                _emptyRecentMangaNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyRecentMangaNoticeVisibility);
            }
        }

        public Visibility EmptyRecentAnimeNoticeVisibility
        {
            get { return _emptyRecentAnimeNoticeVisibility; }
            set
            {
                _emptyRecentAnimeNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyRecentAnimeNoticeVisibility);
            }
        }

        public Visibility EmptyFavPeopleNoticeVisibility
        {
            get { return _emptyFavPeopleNoticeVisibility; }
            set
            {
                _emptyFavPeopleNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavPeopleNoticeVisibility);
            }
        }

        private Visibility _loadingOhersLibrariesProgressVisiblity = Visibility.Collapsed;

        public Visibility LoadingOhersLibrariesProgressVisiblity
        {
            get { return _loadingOhersLibrariesProgressVisiblity; }
            set
            {
                _loadingOhersLibrariesProgressVisiblity = value;
                RaisePropertyChanged(() => LoadingOhersLibrariesProgressVisiblity);
            }
        }

        private Visibility _loadingCommentsVisiblity = Visibility.Collapsed;

        public Visibility LoadingCommentsVisiblity
        {
            get { return _loadingCommentsVisiblity; }
            set
            {
                _loadingCommentsVisiblity = value;
                RaisePropertyChanged(() => LoadingCommentsVisiblity);
            }
        }

        private string _commentText;

        public string CommentText
        {
            get { return _commentText; }
            set
            {
                _commentText = value;
                RaisePropertyChanged(() => CommentText);
            }
        }

        private bool _isSendCommentButtonEnabled = true;

        public bool IsSendCommentButtonEnabled
        {
            get { return _isSendCommentButtonEnabled; }
            set
            {
                _isSendCommentButtonEnabled = value;
                RaisePropertyChanged(() => IsSendCommentButtonEnabled);
            }
        }

        #endregion
    }
}