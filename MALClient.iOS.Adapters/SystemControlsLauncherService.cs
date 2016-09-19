using System;
using MALClient.Adapters;
using UIKit;
using Foundation;

namespace MALClient.iOS.Adapters
{
	public class SystemControlLauncherService : ISystemControlsLauncherService
	{
		public void LaunchUri(Uri uri)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl(uri.AbsoluteUri));
		}
	}
}
