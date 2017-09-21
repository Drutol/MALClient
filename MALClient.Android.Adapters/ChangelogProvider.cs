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

        public string DateWithVersion => $"{_currentVersion} - 10.09.2017";

        public List<string> Changelog => new List<string>
        {
           "Added offline sync! Any changes made without internet will be stored locally and synced on next app launch, they also may be synced during runtime once internet comes back but that's not guaranteed.",
           "Added two filters to anime list, you can now for example type \"score>8\" or \"ep<12\" in search bar to show shows that meet given condition. If you'd like to see any other create issue on github ^^.",
           "Swiping from right side in anime list will now result in displaying filters drawer."
        };

    }
}