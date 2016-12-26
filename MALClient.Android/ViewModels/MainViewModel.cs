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
using MALClient.Android.Fragments.RecommendationsFragments;
using MALClient.Android.Fragments.SearchFragments;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
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
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Log in first in order to access this page.","");               
                return;
            }
            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.Navigated, index.ToString());
            ScrollToTopButtonVisibility = false;
            RefreshButtonVisibility = false;

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
                    //ViewModelLocator.Hamburger.SetActiveButton(HamburgerButtons.AnimeList);
                    ShowSearchStuff();
                    if ((_searchStateBeforeNavigatingToSearch == null || !_searchStateBeforeNavigatingToSearch.Value) &&
                        (wasOnSearchPage || _wasOnDetailsFromSearch))
                    {
                        CurrentSearchQuery = "";
                        _wasOnDetailsFromSearch = false;
                        UnToggleSearchStuff();
                    }
                    if (CurrentMainPage == PageIndex.PageAnimeList)
                        ViewModelLocator.AnimeList.Init(args as AnimeListPageNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(AnimeListPageFragment.BuildInstance(args));
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
                        MainNavigationRequested?.Invoke(AnimeDetailsPageFragment.BuildInstance(args));
                    break;
                case PageIndex.PageSettings:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(new SettingsPageFragment());
                    break;
                case PageIndex.PageSearch:
                case PageIndex.PageMangaSearch:
                    if (CurrentMainPage != PageIndex.PageSearch && CurrentMainPage != PageIndex.PageMangaSearch && CurrentMainPage != PageIndex.PageCharacterSearch)
                        _searchStateBeforeNavigatingToSearch = SearchToggleStatus;

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
                    MainNavigationRequested?.Invoke(SearchPageFragment.BuildInstance(args as SearchPageNavigationArgs));
                    break;
                case PageIndex.PageLogIn:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(LogInPageFragment.Instance);
                    break;
                case PageIndex.PageProfile:
                    //HideSearchStuff();
                    //RefreshButtonVisibility = true;
                    //if (Settings.SelectedApiType == ApiType.Mal)
                    //    RefreshDataCommand =
                    //        new RelayCommand(() => ViewModelLocator.ProfilePage.LoadProfileData(null, true));
                    //else
                    //    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.HumProfilePage.Init(true));
                    //if (Settings.SelectedApiType == ApiType.Mal)
                    //{
                    //    if (CurrentMainPage == PageIndex.PageProfile)
                    //        ViewModelLocator.ProfilePage.LoadProfileData(args as ProfilePageNavigationArgs);
                    //    else
                    //        MainNavigationRequested?.Invoke(typeof(ProfilePage), args);
                    //}
                    //else
                    //    MainNavigationRequested?.Invoke(typeof(HummingbirdProfilePage), args);
                    break;
                case PageIndex.PageRecomendations:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(new RecommendationsPageFragment(args as RecommendationPageNavigationArgs));
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
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.MalArticles.Init(null));
                    if (CurrentMainPage == PageIndex.PageArticles)
                        ViewModelLocator.MalArticles.Init(args as MalArticlesPageNavigationArgs);
                    else
                        MainNavigationRequested?.Invoke(new ArticlesPageFragment());
                    break;
                case PageIndex.PageMessanging:
                    //HideSearchStuff();
                    //CurrentStatus = $"{Credentials.UserName} - Messages";
                    //RefreshButtonVisibility = true;
                    //RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.MalMessaging.Init(true); });
                    //MainNavigationRequested?.Invoke(typeof(MalMessagingPage), args);
                    break;
                case PageIndex.PageMessageDetails:
                    //var msgModel = args as MalMessageDetailsNavArgs;
                    //CurrentOffStatus = msgModel.WorkMode == MessageDetailsWorkMode.Message
                    //    ? (msgModel.Arg != null
                    //        ? $"{(msgModel.Arg as MalMessageModel)?.Sender} - {(msgModel.Arg as MalMessageModel)?.Subject}"
                    //        : "New Message")
                    //    : $"Comments {Credentials.UserName} - {(msgModel.Arg as MalComment)?.User.Name}";
                    //MainNavigationRequested?.Invoke(typeof(MalMessageDetailsPage), args);
                    break;
                case PageIndex.PageForumIndex:
                    //HideSearchStuff();
                    //CurrentStatus = "Forums";
                    //if (args == null || (args as ForumsNavigationArgs)?.Page == ForumsPageIndex.PageIndex)
                    //{
                    //    RefreshButtonVisibility = true;
                    //    RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.ForumsIndex.Init(true); });
                    //}
                    //else
                    //{
                    //    var navArgs = args as ForumsNavigationArgs;
                    //    if (navArgs?.Page == ForumsPageIndex.PageBoard)
                    //    {
                    //        RefreshButtonVisibility = true;
                    //        RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.ForumsBoard.Reload(); });
                    //    }
                    //}
                    //if (CurrentMainPage == PageIndex.PageForumIndex)
                    //    ViewModelLocator.ForumsMain.Init(args as ForumsNavigationArgs);
                    //else
                    //    MainNavigationRequested?.Invoke(typeof(ForumsMainPage), args);
                    break;
                case PageIndex.PageHistory:
                    //HideSearchStuff();
                    //RefreshButtonVisibility = true;
                    //RefreshDataCommand = new RelayCommand(() => { ViewModelLocator.History.Init(null, true); });
                    //CurrentStatus = $"History - {(args as HistoryNavigationArgs)?.Source ?? Credentials.UserName}";
                    //MainNavigationRequested?.Invoke(typeof(HistoryPage), args);
                    break;
                case PageIndex.PageCharacterDetails:
                    //HideSearchStuff();
                    //RefreshButtonVisibility = true;
                    //RefreshDataCommand = new RelayCommand(() => ViewModelLocator.CharacterDetails.RefreshData());
                    //OffContentVisibility = true;

                    //if (CurrentOffPage == PageIndex.PageCharacterDetails)
                    //    ViewModelLocator.CharacterDetails.Init(args as CharacterDetailsNavigationArgs);
                    //else
                    //    MainNavigationRequested?.Invoke(typeof(CharacterDetailsPage), args);
                    break;
                case PageIndex.PageStaffDetails:
                    //HideSearchStuff();
                    //RefreshButtonVisibility = true;
                    //RefreshDataCommand = new RelayCommand(() => ViewModelLocator.StaffDetails.RefreshData());
                    //OffContentVisibility = true;

                    //if (CurrentOffPage == PageIndex.PageStaffDetails)
                    //    ViewModelLocator.StaffDetails.Init(args as StaffDetailsNaviagtionArgs);
                    //else
                    //    MainNavigationRequested?.Invoke(typeof(StaffDetailsPage), args);
                    break;
                case PageIndex.PageCharacterSearch:
                    //if (CurrentMainPage != PageIndex.PageSearch && CurrentMainPage != PageIndex.PageMangaSearch && CurrentMainPage != PageIndex.PageCharacterSearch)
                    //    _searchStateBeforeNavigatingToSearch = SearchToggleStatus;
                    //ShowSearchStuff();
                    //ToggleSearchStuff();

                    //SearchToggleLock = true;

                    //MainNavigationRequested?.Invoke(typeof(CharacterSearchPage));
                    //View.SearchInputFocus(FocusState.Keyboard);
                    break;
                case PageIndex.PageWallpapers:
                    //HideSearchStuff();
                    //RefreshButtonVisibility = false;
                    //MainNavigationRequested?.Invoke(typeof(WallpapersPage), args);
                    break;
                case PageIndex.PagePopularVideos:
                    //HideSearchStuff();
                    //CurrentStatus = "Popular Videos";
                    //MainNavigationRequested?.Invoke(typeof(PopularVideosPage), args);
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
    }
}