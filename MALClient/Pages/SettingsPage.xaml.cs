using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Command;
using MALClient.Models;
using MALClient.Pages.SettingsPages;
using MALClient.UserControls;
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
            ViewModelLocator.Main.CurrentStatus = $"Settings - {Utils.GetAppVersion()}";
            _initialized = true;
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