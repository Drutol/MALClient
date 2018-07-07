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

        public string DateWithVersion => $"{_currentVersion} - 07.07.2018";

        public List<string> Changelog => new List<string>
        {
            "Staff details page now works better in landscape mode.",
            "",
            "PLEASE NOTE that it's still one big pile of workarounds because MAL is still missing APIs. I'm doing my best to make it work but it's hard when the website itself is unstable sometimes.",
            "Right now only status, score and watched episodes can be updated. That's because of lacking API.",
            "If something explodes come join us on Discord, link can be found on GitHub",
            "Great thanks to developers of jikan.moe for providing altenative 3rd party search API!"
        };

    }
}