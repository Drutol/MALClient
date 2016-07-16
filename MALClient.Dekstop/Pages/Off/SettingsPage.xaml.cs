using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.Utils;
using MalClient.Shared.ViewModels;
using MALClient.Pages.SettingsPages;
using MALClient.Utils;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    public delegate void SettingsNavigationRequest(Type pageType);

    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private bool _initialized;

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel.NavigationRequest += ViewModelOnNavigationRequest;
            SettingsNavFrame.Navigate(typeof(SettingsHomePage), null);
            ViewModelLocator.GeneralMain.CurrentOffStatus = $"Settings - {Utilities.GetAppVersion()}";
            _initialized = true;
        }

        public SettingsPageViewModel ViewModel => DataContext as SettingsPageViewModel;

        private void ViewModelOnNavigationRequest(Type pageType)
        {
            SettingsNavFrame.Navigate(pageType, null);
        }


        private void SettingsNavFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            (SettingsNavFrame.Content as FrameworkElement).DataContext = ViewModel;
        }
    }
}