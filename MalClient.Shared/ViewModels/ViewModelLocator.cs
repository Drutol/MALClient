using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using MalClient.Shared.NavArgs;
using MALClient.Items;
using MALClient.Utils.Enums;
using MALClient.ViewModels.Messages;
using Microsoft.Practices.ServiceLocation;

namespace MALClient.ViewModels
{
    public interface IMainViewModel
    {
        void Navigate(PageIndex page, object args = null);
        string CurrentStatus { get; set; }
        AnimeListPageNavigationArgs GetCurrentListOrderParams();
        PinTileDialogViewModel PinDialogViewModel { get; }
        void PopulateSearchFilters(HashSet<string> filters);
    }

    public interface IHamburgerViewModel
    {
        Task UpdateProfileImg(bool dl = true);
    }

    public interface IAnimeListViewModel
    {
        void AddAnimeEntry(AnimeItemAbstraction animeItemAbstraction);
        List<AnimeItemAbstraction> AllLoadedAnimeItemAbstractions { get; }
        List<AnimeItemAbstraction> AllLoadedMangaItemAbstractions { get; }
        Task<IAnimeData> TryRetrieveAuthenticatedAnimeItem(int id, bool anime = true, bool forceMal = false);
    }

    public interface IAnimeDetailsViewModel
    {
        int Id { get; }
        void CurrentAnimeHasBeenAddedToList(IAnimeData viewModel);
        void UpdateAnimeReferenceUiBindings(int Id);
    }

    public interface INavMgr
    {
        void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout);
        void RegisterOneTimeOverride(ICommand command);
        void DeregisterBackNav();
        void ResetBackNav();
    }

    public class ViewModelLocator
    {
        private static bool _initialized;

        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            if (_initialized)
                return;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<RecommendationsViewModel>();
            SimpleIoc.Default.Register<SearchPageViewModel>();
            SimpleIoc.Default.Register<ProfilePageViewModel>();
            SimpleIoc.Default.Register<HummingbirdProfilePageViewModel>();
            SimpleIoc.Default.Register<CalendarPageViewModel>();
            SimpleIoc.Default.Register<MalArticlesViewModel>();
            SimpleIoc.Default.Register<MalMessagingViewModel>();
            SimpleIoc.Default.Register<MalMessageDetailsViewModel>();

            _initialized = true;
        }


        public static IMainViewModel GeneralMain => ServiceLocator.Current.GetInstance<IMainViewModel>();

        public static IHamburgerViewModel GeneralHamburger => ServiceLocator.Current.GetInstance<IHamburgerViewModel>();

        public static IAnimeListViewModel GeneralAnimeList => ServiceLocator.Current.GetInstance<IAnimeListViewModel>();

        public static IAnimeDetailsViewModel GeneralAnimeDetails => ServiceLocator.Current.GetInstance<IAnimeDetailsViewModel>();

        public static INavMgr NavMgr => ServiceLocator.Current.GetInstance<INavMgr>();

        public static RecommendationsViewModel Recommendations
            => ServiceLocator.Current.GetInstance<RecommendationsViewModel>();

        public static SearchPageViewModel SearchPage => ServiceLocator.Current.GetInstance<SearchPageViewModel>();

        public static ProfilePageViewModel ProfilePage => ServiceLocator.Current.GetInstance<ProfilePageViewModel>();

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
    }
}