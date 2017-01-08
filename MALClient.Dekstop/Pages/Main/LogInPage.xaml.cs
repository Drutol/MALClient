using System;
using System.Xml.Linq;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.Shared;
using MALClient.Models.Enums;
using MALClient.XShared.Comm;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    public sealed partial class LogInPage : Page
    {
        public LogInPage()
        {
            InitializeComponent();
            //if (Settings.SelectedApiType == ApiType.Mal)
            //{
            //    MalLoginButton.IsChecked = true;
            //    MalLoginButton_OnChecked(null, null);
            //}
            //else
            //{
            //    HumLoginButton.IsChecked = true;
            //    HumLoginButton_OnChecked(null,null);
            //}
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.LogIn.Init();
            base.OnNavigatedTo(e);
        }

        private async void ButtonProblems_OnClick(object sender, RoutedEventArgs e)
        {
            var msg =
                new MessageDialog(
                    "If you are experiencing constant error messages while trying to log in , resetting your password on MAL may solve this issue. Why you may ask... MAL api is just very very bad and it tends to do such things which are beyond my control.");
            await msg.ShowAsync();
        }
      
        private void UserName_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                UserPassword.Focus(FocusState.Keyboard);
                e.Handled = true;
            }
        }

        private void Password_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                var txt = sender as PasswordBox;
                if (txt.Password.Length == 0)
                    return;
                txt.IsEnabled = false; //reset input
                txt.IsEnabled = true;
                ViewModelLocator.LogIn.LogInCommand.Execute(null);
            }
        }

        //private void HumLoginButton_OnChecked(object sender, RoutedEventArgs e)
        //{
        //    HumLoginButton.LockToggle = true;
        //    MalLoginButton.LockToggle = false;
        //    MalLoginButton.IsChecked = false;
        //}

        //private void MalLoginButton_OnChecked(object sender, RoutedEventArgs e)
        //{
        //    MalLoginButton.LockToggle = true;
        //    HumLoginButton.LockToggle = false;
        //    HumLoginButton.IsChecked = false;
        //}
    }
}