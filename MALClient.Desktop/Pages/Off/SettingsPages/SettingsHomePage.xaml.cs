using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Off.SettingsPages
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsHomePage : Page
    {
        public SettingsHomePage()
        {
            InitializeComponent();
        }

        private void PrivacyPolicyButtonOnClick(object sender, RoutedEventArgs e)
        {
            ResourceLocator.SystemControlsLauncherService.LaunchUri(
                new Uri("https://1drv.ms/b/s!Am1xQK-HVf_Ik9onFBtLN-4G0Pm_AQ"));
        }
    }
}