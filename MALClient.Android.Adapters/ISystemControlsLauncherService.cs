using System;
using Android.App;
using Android.Content;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class SystemControlLauncherService : ISystemControlsLauncherService
    {
        public void LaunchUri(Uri uri)
        {
            Intent i = new Intent(Intent.ActionView);
            i = i.AddFlags(ActivityFlags.NewTask);
            i = i.SetData(global::Android.Net.Uri.Parse(uri.ToString()));
            Application.Context.StartActivity(i);
        }
    }
}
