using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Delegates;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels.Clubs;
using MALClient.XShared.ViewModels.Details;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.XShared.ViewModels
{
    public class ViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public static void RegisterBase()
        {
            
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
            SimpleIoc.Default.Register<ListComparisonViewModel>();
            SimpleIoc.Default.Register<FriendsPageViewModel>();
            SimpleIoc.Default.Register<ClubIndexViewModel>();
            SimpleIoc.Default.Register<ClubDetailsViewModel>();

            ResourceLocator.RegisterBase();
        }

        public static bool Mobile { get; set; }

        public static MainViewModelBase GeneralMain => SimpleIoc.Default.GetInstance<MainViewModelBase>();

        public static IHamburgerViewModel GeneralHamburger => SimpleIoc.Default.GetInstance<IHamburgerViewModel>();

        public static INavMgr NavMgr => SimpleIoc.Default.GetInstance<INavMgr>();

        public static AnimeDetailsPageViewModel AnimeDetails => SimpleIoc.Default.GetInstance<AnimeDetailsPageViewModel>();

        public static AnimeListViewModel AnimeList => SimpleIoc.Default.GetInstance<AnimeListViewModel>();

        public static ProfilePageViewModel ProfilePage => SimpleIoc.Default.GetInstance<ProfilePageViewModel>();

        public static RecommendationsViewModel Recommendations
            => SimpleIoc.Default.GetInstance<RecommendationsViewModel>();

        public static SearchPageViewModel SearchPage => SimpleIoc.Default.GetInstance<SearchPageViewModel>();      

        public static HummingbirdProfilePageViewModel HumProfilePage
            => SimpleIoc.Default.GetInstance<HummingbirdProfilePageViewModel>();

        public static CalendarPageViewModel CalendarPage
            => SimpleIoc.Default.GetInstance<CalendarPageViewModel>();

        public static MalArticlesViewModel MalArticles
            => SimpleIoc.Default.GetInstance<MalArticlesViewModel>();

        public static MalMessagingViewModel MalMessaging
            => SimpleIoc.Default.GetInstance<MalMessagingViewModel>();

        public static MalMessageDetailsViewModel MalMessageDetails
            => SimpleIoc.Default.GetInstance<MalMessageDetailsViewModel>();

        public static HistoryViewModel History
            => SimpleIoc.Default.GetInstance<HistoryViewModel>();

        public static CharacterDetailsViewModel CharacterDetails
            => SimpleIoc.Default.GetInstance<CharacterDetailsViewModel>();

        public static StaffDetailsViewModel StaffDetails
            => SimpleIoc.Default.GetInstance<StaffDetailsViewModel>();

        public static CharacterSearchViewModel CharacterSearch
            => SimpleIoc.Default.GetInstance<CharacterSearchViewModel>();

        public static LogInViewModel LogIn
            => SimpleIoc.Default.GetInstance<LogInViewModel>();

        public static SettingsViewModelBase Settings
            => SimpleIoc.Default.GetInstance<SettingsViewModelBase>();

        public static WallpapersViewModel Wallpapers
            => SimpleIoc.Default.GetInstance<WallpapersViewModel>();

        public static PopularVideosViewModel PopularVideos
            => SimpleIoc.Default.GetInstance<PopularVideosViewModel>();

        public static FriendsFeedsViewModel FriendsFeeds
            => SimpleIoc.Default.GetInstance<FriendsFeedsViewModel>();

        public static NotificationsHubViewModel NotificationsHub
            => SimpleIoc.Default.GetInstance<NotificationsHubViewModel>();

        public static ListComparisonViewModel Comparison
            => SimpleIoc.Default.GetInstance<ListComparisonViewModel>();

        public static FriendsPageViewModel Friends
            => SimpleIoc.Default.GetInstance<FriendsPageViewModel>();

        public static ClubIndexViewModel ClubIndex
            => SimpleIoc.Default.GetInstance<ClubIndexViewModel>();

        public static ClubDetailsViewModel ClubDetails
            => SimpleIoc.Default.GetInstance<ClubDetailsViewModel>();


        //Forums

        public static ForumsMainViewModel ForumsMain
            => SimpleIoc.Default.GetInstance<ForumsMainViewModel>();

        public static ForumIndexViewModel ForumsIndex
            => SimpleIoc.Default.GetInstance<ForumIndexViewModel>();

        public static ForumBoardViewModel ForumsBoard 
            => SimpleIoc.Default.GetInstance<ForumBoardViewModel>();

        public static ForumTopicViewModel ForumsTopic
            => SimpleIoc.Default.GetInstance<ForumTopicViewModel>();

        public static ForumNewTopicViewModel ForumsNewTopic
            => SimpleIoc.Default.GetInstance<ForumNewTopicViewModel>();

        public static ForumsStarredMessagesViewModel StarredMessages
            => SimpleIoc.Default.GetInstance<ForumsStarredMessagesViewModel>();
    }
}