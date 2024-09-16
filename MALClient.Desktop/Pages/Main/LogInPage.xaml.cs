using System;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Main
{
    public sealed partial class LogInPage : Page
    {
        private bool _headerSent;
        private string _cookies;

        public LogInPage()
        {
            InitializeComponent();
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

        private void SignInButtonClick(object sender, RoutedEventArgs e)
        {
            SignInWebView.Visibility = Visibility.Visible;
            SignInWebView.Settings.IsJavaScriptEnabled = true;
            Navigate(new Uri("https://myanimelist.net/login.php"));
            SignInWebView.NavigationStarting += SignInWebViewOnNavigationStarting;
        }

        private void SignInWebViewOnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            var targeturl = args.Uri.ToString();
            
            
            if (targeturl.Contains("maloauth"))
            {
                try
                {
                    var regex = new Regex(".*maloauth\\?code=(.*)\\&.*");
                    var match = regex.Matches(targeturl);
                    ViewModelLocator.LogIn.SignIn(_cookies, match[0].Groups[1].Value);
                    SignInWebView.Visibility = Visibility.Collapsed;
                    return;
                }
                catch
                {
                    //error, display the error page?
                    args.Cancel = false;
                    return;
                }
            }
            

            if (targeturl == "https://myanimelist.net/" ||
                targeturl == "https://myanimelist.net/#" || //from google signi
                targeturl.StartsWith("https://myanimelist.net/#"))  //from fb signin
            {
                var filter = new HttpBaseProtocolFilter();
                var cookieCollection = filter.CookieManager.GetCookies(new Uri("https://myanimelist.net"));
                var cookieString = cookieCollection.Aggregate("", (s, cookie) => s += $"{cookie.Name}={cookie.Value};");

                _cookies = cookieString;
                var url = "https://myanimelist.net/v1/oauth2/authorize?response_type=code&" +
                          "client_id=183063f74126e7551b00c3b4de66986c&" +
                          "state=signin&" +
                          $"code_challenge={ViewModelLocator.LogIn.PkceChallenge}&" +
                          "code_challenge_method=plain";
                Navigate(new Uri(url));
                
               
                return;
            }

            if (_headerSent)
            {
                _headerSent = false;
                return;
            }

            if (targeturl == "https://myanimelist.net/register.php")
            {
                ViewModelLocator.LogIn.NavigateRegister.Execute(null);
            }

            if (targeturl.StartsWith("https://api.twitter") ||
                targeturl.StartsWith("https://myanimelist.net/sns/login") ||
                targeturl.StartsWith("https://myanimelist.net/sns/callback") ||
                targeturl.Contains("apple") ||
                targeturl.StartsWith("https://www.facebook") ||
                targeturl.StartsWith("https://m.facebook") ||
                targeturl.StartsWith("https://accounts.google") ||
                targeturl.StartsWith("https://accounts.youtube") ||
                targeturl.StartsWith("https://myanimelist.net/login.php") ||
                targeturl.StartsWith("https://myanimelist.net/submission/authorization") ||
                targeturl.StartsWith("https://myanimelist.net/dialog/authorization"))
            {
                args.Cancel = false;
                return;
            }

            args.Cancel = true;
            Navigate(new Uri("https://myanimelist.net/login.php"));
        }

        private void Navigate(Uri uri)
        {
            _headerSent = true;
            var rm = new HttpRequestMessage(HttpMethod.Get, uri);

            rm.Headers.Add("User-Agent", @"Mozilla/5.0 (Linux; Android 7.0; SM-G930V Build/NRD90M) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.125 Mobile Safari/537.36");
            SignInWebView.NavigateWithHttpRequestMessage(rm);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var myFilter = new HttpBaseProtocolFilter();
            var cookieManager = myFilter.CookieManager;
            var myCookieJar = cookieManager.GetCookies(new Uri("https://myanimelist.net"));
            foreach (var cookie in myCookieJar)
            {
                cookieManager.DeleteCookie(cookie);
            }

            ViewModelLocator.LogIn.LogOutCommand.Execute(null);
        }
    }
}