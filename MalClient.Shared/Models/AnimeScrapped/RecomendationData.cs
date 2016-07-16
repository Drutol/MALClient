using System;
using System.Threading.Tasks;
using MalClient.Shared.Comm;
using MalClient.Shared.Comm.Anime;
using MalClient.Shared.Models.Anime;

namespace MalClient.Shared.Models.AnimeScrapped
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
                    new AnimeGeneralDetailsQuery().GetAnimeDetails(false, DependentId.ToString(), DependentTitle, true,
                        ApiType.Mal);

            //Find for second
            RecommendationData =
                await
                    new AnimeGeneralDetailsQuery().GetAnimeDetails(false, RecommendationId.ToString(),
                        RecommendationTitle, true, ApiType.Mal);

            //If for some reason we fail
            if (DependentData == null || RecommendationData == null)
                throw new ArgumentNullException(); // I'm to lazy to create my own so this will suffice
            _loaded = true;
        }
    }
}