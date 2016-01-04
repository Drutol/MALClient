using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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
        }

        private async void AttemptAuthentication(object sender, RoutedEventArgs e)
        {
            Creditentials.Update(UserName.Text,UserPassword.Password);
            try
            {
                var response = await new AuthQuery().GetRequestResponse();
                Creditentials.SetAuthStatus(true);
                Utils.GetMainPageInstance().NavigateList();
            }
            catch (Exception exc)
            {
                var msg = new MessageDialog("Unable to authorize with provided creditentials.");
                await msg.ShowAsync();
                Creditentials.SetAuthStatus(false);
            }
            
        }
    }
}
