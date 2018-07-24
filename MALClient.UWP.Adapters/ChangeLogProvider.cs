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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 25.07.2018";

        public List<string> Changelog => new List<string>
        {
            "Fixed hamburger pane not entering expanded state while the pane itself was expanded.",
            "Fixed calendar not displaying PTW shows when there was a lot of them."
        };
    }
}
