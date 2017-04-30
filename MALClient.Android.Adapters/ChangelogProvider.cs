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

        private static string _currentVersion;


        public string CurrentVersion => _currentVersion;

        public static string Version => _currentVersion;

        public bool NewChangelog { get; set; }

        public string DateWithVersion => $"{_currentVersion} - 30.04.2017";

        public List<string> Changelog => new List<string>
        {
            "Fixes from @FoxInFlame's & @Kimod's issues.",
            "Added stats page.",
            "Added wallpapers page",
            "Profile page little reorganisation.",
            "More dialogs and clickable stuff in forums",
            "Scrolling on forums should be more fluid, not perfect though.",
            "Pinned topics are now working",
            "WebViews in Forums&Articles/News will now redirect into app if possible.",
            "Notification hub fixes and little redesign",
            "App should now launch on Android 5.0",
            "Ascending/Descending sorting",
            "Rewathing support, partial because I don't know where can I squeeze more controls...",
            "Uff, big one..."
        };

    }
}