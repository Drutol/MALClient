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

        public string DateWithVersion => $"{_currentVersion} - 28.04.2019";

        public List<string> Changelog => new List<string>
        {
            "Added option to add(squeeze) one more column to grid in anime list.",
            "Added option to hide decrement buttons and enlarge increment ones.",
            "Added option to hide global score on anime details page for yet to be rated shows.",
            "Added option to disable prompts for status change for OnHold shows.",
            "Fix for manga volumes number change prompt.",
            "Major changes on the behind the scenes side of things.",
        };
    }
}