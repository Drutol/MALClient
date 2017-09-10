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
           "Added friend requests. You can now add friends and manage pending requests.",
           "Added about section in profile page containing user description.",
           "Fixed 'more' button in hamburger's top anime section; it won't show anime status filters from now on",
           "Fixed lack of content in certain genre categories.",
           "A few fixes to widget.",
           "Disabled GIFs in forums as they were causing deadlocks... I'll enable them once the problem is solved."
        };

    }
}