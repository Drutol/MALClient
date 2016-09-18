using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using MALClient.Adapters;

namespace MALClient.UWP.Adapters
{
    public class SystemControlLauncherService : ISystemControlsLauncherService
    {
        public async void LaunchUri(Uri uri)
        {
            await Launcher.LaunchUriAsync(uri);
        }
    }
}
