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

        public string DateWithVersion =>  $"v{UWPUtilities.GetAppVersion()} - 13.10.2019";

        public List<string> Changelog => new List<string>
        {
            "Fixed airing info and calendar.",
            "You can now find discord invite link in settings. Join out little community there. Also the easiest place to catch me if something happens.",
        };
    }
}
