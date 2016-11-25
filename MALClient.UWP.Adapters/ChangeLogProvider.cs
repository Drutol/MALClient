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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 24.11.2016";

        public List<string> Changelog => new List<string>
        {
            "Fixed a few search issues.",
            "Fixed articles&news live tiles + tweaked their look.",
            "Aside from changelog: if you are wondering why development speed dropped, I'm working on android version right now and it takes majority of my time right now, but I'll still continue this version once android catches up."
        };

    }
}
