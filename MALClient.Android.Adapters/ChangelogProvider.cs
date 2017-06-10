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

        public string DateWithVersion => $"{_currentVersion} - 10.06.2017";

        public List<string> Changelog => new List<string>
        {
            "#64 Fixes, see github.",
            "Added profile pinning.",
            "Added \"Did you know\" page to Settings&More",
            "Fixes to anime reviews.",
            "Various optimizations...",
            "---",
            "I'm aiming for release this weekend, if you've been keeping some bugs for yourself now is the time to share them. Therefore I'll mark this build with \"RC\" suffix."
  
        };

    }
}