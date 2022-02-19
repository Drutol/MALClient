using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JikanDotNet;
using JikanDotNet.Config;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeSeasonalQuery : Query
    {
        private readonly AnimeSeason _season;

        public AnimeSeasonalQuery(AnimeSeason season)
        {
            _season = season;
        }

        public async Task<List<SeasonalAnimeData>> GetSeasonalAnime(bool force = false)
        {
            var output = force /*|| DataCache.SeasonalUrls?.Count == 0*/ //either force or urls are empty after update
                ? new List<SeasonalAnimeData>()
                : await DataCache.RetrieveSeasonalData(_season.Name) ?? new List<SeasonalAnimeData>();
            //current season without suffix
            if (output.Count != 0) return output;

            var jikan = new Jikan(new JikanClientConfiguration { SuppressException = true });
            PaginatedJikanResponse<ICollection<JikanDotNet.Anime>> season;
            try
            {
                if (_season.Year != 0)
                    season = await jikan.GetSeasonAsync(_season.Year, _season.Season);
                else
                    season = await jikan.GetSeasonAsync(DateTime.UtcNow.Year, DateTime.UtcNow.Month switch
                    {
                        > 0 and <= 3 => Season.Winter,
                        > 3 and <= 6 => Season.Spring,
                        > 6 and <= 9 => Season.Summer,
                        > 0 and <= 12 => Season.Fall
                    });

                foreach (var seasonSeasonEntry in season.Data)
                {
                    output.Add(new SeasonalAnimeData
                    {
                        Title = seasonSeasonEntry.Title,
                        Id = (int)(seasonSeasonEntry.MalId ?? -1),
                        ImgUrl = seasonSeasonEntry.Images.JPG.ImageUrl,
                        Episodes = (seasonSeasonEntry.Episodes ?? 0).ToString(),
                        Score = (float)(seasonSeasonEntry.Score ?? 0),
                        Genres = seasonSeasonEntry.Genres.Select(item => item.Name).ToList(),
                        Index = season.Data.FindIndex(seasonSeasonEntry) + 1
                    });
                }

                DataCache.SaveSeasonalData(output, _season.Name);

                //We are done.
                return output;
            }
            catch (Exception e)
            {
                return output;
            }
        }
    }
}