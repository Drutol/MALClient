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
using GalaSoft.MvvmLight.Ioc;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class ChangelogProvider : IChangeLogProvider
    {
        static ChangelogProvider()
        {
            var context = SimpleIoc.Default.GetInstance<Activity>();
            var package = context.PackageManager.GetPackageInfo(context.PackageName, 0);
            _currentVersion = package.VersionName;
        }

        private static readonly string _currentVersion;

        public string CurrentVersion => _currentVersion;

        public static string Version => _currentVersion;

        public bool NewChangelog { get; set; }

        public string DateWithVersion => $"{_currentVersion} - 16.08.2017";

        public List<string> Changelog => new List<string>
        {
           "Added clubs! I'm pretty sure there are bugs... go catch them!",
           "Profile page got visual tweaks.",
           "Navigating to profile will now refresh comments if profile data was cached.",
           "Heart in hamburger now has \"pulse\" animation ^^",
           "Images page will now fetch thumbnails of lower quality... works better but it won't display resolution anymore :(",
        };

    }
}