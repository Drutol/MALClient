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

        public string DateWithVersion => $"{_currentVersion} - 09.07.2017";

        public List<string> Changelog => new List<string>
        {
            "Added anime list comparison.",
            "Notifications *should* be more reliable and now will properly start working after reboot.",
            "Added option to reverse swipe direction on grid items.",
            "Added alternate title display to anime details page (details tab ^^) for en/jp title depending on language preference.",
            "Friends feeds now look better in smaller grid mode.",
            "Fixed misc settings page.",
            "Fixed issues on devices with arabic culture info.",
            "A few crashfixes.",
            "I'll be unavailable during 13.07-20.07 period so if anything breaks I won't be able to fix it. Let's hope that MAL doesn't change something abruptly :)"
        };

    }
}