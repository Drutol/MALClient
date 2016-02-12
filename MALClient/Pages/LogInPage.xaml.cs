using System;
using System.Xml.Linq;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
        public LogInPage()
        {
            InitializeComponent();
            if (Creditentials.Authenticated)
                BtnLogOff.Visibility = Visibility.Visible;
            Utils.GetMainPageInstance()
                .CurrentStatus = Creditentials.Authenticated ? $"Logged in as {Creditentials.UserName}" : "Log In";
        }

        private bool _authenticating;

        private async void AttemptAuthentication(object sender, RoutedEventArgs e)
        {
            if(_authenticating)
                return;
            _authenticating = true;
            Creditentials.Update(UserName.Text, UserPassword.Password);
            try
            {
                var response = await new AuthQuery().GetRequestResponse(false);
                if (string.IsNullOrEmpty(response))
                    throw new Exception();
                XDocument doc = XDocument.Parse(response);
                Creditentials.SetId(int.Parse(doc.Element("user").Element("id").Value));               
                Creditentials.SetAuthStatus(true);
                await Utils.RemoveProfileImg();
                var hamburger = ViewModelLocator.Hamburger;
                ViewModelLocator.AnimeList.LogIn();
                await ViewModelLocator.Main.Navigate(PageIndex.PageAnimeList);
                hamburger.SetActiveButton(HamburgerButtons.AnimeList);
                await hamburger.UpdateProfileImg();
            }
            catch (Exception)
            {
                Creditentials.SetAuthStatus(false);
                Creditentials.Update(string.Empty, string.Empty);
                var msg = new MessageDialog("Unable to authorize with provided creditentials.");
                await msg.ShowAsync();
            }
            _authenticating = false;
        }

        private async void LogOut(object sender, RoutedEventArgs e)
        {
            var page = Utils.GetMainPageInstance();
            Creditentials.SetAuthStatus(false);
            Creditentials.Update("", "");
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
            }
        }

        private void Password_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
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
    }
}