using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public partial class LogInPageFragment
    {
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
        }

        public override void OnResume()
        {
            MainActivity.CurrentContext.RequestedOrientation = ScreenOrientation.Portrait;
            base.OnResume();
        }

        protected override void InitBindings()
        {
            MainActivity.CurrentContext.RequestedOrientation = ScreenOrientation.Portrait;
            Bindings = new List<Binding>();
            Bindings.Add(this.SetBinding(() => ViewModel.UserNameInput, () => UsernameInput.Text, BindingMode.TwoWay));
            Bindings.Add(this.SetBinding(() => ViewModel.Authenticating, () => LoginPageLoadingSpinner.Visibility,
                    BindingMode.OneWay)
                .ConvertSourceToTarget(Converters.BoolToVisibility));
            Bindings.Add(this.SetBinding(() => ViewModel.PasswordInput, () => PasswordInput.Text, BindingMode.TwoWay));

            Bindings.Add(
                this.SetBinding(() => ViewModel.LogOutButtonVisibility,
                    () => LoginPageLogOutButton.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            LoginPageRegisterButton.SetCommand(ViewModel.NavigateRegister);
            LoginPageProblemsButton.SetCommand(ViewModel.ProblemsCommand);

            SignInButton.SetCommand(ViewModel.LogInCommand);
            LoginPageLogOutButton.SetCommand(ViewModel.LogOutCommand);

            RootView.ViewTreeObserver.GlobalLayout += (sender, args) =>
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
                    BottomButtonsSection.Visibility = ViewStates.Visible;
                }
            };
        }

        protected override void Cleanup()
        {
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