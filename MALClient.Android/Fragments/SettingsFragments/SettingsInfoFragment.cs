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
using MALClient.Android.Listeners;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.SettingsFragments
{
    public class SettingsInfoFragment : MalFragmentBase
    {
        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {

        }

        public override int LayoutResourceId => Resource.Layout.SettingsInfoPage;
    }
}