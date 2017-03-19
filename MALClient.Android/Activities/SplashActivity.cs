using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MALClient.Android.Activities
{
    [Activity(Label = "MALClient",Icon = "@drawable/icon", NoHistory = true, MainLauncher = false, Theme = "@style/Theme.Splash",ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var intent = new Intent(this, typeof(MainActivity));
            if(Intent.Extras != null)
                intent.PutExtra("launchArgs", Intent.Extras.GetString("launchArgs"));
            StartActivity(intent);
        }
    }
}