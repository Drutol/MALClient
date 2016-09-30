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
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Activities
{
    [Activity(Label = "LogInActivity")]
    public partial class LogInActivity : Activity
    {
        private LogInViewModel ViewModel;
        private List<Binding> _bindings;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ViewModel = ViewModelLocator.LogIn;
            ViewModel.Init();
            SetContentView(Resource.Layout.login_page);

            InitBindings();
        }


        private void InitBindings()
        {
            _bindings = new List<Binding>
            {
                this.SetBinding(() => ViewModel.UserNameInput,() => UsernameInput.Text,BindingMode.TwoWay),
                this.SetBinding(() => ViewModel.PasswordInput,() => PasswordInput.Text,BindingMode.TwoWay),    
                this.SetBinding(() => ViewModel.Authenticating,() => ProgressSpinner.Visibility,BindingMode.OneWay).ConvertSourceToTarget(Converters.BoolToVisibility)       
            };
            SignInButton.SetCommand(ViewModel.LogInCommand);
        }

       
    }
}