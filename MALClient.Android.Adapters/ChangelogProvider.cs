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

        public string DateWithVersion => $"{_currentVersion} - 17.06.2017";

        public List<string> Changelog => new List<string>
        {
            "Fixed item refresh in detailed grid display mode after adding show to list.",
            "Maybe fixed possible issue with Thai calendar. Just a guess based on vague crashlogs.",
            "Crashfixes for wallpapers page.",
            "Fixed marking notifications as read.",
            "Yeah boring update... new stuff will come once I finish my exams. Bear with me please :)",
            "I've made a list with plans for future features, see github!"
        };

    }
}