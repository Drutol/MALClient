using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Renderscripts;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.PagerAdapters;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.ProfilePageFragments
{
    public class ProfilePageStatsTabFragment : MalFragmentBase
    {

        private ProfilePageViewModel ViewModel = ViewModelLocator.ProfilePage;

        public ProfilePageStatsTabFragment() : base(true, false)
        {
            
        }

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            
        }

        public override int LayoutResourceId => Resource.Layout.ProfilePageStatsTab;
    }
}