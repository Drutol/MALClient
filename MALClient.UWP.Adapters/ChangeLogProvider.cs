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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 08.11.2016";

        public List<string> Changelog => new List<string>
        {
            "Added this very popup, you will see it whenever the app updates itself. (you can find it in about page too!)",
            "Added searching by anime/manga type (movie,ova etc.)",
            "Added settings for episodes/status prompts.",
            "Fixed a few issues with light theme.",
            "Fixed news crashes.",
            "Resolved a few things with volumes focusing.",
        };

    }
}
