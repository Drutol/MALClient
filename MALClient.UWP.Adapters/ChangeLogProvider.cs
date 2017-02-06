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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 02.02.2017";

        public List<string> Changelog => new List<string>
        {
            "New native forum topic page!",
            "Native forum topic creation page.",
            "You can now locally 'star' others' messages.",
            "Fixed issues with 'load more' functionality.",
            "Messaging will now display images.",
            "Resolved random crashes when app was in the background.",
            "Notifications interacted with will be marked as read.",
        };

    }
}
