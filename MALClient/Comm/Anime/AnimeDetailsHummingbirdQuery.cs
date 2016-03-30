using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using MALClient.Models;
using Newtonsoft.Json;

namespace MALClient.Comm.Anime
{
    internal class AnimeDetailsHummingbirdQuery : Query
    {
        private static readonly string _apiKey;

        private readonly int _id;

        static AnimeDetailsHummingbirdQuery()
        {
            var resources = new ResourceLoader("MalClientResources");
            _apiKey = resources.GetString("secret");
        }

        public AnimeDetailsHummingbirdQuery(int id)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"https://hummingbird.me/api/v2/anime/myanimelist:{id}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _id = id;
        }

        public async Task<AnimeDetailsData> GetAnimeDetails(bool force = false)
        {
            var possibleData = force
                ? null
                : await DataCache.RetrieveAnimeGeneralDetailsData(_id, DataSource.Hummingbird);
            if (possibleData != null)
                return possibleData;
            Request.Headers["X-Client-Id"] = _apiKey;
            var raw = await GetRequestResponse();

            try
            {
                dynamic obj = JsonConvert.DeserializeObject(raw);

                var current = new AnimeHummingbirdDetailsData();

                foreach (var genre in obj.anime.genres)
                    current.Genres.Add(genre.Value);

                var eps = new List<Tuple<string, int>>();

                foreach (var episode in obj.linked.episodes)
                    eps.Add(new Tuple<string, int>(episode.title.Value, (int) episode.number.Value));

                eps = eps.OrderBy(tuple => tuple.Item2).ToList();
                current.Episodes.AddRange(eps.Select(tuple => tuple.Item1));
                current.SourceId = obj.anime.id;

                var output = current.ToAnimeDetailsData();

                DataCache.SaveAnimeDetails(_id, output);
                return output;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}