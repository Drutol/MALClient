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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 06.11.2016";

        public List<string> Changelog => new List<string>
        {
            "Fixed a few search issues.",
            "Fixed articles&news live tiles + tweaked their look.",
            "Completing series instantaneously will correctly set both dates now.",
            "Fixed comment-to-comment coversation refresh + added refresh button.",
            "Aside from changelog: if you are wondering why development speed dropped, I'm working on android version and it takes majority of my (limited)time right now, I'm and will be around though :)"
        };

    }
}
