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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 17.01.2017";

        public List<string> Changelog => new List<string>
        {
            "Added anime/manga suggestions.",
            "Added notification hub page.",
            "Added friends feeds settings page.",
            "Fixed searching by tags.",
            "Mal link parsing should be much more stable now. (apps for websites)",
            "Related tab in details page will now display all possible data. (you may see some oddities after this update, just refresh the page in that case)",
            "Changing anime status on filtered list will now make it opaque.",
            "Details page tab headers will now shrink once when there's not enough space.",
            "Notifications can now be pulled at steady intervals when app is running.",
        };

    }
}
