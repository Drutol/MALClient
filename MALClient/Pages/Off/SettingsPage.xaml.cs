using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MALClient.Pages.Off.SettingsPages;
using MALClient.ViewModels;

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
            MobileViewModelLocator.Main.CurrentStatus = $"Settings - {Utilities.GetAppVersion()}";
            _initialized = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_initialized)
                ViewModelLocator.NavMgr.DeregisterBackNav();
            base.OnNavigatingFrom(e);
        }

        private void ViewModelOnNavigationRequest(Type pageType)
        {
            SettingsNavFrame.Navigate(pageType, null);
        }

        public SettingsPageViewModel ViewModel => DataContext as SettingsPageViewModel;


        private void SettingsNavFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            (SettingsNavFrame.Content as FrameworkElement).DataContext = ViewModel;
        }
    }
}