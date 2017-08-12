using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Command;
using MALClient.Android.Fragments;
using MALClient.Android.Fragments.ArticlesPageFragments;
using MALClient.Android.Fragments.CalendarFragments;
using MALClient.Android.Fragments.Clubs;
using MALClient.Android.Fragments.DetailsFragments;
using MALClient.Android.Fragments.ForumFragments;
using MALClient.Android.Fragments.HistoryFragments;
using MALClient.Android.Fragments.MessagingFragments;
using MALClient.Android.Fragments.ProfilePageFragments;
using MALClient.Android.Fragments.RecommendationsFragments;
using MALClient.Android.Fragments.SearchFragments;
using MALClient.Android.Fragments.SettingsFragments;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.ViewModels
{
    public delegate void AndroidNavigationRequest(Fragment fragment);

    public class MainViewModel : MainViewModelBase
    {
        public new event AndroidNavigationRequest MainNavigationRequested;

        protected override void CurrentStatusStoryboardBegin()
        {
            //throw new NotImplementedException();
        }

        protected override void CurrentOffSubStatusStoryboardBegin()
        {
           // throw new NotImplementedException();
        }

        protected override void CurrentOffStatusStoryboardBegin()
        {
          //  throw new NotImplementedException();
        }

        public override void Navigate(PageIndex index, object args = null)
        {
            PageIndex originalIndex = index;
            var wasOnSearchPage = SearchToggleLock;
            SearchToggleLock = false;
            CurrentStatusSub = "";
            IsCurrentStatusSelectable = false;
            if (!Credentials.Authenticated && PageUtils.PageRequiresAuth(index))
            {
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Log in first in order to access this page.","Log in required");               
                return;
            }
            if(index == PageIndex.PageForumIndex && args is ForumsNavigationArgs arg)
                ResourceLocator.TelemetryProvider.TelemetryTrackNavigation(arg.Page);
            else
                ResourceLocator.TelemetryProvider.TelemetryTrackNavigation(index);

            ScrollToTopButtonVisibility = false;
            RefreshButtonVisibility = false;
            ViewModelLocator.AnimeDetails.Id = -1;

            if (index == PageIndex.PageMangaList && args == null) // navigating from startup
                args = AnimeListPageNavigationArgs.Manga;

            if (index == PageIndex.PageSeasonal ||
                index == PageIndex.PageMangaList ||
                index == PageIndex.PageTopManga ||
                index == PageIndex.PageTopAnime)
                index = PageIndex.PageAnimeList;

            

            if (index == PageIndex.PageAnimeList && _searchStateBeforeNavigatingToSearch != null)
            {
                SearchToggleStatus = (bool)_searchStateBeforeNavigatingToSearch;
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
                    switch ((args as AnimeListPageNavigationArgs)?.WorkMode ?? AnimeListWorkModes.Anime)
                    {
                        case AnimeListWorkModes.Anime:
                            ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.AnimeList);
                            break;
                        case AnimeListWorkModes.SeasonalAnime:
                            ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.Seasonal);
                            break;
                        case AnimeListWorkModes.Manga:
                            ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.MangaList);
                            break;
                        case AnimeListWorkModes.TopAnime:
                            ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.TopAnime);
                            break;
                        case AnimeListWorkModes.TopManga:
                            ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.TopManga);
                            break;
                        case AnimeListWorkModes.AnimeByGenre:
                            ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.AnimeList);
                            break;
                        case AnimeListWorkModes.AnimeByStudio:
                            ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.AnimeList);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    ShowSearchStuff();
                    if ((_searchStateBeforeNavigatingToSearch == null || !_searchStateBeforeNavigatingToSearch.Value) &&
                        (wasOnSearchPage || _wasOnDetailsFromSearch))
                    {
                        CurrentSearchQuery = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }

                    var alargs = args as AnimeListPageNavigationArgs;

                    if (CurrentMainPage == PageIndex.PageAnimeList)
                        ViewModelLocator.AnimeList.Init(args as AnimeListPageNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(new AnimeListPageFragment(args as AnimeListPageNavigationArgs));

                    if (alargs != null && (alargs.WorkMode == AnimeListWorkModes.Manga && alargs.ResetBackNav))
                    {
                        ViewModelLocator.NavMgr.DeregisterBackNav();
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
                    }

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
                    //if (CurrentMainPage == PageIndex.PageAnimeDetails)
                    //    ViewModelLocator.AnimeDetails.Init(args as AnimeDetailsPageNavigationArgs);
                    //else
                    MainNavigationRequested?.Invoke(
                        new AnimeDetailsPageFragment(args as AnimeDetailsPageNavigationArgs));
                    break;
                case PageIndex.PageSettings:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(new SettingsPageFragment(args as SettingsPageIndex?));
                    break;
                case PageIndex.PageSearch:
                case PageIndex.PageMangaSearch:
                case PageIndex.PageCharacterSearch:
                    if (CurrentMainPage != PageIndex.PageSearch && CurrentMainPage != PageIndex.PageMangaSearch &&
                        CurrentMainPage != PageIndex.PageCharacterSearch)
                        _searchStateBeforeNavigatingToSearch = SearchToggleStatus;

                    if (args != null)
                    {
                        var searchArg = args as SearchPageNavigationArgs;
                        if (string.IsNullOrWhiteSpace(searchArg.Query))
                        {
                            searchArg.Query = CurrentSearchQuery;
                        }
                        if (!searchArg.ByGenre && !searchArg.ByStudio)
                        {
                            //View.SearchInputFocus(FocusState.Keyboard);
                            SearchToggleLock = true;
                            ShowSearchStuff();
                            ToggleSearchStuff();
                        }
                        else
                        {
                            HideSearchStuff();
                            CurrentStatus = searchArg.ByGenre ? "Anime by Genre" : "Anime By Studio";
                        }
                    }
                    MainNavigationRequested?.Invoke(SearchPageFragment.BuildInstance(args as SearchPageNavigationArgs));
                    break;
                case PageIndex.PageLogIn:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(LogInPageFragment.Instance);
                    break;
                case PageIndex.PageProfile:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    if (Settings.SelectedApiType == ApiType.Mal)
                        RefreshDataCommand =
                            new RelayCommand(
                                async () => await ViewModelLocator.ProfilePage.LoadProfileData(null, true));
                    if (Settings.SelectedApiType == ApiType.Mal)
                    {
                        if (CurrentMainPage == PageIndex.PageProfile)
                            ViewModelLocator.ProfilePage.LoadProfileData(args as ProfilePageNavigationArgs);
                        else
                            MainNavigationRequested?.Invoke(new ProfilePageFragment(args as ProfilePageNavigationArgs));
                    }
                    break;
                case PageIndex.PageRecomendations:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(
                        new RecommendationsPageFragment(args as RecommendationPageNavigationArgs));
                    break;
                case PageIndex.PageCalendar:
                    HideSearchStuff();
                    CurrentStatus = "Calendar";
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.CalendarPage.Init(true); });
                    MainNavigationRequested?.Invoke(CalendarPageFragment.Instance);
                    break;
                case PageIndex.PageArticles:
                case PageIndex.PageNews:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(new ArticlesPageFragment(args as MalArticlesPageNavigationArgs));
                    break;
                case PageIndex.PageMessanging:
                    HideSearchStuff();
                    CurrentStatus =
                        $"{Credentials.UserName} - {(ViewModelLocator.MalMessaging.DisplaySentMessages ? "Sent Messages" : "Messages")}";
                    //RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.MalMessaging.Init(true); });
                    MainNavigationRequested?.Invoke(new MessagingPageFragment());
                    break;
                case PageIndex.PageMessageDetails:
                    var msgModel = args as MalMessageDetailsNavArgs;
                    CurrentOffStatus = msgModel.WorkMode == MessageDetailsWorkMode.Message
                        ? (msgModel.Arg != null
                            ? $"{(msgModel.Arg as MalMessageModel)?.Sender} - {(msgModel.Arg as MalMessageModel)?.Subject}"
                            : "New Message")
                        : $"Comments {Credentials.UserName} - {(msgModel.Arg as MalComment)?.User.Name}";
                    MainNavigationRequested?.Invoke(new MessagingDetailsPageFragment(args as MalMessageDetailsNavArgs));
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
                    if (CurrentMainPage != null && CurrentMainPage == PageIndex.PageForumIndex)
                        ViewModelLocator.ForumsMain.Init(args as ForumsNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(new ForumMainPageFragment(args as ForumsNavigationArgs));
                    break;
                case PageIndex.PageHistory:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.History.Init(null, true); });
                    CurrentStatus = $"History - {(args as HistoryNavigationArgs)?.Source ?? Credentials.UserName}";
                    MainNavigationRequested?.Invoke(new HistoryPageFragment(args as HistoryNavigationArgs));
                    break;
                case PageIndex.PageCharacterDetails:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.CharacterDetails.RefreshData());
                    OffContentVisibility = true;

                    if (CurrentOffPage == PageIndex.PageCharacterDetails)
                        ViewModelLocator.CharacterDetails.Init(args as CharacterDetailsNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(
                            new CharacterDetailsPageFragment(args as CharacterDetailsNavigationArgs));
                    break;
                case PageIndex.PageStaffDetails:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.StaffDetails.RefreshData());
                    OffContentVisibility = true;

                    if (CurrentOffPage == PageIndex.PageStaffDetails)
                        ViewModelLocator.StaffDetails.Init(args as StaffDetailsNaviagtionArgs);
                    else
                        MainNavigationRequested?.Invoke(
                            new PersonDetailsPageFragment(args as StaffDetailsNaviagtionArgs));
                    break;
                case PageIndex.PageWallpapers:
                    HideSearchStuff();
                    RefreshButtonVisibility = false;
                    MainNavigationRequested?.Invoke(new WallpapersPageFragment());
                    break;
                case PageIndex.PagePopularVideos:
                    HideSearchStuff();
                    CurrentStatus = "Promotional Videos";
                    MainNavigationRequested?.Invoke(new PromoVideosPageFragment());
                    break;
                case PageIndex.PageFeeds:
                    HideSearchStuff();
                    //RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.FriendsFeeds.Init(true));
                    CurrentStatus = "Friends Feeds";
                    MainNavigationRequested?.Invoke(new FriendsFeedsPageFragment());
                    break;
                case PageIndex.PageNotificationHub:
                    HideSearchStuff();
                    //RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.NotificationsHub.Init(true));
                    CurrentStatus = "Notifications";
                    MainNavigationRequested?.Invoke(new NotificationHubPageFragment());
                    break;
                case PageIndex.PageListComparison:
                    HideSearchStuff();
                    CurrentStatus = "List Comparison";
                    MainNavigationRequested?.Invoke(
                        new ListComparisonPageFragment(args as ListComparisonPageNavigationArgs));
                    break;
                case PageIndex.PageFriends:
                    HideSearchStuff();
                    RefreshButtonVisibility = false;
                    CurrentStatus = $"{(args as FriendsPageNavArgs).TargetUser.Name}'s friends.";
                    MainNavigationRequested?.Invoke(new FriendsPageFragment(args as FriendsPageNavArgs));
                    break;
                case PageIndex.PageClubIndex:
                    HideSearchStuff();
                    RefreshButtonVisibility = false;
                    CurrentStatus = "Clubs";
                    MainNavigationRequested?.Invoke(new ClubsIndexPageFragment());
                    break;
                case PageIndex.PageClubDetails:
                    HideSearchStuff();
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.ClubDetails.Reload());
                    CurrentStatus = "Club details";
                    MainNavigationRequested?.Invoke(new ClubDetailsPageFragment(args as ClubDetailsPageNavArgs));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            CurrentMainPage = index;
            CurrentMainPageKind = index;
            RaisePropertyChanged(() => SearchToggleLock);
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

        public void PerformFirstNavigation()
        {

            bool hasArgumentsWithSync =
                    InitDetailsFull?.Item1.GetAttribute<EnumUtilities.PageIndexEnumMember>().RequiresSyncBlock ?? true;
            if (Credentials.Authenticated)
            {
                if (hasArgumentsWithSync)
                    Navigate(Settings.DefaultMenuTab == "anime" ? PageIndex.PageAnimeList : PageIndex.PageMangaList);
                //entry point whatnot
                else if (InitDetailsFull != null)
                {
                    ViewModelLocator.AnimeList.Init(null);
                    Navigate(InitDetailsFull.Item1, InitDetailsFull.Item2);
                }
            }
            else
            {
                Navigate(PageIndex.PageLogIn);
            }
            if (InitDetails != null || hasArgumentsWithSync)
            {
                ViewModelLocator.AnimeList.Initialized += AnimeListOnInitializedLoadArgs;
            }
        }
    }
}