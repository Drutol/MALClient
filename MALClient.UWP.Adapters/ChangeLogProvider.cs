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
        public static bool NewChangelog { get; set; }

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 08.02.2017";

        public List<string> Changelog => new List<string>
        {
            "Various forum tweaks and fixes.",
            "Added simple BBCode editor.",
            "Notification will now how 'Mark as read' button instead of 'Dismiss'",
            "Friends feeds will now display time difference.",
            "Profile comments and converstions will now display images.",
        };

    }
}
