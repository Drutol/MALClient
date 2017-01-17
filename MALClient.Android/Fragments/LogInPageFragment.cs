using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
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

        public static LogInPageFragment Instance => new LogInPageFragment();
    }
}