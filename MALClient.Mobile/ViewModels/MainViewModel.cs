using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.UWP.Pages.Forums;
using MALClient.UWP.Pages.Main;
using MALClient.UWP.Pages.Messages;
using MALClient.UWP.Pages.Off;
using MALClient.UWP.Shared.ViewModels.Interfaces;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.UWP.ViewModels
{
    public class MainViewModel : MainViewModelBase
    {
        static MainViewModel()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            //var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            AnimeItemViewModel.MaxWidth = bounds.Width / 2.05;
            if (AnimeItemViewModel.MaxWidth > 200)
                AnimeItemViewModel.MaxWidth = 200;
        }

        public override event NavigationRequest MainNavigationRequested;


        public override async void Navigate(PageIndex index, object args = null)
        {
            PageIndex originalIndex = index;
            var wasOnSearchPage = SearchToggleLock;
            SearchToggleLock = false;
            if(View.CurrentDisplayMode == SplitViewDisplayMode.CompactOverlay)
                MenuPaneState = false;
            CurrentStatusSub = "";
            IsCurrentStatusSelectable = false;
            if (!Credentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                var msg = new MessageDialog("Log in first in order to access this page.");
                await msg.ShowAsync();
                return;
            }
            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.Navigated, index.ToString());
            ScrollToTopButtonVisibility = false;
            RefreshButtonVisibility = false;

            if (index == PageIndex.PageMangaList && args == null) // navigating from startup
                args = AnimeListPageNavigationArgs.Manga;

            if (index == PageIndex.PageSeasonal || //index is always angry btw
                index == PageIndex.PageMangaList ||
                index == PageIndex.PageTopManga ||
                index == PageIndex.PageTopAnime)
                index = PageIndex.PageAnimeList;

            MobileViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageAnimeList || //and she likes to bit certain individual
                                                                    index == PageIndex.PageMessanging || 
                                                                    index == PageIndex.PageSearch || 
                                                                    index == PageIndex.PageWallpapers);

            if (index == PageIndex.PageAnimeList && _searchStateBeforeNavigatingToSearch != null)
            {
                SearchToggleStatus = (bool) _searchStateBeforeNavigatingToSearch;
                if (SearchToggleStatus)
                    ShowSearchStuff();
                else
                    HideSearchStuff();
            }

            ResetSearchFilter();
            switch (index) //his head specifficaly
            {
                case PageIndex.PageAnimeList:
                    if (MobileViewModelLocator.AnimeList.Initializing)
                    {
                        if (!_subscribed)
                        {
                            MobileViewModelLocator.AnimeList.Initialized += AnimeListOnInitialized;
                            _subscribed = true;
                        }
                        _postponedNavigationArgs = new Tuple<PageIndex, object>(originalIndex, args);
                        return;
                    }
                    MobileViewModelLocator.Hamburger.SetActiveButton(HamburgerButtons.AnimeList);
                    ShowSearchStuff();
                    if ((_searchStateBeforeNavigatingToSearch == null || !_searchStateBeforeNavigatingToSearch.Value) &&
                        (wasOnSearchPage || _wasOnDetailsFromSearch))
                    {
                        CurrentSearchQuery = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }
                    if (CurrentMainPage == PageIndex.PageAnimeList)
                        MobileViewModelLocator.AnimeList.Init(args as AnimeListPageNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(typeof(AnimeListPage), args);
                    break;
                case PageIndex.PageAnimeDetails:
                    HideSearchStuff();
                    var detail = ViewModelLocator.AnimeDetails;
                    detail.DetailImage = null;
                    detail.LeftDetailsRow.Clear();
                    detail.RightDetailsRow.Clear();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.AnimeDetails.RefreshData());
                    _wasOnDetailsFromSearch = (args as AnimeDetailsPageNavigationArgs).Source == PageIndex.PageSearch;
                    //from search , details are passed instead of being downloaded once more
                    if (CurrentMainPage == PageIndex.PageAnimeDetails)
                        ViewModelLocator.AnimeDetails.Init(args as AnimeDetailsPageNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(typeof(AnimeDetailsPage), args);
                    break;
                case PageIndex.PageSettings:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(typeof(SettingsPage));
                    break;
                case PageIndex.PageSearch:
                case PageIndex.PageMangaSearch:
                    if(CurrentMainPage != PageIndex.PageSearch && CurrentMainPage != PageIndex.PageMangaSearch && CurrentMainPage != PageIndex.PageCharacterSearch)
                        _searchStateBeforeNavigatingToSearch = SearchToggleStatus;

                    var searchArg = args as SearchPageNavigationArgs;
                    if (string.IsNullOrWhiteSpace(searchArg.Query))
                    {
                        searchArg.Query = CurrentSearchQuery;
                    }
                    if (!searchArg.ByGenre && !searchArg.ByStudio)
                    {
                        View.SearchInputFocus(FocusState.Keyboard);
                        SearchToggleLock = true;
                        ShowSearchStuff();
                        ToggleSearchStuff();
                    }
                    else
                    {
                        HideSearchStuff();
                        CurrentStatus = searchArg.ByGenre ? "Anime by Genre" : "Anime By Studio";
                    }
                    MainNavigationRequested?.Invoke(typeof(AnimeSearchPage), args);
                    break;
                case PageIndex.PageLogIn:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(typeof(LogInPage));
                    break;
                case PageIndex.PageProfile:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    if (Settings.SelectedApiType == ApiType.Mal)
                        RefreshDataCommand =
                            new RelayCommand(() => ViewModelLocator.ProfilePage.LoadProfileData(null, true));
                    else
                        RefreshDataCommand = new RelayCommand(() => ViewModelLocator.HumProfilePage.Init(true));
                    if (Settings.SelectedApiType == ApiType.Mal)
                    {
                        if (CurrentMainPage == PageIndex.PageProfile)
                            MobileViewModelLocator.ProfilePage.LoadProfileData(args as ProfilePageNavigationArgs);
                        else
                            MainNavigationRequested?.Invoke(typeof(ProfilePage), args);
                    }
                    else
                        MainNavigationRequested?.Invoke(typeof(HummingbirdProfilePage), args);
                    break;
                case PageIndex.PageRecomendations:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(typeof(RecommendationsPage), args);
                    break;
                case PageIndex.PageCalendar:
                    HideSearchStuff();
                    CurrentStatus = "Calendar";
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.CalendarPage.Init(true); });
                    MainNavigationRequested?.Invoke(typeof(CalendarPage), args);
                    break;
                case PageIndex.PageArticles:
                case PageIndex.PageNews:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.MalArticles.Init(null));
                    if (CurrentMainPage == PageIndex.PageArticles)
                        ViewModelLocator.MalArticles.Init(args as MalArticlesPageNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(typeof(MalArticlesPage), args);
                    break;
                case PageIndex.PageMessanging:
                    HideSearchStuff();
                    CurrentStatus = $"{Credentials.UserName} - Messages";
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.MalMessaging.Init(true); });
                    MainNavigationRequested?.Invoke(typeof(MalMessagingPage), args);
                    break;
                case PageIndex.PageMessageDetails:
                    var msgModel = args as MalMessageDetailsNavArgs;
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.MalMessageDetails.RefreshData());
                    CurrentOffStatus = msgModel.WorkMode == MessageDetailsWorkMode.Message
                        ? (msgModel.Arg != null
                            ? $"{(msgModel.Arg as MalMessageModel)?.Sender} - {(msgModel.Arg as MalMessageModel)?.Subject}"
                            : "New Message")
                        : $"Comments {Credentials.UserName} - {(msgModel.Arg as MalComment)?.User.Name}";
                    MainNavigationRequested?.Invoke(typeof(MalMessageDetailsPage), args);
                    break;
                case PageIndex.PageForumIndex:
                    HideSearchStuff();
                    CurrentStatus = "Forums";
                    if (args == null || (args as ForumsNavigationArgs)?.Page == ForumsPageIndex.PageIndex)
                    {
                            RefreshButtonVisibility = true;
                            RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.ForumsIndex.Init(true); });
                    }
                    else
                    {
                        var navArgs = args as ForumsNavigationArgs;
                        if (navArgs?.Page == ForumsPageIndex.PageBoard)
                        {
                            RefreshButtonVisibility = true;
                            RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.ForumsBoard.Reload(); });
                        }
                        else if (navArgs?.Page == ForumsPageIndex.PageTopic)
                        {
                            RefreshButtonVisibility = true;
                            RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.ForumsTopic.Reload(); });
                        }
                    }
                    if (CurrentMainPage == PageIndex.PageForumIndex)
                        ViewModelLocator.ForumsMain.Init(args as ForumsNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(typeof(ForumsMainPage), args);
                    break;
                case PageIndex.PageHistory:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.History.Init(null, true); });
                    CurrentStatus = $"History - {(args as HistoryNavigationArgs)?.Source ?? Credentials.UserName}";
                    MainNavigationRequested?.Invoke(typeof(HistoryPage), args);
                    break;
                case PageIndex.PageCharacterDetails:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.CharacterDetails.RefreshData());
                    OffContentVisibility = true;

                    if (CurrentOffPage == PageIndex.PageCharacterDetails)
                        ViewModelLocator.CharacterDetails.Init(args as CharacterDetailsNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(typeof(CharacterDetailsPage), args);
                    break;
                case PageIndex.PageStaffDetails:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.StaffDetails.RefreshData());
                    OffContentVisibility = true;

                    if (CurrentOffPage == PageIndex.PageStaffDetails)
                        ViewModelLocator.StaffDetails.Init(args as StaffDetailsNaviagtionArgs);
                    else
                        MainNavigationRequested?.Invoke(typeof(StaffDetailsPage), args);
                    break;
                case PageIndex.PageCharacterSearch:
                    if (CurrentMainPage != PageIndex.PageSearch && CurrentMainPage != PageIndex.PageMangaSearch && CurrentMainPage != PageIndex.PageCharacterSearch)
                        _searchStateBeforeNavigatingToSearch = SearchToggleStatus;
                    ShowSearchStuff();
                    ToggleSearchStuff();

                    SearchToggleLock = true;

                    MainNavigationRequested?.Invoke(typeof(CharacterSearchPage));
                    await Task.Delay(10);
                    View.SearchInputFocus(FocusState.Keyboard);
                    break;
                case PageIndex.PageWallpapers:
                    HideSearchStuff();
                    RefreshButtonVisibility = false;
                    MainNavigationRequested?.Invoke(typeof(WallpapersPage),args);
                    break;
                case PageIndex.PagePopularVideos:
                    HideSearchStuff();
                    CurrentStatus = "Popular Videos";
                    MainNavigationRequested?.Invoke(typeof(PopularVideosPage), args);
                    break;
                case PageIndex.PageFeeds:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.FriendsFeeds.Init(true));
                    CurrentStatus = "Friends Feeds";
                    MainNavigationRequested?.Invoke(typeof(FriendsFeedsPage), args);
                    break;
                case PageIndex.PageNotificationHub:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.NotificationsHub.Init(true));
                    CurrentStatus = "Notifications";
                    MainNavigationRequested?.Invoke(typeof(NotificationsHubPage), args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            CurrentMainPage = index;
            CurrentMainPageKind = index;
            RaisePropertyChanged(() => SearchToggleLock);


        }
        #region PropertyPairs

       

        private IMainViewInteractions _view;

        public IMainViewInteractions View
        {
            get { return _view; }
            set
            {
                _view = value;
                Navigate(Credentials.Authenticated
                    ? (Settings.DefaultMenuTab == "anime" ? PageIndex.PageAnimeList : PageIndex.PageMangaList)
                    : PageIndex.PageLogIn);
                if (InitDetails != null || InitDetailsFull != null)
                    MobileViewModelLocator.AnimeList.Initialized += AnimeListOnInitializedLoadArgs;
            }
        }

          
        private ICommand _changeStatusCommand;

        public ICommand ChangeStatusCommand
        {
            get
            {
                return _changeStatusCommand ??
                       (_changeStatusCommand =
                           new RelayCommand<string>(
                               s => MobileViewModelLocator.AnimeList.StatusSelectorSelectedIndex = int.Parse(s)));
            }
        }




        #endregion

        protected override void CurrentStatusStoryboardBegin()
        {
            View.CurrentStatusStoryboard.Begin();
        }

        protected override void CurrentOffSubStatusStoryboardBegin()
        {
            View.CurrentOffSubStatusStoryboard.Begin();
        }

        protected override void CurrentOffStatusStoryboardBegin()
        {
            View.CurrentStatusStoryboard.Begin();
        }

        public override string CurrentOffStatus
        {
            get { return CurrentStatus; }
            set { CurrentStatus = value; }
        }

        public override ICommand RefreshOffDataCommand
        {
            get { return RefreshDataCommand; }
            set { RefreshDataCommand = value; }
        }
    }
}