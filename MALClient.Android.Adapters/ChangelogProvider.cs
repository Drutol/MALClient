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

        public string DateWithVersion => $"{_currentVersion} - 20.05.2017";

        public List<string> Changelog => new List<string>
        {
            "A few bugfixes from #64",
            "Compact view usable?",
            "Added used memory watching, app should no longer close for no apparent reason if this reason is lack of memory... xd. Images will start to load to memory again from storage.",
            "I want this to be last closed beta update and release this to open beta this weekend, if there's something big that I'm not aware of this is the time to speak!"
        };

    }
}