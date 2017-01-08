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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 10.01.2017";

        public List<string> Changelog => new List<string>
        {
            "Added friends feeds!",
            "Added support for \"Apps for Websites\"*. (More info on github)",
            "Profile page got refreshed look.",
            "Refreshed history page.",
            "Tweaked hamburger with new bar under account button.",
            "Added search on copy in forums.",
            "Added email option to feedback menu.",
            "Fixed various messaging bugs.",
            "Forum notifications should now target last post of the topic.",
            "Various UI improvements.",
        };

    }
}
