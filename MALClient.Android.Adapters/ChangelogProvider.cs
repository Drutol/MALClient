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

        public string DateWithVersion => $"{_currentVersion} - 08.12.2020";

        public List<string> Changelog => new List<string>
        {
           "Reworked login flow due to recent changes that occured on MAL. " +
           "**You are required to sign in again**, this time directly on MAL's website. \n" +
           "This was quite sudden change and I'm trying to address the issue " +
           "as fast as possible so if anything will not work as expected I'll issue following updates.\n\n" +
           "Sorry for inconvenience. :("
        };
    }
}