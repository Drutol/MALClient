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

        public string DateWithVersion => $"{_currentVersion} - 16.06.2018";

        public List<string> Changelog => new List<string>
        {
            "Okay, so I have merged stuff that I had prepared for next update from before MAL going down.",
            "Added 3 new colour themes as requested on GitHub.",
            "Fixed being unable to donate twice (hopefully), thanks for GitHub report!",
            "We have brand new icon! Big thanks to @richardbmx!",
            "Fixed adding anime with new workaround.",
            "Manga should work again.",
            "",
            "PLEASE NOTE that it's still one big pile of workarounds because MAL is still missing APIs. I'm doing my best to make it work but it's hard when the website itself is unstable sometimes.",
            "Start and End dates are something I'd like to do next but from early research it's going to be a wild ride...",
            "If something explodes come join us on Discord, link can be found on GitHub",
            "Great thanks to developers of jikan.moe for providing altenative 3rd party search API!"
        };

    }
}