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

        public string DateWithVersion => $"{_currentVersion} - 03.09.2017";

        public List<string> Changelog => new List<string>
        {
           "Further unified airing data. Now Grid&List items will display smaller countdown to episode if it's airing today and countdown to first episode if its more than a week away.",
           "Added switch in settings to make dark theme darker to make it better on AMOLED displays. Requested on GitHub.",
           "There will be now more reviews for show in details, capped at 20.",
        };

    }
}