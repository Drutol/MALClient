using System;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;

namespace MALClient.Models.Models.AnimeScrapped
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

        public AnimeGeneralDetailsData DependentData { get; set; }

        public AnimeGeneralDetailsData RecommendationData { get; set; }

    }
}