using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.Pages.Main;
using MALClient.Shared;
using MALClient.Pages.Off.SettingsPages;
using MALClient.ViewModels;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Off
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private bool _initialized;

        public SettingsPage()
        {
            InitializeComponent();
            Loaded += (sender, args) => ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            ViewModel.NavigationRequest += ViewModelOnNavigationRequest;
            SettingsNavFrame.Navigate(typeof(SettingsHomePage), null);
            MobileViewModelLocator.Main.CurrentStatus = $"Settings - {UWPUtilities.GetAppVersion()}";
            _initialized = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_initialized)
                ViewModelLocator.NavMgr.DeregisterBackNav();
            base.OnNavigatingFrom(e);
        }

        private void ViewModelOnNavigationRequest(SettingsPageIndex page)
        {
            Type pageType;
            switch (page)
            {
                case SettingsPageIndex.General:
                    pageType = typeof(SettingsGeneralPage);
                    break;
                case SettingsPageIndex.Caching:
                    pageType = typeof(SettingsCachingPage);
                    break;
                case SettingsPageIndex.Calendar:
                    pageType = typeof(SettingsCalendarPage);
                    break;
                case SettingsPageIndex.Articles:
                    pageType = typeof(SettingsArticlesPage);
                    break;
                case SettingsPageIndex.News:
                    pageType = typeof(SettingsNewsPage);
                    break;
                case SettingsPageIndex.About:
                    pageType = typeof(SettingsAboutPage);
                    break;
                case SettingsPageIndex.LogIn:
                    pageType = typeof(LogInPage);
                    break;
                case SettingsPageIndex.Misc:
                    pageType = typeof(SettingsMiscPage);
                    break;
                case SettingsPageIndex.Homepage:
                    pageType = typeof(SettingsHomePage);
                    break;
                case SettingsPageIndex.Notifications:
                    pageType = typeof(SettingsNotificationsPage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
            SettingsNavFrame.Navigate(pageType, null);
        }

        public SettingsViewModel ViewModel => DataContext as SettingsViewModel;


        private void SettingsNavFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            (SettingsNavFrame.Content as FrameworkElement).DataContext = ViewModel;
        }
    }
}