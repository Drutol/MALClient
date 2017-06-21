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

        public string DateWithVersion => $"{_currentVersion} - 21.06.2017";

        public List<string> Changelog => new List<string>
        {
            "Added option to make grid items in various places smaller. Useful for devices with smaller displays. Let me know if it doesn't work well on yours.",
            "Devices with less than 1G of RAM will display lower quality show covers by default. I've been getting OOM crashes lately from this kind of devices.",
            "Added setting for default show status after adding to list.",
            "Added setting to enable/disable asking whether to send crash reports.",
            "After finishing rewatching, popup will appear asking about status change.",
            "Fixed a few visual bugs related to rewatching.",
        };

    }
}