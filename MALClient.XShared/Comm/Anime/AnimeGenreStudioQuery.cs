using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeGenreStudioQuery : Query
    {
        private readonly AnimeStudios _studio;
        private readonly AnimeGenres _genre;
        private readonly bool _genreMode;

        public AnimeGenreStudioQuery(AnimeGenres genre)
        {
            _genre = genre;
            _genreMode = true;
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"https://myanimelist.net/anime/genre/{(int) genre}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public AnimeGenreStudioQuery(AnimeStudios studio)
        {
            _studio = studio;
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"https://myanimelist.net/anime/producer/{(int) studio}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<SeasonalAnimeData>> GetSeasonalAnime()
        {

            var output =
                await
                    DataCache.RetrieveData<List<SeasonalAnimeData>>(
                        _genreMode ? $"genre_{_genre}" : $"studio_{_studio}",
                        _genreMode ? "AnimesByGenre" : "AnimesByStudio", 7) ??
                new List<SeasonalAnimeData>();
            if (output.Count > 0)
                return output;

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return output;

            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            int i = 0;
            try
            {
                foreach (var htmlNode in doc.WhereOfDescendantsWithClass("div", "seasonal-anime js-seasonal-anime"))
                {
                    var model = AnimeSeasonalQuery.ParseFromHtml(htmlNode,i,false);
                    if(model == null)
                        continue;
                    output.Add(model);
                    i++;
                }
            }
            catch (Exception)
            {
                //ejchtiemel
            }

            if(_genreMode)
                DataCache.SaveData(output, $"genre_{_genre}", "AnimesByGenre");
            else
                DataCache.SaveData(output, $"studio_{_studio}", "AnimesByStudio");

            return output;
        }
    }
}
