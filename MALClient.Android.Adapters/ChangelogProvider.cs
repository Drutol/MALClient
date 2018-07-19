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

        public string DateWithVersion => $"{_currentVersion} - 19.07.2018";

        public List<string> Changelog => new List<string>
        {
            "I hope to have stabilised http connections with MAL.",
            "Fixed html decoding on search page.",
            "Moved permissions check for saving images to happen on navigation to wallpapers page.",
            "Fixed mismatched chapters&volumes when searching for manga.",
            "Register button will now display a message that registration is disabled by MAL... I've received tons of 1 star reviews stating that registration doesn't work while it's not my responsibility at all. I hope that it will help ¯\\_(ツ)_/¯",
            "",
            "PLEASE NOTE that it's still one big pile of workarounds because MAL is still missing APIs. I'm doing my best to make it work but it's hard when the website itself is unstable sometimes.",
            "If something explodes come join us on Discord, link can be found on GitHub",
        };

    }
}