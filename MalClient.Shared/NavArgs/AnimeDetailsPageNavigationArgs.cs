using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Items;
using MALClient.Models;
using MALClient.Utils.Enums;

namespace MalClient.Shared.NavArgs
{
    public class AnimeDetailsPageNavigationArgs
    {
        public readonly AnimeGeneralDetailsData AnimeElement;
        public readonly IAnimeData AnimeItem;
        public readonly int Id;
        public readonly object PrevPageSetup;
        public readonly string Title;
        public bool AnimeMode = true;
        public bool RegisterBackNav = true;
        public PageIndex Source;
        public int SourceTabIndex;

        public AnimeDetailsPageNavigationArgs(int id, string title, AnimeGeneralDetailsData element,
            IAnimeData animeReference,
            object args = null)
        {
            Id = id;
            Title = title;
            AnimeElement = element;
            PrevPageSetup = args;
            AnimeItem = animeReference;
        }
    }
}
