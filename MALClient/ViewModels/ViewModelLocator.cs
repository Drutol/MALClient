using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using MALClient.Pages;
using Microsoft.Practices.ServiceLocation;

namespace MALClient.ViewModels
{

    public class ViewModelLocator
    {

        private static bool _initialized;
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            if(_initialized)
                return;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<HamburgerControlViewModel>();

            SimpleIoc.Default.Register<RecommendationsViewModel>();
            SimpleIoc.Default.Register<AnimeListViewModel>();
            
            _initialized = true;
        }

        public static MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public static RecommendationsViewModel Recommendations => ServiceLocator.Current.GetInstance<RecommendationsViewModel>();      
        public static HamburgerControlViewModel Hamburger => ServiceLocator.Current.GetInstance<HamburgerControlViewModel>();
        public static AnimeListViewModel AnimeList => ServiceLocator.Current.GetInstance<AnimeListViewModel>();

        public static void Cleanup()
        {

        }
    }
}

