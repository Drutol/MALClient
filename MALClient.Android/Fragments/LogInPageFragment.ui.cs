using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using Java.Lang;
using MALClient.Android.BindingConverters;
using MALClient.Android.Resources;
using MALClient.Models.Enums;

namespace MALClient.Android.Fragments
{
    public partial class LogInPageFragment : MalFragmentBase
    {
        #region Views

        private WebView _authWebView;
        private Button _signInButton;
        private ProgressBar _loginPageLoadingSpinner;
        private Button _loginPageLogOutButton;
        private Button _loginPageProblemsButton;
        private Button _loginPageRegisterButton;
        private FrameLayout _bottomButtonsSection;

        public WebView AuthWebView => _authWebView ?? (_authWebView = FindViewById<WebView>(Resource.Id.AuthWebView));
        public Button SignInButton => _signInButton ?? (_signInButton = FindViewById<Button>(Resource.Id.SignInButton));
        public ProgressBar LoginPageLoadingSpinner => _loginPageLoadingSpinner ?? (_loginPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.LoginPageLoadingSpinner));
        public Button LoginPageLogOutButton => _loginPageLogOutButton ?? (_loginPageLogOutButton = FindViewById<Button>(Resource.Id.LoginPageLogOutButton));
        public Button LoginPageProblemsButton => _loginPageProblemsButton ?? (_loginPageProblemsButton = FindViewById<Button>(Resource.Id.LoginPageProblemsButton));
        public Button LoginPageRegisterButton => _loginPageRegisterButton ?? (_loginPageRegisterButton = FindViewById<Button>(Resource.Id.LoginPageRegisterButton));
        public FrameLayout BottomButtonsSection => _bottomButtonsSection ?? (_bottomButtonsSection = FindViewById<FrameLayout>(Resource.Id.BottomButtonsSection));

        #endregion
    }
}