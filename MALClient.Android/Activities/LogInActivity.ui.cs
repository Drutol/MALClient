using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;

namespace MALClient.Android.Activities
{
    public partial class LogInActivity
    {
        protected override void InitBindings()
        {
            _bindings = new Dictionary<int, Binding>
            {
                { UsernameInput.Id, this.SetBinding(() => ViewModel.UserNameInput,() => UsernameInput.Text,BindingMode.TwoWay)},
                { PasswordInput.Id, this.SetBinding(() => ViewModel.PasswordInput,() => PasswordInput.Text,BindingMode.TwoWay)},
                { ProgressSpinner.Id, this.SetBinding(() => ViewModel.Authenticating,() => ProgressSpinner.Visibility,BindingMode.OneWay).ConvertSourceToTarget(Converters.BoolToVisibility)},
            };
            SignInButton.SetCommand(ViewModel.LogInCommand);
        }

        private Button _signInButton;
        private EditText _usernameInput;
        private EditText _passwordInput;
        private ProgressBar _progressSpinner;

        public Button SignInButton => _signInButton ?? (_signInButton = FindViewById<Button>(Resource.Id.SignInButton));


        public EditText UsernameInput => _usernameInput ?? (_usernameInput = FindViewById<EditText>(Resource.Id.UsernameInput));


        public EditText PasswordInput => _passwordInput ?? (_passwordInput = FindViewById<EditText>(Resource.Id.PasswordInput));


        public ProgressBar ProgressSpinner => _progressSpinner ?? (_progressSpinner = FindViewById<ProgressBar>(Resource.Id.LoadingSpinner));

    }
}