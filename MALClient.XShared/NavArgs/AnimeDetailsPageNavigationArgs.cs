using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.Library;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;

namespace MALClient.XShared.NavArgs
{
    public class AnimeDetailsPageNavigationArgs
    {
        public readonly AnimeGeneralDetailsData AnimeElement;
        public readonly IAnimeData AnimeItem;
        public int Id { get; private set; }
        public object PrevPageSetup;
        public readonly string Title;
        public bool AnimeMode = true;
        public bool RegisterBackNav = true;
        public PageIndex Source;
        public int SourceTabIndex;
        private bool _isHumId;

        public AnimeDetailsPageNavigationArgs(int id, string title, AnimeGeneralDetailsData element,
            IAnimeData animeReference,
            object args = null,bool humId = false)
        { 
            Id = id;
            Title = title;
            AnimeElement = element;
            PrevPageSetup = args;
            AnimeItem = animeReference;
            _isHumId = humId;

        }

        /// <summary>
        /// Prepares Id -> converts from hummingbird Id to mal one
        /// </summary>
        /// <returns></returns>
        public async Task Prepare()
        {
            Id = await new AnimeDetailsHummingbirdQuery(Id).GetHummingbirdId();
        }
    }
}
