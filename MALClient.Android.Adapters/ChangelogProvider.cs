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

        public string DateWithVersion => $"{_currentVersion} - 19.01.2018";

        public List<string> Changelog => new List<string>
        {
            "Hello! Happy new year!\nA bit late... I know :) Sorry for such long period of inactivity, I got burned out, University & work took their toll on my time and such. Anyway, today's changes:",
            "Upgraded tons of libraries so things should work better.",
            "Upgraded Android SDK to 8.0",
            "Fixed characters pages.",
            "Fixed staff pages.",
            "You will be able to see your own history even when you set your list to be private.",
            "Fixed recommendations in details and general ones.",
            "Current episode completion of airing anime will be visible next to entry type like so \"TV 3/12\"",
            "Current episode highlight will be now much more visible in episode chooser.",
            "Okay... sooo... yeah. MAL is said to be releasing new API which should make things easier both for me and you, it's supposed to be availble in the coming months. I'll be definitely using it so things will stop breaking after some time ^^ Other than that? I'm still on hiatus, just a bit more let's say \"shallow\" hiatus. For any news check github... or just wait for another news-ish changelog :)"

        };

    }
}