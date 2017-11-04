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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 04.11.2017";

        public List<string> Changelog => new List<string>
        {
            "Various tweaks and fixes.",
            "Updated SDK to Fall Creators Update and added some blurs and such!"
        };


    }
}
