using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Adapters;
using MALClient.Shared;

namespace MALClient.UWP.Adapters
{
    public class ChangeLogProvider : IChangeLogProvider
    {
        public static bool NewChangelog { get; set; }

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 05.11.2016";

        public List<string> Changelog => new List<string>
        {
            "Added anime by studio and genre.",
            "Fixed missing image issues.",
            "Tweaked watched episodes flyout with quick selection buttons.",
            "Calendar will now not display OVAs.",
            "Start date will properly set on 1 episode shows.",
            "More search page bugfixes.",
            "Opening app from live tile will now properly check whether this entry is on your list.",
        };

    }
}
