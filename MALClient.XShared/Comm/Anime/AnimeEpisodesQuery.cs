using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.Runtime;
using MALClient.Models.Models.Anime;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeEpisodesQuery : Query
    {
        private readonly Dictionary<int, List<AnimeEpisode>> _cache = new Dictionary<int, List<AnimeEpisode>>();

        public async Task<List<AnimeEpisode>> GetEpisodes(int animeId, bool force = false)
        {
            if (_cache.ContainsKey(animeId) && !force)
                return _cache[animeId];

            try
            {
                var json = await _client.GetStringAsync($"https://api.jikan.moe/v3/anime/{animeId}/episodes/1");

                var eps = JsonConvert.DeserializeObject<RootObject>(json);

                _cache[animeId] = eps.Episodes;

                return eps.Episodes;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [Preserve(AllMembers = true)]
        class RootObject
        {
            [JsonProperty("episodes")]
            public List<AnimeEpisode> Episodes { get; set; }
        }
    }
}
