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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 17.06.2017";

        public List<string> Changelog => new List<string>
        {
            "Minor bugfixes and some crashfixes.",
            "Better offiline handling",
            "Android is now live on the store and so I'll now continue adding more stuff. See github for future plans! :)"
        };


    }
}
