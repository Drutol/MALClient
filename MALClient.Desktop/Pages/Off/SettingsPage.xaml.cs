using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.UWP.Pages.Main;
using MALClient.UWP.Pages.Off.SettingsPages;
using MALClient.UWP.Shared;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Off
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel.NavigationRequest += ViewModelOnNavigationRequest;
            SettingsNavFrame.Navigate(typeof(SettingsHomePage), null);
            ViewModelLocator.GeneralMain.CurrentOffStatus = $"Settings - {UWPUtilities.GetAppVersion()}";
        }

        public SettingsViewModelBase ViewModel => DataContext as SettingsViewModelBase;

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
                case SettingsPageIndex.Ads:
                    pageType = typeof(SettingsAdsPage);
                    break;
                case SettingsPageIndex.Feeds:
                    pageType = typeof(SettingsFeedsPage);
                    break;
                case SettingsPageIndex.Discord:
                    ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://discord.gg/5yETtFT"));
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
            SettingsNavFrame.Navigate(pageType, null);
        }


        private void SettingsNavFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            (SettingsNavFrame.Content as FrameworkElement).DataContext = ViewModel;
        }
    }
}