using System;
using System.Xml.Linq;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MalClient.Shared.Comm;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogInPage : Page
    {
        private bool _authenticating;

        public LogInPage()
        {
            InitializeComponent();

            ViewModelLocator.GeneralMain
                .CurrentOffStatus = Credentials.Authenticated ? $"Logged in as {Credentials.UserName}" : "Log In";
            switch (Settings.SelectedApiType)
            {
                case ApiType.Mal:
                    ToggleMal.IsChecked = true;
                    ToggleMal.LockToggle = true;
                    MALLoginGrid.Visibility = Visibility.Visible;
                    if (Credentials.Authenticated)
                        BtnLogOff.Visibility = Visibility.Visible;
                    break;
                case ApiType.Hummingbird:
                    ToggleHum.IsChecked = true;
                    ToggleHum.LockToggle = true;
                    HumLoginGrid.Visibility = Visibility.Visible;
                    if (Credentials.Authenticated)
                        BtnLogOffHum.Visibility = Visibility.Visible;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void LogOut(object sender, RoutedEventArgs e)
        {
            Credentials.SetAuthStatus(false);
            Credentials.Reset();
            Credentials.SetAuthToken("");
            await Utilities.RemoveProfileImg();
            //ViewModelLocator.GeneralAnimeList.LogOut();
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
            await ViewModelLocator.GeneralHamburger.UpdateProfileImg();
        }

        private async void ButtonRegister_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://myanimelist.net/register.php"));
        }

        private async void ButtonRegisterHum_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://hummingbird.me/sign-up"));
        }

        private async void ButtonProblems_OnClick(object sender, RoutedEventArgs e)
        {
            var msg =
                new MessageDialog(
                    "If you are experiencing constant error messages while trying to log in , resetting your password on MAL may solve this issue. Why you may ask... MAL api is just very very bad and it tends to do such things which are beyond my control.");
            await msg.ShowAsync();
        }

        private void HummingbirdToggleButtonOnCheck(object sender, RoutedEventArgs e)
        {
            ToggleHum.LockToggle = true;
            ToggleMal.LockToggle = false;
            ToggleMal.IsChecked = false;
            HumLoginGrid.Visibility = Visibility.Visible;
            MALLoginGrid.Visibility = Visibility.Collapsed;
        }

        private void MalToggleButtonOnCheck(object sender, RoutedEventArgs e)
        {
            ToggleMal.LockToggle = true;
            ToggleHum.LockToggle = false;
            ToggleHum.IsChecked = false;
            HumLoginGrid.Visibility = Visibility.Collapsed;
            MALLoginGrid.Visibility = Visibility.Visible;
        }

        //prepare for big copy pasteeee...

        #region MAL

        private async void AttemptAuthentication(object sender, RoutedEventArgs e)
        {
            if (_authenticating)
                return;
            ProgressRing.Visibility = Visibility.Visible;
            _authenticating = true;
            Credentials.Update(UserName.Text, UserPassword.Password, ApiType.Mal);
            try
            {
                var response = await new AuthQuery(ApiType.Mal).GetRequestResponse(false);
                if (string.IsNullOrEmpty(response))
                    throw new Exception();
                var doc = XDocument.Parse(response);
                Settings.SelectedApiType = ApiType.Mal;
                Credentials.SetId(int.Parse(doc.Element("user").Element("id").Value));
                Credentials.SetAuthStatus(true);
                Utilities.TelemetryTrackEvent(TelemetryTrackedEvents.LoggedInMyAnimeList);
            }
            catch (Exception)
            {
                Credentials.SetAuthStatus(false);
                Credentials.Update(string.Empty, string.Empty, ApiType.Mal);
                var msg = new MessageDialog("Unable to authorize with provided credentials.");
                await msg.ShowAsync();
                ProgressRing.Visibility = Visibility.Collapsed;
                _authenticating = false;
                return;
            }
            try
            {
                await Utilities.RemoveProfileImg();
                await ViewModelLocator.GeneralHamburger.UpdateProfileImg();
            }
            catch (Exception)
            {
                //
            }
            ViewModelLocator.GeneralMain.HideOffContentCommand.Execute(null);
            await DataCache.ClearApiRelatedCache();
            //ViewModelLocator.GeneralAnimeList.LogIn();
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList);
            ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.AnimeList);

            _authenticating = false;
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
                AttemptAuthentication(null, null);
            }
        }

        #endregion

        #region Hum

        private async void AttemptHumAuthentication(object sender, RoutedEventArgs e)
        {
            if (_authenticating)
                return;
            ProgressRingHum.Visibility = Visibility.Visible;
            _authenticating = true;
            Credentials.Update(UserNameHum.Text, UserPasswordHum.Password, ApiType.Hummingbird);
            try
            {
                var response = await new AuthQuery(ApiType.Hummingbird).GetRequestResponse(false);
                if (string.IsNullOrEmpty(response))
                    throw new Exception();
                if (response.Contains("\"error\": \"Invalid credentials\""))
                    throw new Exception();
                Settings.SelectedApiType = ApiType.Hummingbird;
                Credentials.SetAuthToken(response);
                Credentials.SetAuthStatus(true);
                Utilities.TelemetryTrackEvent(TelemetryTrackedEvents.LoggedInHummingbird);
            }
            catch (Exception)
            {
                Credentials.SetAuthStatus(false);
                Credentials.Update(string.Empty, string.Empty, ApiType.Hummingbird);
                var msg = new MessageDialog("Unable to authorize with provided credentials.");
                await msg.ShowAsync();
                ProgressRingHum.Visibility = Visibility.Collapsed;
                _authenticating = false;
                return;
            }
            try
            {
                await Utilities.RemoveProfileImg();
                await ViewModelLocator.GeneralHamburger.UpdateProfileImg();
            }
            catch (Exception)
            {
                //
            }
            ViewModelLocator.GeneralMain.HideOffContentCommand.Execute(null);
            await DataCache.ClearApiRelatedCache();
            ViewModelLocator.AnimeList.LogIn();
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList);
            ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.AnimeList);

            _authenticating = false;
        }

        private void UserNameHum_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                UserPasswordHum.Focus(FocusState.Keyboard);
                e.Handled = true;
            }
        }

        private void PasswordHum_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                var txt = sender as PasswordBox;
                if (txt.Password.Length == 0)
                    return;
                txt.IsEnabled = false; //reset input
                txt.IsEnabled = true;
                AttemptHumAuthentication(null, null);
            }
        }

        #endregion
    }
}