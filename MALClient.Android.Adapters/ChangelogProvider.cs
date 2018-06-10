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

        public string DateWithVersion => $"{_currentVersion} - 10.06.2018";

        public List<string> Changelog => new List<string>
        {
            "-- WARNING --",
            "As you are probably aware, MAL is going through difficult period, portions of website are down and there's no API whatsoever. I've managed to create some workarounds to restore basic functionality.",
            "Following things should work:",
            "Anime search",
            "Fetching your own AnimeList,",
            "Updating Status,Watched Episodes,Score",
            "Anime Details",
            "Airing anime calculations",
            "And some other lesser features...",
            "",
            "This update will force sign out you so please sign in again."
        };

    }
}