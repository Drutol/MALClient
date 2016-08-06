using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using GalaSoft.MvvmLight.Ioc;
using MalClient.Shared.Comm.Anime;
using MalClient.Shared.Delegates;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels.Forums;
using MalClient.Shared.ViewModels.Main;
using Microsoft.Practices.ServiceLocation;

namespace MalClient.Shared.ViewModels
{

    public interface IMainViewInteractions
    {
        Storyboard CurrentStatusStoryboard { get; }
        Storyboard CurrentOffStatusStoryboard { get; }
        Storyboard CurrentOffSubStatusStoryboard { get; }
        Storyboard PinDialogStoryboard { get; }
        Storyboard HidePinDialogStoryboard { get;}
        SplitViewDisplayMode CurrentDisplayMode { get; }
        void SearchInputFocus(FocusState state);
        void InitSplitter();
    }

    public interface IMainViewModel
    {
        void Navigate(PageIndex page, object args = null);
        string CurrentStatus { get; set; }
        AnimeListPageNavigationArgs GetCurrentListOrderParams();
        PinTileDialogViewModel PinDialogViewModel { get; }
        void PopulateSearchFilters(HashSet<string> filters);
        //Desktop
        void OnSearchInputSubmit();
        event OffContentPaneStateChanged OffContentPaneStateChanged ;
        ICommand HideOffContentCommand { get; }
        string CurrentOffStatus { get; set; }
        Visibility NavigateOffBackButtonVisibility { get; set; }
        Visibility NavigateMainBackButtonVisibility { get; set; }
        string CurrentSearchQuery { get; set; }
        List<string> SearchHints { get; set; }
        Visibility ScrollToTopButtonVisibility { get; set; }
        string CurrentStatusSub { get; set; }
        IMainViewInteractions View { get; }
        bool IsCurrentStatusSelectable { get; set; }
    }

    public interface IHamburgerViewModel
    {
        Task UpdateProfileImg(bool dl = true);
        //Desktop
        void SetActiveButton(HamburgerButtons val);
        void UpdateApiDependentButtons();
        void UpdateAnimeFiltersSelectedIndex();
        void UpdateLogInLabel();
        Visibility MangaSectionVisbility { get; set; }
        void SetActiveButton(TopAnimeType topType);
    }

    public interface INavMgr
    {
        void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout);
        void RegisterOneTimeOverride(ICommand command);
        void DeregisterBackNav();
        void ResetOffBackNav();
        //Desktop
        void RegisterBackNav(ProfilePageNavigationArgs args);
        void CurrentMainViewOnBackRequested();
        void CurrentOffViewOnBackRequested();
        void ResetMainBackNav();
        void RegisterBackNav(AnimeDetailsPageNavigationArgs args);
        void RegisterOneTimeMainOverride(ICommand command);
        void ResetOneTimeOverride();
        void ResetOneTimeMainOverride();
    }

    public interface IProfileViewModel
    {
        Dictionary<string, Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>> OthersAbstractions { get; }
    }

    public class ViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public static void RegisterBase()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<RecommendationsViewModel>();
            SimpleIoc.Default.Register<SearchPageViewModel>();
            SimpleIoc.Default.Register<HummingbirdProfilePageViewModel>();
            SimpleIoc.Default.Register<CalendarPageViewModel>();
            SimpleIoc.Default.Register<MalArticlesViewModel>();
            SimpleIoc.Default.Register<MalMessagingViewModel>();
            SimpleIoc.Default.Register<MalMessageDetailsViewModel>();
            SimpleIoc.Default.Register<AnimeDetailsPageViewModel>();
            SimpleIoc.Default.Register<AnimeListViewModel>();
            SimpleIoc.Default.Register<ForumIndexViewModel>();
            SimpleIoc.Default.Register<ForumsMainViewModel>();
            SimpleIoc.Default.Register<ForumBoardViewModel>();
            SimpleIoc.Default.Register<ForumTopicViewModel>();
            SimpleIoc.Default.Register<HistoryViewModel>();

        }


        public static IMainViewModel GeneralMain => ServiceLocator.Current.GetInstance<IMainViewModel>();

        public static IHamburgerViewModel GeneralHamburger => ServiceLocator.Current.GetInstance<IHamburgerViewModel>();

        public static INavMgr NavMgr => ServiceLocator.Current.GetInstance<INavMgr>();

        public static IProfileViewModel GeneralProfile => ServiceLocator.Current.GetInstance<IProfileViewModel>();

        public static AnimeDetailsPageViewModel AnimeDetails => ServiceLocator.Current.GetInstance<AnimeDetailsPageViewModel>();

        public static AnimeListViewModel AnimeList => ServiceLocator.Current.GetInstance<AnimeListViewModel>();

        public static RecommendationsViewModel Recommendations
            => ServiceLocator.Current.GetInstance<RecommendationsViewModel>();

        public static SearchPageViewModel SearchPage => ServiceLocator.Current.GetInstance<SearchPageViewModel>();      

        public static HummingbirdProfilePageViewModel HumProfilePage
            => ServiceLocator.Current.GetInstance<HummingbirdProfilePageViewModel>();

        public static CalendarPageViewModel CalendarPage
            => ServiceLocator.Current.GetInstance<CalendarPageViewModel>();

        public static MalArticlesViewModel MalArticles
            => ServiceLocator.Current.GetInstance<MalArticlesViewModel>();

        public static MalMessagingViewModel MalMessaging
            => ServiceLocator.Current.GetInstance<MalMessagingViewModel>();

        public static MalMessageDetailsViewModel MalMessageDetails
            => ServiceLocator.Current.GetInstance<MalMessageDetailsViewModel>();

        public static HistoryViewModel History
            => ServiceLocator.Current.GetInstance<HistoryViewModel>();

        //Forums

        public static ForumsMainViewModel ForumsMain
            => ServiceLocator.Current.GetInstance<ForumsMainViewModel>();

        public static ForumIndexViewModel ForumsIndex
            => ServiceLocator.Current.GetInstance<ForumIndexViewModel>();

        public static ForumBoardViewModel ForumsBoard 
            => ServiceLocator.Current.GetInstance<ForumBoardViewModel>();

        public static ForumTopicViewModel ForumsTopic
            => ServiceLocator.Current.GetInstance<ForumTopicViewModel>();
    }
}