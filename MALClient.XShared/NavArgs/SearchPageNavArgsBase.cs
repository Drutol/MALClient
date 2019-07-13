using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.NavArgs
{
    public enum SearchPageDisplayModes
    {
        Main,
        Off
    }

    public class SearchPageNavArgsBase
    {
        public SearchPageDisplayModes DisplayMode { get; set; }
    }

    public class SearchPageNavigationArgs : SearchPageNavArgsBase
    {
        public bool ByGenre { get; set; }
        public bool ByStudio { get; set; }
        public bool Anime { get; set; } = true;
        public string Query { get; set; }
        public bool ForceQuery { get; set; }
        public bool Everywhere { get; set; }
    }
}
