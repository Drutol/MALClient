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

        public string DateWithVersion => $"{_currentVersion} - 03.06.2017";

        public List<string> Changelog => new List<string>
        {
            "#64 Fixes, see github.",
            "Big startup optimizations (5 secs on my test device), due to nature of these changes some random crashes may appear which I didn't spot while testing. Please report on github :)",
            "Articles crashfix & some other crashfixes.",
            "Color themes can now be changed without restarting app.",
            "Added 'Load all details' and 'Set list source' to action button in anime list (menu appears on long click, yeah I didn't have better idea)",
            "Random fixes here and there as always.",
            "---",
            "Well, semester is coming to an end, and so exams are approaching. Updates will be less beefy for next 2/3 weeks, full release will be postponed too unfortunately :("
  
        };

    }
}