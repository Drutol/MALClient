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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 20.03.2017";

        public List<string> Changelog => new List<string>
        {
            "Added application tile Jumplist.",
            "Improved \"Mark as Read\" button in toast notifications.",
            "Mobile&Desktop apps are now communicating with each other whether there was an library update and sync local data.",
            "Loading pages from links will no longer display Anime List before navigating to desired page if not needed."
        };

    }
}
