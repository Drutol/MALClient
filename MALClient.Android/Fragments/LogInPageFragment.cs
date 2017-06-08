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
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
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
            RootView.ViewTreeObserver.GlobalLayout += ViewTreeObserverOnGlobalLayout;
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

            PasswordInput.SetOnEditorActionListener(new OnEditorActionListener(action =>
            {
                if(action == ImeAction.Done)
                    ViewModel.LogInCommand.Execute(null);
                AndroidUtilities.HideKeyboard();
            }));

            LoginPageRegisterButton.SetOnClickListener(new OnClickListener(v => ViewModel.NavigateRegister.Execute(null)));
            LoginPageProblemsButton.SetOnClickListener(new OnClickListener(v => ViewModel.ProblemsCommand.Execute(null)));

            SignInButton.SetOnClickListener(new OnClickListener(v =>
            {
                ViewModel.LogInCommand.Execute(null);
                AndroidUtilities.HideKeyboard();
            }));
            LoginPageLogOutButton.SetOnClickListener(new OnClickListener(v=>ViewModel.LogOutCommand.Execute(null)));
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