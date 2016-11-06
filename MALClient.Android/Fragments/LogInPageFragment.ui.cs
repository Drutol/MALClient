using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Models.Enums;

namespace MALClient.Android.Fragments
{
    public partial class LogInPageFragment : MalFragmentBase
    {
        protected override void InitBindings()
        {
            Bindings = new Dictionary<int, List<Binding>>
            {
                {
                    UsernameInput.Id,
                    new List<Binding> { this.SetBinding(() => ViewModel.UserNameInput, () => UsernameInput.Text, BindingMode.TwoWay)}
                },
                {
                    PasswordInput.Id,
                    new List<Binding> { this.SetBinding(() => ViewModel.PasswordInput, () => PasswordInput.Text, BindingMode.TwoWay)}
                },
                {
                    LoadingSpinner.Id,
                    new List<Binding> { this.SetBinding(() => ViewModel.Authenticating, () => LoadingSpinner.Visibility, BindingMode.OneWay)
                        .ConvertSourceToTarget(Converters.BoolToVisibility)}
                },
            };
            LoginPageButtonHum.Click += LoginPageButtonOnClick;
            LoginPageButtonMal.Click += LoginPageButtonOnClick;
            SignInButton.SetCommand(ViewModel.LogInCommand);
        }

        private void LoginPageButtonOnClick(object sender, EventArgs eventArgs)
        {
            var btn = sender as ToggleButton;
            switch (btn.Id)
            {
                case Resource.Id.LoginPageButtonMal:
                    LoginPageButtonHum.Checked = false;
                    ViewModel.FocusMalCommand.Execute(null);
                    break;
                case Resource.Id.LoginPageButtonHum:
                    LoginPageButtonMal.Checked = false;
                    ViewModel.FocusHumCommand.Execute(null);
                    break;
            }
        }

        private ToggleButton _loginPageButtonMal;
        private ToggleButton _loginPageButtonHum;
        private EditText _usernameInput;
        private EditText _passwordInput;
        private Button _signInButton;
        private ProgressBar _loadingSpinner;

        public ToggleButton LoginPageButtonMal => _loginPageButtonMal ?? (_loginPageButtonMal = FindViewById<ToggleButton>(Resource.Id.LoginPageButtonMal));

        public ToggleButton LoginPageButtonHum => _loginPageButtonHum ?? (_loginPageButtonHum = FindViewById<ToggleButton>(Resource.Id.LoginPageButtonHum));

        public EditText UsernameInput => _usernameInput ?? (_usernameInput = FindViewById<EditText>(Resource.Id.UsernameInput));

        public EditText PasswordInput => _passwordInput ?? (_passwordInput = FindViewById<EditText>(Resource.Id.PasswordInput));

        public Button SignInButton => _signInButton ?? (_signInButton = FindViewById<Button>(Resource.Id.SignInButton));

        public ProgressBar LoadingSpinner => _loadingSpinner ?? (_loadingSpinner = FindViewById<ProgressBar>(Resource.Id.LoadingSpinner));




    }
}