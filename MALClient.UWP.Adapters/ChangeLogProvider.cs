using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Adapters;
using MALClient.UWP.Shared;

namespace MALClient.UWP.Adapters
{
    public class ChangeLogProvider : IChangeLogProvider
    {
        public bool NewChangelog { get; set; }
        public string CurrentVersion => UWPUtilities.GetAppVersion();

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 15.04.2017";

        public List<string> Changelog => new List<string>
        {
            "Mainteance, fixes to charaters query and videos.",
            "UI tweaks in recommendations and anime details page.",
            "Notifications should no longer duplicate while app is running"        
        };


    }
}
