using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Comm;
using MALClient.Comm.Anime;
using MALClient.Models;

namespace MALClient.Items
{
    public class RecomendationData
    {
        private bool _loaded;
        //Keys
        public string DependentTitle { get; set; }
        public int DependentId { get; set; }

        //Keys
        public string RecommendationTitle { get; set; }
        public int RecommendationId { get; set; }


        public string Description { get; set; }

        public AnimeGeneralDetailsData DependentData { get; private set; }

        public AnimeGeneralDetailsData RecommendationData { get; private set; }

        public async Task FetchData()
        {
            if (_loaded)
                return;
            //Find for first
            DependentData =
                await
                    new AnimeGeneralDetailsQuery().GetAnimeDetails(false, DependentId.ToString(), DependentTitle, true);

            //Find for second
            RecommendationData =
                await
                    new AnimeGeneralDetailsQuery().GetAnimeDetails(false, RecommendationId.ToString(), RecommendationTitle, true);

            //If for some reason we fail
            if (DependentData == null || RecommendationData == null)
                throw new ArgumentNullException(); // I'm to lazy to create my own so this will suffice
            _loaded = true;
        }
    }
}