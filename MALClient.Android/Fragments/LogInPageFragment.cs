using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Webkit;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Android.Web;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
 
namespace MALClient.Android.Fragments
{
    public partial class LogInPageFragment
    {
        private ListenableWebClient _client;
        public override int LayoutResourceId => Resource.Layout.LogInPage;

        private LogInViewModel ViewModel { get; set; }

        protected override void Init(Bundle savedInstanceState)
        {

            ViewModel = ViewModelLocator.LogIn;
            ViewModel.Init();
            if (Credentials.Authenticated)
            {
                ViewModelLocator.NavMgr.ResetMainBackNav();
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            }

            ViewModel.Authenticating = false;
        }

        public override void OnResume()
        {
            RootView.ViewTreeObserver.GlobalLayout += ViewTreeObserverOnGlobalLayout;
            MainActivity.CurrentContext.RequestedOrientation = ScreenOrientation.Portrait;
            base.OnResume();
        }

        protected override void InitBindings()
        {
            MainActivity.CurrentContext.RequestedOrientation = ScreenOrientation.Portrait;
            Bindings = new List<Binding>();
            //Bindings.Add(this.SetBinding(() => ViewModel.UserNameInput, () => UsernameInput.Text, BindingMode.TwoWay));
            Bindings.Add(this.SetBinding(() => ViewModel.Authenticating, () => LoginPageLoadingSpinner.Visibility,
                    BindingMode.OneWay)
                .ConvertSourceToTarget(Converters.BoolToVisibility));
            //Bindings.Add(this.SetBinding(() => ViewModel.PasswordInput, () => PasswordInput.Text, BindingMode.TwoWay));

            //Bindings.Add(
            //    this.SetBinding(() => ViewModel.LogOutButtonVisibility,
            //        () => LoginPageLogOutButton.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            LoginPageLogOutButton.Visibility = ViewStates.Visible;

            //PasswordInput.SetOnEditorActionListener(new OnEditorActionListener(action =>
            //{
            //    if(action == ImeAction.Done)
            //        ViewModel.LogInCommand.Execute(null);
            //    AndroidUtilities.HideKeyboard();
            //}));

            LoginPageRegisterButton.SetOnClickListener(new OnClickListener(v => ViewModel.NavigateRegister.Execute(null)));
            LoginPageProblemsButton.SetOnClickListener(new OnClickListener(v => ViewModel.ProblemsCommand.Execute(null)));

            SignInButton.SetOnClickListener(new OnClickListener(v =>
            {
                if(ViewModel.Authenticating)
                    return;

                ViewModel.Authenticating = true;

                AuthWebView.Visibility = ViewStates.Visible;
                _client = new ListenableWebClient
                {
                    NavigateIfNoInterception = true
                };
                AuthWebView.SetWebViewClient(_client);
                AuthWebView.Settings.JavaScriptEnabled = true;
                AuthWebView.Settings.UserAgentString =
                    "Mozilla/5.0 (Linux; Android 7.0; SM-G930V Build/NRD90M) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.125 Mobile Safari/537.36";
                AuthWebView.LoadUrl("https://myanimelist.net/login.php");

                _client.NavigationInterceptOpportunity += NavigationInterceptOpportunity;
            }));

            LoginPageLogOutButton.SetOnClickListener(new OnClickListener(v=>
            {
                CookieManager.Instance.RemoveAllCookies(null);
                CookieManager.Instance.Flush();

                ViewModel.LogOutCommand.Execute(null);
            }));
        }

        private async Task<string> NavigationInterceptOpportunity(string targeturl)
        {
            if (targeturl == "https://myanimelist.net/" || 
                targeturl == "https://myanimelist.net/#" || //from google signi
                targeturl.StartsWith("https://myanimelist.net/#"))  //from fb signin
            {
                AuthWebView.Visibility = ViewStates.Gone;
                var cookies = CookieManager.Instance.GetCookie("https://myanimelist.net");

                ViewModel.SignIn(cookies);

                _client.NavigateIfNoInterception = false;
                return null;
            }

            if (targeturl == "https://myanimelist.net/register.php")
            {
                ViewModel.NavigateRegister.Execute(null);
            }

            if (targeturl.StartsWith("https://api.twitter") ||
                targeturl.StartsWith("https://myanimelist.net/sns/login") ||
                targeturl.StartsWith("https://myanimelist.net/sns/callback") ||
                targeturl.Contains("apple") ||
                targeturl.StartsWith("https://www.facebook") ||
                targeturl.StartsWith("https://m.facebook") ||
                targeturl.StartsWith("https://accounts.google") ||
                targeturl.StartsWith("https://accounts.youtube") ||
                targeturl.StartsWith("https://myanimelist.net/login.php"))
                return targeturl;

            return "https://myanimelist.net/login.php";
        }

        private async void ViewTreeObserverOnGlobalLayout(object sender, EventArgs eventArgs)
        {
            Rect r = new Rect();
            RootView.GetWindowVisibleDisplayFrame(r);
            int keypadHeight = RootView.RootView.Height - r.Bottom;

            if (keypadHeight > RootView.Height * 0.15)
            {
                BottomButtonsSection.Visibility = ViewStates.Gone;
            }
            else
            {
                await Task.Delay(100);
                BottomButtonsSection.Visibility = ViewStates.Visible;
            }
        }


        protected override void Cleanup()
        {
            RootView.ViewTreeObserver.GlobalLayout -= ViewTreeObserverOnGlobalLayout;
            MainActivity.CurrentContext.RequestedOrientation = ScreenOrientation.Unspecified;
            base.Cleanup();
        }

        public override void OnDestroy()
        {
            MainActivity.CurrentContext.RequestedOrientation = ScreenOrientation.Unspecified;
            base.OnDestroy();
        }


        public static LogInPageFragment Instance => new LogInPageFragment();
    }
}