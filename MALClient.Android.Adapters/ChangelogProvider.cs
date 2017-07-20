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

        public string DateWithVersion => $"{_currentVersion} - 21.07.2017";

        public List<string> Changelog => new List<string>
        {
            "Fixes to reversing swipe order affecting other display modes.",
            "Fixed recent forum posts.",
            "I've found one more issue with notifications, maybe now they won't duplicate...",
            "Notifications on Nougat are now groupped.",
            "I'm back from my holidays and I'll start working on something now :)"           
        };

    }
}