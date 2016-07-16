using GalaSoft.MvvmLight.Ioc;
using MalClient.Shared.ViewModels;
using MALClient.Utils;
using MALClient.ViewModels.Main;
using MALClient.ViewModels.Off;
using Microsoft.Practices.ServiceLocation;

namespace MALClient.ViewModels
{
    public class MobileViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public static void RegisterDependencies()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();         
            SimpleIoc.Default.Register<HamburgerControlViewModel>();
            SimpleIoc.Default.Register<AnimeListViewModel>();
            SimpleIoc.Default.Register<AnimeDetailsPageViewModel>();
            SimpleIoc.Default.Register<SettingsPageViewModel>();
            SimpleIoc.Default.Register<IMainViewModel>(() => SimpleIoc.Default.GetInstance<MainViewModel>());
            SimpleIoc.Default.Register<IAnimeListViewModel>(() => SimpleIoc.Default.GetInstance<AnimeListViewModel>());
            SimpleIoc.Default.Register<IHamburgerViewModel>(() => SimpleIoc.Default.GetInstance<HamburgerControlViewModel>());
            SimpleIoc.Default.Register<IAnimeDetailsViewModel>(() => SimpleIoc.Default.GetInstance<AnimeDetailsPageViewModel>());
            SimpleIoc.Default.Register<INavMgr,NavMgr>();

        }

        public static MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static HamburgerControlViewModel Hamburger
            => ServiceLocator.Current.GetInstance<HamburgerControlViewModel>();

        public static AnimeListViewModel AnimeList => ServiceLocator.Current.GetInstance<AnimeListViewModel>();

        public static AnimeDetailsPageViewModel AnimeDetails
            => ServiceLocator.Current.GetInstance<AnimeDetailsPageViewModel>();

        public static SettingsPageViewModel SettingsPage
            => ServiceLocator.Current.GetInstance<SettingsPageViewModel>();
    }
}