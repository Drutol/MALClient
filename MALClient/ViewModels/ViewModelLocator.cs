using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using MALClient.Pages;
using Microsoft.Practices.ServiceLocation;

namespace MALClient.ViewModels
{
    public class ViewModelLocator
    {/// <summary>
     /// Initializes a new instance of the ViewModelLocator class.
     /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            var navigationService = this.CreateNavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            SimpleIoc.Default.Register<RecommendationsViewModel>();
        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            navigationService.Configure(PageIndex.PageRecomendations.ToString(), typeof(RecomendationsPage));
            navigationService.Configure(PageIndex.PageAbout.ToString(), typeof(AboutPage));
            navigationService.Configure(PageIndex.PageAnimeDetails.ToString(), typeof(AnimeDetailsPage));
            navigationService.Configure(PageIndex.PageAnimeList.ToString(), typeof(AnimeListPage));
            navigationService.Configure(PageIndex.PageLogIn.ToString(), typeof(LogInPage));
            navigationService.Configure(PageIndex.PageProfile.ToString(), typeof(ProfilePage));
            navigationService.Configure(PageIndex.PageSearch.ToString(), typeof(AnimeSearchPage));
            return navigationService;
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public RecommendationsViewModel Recommendations => ServiceLocator.Current.GetInstance<RecommendationsViewModel>();      
        public HamburgerControlViewModel Hamburger => ServiceLocator.Current.GetInstance<HamburgerControlViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}

