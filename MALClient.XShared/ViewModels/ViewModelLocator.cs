using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Delegates;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels.Details;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Main;
using Microsoft.Practices.ServiceLocation;

namespace MALClient.XShared.ViewModels
{
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
            SimpleIoc.Default.Register<ForumsStarredMessagesViewModel>();
            SimpleIoc.Default.Register<ForumNewTopicViewModel>();
            SimpleIoc.Default.Register<HistoryViewModel>();
            SimpleIoc.Default.Register<CharacterDetailsViewModel>();
            SimpleIoc.Default.Register<StaffDetailsViewModel>();
            SimpleIoc.Default.Register<CharacterSearchViewModel>();
            SimpleIoc.Default.Register<ProfilePageViewModel>();
            SimpleIoc.Default.Register<LogInViewModel>();
            SimpleIoc.Default.Register<WallpapersViewModel>();
            SimpleIoc.Default.Register<PopularVideosViewModel>();
            SimpleIoc.Default.Register<FriendsFeedsViewModel>();
            SimpleIoc.Default.Register<NotificationsHubViewModel>();

            ResourceLocator.RegisterBase();
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

        public static PopularVideosViewModel PopularVideos
            => ServiceLocator.Current.GetInstance<PopularVideosViewModel>();

        public static FriendsFeedsViewModel FriendsFeeds
            => ServiceLocator.Current.GetInstance<FriendsFeedsViewModel>();

        public static NotificationsHubViewModel NotificationsHub
            => ServiceLocator.Current.GetInstance<NotificationsHubViewModel>();


        //Forums

        public static ForumsMainViewModel ForumsMain
            => ServiceLocator.Current.GetInstance<ForumsMainViewModel>();

        public static ForumIndexViewModel ForumsIndex
            => ServiceLocator.Current.GetInstance<ForumIndexViewModel>();

        public static ForumBoardViewModel ForumsBoard 
            => ServiceLocator.Current.GetInstance<ForumBoardViewModel>();

        public static ForumTopicViewModel ForumsTopic
            => ServiceLocator.Current.GetInstance<ForumTopicViewModel>();

        public static ForumNewTopicViewModel ForumsNewTopic
            => ServiceLocator.Current.GetInstance<ForumNewTopicViewModel>();

        public static ForumsStarredMessagesViewModel StarredMessages
            => ServiceLocator.Current.GetInstance<ForumsStarredMessagesViewModel>();
    }
}