using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Runtime;
using JikanDotNet;
using JikanDotNet.Config;
using Newtonsoft.Json;
using AnimeEpisode = MALClient.Models.Models.Anime.AnimeEpisode;

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
                var result = new List<AnimeEpisode>();
                int page = 1;
                var jikan = JikanClient.Jikan;
                while (true)
                {
                    try
                    {

                        var episodes = await jikan.GetAnimeEpisodesAsync(animeId, page);
                        result.AddRange(episodes.Data.Select(episode => new AnimeEpisode
                        {
                            EpisodeId = episode.MalId,
                            Filler = episode.Filler ?? false,
                            ForumUrl = episode.ForumUrl,
                            Recap = episode.Recap ?? false,
                            Title = episode.Title,
                            TitleJapanese = episode.TitleJapanese,
                            TitleRomanji = episode.Title,
                            VideoUrl = episode.Url
                        }));

                        if (episodes.Data.Count < 100)
                            break;

                        page++;

                        if (page > 10)
                            break;
                    }
                    catch (Exception e)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
      
                _cache[animeId] = result;

                return result;
            }
            catch (Exception)
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
