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
           "I believe I've finally fixed widget. Fingers crossed.",
           "Added option to display score dialog after unraked is marked as completed.",
           "Clicking on cover image in anime details page will now show zoomable version + button to download.",
           "About button in profile page now looks a bit less awful.",
           "Fixed duplicated messages issue."
        };

    }
}