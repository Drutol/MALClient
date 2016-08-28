using System;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class SystemControlLauncherService : ISystemControlsLauncherService
    {
        public async void LaunchUri(Uri uri)
        {
           // await Launcher.LaunchUriAsync(uri);
        }
    }
}
