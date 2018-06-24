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

        public string DateWithVersion => $"{_currentVersion} - 25.06.2018";

        public List<string> Changelog => new List<string>
        {
            "Anime and manga details are back to their more detailed state. (Dates, Synopsis)",
            "Possible fixes for missing Anime/Manga lists",
            "Restored sorting by last updated.",
            "",
            "PLEASE NOTE that it's still one big pile of workarounds because MAL is still missing APIs. I'm doing my best to make it work but it's hard when the website itself is unstable sometimes.",
            "Start and End dates are something I'd like to do next but from early research it's going to be a wild ride...",
            "If something explodes come join us on Discord, link can be found on GitHub",
            "Great thanks to developers of jikan.moe for providing altenative 3rd party search API!"
        };

    }
}