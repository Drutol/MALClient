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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 15.11.2016";

        public List<string> Changelog => new List<string>
        {
            "Articles&News tiles will now be updated from background (every 12 hours or so)",
            "Fixed notifications - there was slight change in html",
        };

    }
}
