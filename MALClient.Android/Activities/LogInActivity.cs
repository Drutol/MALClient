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
    [Activity(Label = "LogInMalActivity")]
    public partial class LogInActivity : MalActivityBase
    {
        private LogInViewModel ViewModel { get; set; }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.LogIn;
            ViewModel.Init();
            SetContentView(Resource.Layout.login_page);
        }

    }
}