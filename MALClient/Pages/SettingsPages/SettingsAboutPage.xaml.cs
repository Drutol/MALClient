using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsAboutPage : Page
    {
        public SettingsAboutPage()
        {
            this.InitializeComponent();
            ListTodo.ItemsSource = new ObservableCollection<string>
            {
                "Check out github issue with my future plans.",
                "Want something? Let me know! Scroll just a little bit and go to the issues board :)"
            };
        }

        private async void Donate(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as MenuFlyoutItem;
                await CurrentApp.RequestProductPurchaseAsync(btn.Name, false);
                Settings.Donated = true;
            }
            catch (Exception)
            {
                // no donation
            }
        }

        private async void LaunchIssues(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Mordonus/MALClient/issues"));
        }

        private async void LaunchRepo(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Mordonus/MALClient"));
        }
    }
}
