using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HtmlAgilityPack;
using MALClient.Comm;
using MALClient.Models;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{


    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            SpinnerLoading.Background = new SolidColorBrush(Color.FromArgb(160, 230, 230, 230));
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            (DataContext as ProfilePageViewModel).LoadProfileData();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            Utils.GetMainPageInstance().CurrentStatus = $"{Creditentials.UserName} - Profile";

            NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (DataContext as ProfilePageViewModel).Cleanup();
            base.OnNavigatedFrom(e);
            NavMgr.DeregisterBackNav();
        }


    }
}