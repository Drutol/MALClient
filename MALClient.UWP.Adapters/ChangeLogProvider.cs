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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 07.02.2018";

        public List<string> Changelog => new List<string>
        {
            "It's been a while... Yeah...",
            "Whole tones of acrylics, reveals and other good stuff from fluent design. Say thanks to @Guerra24 from github for adding major part of it :)",
            "Fixed things that broke over the time, like recommendations and characters queries etc.",
        };
    }
}
