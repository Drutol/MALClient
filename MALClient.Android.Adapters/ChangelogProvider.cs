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

        public string DateWithVersion => $"{_currentVersion} - 27.05.2017";

        public List<string> Changelog => new List<string>
        {
            "#64 Fixes, see github.",
            "Added english title preference to settings.",
            "Better offline mode handling.",
            "Improved landscape space usage.",
            "Added Lime theme.",
            "Visual tweaks for Charcter&Person pages.",
            "Fixed crashes on Android 5.0 devices.",   
        };

    }
}