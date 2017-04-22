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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 23.04.2017";

        public List<string> Changelog => new List<string>
        {
            "Maintenance update, bugfixes tweaks and such. Upped SDK to Creators Update.",
            "Sorry for such infrequent lackluster updates... Android is still in development, but on the bright side I've reached closed beta already! Once finished, updates should become more interesting :)"
        };


    }
}
