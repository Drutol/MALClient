using System;
using System.Xml.Linq;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.Comm;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogInPage : Page
    {
        public LogInPage()
        {
            this.InitializeComponent();
            if(Creditentials.Authenticated)
                BtnLogOff.Visibility = Visibility.Visible;
            Utils.GetMainPageInstance()?.SetStatus(Creditentials.Authenticated ? $"Logged in as {Creditentials.UserName}" : "Log In");
        }


        private async void AttemptAuthentication(object sender, RoutedEventArgs e)
        {
            Creditentials.Update(UserName.Text,UserPassword.Password);
            try
            {
                var response = await new AuthQuery().GetRequestResponse();
                var doc = XDocument.Parse(response);
                Creditentials.SetId(int.Parse(doc.Element("user").Element("id").Value));
                Creditentials.SetAuthStatus(true);
                Utils.GetMainPageInstance().Navigate(PageIndex.PageAnimeList);
                Utils.DownloadProfileImg();             
            }
            catch (Exception)
            { 
                Creditentials.SetAuthStatus(false);
                var msg = new MessageDialog("Unable to authorize with provided creditentials.");
                await msg.ShowAsync();             
            }           
        }

        private void LogOut(object sender, RoutedEventArgs e)
        {
            Creditentials.Update("","");
            Creditentials.SetAuthStatus(false);
            Utils.GetMainPageInstance().Navigate(PageIndex.PageLogIn);
            Utils.GetMainPageInstance().UpdateHamburger();
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
                if(txt.Password.Length == 0)
                    return;
                txt.IsEnabled = false; //reset input
                txt.IsEnabled = true;
                AttemptAuthentication(null,null);
            }
        }
    }
}
