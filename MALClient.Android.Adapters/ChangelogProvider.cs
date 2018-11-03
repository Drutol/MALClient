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

        public string DateWithVersion => $"{_currentVersion} - 03.11.2018";

        public List<string> Changelog => new List<string>
        {
            "Added new option to share your anime/manga list changes! Whenever you change status/score/episodes of an anime or manga share button will appear for 10 seconds in bottom left corner.",
            "I've decided to switch the ads from opt-in to opt-out, this change will only affect people who never ever enabled ads. " +
            "I'm doing this partly due to curiosity, partly to raise awareness as some people didn't even know it's possible while wanting to support the app. You can disable them in settings as always."           
        };

    }
}