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

        public string DateWithVersion => $"{_currentVersion} - 05.08.2017";

        public List<string> Changelog => new List<string>
        {
            "Small graphical fixes here and there:",
            "Grid items now have elevation",
            "Recommendation tabs are now not outright ugly,",
            "Peek posts in forum boards now have circular background",
            "You can now refresh message thread via pull to refresh.",
            "Okay... why so little and why so long since last update... I've been doing clubs! They took quite a bit of time but I've all logic ready and all what's left now is to write Android UI. (Windows 10 version is finished already). Stay tuned! :)",
            "Oh and I'm running out of bigger stuff to do... if you have any ideas feel free to write me on github (preferably), email or wherever you can find me ^^"
        };

    }
}