using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Ioc;
using MalClient.Shared.Delegates;
using MalClient.Shared.Models.Library;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels.Main;
using Microsoft.Practices.ServiceLocation;
using AnimeDetailsPageNavigationArgs = MalClient.Shared.NavArgs.AnimeDetailsPageNavigationArgs;

namespace MalClient.Shared.ViewModels
{
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
        Visibility NavigateBackButtonVisibility { get; set; }
        Visibility NavigateMainBackButtonVisibility { get; set; }
        string CurrentSearchQuery { get; set; }
        List<string> SearchHints { get; set; }
        Visibility ScrollToTopButtonVisibility { get; set; }
        string CurrentStatusSub { get; set; }
    }

    public interface IHamburgerViewModel
    {
        Task UpdateProfileImg(bool dl = true);
        //Desktop
        void SetActiveButton(HamburgerButtons val);
        void UpdateApiDependentButtons();
        void UpdateAnimeFiltersSelectedIndex();
    }

    public interface INavMgr
    {
        void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout);
        void RegisterOneTimeOverride(ICommand command);
        void DeregisterBackNav();
        void ResetBackNav();
        //Desktop
        void RegisterBackNav(ProfilePageNavigationArgs args);
        void CurrentMainViewOnBackRequested();
        void CurrentViewOnBackRequested();
        void ResetMainBackNav();
        void RegisterBackNav(AnimeDetailsPageNavigationArgs args);
        void RegisterOneTimeMainOverride(ICommand command);
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

        }


        public static IMainViewModel GeneralMain => ServiceLocator.Current.GetInstance<IMainViewModel>();

        public static IHamburgerViewModel GeneralHamburger => ServiceLocator.Current.GetInstance<IHamburgerViewModel>();

        public static INavMgr NavMgr => ServiceLocator.Current.GetInstance<INavMgr>();

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
    }
}