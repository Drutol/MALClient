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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 02.03.2017";

        public List<string> Changelog => new List<string>
        {
            "Added option to prefer English titles.",
            "Fixed anime reviews fetching.",
            "Opening forum from show details will now allow to navigate back to whatever page was previously active.",
            "Why so little and why so infrequent updates? I'm working on Android version and real life has been crazy lately so I didn't have chance to do much :(\nThat being said you can catch me on github every day!",
        };

    }
}
