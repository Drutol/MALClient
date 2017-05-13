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

        public string DateWithVersion => $"{_currentVersion} - 04.05.2017";

        public List<string> Changelog => new List<string>
        {
            "Fixes from #64. Yup again... Just go and see the amount of bugs contained there...",
            "Added new topic page.",
            "Added message/see other posts buttons in topic itmes.",
            "Added option to hide manga section in hamburger.",
            "Fixed characters data scrapping.",
            "Other big change is that Xamarin Android 7.3 is bringing new Garbage Collector, which is currently marked as 'Experimental' and I've enabled it in this build in order to well... experiment? Let me know whether you feel any difference, in theory it should work faster."
        };

    }
}