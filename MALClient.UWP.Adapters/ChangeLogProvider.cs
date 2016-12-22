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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 24.12.2016";

        public List<string> Changelog => new List<string>
        {
            "Articles are now fetching 4 newest articles.",
            "Anime list by genre/studio can now load more pages.",
            "Fixed rewatching status change bug.",
            "Movies are now present in seasonal list.",
            "Tweaked anime reviews a bit.",
            "Removed hummingbird from login page, hummingbird is dead :(",
            "Fix K-ON! genre bug...",
            "Did you know? You can swipe/drag grid items in order to increment or decrement watched episodes. Many people seem to have missed this :)",
            "Oh and very Merry Christmas!",
        };

    }
}
