using GalaSoft.MvvmLight.Ioc;
using MalClient.Shared.ViewModels;
using MALClient.Utils;
using MALClient.ViewModels.Main;
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
            ViewModelLocator.RegisterBase();

            SimpleIoc.Default.Register<MainViewModel>();         
            SimpleIoc.Default.Register<HamburgerControlViewModel>();
            SimpleIoc.Default.Register<AnimeListViewModel>();
            SimpleIoc.Default.Register<SettingsPageViewModel>();
            SimpleIoc.Default.Register<ProfilePageViewModel>();
            SimpleIoc.Default.Register<IMainViewModel>(() => SimpleIoc.Default.GetInstance<MainViewModel>());
            SimpleIoc.Default.Register<IAnimeListViewModel>(() => SimpleIoc.Default.GetInstance<AnimeListViewModel>());
            SimpleIoc.Default.Register<IHamburgerViewModel>(() => SimpleIoc.Default.GetInstance<HamburgerControlViewModel>());          
            SimpleIoc.Default.Register<INavMgr,NavMgr>();

        }

        public static MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static HamburgerControlViewModel Hamburger
            => ServiceLocator.Current.GetInstance<HamburgerControlViewModel>();

        public static AnimeListViewModel AnimeList => ServiceLocator.Current.GetInstance<AnimeListViewModel>();

        public static SettingsPageViewModel SettingsPage
            => ServiceLocator.Current.GetInstance<SettingsPageViewModel>();

        public static ProfilePageViewModel ProfilePage => ServiceLocator.Current.GetInstance<ProfilePageViewModel>();
    }
}