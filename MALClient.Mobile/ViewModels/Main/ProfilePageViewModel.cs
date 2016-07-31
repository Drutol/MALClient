using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

namespace MALClient.ViewModels.Main
{
    public sealed class ProfilePageViewModel : ViewModelBase
    {
        //anime -<>- manga
        private readonly Dictionary<string, Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>>
            _othersAbstractions =
                new Dictionary<string, Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>>();

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
        private bool _authenticatedUser;

        public string CurrentUser { get; private set; }

        public async void LoadProfileData(ProfilePageNavigationArgs args, bool force = false)
        {
            if (CurrentUser == null || CurrentUser != args?.TargetUser || force)
            {
                LoadingVisibility = Visibility.Visible;
                await
                    Task.Run(
                        async () =>
                            CurrentData = await new ProfileQuery(false, args?.TargetUser ?? "").GetProfileData(force));
                CurrentUser = args?.TargetUser ?? Credentials.UserName;
            }
            _authenticatedUser = args == null || args.TargetUser == Credentials.UserName;
            MobileViewModelLocator.Main.CurrentStatus = $"{CurrentUser} - Profile";
            _loadedFavManga = false;
            _loadedFavAnime = false;
            _loadedRecent = false;
            _loadedStats = false;
            FavAnime = new List<AnimeItemViewModel>();
            FavManga = new List<AnimeItemViewModel>();
            RecentAnime = new List<AnimeItemViewModel>();
            RecentManga = new List<AnimeItemViewModel>();
            RaisePropertyChanged(() => IsPinned);
            RaisePropertyChanged(() => PinProfileVisibility);
            MalComments = new ObservableCollection<MalComment>(CurrentData.Comments);
            CommentInputBoxVisibility = string.IsNullOrEmpty(CurrentData.ProfileMemId) ? Visibility.Collapsed : Visibility.Visible; //posting restricted
            EmptyFavCharactersNoticeVisibility = CurrentData.FavouriteCharacters.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            EmptyFavPeopleNoticeVisibility = CurrentData.FavouritePeople.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            RaisePropertyChanged(() => CurrentData);
            LoadingVisibility = Visibility.Collapsed;
            _initialized = true;
        }

        private async void InnerPivotItemChanged(string tag)
        {
            if (!_initialized)
                return;

            switch (tag)
            {
                case "Anime":
                    if (_loadedFavAnime)
                        break;

                    var list = new List<AnimeItemViewModel>();
                    if (_authenticatedUser)
                    {
                        foreach (var id in CurrentData.FavouriteAnime)
                        {
                            var data = await MobileViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);
                            if (data != null)
                            {
                                list.Add(data as AnimeItemViewModel);
                            }
                        }
                    }
                    else
                    {
                        if (!_othersAbstractions.ContainsKey(CurrentUser))
                            return;
                        var source = _othersAbstractions[CurrentUser]; //loaded by outer pivot
                        foreach (var id in CurrentData.FavouriteAnime)
                        {
                            var data = source.Item1.FirstOrDefault(abs => abs.Id == id);
                            if (data != null)
                            {
                                list.Add(data.ViewModel);
                            }
                        }
                    }
                    FavAnime = list;
                    _loadedFavAnime = true;
                    EmptyFavAnimeNoticeVisibility = FavAnime.Count == 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;
                case "Manga":
                    if (_loadedFavManga)
                        break;
                    var mlist = new List<AnimeItemViewModel>();
                    if (_authenticatedUser)
                    {
                        foreach (var id in CurrentData.FavouriteManga)
                        {
                            var data =
                                await MobileViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                            if (data != null)
                            {
                                mlist.Add(data as AnimeItemViewModel);
                            }
                        }
                    }
                    else
                    {
                        if (!_othersAbstractions.ContainsKey(CurrentUser))
                            return;
                        var source = _othersAbstractions[CurrentUser]; //loaded by outer pivot
                        foreach (var id in CurrentData.FavouriteManga)
                        {
                            var data = source.Item2.FirstOrDefault(abs => abs.Id == id);
                            if (data != null)
                            {
                                mlist.Add(data.ViewModel);
                            }
                        }
                    }
                    FavManga = mlist;
                    _loadedFavManga = true;
                    EmptyFavMangaNoticeVisibility = FavManga.Count == 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;
            }
        }

        private async void OuterPivotItemChanged(string tag)
        {
            if (!_initialized)
                return;
            if (!_authenticatedUser && (tag == "Recent" || tag == "Favs"))
            {
                if (!_othersAbstractions.ContainsKey(CurrentUser ?? ""))
                {
                    LoadingOhersLibrariesProgressVisiblity = Visibility.Visible;
                    var sb = StatusBar.GetForCurrentView().ProgressIndicator;
                    sb.Text = "Fetching user's library.";
                    sb.ProgressValue = null;
                    var data = new List<ILibraryData>();
                    await
                        Task.Run(
                            async () =>
                                data =
                                    await new LibraryListQuery(CurrentUser, AnimeListWorkModes.Anime).GetLibrary(false));
                    var abstractions = new List<AnimeItemAbstraction>();
                    foreach (
                        var libraryData in
                            data.Where(
                                entry =>
                                    CurrentData.FavouriteAnime.Any(i => i == entry.Id) ||
                                    CurrentData.RecentAnime.Any(i => i == entry.Id)))
                        abstractions.Add(new AnimeItemAbstraction(false, libraryData as AnimeLibraryItemData));
                    await
                        Task.Run(
                            async () =>
                                data =
                                    data =
                                        await
                                            new LibraryListQuery(CurrentUser, AnimeListWorkModes.Manga).GetLibrary(false));
                    var mangaAbstractions = new List<AnimeItemAbstraction>();
                    foreach (
                        var libraryData in
                            data.Where(
                                entry =>
                                    CurrentData.FavouriteManga.Any(i => i == entry.Id) ||
                                    CurrentData.RecentManga.Any(i => i == entry.Id)))
                        mangaAbstractions.Add(new AnimeItemAbstraction(false, libraryData as MangaLibraryItemData));
                    try
                    {
                        _othersAbstractions.Add(CurrentUser,
                            new Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>(abstractions,
                                mangaAbstractions));
                    }
                    catch (Exception)
                    {
                        //oddity od duplicate
                    }

                    await sb.HideAsync();
                    LoadingOhersLibrariesProgressVisiblity = Visibility.Collapsed;
                }
            }
            switch (tag)
            {
                case "Favs":
                    InnerPivotItemChanged("Anime");
                    break;
                case "Recent":
                    if (_loadedRecent)
                        break;


                    if (!_authenticatedUser)
                    {
                        if (!_othersAbstractions.ContainsKey(CurrentUser))
                            return;
                        var list = new List<AnimeItemViewModel>();
                        var source = _othersAbstractions[CurrentUser];
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
                    else
                    {
                        var list = new List<AnimeItemViewModel>();
                        foreach (var id in CurrentData.RecentAnime)
                        {
                            var data = await MobileViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);
                            if (data != null)
                            {
                                list.Add(data as AnimeItemViewModel);
                            }
                        }
                        RecentAnime = list;
                        list = new List<AnimeItemViewModel>();
                        foreach (var id in CurrentData.RecentManga)
                        {
                            var data =
                                await MobileViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                            if (data != null)
                            {
                                list.Add(data as AnimeItemViewModel);
                            }
                        }
                        RecentManga = list;
                    }
                    _loadedRecent = true;
                    EmptyRecentAnimeNoticeVisibility = RecentAnime.Count == 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    EmptyRecentMangaNoticeVisibility = RecentManga.Count == 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;
                case "Stats":
                    if (_loadedStats)
                        return;
                    _loadedStats = true;
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
                    break;
            }
        }

        private void NavigateDetails(AnimeCharacter character)
        {
            MobileViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(int.Parse(character.ShowId), character.Notes, null,
                    null,
                    new ProfilePageNavigationArgs
                    {
                        TargetUser = CurrentUser
                    })
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

        #region Properties

        private Visibility _emptyFavAnimeNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavCharactersNoticeVisibility = Visibility.Collapsed;
        private Visibility _emptyFavMangaNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavPeopleNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyRecentAnimeNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyRecentMangaNoticeVisibility = Visibility.Collapsed;
        private List<AnimeItemViewModel> _favAnime = new List<AnimeItemViewModel>();
        private List<AnimeItemViewModel> _favManga = new List<AnimeItemViewModel>();


        private bool _initialized;
        private bool _loadedFavAnime;
        private bool _loadedFavManga;
        private bool _loadedRecent;
        private bool _loadedStats;

        private Visibility _loadingVisibility = Visibility.Collapsed;

        private List<int> _mangaChartValues = new List<int>();

        private ICommand _navigateCharPageCommand;

        private ICommand _navigateDetailsCommand;

        private ICommand _navigatePersonPageCommand;

        private List<AnimeItemViewModel> _recentAnime;
        private List<AnimeItemViewModel> _recentManga;

        public ProfilePageViewModel()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            MaxWidth = bounds.Width/2.2;
        }

        //public ProfilePage View { get; set; }

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


        public PivotItem CurrentlySelectedOuterPivotItem
        {
            get { return null; }
            set
            {
                //RaisePropertyChanged(() => CurrentlySelectedOuterPivotItem);
                OuterPivotItemChanged(value.Tag as string);
            }
        }

        public PivotItem CurrentlySelectedInnerPivotItem
        {
            get { return null; }
            set
            {
                //RaisePropertyChanged(() => CurrentlySelectedInnerPivotItem);
                InnerPivotItemChanged(value.Tag as string);
            }
        }

        public AnimeItemViewModel TemporarilySelectedAnimeItem
        {
            get { return null; }
            set
            {
                value?.NavigateDetails(PageIndex.PageProfile,
                    new ProfilePageNavigationArgs
                    {
                        TargetUser = CurrentUser
                    });
            }
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

        public int CurrentlySelectedOuterPivotIndex
        {
            get { return 0; }
            set { RaisePropertyChanged(() => CurrentlySelectedOuterPivotIndex); }
        }

        public int CurrentlySelectedInnerPivotIndex
        {
            get { return 0; }
            set { RaisePropertyChanged(() => CurrentlySelectedInnerPivotIndex); }
        }

        private Visibility _commentInputBoxVisibility;

        public Visibility CommentInputBoxVisibility
        {
            get { return _commentInputBoxVisibility; }
            set
            {
                _commentInputBoxVisibility = value;
                RaisePropertyChanged(() => CommentInputBoxVisibility);
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

        private ICommand _navAnimeListCommand;
        private ICommand _navMangaListCommand;

        public ICommand NavigateAnimeListCommand
            =>
                _navAnimeListCommand ??
                (_navAnimeListCommand =
                    new RelayCommand(
                        () =>
                            MobileViewModelLocator.Main.Navigate(PageIndex.PageAnimeList,
                                new AnimeListPageNavigationArgs(0, AnimeListWorkModes.Anime) {ListSource = CurrentUser})))
            ;

        public ICommand NavigateMangaListCommand
            =>
                _navMangaListCommand ??
                (_navMangaListCommand =
                    new RelayCommand(
                        () =>
                            MobileViewModelLocator.Main.Navigate(PageIndex.PageAnimeList,
                                new AnimeListPageNavigationArgs(0, AnimeListWorkModes.Manga) {ListSource = CurrentUser})))
            ;

        private ICommand _navigateHistoryCommand;

        public ICommand NavigateHistoryCommand
            =>
                _navigateHistoryCommand ??
                (_navigateHistoryCommand =
                    new RelayCommand(
                        () =>
                        {
                            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile,
                                            new ProfilePageNavigationArgs { TargetUser = CurrentData.User.Name });
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageHistory,
                                new HistoryNavigationArgs { Source = CurrentData.User.Name });

                        }));

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

        public ICommand DeleteCommentCommand => _deleteCommentCommand ?? (_deleteCommentCommand = new RelayCommand<MalComment>(async comment =>
        {
            if (await ProfileCommentQueries.DeleteComment(comment.Id))
            {
                MalComments.Remove(comment);
                var data = CurrentData;
                data.Comments = MalComments.ToList();
                DataCache.SaveProfileData(CurrentUser, data);
            }

        }));

        private ICommand _navigateConversationCommand;

        public ICommand NavigateConversationCommand => _navigateConversationCommand ?? (_navigateConversationCommand = new RelayCommand<MalComment>(comment =>
        {
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMessageDetails,
                new MalMessageDetailsNavArgs { WorkMode = MessageDetailsWorkMode.ProfileComments, Arg = comment });
        }));

        private bool _refreshingComments;
        private ICommand _refreshCommentsCommand;

        public ICommand RefreshCommentsCommand => _refreshCommentsCommand ?? (_refreshCommentsCommand = new RelayCommand(async () =>
        {
            if (_refreshingComments)
                return;
            LoadingCommentsVisiblity = Visibility.Visible;
            _refreshingComments = true;
            await CurrentData.UpdateComments();
            MalComments = new ObservableCollection<MalComment>(CurrentData.Comments);
            _refreshingComments = false;
            LoadingCommentsVisiblity = Visibility.Collapsed;
        }));


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
                    Settings.PinnedProfiles = string.Join(";", pinned);
                }
                MobileViewModelLocator.Hamburger.UpdatePinnedProfiles();
                RaisePropertyChanged(() => IsPinned);
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

        public Visibility PinProfileVisibility
            => CurrentData.User.Name == null || Credentials.UserName == CurrentData.User.Name ? Visibility.Collapsed : Visibility.Visible;

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

        #endregion
    }
}