using System;
using System.Xml.Linq;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Comm;
using MALClient.UserControls;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
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
            if (Credentials.Authenticated)
                BtnLogOff.Visibility = Visibility.Visible;
            Utils.GetMainPageInstance()
                .CurrentStatus = Credentials.Authenticated ? $"Logged in as {Credentials.UserName}" : "Log In";            
        }

        private async void AttemptAuthentication(object sender, RoutedEventArgs e)
        {
            if (_authenticating)
                return;
            ProgressRing.Visibility = Visibility.Visible;
            _authenticating = true;
            Credentials.Update(UserName.Text, UserPassword.Password);
            try
            {
                var response = await new AuthQuery().GetRequestResponse(false);
                if (string.IsNullOrEmpty(response))
                    throw new Exception();
                var doc = XDocument.Parse(response);
                Credentials.SetId(int.Parse(doc.Element("user").Element("id").Value));
                Credentials.SetAuthStatus(true);
            }
            catch (Exception)
            {
                Credentials.SetAuthStatus(false);
                Credentials.Update(string.Empty, string.Empty);
                var msg = new MessageDialog("Unable to authorize with provided credentials.");
                await msg.ShowAsync();
            }
            try
            {
                await Utils.RemoveProfileImg();
                await ViewModelLocator.Hamburger.UpdateProfileImg();
            }
            catch (Exception)
            {
                //
            }

            ViewModelLocator.AnimeList.LogIn();
            await ViewModelLocator.Main.Navigate(PageIndex.PageAnimeList);
            ViewModelLocator.Hamburger.SetActiveButton(HamburgerButtons.AnimeList);

            _authenticating = false;
            ProgressRing.Visibility = Visibility.Collapsed;
        }

        private async void LogOut(object sender, RoutedEventArgs e)
        {
            var page = Utils.GetMainPageInstance();
            Credentials.SetAuthStatus(false);
            Credentials.Update("", "");
            await Utils.RemoveProfileImg();
            ViewModelLocator.AnimeList.LogOut();
            await page.Navigate(PageIndex.PageLogIn);
            ViewModelLocator.Hamburger.UpdateProfileImg();
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

        private async void ButtonRegister_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://myanimelist.net/register.php"));
        }

        private async void ButtonProblems_OnClick(object sender, RoutedEventArgs e)
        {
            var msg = new MessageDialog("If you are experiencing constant error messages while trying to log in , resetting your password on MAL may solve this issue. Why you may ask... MAL api is just very very bad and it tends to do such things which are beyond my control.");
            await msg.ShowAsync();
        }
    }
}