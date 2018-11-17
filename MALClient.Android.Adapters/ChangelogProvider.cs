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

        public string DateWithVersion => $"{_currentVersion} - 17.11.2018";

        public List<string> Changelog => new List<string>
        {
            "Sign in will now handle more errors that MAL can throw.",
            "Episodes are no longer limited to 100.",
            "Fixed an issue with different display modes per filter setting.",
            "SDK upgrade to Android 9.0",
            "Various fixes around episodes.",
            "Fixes to end dates in entries which were airing only one day (movies for example).",
            "Fixed avatar in hamburger",
            "After navigating anime from search it will automatically load more data.",
            "Text adjustments here and there."
        };

    }
}