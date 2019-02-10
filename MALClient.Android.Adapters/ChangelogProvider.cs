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

        public string DateWithVersion => $"{_currentVersion} - 20.12.2018";

        public List<string> Changelog => new List<string>
        {
            "Added workaround for english titles. Read more in settings.",
            "Added button to mark review as helpful.",
            "Added placeholder texts for search input to make it less confusing.",
            "You can now hide \"N/A\" labels on grid items.",
            "Added \"Days\" label in profiles stats.",
            "Added last read to default manga sorting options.",
            "----",
            "I've partnered up with Cuddly Octopus to display their ads when other are unavailable. " +
            "They are making quality authentic dakis for reasonable prices, " +
            "I've bought a few from them personally and can vouch for their products. Feel free to check them out! :)"
        };

    }
}