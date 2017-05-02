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

        public string DateWithVersion => $"{_currentVersion} - 30.04.2017";

        public List<string> Changelog => new List<string>
        {
            "Added ads... yay...",
            "Added dontations... yay...",
            "Added about page...",
            "Redesigned login page.",
            "Added popup which encourages to review app on the store.",
            "Tweaked hamburger a bit.",
            "Wallpapers page items are now a bit smarter about their sizing.",
            "Tweaked settings pages.",
            "The theme of this update: let's get ready for open beta... I'm estimating 1 month at most.",
        };

    }
}