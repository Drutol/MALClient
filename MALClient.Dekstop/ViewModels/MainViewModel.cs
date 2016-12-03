using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Shared.ViewModels;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.Pages;
using MALClient.Pages.Forums;
using MALClient.Pages.Main;
using MALClient.Pages.Messages;
using MALClient.Pages.Off;
using MALClient.Shared.Managers;
using MALClient.Shared.ViewModels.Interfaces;
using MALClient.UserControls;
using MALClient.Utils.Managers;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.ViewModels
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
        public override event NavigationRequest OffNavigationRequested;

        public override async void Navigate(PageIndex index, object args = null)
        {
            PageIndex? currPage = null;
            PageIndex? currOffPage = null;
            var mainPage = true;
            var originalIndex = index;
            var wasOnSearchPage = SearchToggleLock;
            if (!Credentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                var msg = new MessageDialog("Log in first in order to access this page."
                    ,"Login required.");
                await msg.ShowAsync();
                return;
            }
            _navigating = true;
            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.Navigated, index.ToString());
           

            DesktopViewModelLocator.Hamburger.UpdateAnimeFiltersSelectedIndex();
            //prepare for some index mess
            if (index == PageIndex.PageMangaList && args == null) // navigating from startup
                args = AnimeListPageNavigationArgs.Manga;

            if (index == PageIndex.PageAbout ||
                index == PageIndex.PageSettings ||
                index == PageIndex.PageAbout ||
                index == PageIndex.PageAnimeDetails ||
                index == PageIndex.PageMessageDetails ||
                index == PageIndex.PageCharacterDetails ||
                index == PageIndex.PageStaffDetails ||
                index == PageIndex.PageLogIn)
            {
                OffRefreshButtonVisibility = false;
                mainPage = false;
                IsCurrentStatusSelectable = false;
                currOffPage = index;
                if (index != PageIndex.PageAnimeDetails)
                {
                    ViewModelLocator.AnimeDetails.Id = 0; //reset this because we no longer are there
                    if(index != PageIndex.PageCharacterDetails && index != PageIndex.PageStaffDetails)
                        ViewModelLocator.NavMgr.ResetOffBackNav();
                }
                if(CurrentOffPage == PageIndex.PageSettings)
                    ViewModelLocator.NavMgr.ResetOffBackNav();
            }
            else //naviagating main page
            {
                ScrollToTopButtonVisibility = false;
                RefreshButtonVisibility = false;
                ResetSearchFilter();
                SearchToggleLock = false;
                CurrentHintSet = null;
            }

            switch (index)
            {
                case PageIndex.PageSeasonal:
                case PageIndex.PageMangaList:
                case PageIndex.PageTopManga:
                case PageIndex.PageTopAnime:
                case PageIndex.PageAnimeList:
                    if (index == PageIndex.PageSeasonal || index == PageIndex.PageTopAnime ||
                        index == PageIndex.PageTopManga || index == PageIndex.PageMangaList)
                        currPage = index; //used by hamburger's filters
                    else
                        currPage = PageIndex.PageAnimeList;
                    DesktopViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(true);
                    index = PageIndex.PageAnimeList;
                    break;
                case PageIndex.PageSearch:
                case PageIndex.PageRecomendations:
                case PageIndex.PageProfile:
                case PageIndex.PageMangaSearch:
                case PageIndex.PageCalendar:
                case PageIndex.PageArticles:
                case PageIndex.PageNews:
                case PageIndex.PageMessanging:
                case PageIndex.PageForumIndex:
                case PageIndex.PageHistory:
                case PageIndex.PageCharacterSearch:
                case PageIndex.PageWallpapers:
                case PageIndex.PagePopularVideos:

                    if (index == PageIndex.PageSearch || index == PageIndex.PageMangaSearch ||
                        ((index == PageIndex.PageSearch || index == PageIndex.PageMangaSearch) &&
                         CurrentOffPage == PageIndex.PageSearch))
                    {
                        var arg = args as SearchPageNavigationArgs;
                        if (Settings.ForceSearchIntoOffPage || CurrentMainPage == PageIndex.PageForumIndex ||
                            CurrentMainPage == PageIndex.PageProfile || CurrentOffPage == PageIndex.PageSearch)
                        {
                            arg.DisplayMode = SearchPageDisplayModes.Off;
                            args = arg;
                        }
                        if ((args as SearchPageNavArgsBase).DisplayMode == SearchPageDisplayModes.Main)
                        {
                            CurrentStatusSub = "";
                            DesktopViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageMessanging || index == PageIndex.PageWallpapers);
                        }
                    }
                    else
                    {
                        CurrentStatusSub = "";
                        DesktopViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageMessanging || index == PageIndex.PageWallpapers);
                    }
                    currPage = index;
                    break;
            }            

            if (index == PageIndex.PageAnimeList && _searchStateBeforeNavigatingToSearch != null)
            {
                SearchToggleStatus = (bool) _searchStateBeforeNavigatingToSearch;
                if (SearchToggleStatus)
                    ShowSearchStuff();
                else
                    HideSearchStuff();
            }

            switch (index)
            {
                case PageIndex.PageAnimeList:
                    if (ViewModelLocator.AnimeList.Initializing)
                    {
                        if (!_subscribed)
                        {
                            ViewModelLocator.AnimeList.Initialized += AnimeListOnInitialized;
                            _subscribed = true;
                        }
                        _postponedNavigationArgs = new Tuple<PageIndex, object>(originalIndex, args);
                        return;
                    }
                    _postponedNavigationArgs = null;
                    ShowSearchStuff();
                    if ((_searchStateBeforeNavigatingToSearch == null || !_searchStateBeforeNavigatingToSearch.Value) &&
                        (wasOnSearchPage || _wasOnDetailsFromSearch))
                    {
                        CurrentSearchQuery = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }
                    if (CurrentMainPageKind == PageIndex.PageAnimeList)
                        ViewModelLocator.AnimeList.Init(args as AnimeListPageNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(typeof(AnimeListPage), args);
                    break;
                case PageIndex.PageAnimeDetails:
                    var detail = ViewModelLocator.AnimeDetails;
                    detail.DetailImage = null;
                    detail.LeftDetailsRow.Clear();
                    detail.RightDetailsRow.Clear();
                    OffRefreshButtonVisibility = true;
                    RefreshOffDataCommand = new RelayCommand(() => ViewModelLocator.AnimeDetails.RefreshData());
                    _wasOnDetailsFromSearch = (args as AnimeDetailsPageNavigationArgs).Source == PageIndex.PageSearch;
                    //from search , details are passed instead of being downloaded once more
                    OffContentVisibility = true;

                    if (CurrentOffPage == PageIndex.PageAnimeDetails)
                        ViewModelLocator.AnimeDetails.Init(args as AnimeDetailsPageNavigationArgs);
                    else
                        OffNavigationRequested?.Invoke(typeof(AnimeDetailsPage), args);
                    break;
                case PageIndex.PageSettings:
                    OffContentVisibility = true;
                    OffNavigationRequested?.Invoke(typeof(SettingsPage));
                    break;
                case PageIndex.PageSearch:
                case PageIndex.PageMangaSearch:
                    var arg = args as SearchPageNavigationArgs;
                    if (Settings.ForceSearchIntoOffPage || CurrentMainPage == PageIndex.PageForumIndex || CurrentMainPage == PageIndex.PageProfile)
                        arg.DisplayMode = SearchPageDisplayModes.Off;
                    if (arg.DisplayMode == SearchPageDisplayModes.Off)
                    {
                        if (CurrentMainPage == PageIndex.PageCharacterSearch)
                        {
                            ToggleSearchStuff();
                            SearchToggleLock = true;
                        }
                        if (CurrentMainPage == PageIndex.PageSearch)
                        {
                            break; // we are already on the left
                        }
                        if (string.IsNullOrWhiteSpace(arg.Query))
                            arg.Query = CurrentSearchQuery;
                        arg.DisplayMode = SearchPageDisplayModes.Off;
                        OffContentVisibility = true;
                        currOffPage = PageIndex.PageSearch;
                        currPage = null;
                        OffRefreshButtonVisibility = false;
                        ViewModelLocator.AnimeDetails.Id = -1;
                        StatusFilterVisibilityLock = false;
                        CurrentOffStatus = "Search";
                        IsCurrentStatusSelectable = false;
                        OffNavigationRequested?.Invoke(typeof(AnimeSearchPage), args);
                    }
                    else
                    {
                        if (CurrentOffPage == PageIndex.PageSearch)
                        {
                            break; // we are already on the right
                        }
                        if (CurrentMainPage.Value != PageIndex.PageSearch &&
                            CurrentMainPage.Value != PageIndex.PageMangaSearch &&
                            CurrentMainPage.Value != PageIndex.PageCharacterSearch)
                            _searchStateBeforeNavigatingToSearch = SearchToggleStatus;
                        SearchToggleLock = true;
                        ShowSearchStuff();
                        ToggleSearchStuff();
                        StatusFilterVisibilityLock = true;
                        if (string.IsNullOrWhiteSpace(arg.Query))
                            arg.Query = CurrentSearchQuery;
                        MainNavigationRequested?.Invoke(typeof(AnimeSearchPage), args);
                        await Task.Delay(10);
                        View.SearchInputFocus(FocusState.Keyboard);
                    }
                    break;
                case PageIndex.PageLogIn:
                    OffContentVisibility = true;
                    OffNavigationRequested?.Invoke(typeof(LogInPage));
                    break;
                case PageIndex.PageProfile:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    if (Settings.SelectedApiType == ApiType.Mal)
                        RefreshDataCommand =
                            new RelayCommand(() => DesktopViewModelLocator.ProfilePage.LoadProfileData(null, true));
                    else
                        RefreshDataCommand = new RelayCommand(() => ViewModelLocator.HumProfilePage.Init(true));
                    if (Settings.SelectedApiType == ApiType.Mal)
                    {
                        if (CurrentMainPage == PageIndex.PageProfile)
                            DesktopViewModelLocator.ProfilePage.LoadProfileData(args as ProfilePageNavigationArgs);
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
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.CalendarPage.Init(true));
                    CurrentStatus = "Calendar";
                    MainNavigationRequested?.Invoke(typeof(CalendarPage), args);
                    break;
                case PageIndex.PageArticles:
                case PageIndex.PageNews:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.MalArticles.Init(null); });
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
                    OffRefreshButtonVisibility = true;
                    RefreshOffDataCommand = new RelayCommand(() => ViewModelLocator.MalMessageDetails.RefreshData());
                    CurrentOffStatus = msgModel.WorkMode == MessageDetailsWorkMode.Message
                        ? (msgModel.Arg != null
                            ? $"{(msgModel.Arg as MalMessageModel)?.Sender} - {(msgModel.Arg as MalMessageModel)?.Subject}"
                            : "New Message")
                        : $"Comments {Credentials.UserName} - {(msgModel.Arg as MalComment)?.User.Name}";
                    OffContentVisibility = true;
                    OffNavigationRequested?.Invoke(typeof(MalMessageDetailsPage), args);
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
                    }
                    if (CurrentMainPage != null && CurrentMainPage == PageIndex.PageForumIndex)
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
                    OffRefreshButtonVisibility = true;
                    RefreshOffDataCommand = new RelayCommand(() => ViewModelLocator.CharacterDetails.RefreshData());
                    OffContentVisibility = true;

                    if (CurrentOffPage == PageIndex.PageCharacterDetails)
                        ViewModelLocator.CharacterDetails.Init(args as CharacterDetailsNavigationArgs);
                    else
                        OffNavigationRequested?.Invoke(typeof(CharacterDetailsPage), args);
                    break;
                case PageIndex.PageStaffDetails:
                    OffRefreshButtonVisibility = true;
                    RefreshOffDataCommand = new RelayCommand(() => ViewModelLocator.StaffDetails.RefreshData());
                    OffContentVisibility = true;

                    if (CurrentOffPage == PageIndex.PageStaffDetails)
                        ViewModelLocator.StaffDetails.Init(args as StaffDetailsNaviagtionArgs);
                    else
                        OffNavigationRequested?.Invoke(typeof(StaffDetailsPage), args);
                    break;
                case PageIndex.PageCharacterSearch:
                    if (CurrentMainPage.Value != PageIndex.PageSearch &&
                        CurrentMainPage.Value != PageIndex.PageMangaSearch &&
                        CurrentMainPage.Value != PageIndex.PageCharacterSearch)
                        _searchStateBeforeNavigatingToSearch = SearchToggleStatus;
                    ShowSearchStuff();
                    ToggleSearchStuff();

                    SearchToggleLock = true;
                    if (CurrentMainPage != PageIndex.PageCharacterSearch)
                        MainNavigationRequested?.Invoke(typeof(CharacterSearchPage));
                    await Task.Delay(10);
                    View.SearchInputFocus(FocusState.Keyboard);
                    break;
                case PageIndex.PageWallpapers:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(typeof(WallpapersPage),args);
                    break;
                case PageIndex.PagePopularVideos:
                    HideSearchStuff();
                    CurrentStatus = "Popular Videos";
                    MainNavigationRequested?.Invoke(typeof(PopularVideosPage), args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (currPage != null)
                CurrentMainPage = currPage;
            if (mainPage)
                CurrentMainPageKind = index;
            if (currOffPage != null)
                CurrentOffPage = currOffPage;
            _navigating = false;
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
                if (Credentials.Authenticated)
                    Navigate(Settings.DefaultMenuTab == "anime" ? PageIndex.PageAnimeList : PageIndex.PageMangaList); //entry point whatnot
                else
                {
                    Navigate(PageIndex.PageLogIn);
                    Navigate(PageIndex.PageAnimeList,AnimeListPageNavigationArgs.TopAnime(TopAnimeType.General));
                }
                if (InitDetails != null || InitDetailsFull != null)
                {
                    ViewModelLocator.AnimeList.Initialized += AnimeListOnInitializedLoadArgs;
                }

                MenuPaneState = Settings.HamburgerMenuDefaultPaneState && ApplicationView.GetForCurrentView().VisibleBounds.Width > 500;
            }
        }
        #endregion

        protected override void InitSplitter()
        {
            View.InitSplitter();
        }

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
            View.CurrentOffStatusStoryboard.Begin();
        }
    }
}