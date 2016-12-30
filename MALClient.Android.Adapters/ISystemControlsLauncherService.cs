using System;
using Android.App;
using Android.Content;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class SystemControlLauncherService : ISystemControlsLauncherService
    {
        public async void LaunchUri(Uri uri)
        {
            String url = "http://www.example.com";
            Intent i = new Intent(Intent.ActionView);
            i = i.AddFlags(ActivityFlags.NewTask);
            i = i.SetData(global::Android.Net.Uri.Parse(uri.ToString()));
            Application.Context.StartActivity(i);
        }
    }
}
