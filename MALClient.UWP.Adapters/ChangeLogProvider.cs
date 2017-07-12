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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 12.07.2017";

        public List<string> Changelog => new List<string>
        {
            "Comparison list slight visual tweaks... moar blur.",
            "A few bugfixes on coparison list page.",
            "Fixed anime recommendations",
        };


    }
}
