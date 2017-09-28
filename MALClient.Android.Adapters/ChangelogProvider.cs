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

        public string DateWithVersion => $"{_currentVersion} - 28.09.2017";

        public List<string> Changelog => new List<string>
        {
           "Minor fixes and crashfixes here and there",
           "Images in related tab are slightly bigger",
           "Added yet another feedback option, this time it's in app chat where you can send me messages :) As always refer to heart button in hamburger.",
           "Unfortunately update freqency will decrease from now on. University is starting and both, uni and my work combined leave veeery little free time for development. I'm planning to rework images page next time :)"
        };

    }
}