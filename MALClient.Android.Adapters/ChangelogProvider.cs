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

        public string DateWithVersion => $"{_currentVersion} - 26.08.2017";

        public List<string> Changelog => new List<string>
        {
            "Calendar now works based on MUCH more accurate airing data.",
            "Anime aring countdows are much more accurate too.",
            "I've added option to watch VideoAd on demand if you wish to support development. You can find the button for it in \"heart\" hamburger button. I'm somewhat curious whether it's going to be more effective than curent banner ads ^^",
            "Random crashfix when opening app from anime details link.",

        };

    }
}