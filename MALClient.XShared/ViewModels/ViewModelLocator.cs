using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels.Details;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Main;
using Microsoft.Practices.ServiceLocation;

namespace MALClient.XShared.ViewModels
{
    public interface IHamburgerViewModel
    {
        Task UpdateProfileImg(bool dl = true);
        //Desktop
        void SetActiveButton(HamburgerButtons val);
        void UpdateApiDependentButtons();
        void UpdateAnimeFiltersSelectedIndex();
        void UpdateLogInLabel();
        bool MangaSectionVisbility { get; set; }
        void SetActiveButton(TopAnimeType topType);
        void UpdatePinnedProfiles();
        void UpdateBottomMargin();
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
            SimpleIoc.Default.Register<CharacterDetailsViewModel>();
            SimpleIoc.Default.Register<StaffDetailsViewModel>();
            SimpleIoc.Default.Register<CharacterSearchViewModel>();
            SimpleIoc.Default.Register<ProfilePageViewModel>();
            SimpleIoc.Default.Register<LogInViewModel>();
            SimpleIoc.Default.Register<WallpapersViewModel>();

        }

        public static bool Mobile { get; set; }

        public static MainViewModelBase GeneralMain => ServiceLocator.Current.GetInstance<MainViewModelBase>();

        public static IHamburgerViewModel GeneralHamburger => ServiceLocator.Current.GetInstance<IHamburgerViewModel>();

        public static INavMgr NavMgr => ServiceLocator.Current.GetInstance<INavMgr>();

        public static AnimeDetailsPageViewModel AnimeDetails => ServiceLocator.Current.GetInstance<AnimeDetailsPageViewModel>();

        public static AnimeListViewModel AnimeList => ServiceLocator.Current.GetInstance<AnimeListViewModel>();

        public static ProfilePageViewModel ProfilePage => ServiceLocator.Current.GetInstance<ProfilePageViewModel>();

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

        public static CharacterDetailsViewModel CharacterDetails
            => ServiceLocator.Current.GetInstance<CharacterDetailsViewModel>();

        public static StaffDetailsViewModel StaffDetails
            => ServiceLocator.Current.GetInstance<StaffDetailsViewModel>();

        public static CharacterSearchViewModel CharacterSearch
            => ServiceLocator.Current.GetInstance<CharacterSearchViewModel>();

        public static LogInViewModel LogIn
            => ServiceLocator.Current.GetInstance<LogInViewModel>();

        public static SettingsViewModelBase Settings
            => ServiceLocator.Current.GetInstance<SettingsViewModelBase>();

        public static WallpapersViewModel Wallpapers
            => ServiceLocator.Current.GetInstance<WallpapersViewModel>();

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