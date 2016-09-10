using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.PlayTo;
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
using MALClient.Shared.ViewModels.Interfaces;
using MALClient.XShared.Comm;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
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

            if (index == PageIndex.PageSeasonal ||
                index == PageIndex.PageMangaList ||
                index == PageIndex.PageTopManga ||
                index == PageIndex.PageTopAnime)
                index = PageIndex.PageAnimeList;




            MobileViewModelLocator.Hamburger.ChangeBottomStackPanelMargin(index == PageIndex.PageAnimeList ||
                                                                    index == PageIndex.PageMessanging ||
                                                                    index == PageIndex.PageForumIndex);

            if (index == PageIndex.PageAnimeList && _searchStateBeforeNavigatingToSearch != null)
            {
                SearchToggleStatus = (bool) _searchStateBeforeNavigatingToSearch;
                if (SearchToggleStatus)
                    ShowSearchStuff();
                else
                    HideSearchStuff();
            }

            ResetSearchFilter();
            switch (index)
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
                    SearchToggleLock = true;
                    ShowSearchStuff();
                    ToggleSearchStuff();
                    if (string.IsNullOrWhiteSpace((args as SearchPageNavigationArgs).Query))
                    {
                        View.SearchInputFocus(FocusState.Keyboard);
                        (args as SearchPageNavigationArgs).Query = CurrentSearchQuery;
                    }
                    MainNavigationRequested?.Invoke(typeof(AnimeSearchPage), args);
                    break;
                case PageIndex.PageLogIn:
                    HideSearchStuff();
                    MainNavigationRequested?.Invoke(typeof(LogInPage));
                    break;
                case PageIndex.PageProfile:
                    HideSearchStuff();
                    RefreshButtonVisibility = false;
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
                    RefreshButtonVisibility = true;
                    RefreshDataCommand = new RelayCommand(() => ViewModelLocator.Recommendations.PopulateData());
                    CurrentStatus = "Recommendations";
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            CurrentMainPage = index;
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
                if (InitDetails != null)
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