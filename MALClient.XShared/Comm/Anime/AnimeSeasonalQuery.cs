using HtmlAgilityPack;
using JikanDotNet;
using JikanDotNet.Config;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.JsonModels.MAL;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using VideoLibrary;

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

            var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();
            try
            {
                while (true)
                {
                    var requestedYear = (_season.Year != 0) switch
                    {
                        true => _season.Year,
                        false => DateTime.UtcNow.Year
                    };

                    var requestedSeason = (_season.Year != 0) switch
                    {
                        true => _season.Season,
                        false => DateTime.UtcNow.Month switch
                        {
                            > 0 and <= 3 => JikanDotNet.Season.Winter,
                            > 3 and <= 6 => JikanDotNet.Season.Spring,
                            > 6 and <= 9 => JikanDotNet.Season.Summer,
                            > 0 and <= 12 => JikanDotNet.Season.Fall
                        }
                    };

                    try
                    {
                        var apiUrl = $"https://api.myanimelist.net/v2/anime/season/{requestedYear}/{requestedSeason.ToString().ToLower()}?limit=500&nsfw=true&fields=id,title,main_picture,num_episodes,mean,genres,num_list_users,start_season";
                        var season = JsonSerializer.Deserialize<PaginatedMALResponse<ICollection<AnimeNode<SeasonEntry>>>>(
                            await client.GetStringAsync(apiUrl));
                        var orderedData = season.Data.OrderBy(seasonEntry => 
                            ((seasonEntry.Node.StartSeason.Name == requestedSeason.ToString().ToLower() 
                            & seasonEntry.Node.StartSeason.Year == requestedYear) ? 100000000 : 0) 
                            + seasonEntry.Node.MembersCount)
                            .Reverse();

                        foreach (var seasonSeasonEntry in orderedData)
                        {
                            output.Add(new SeasonalAnimeData
                            {
                                Title = seasonSeasonEntry.Node.Title,
                                Id = (int)(seasonSeasonEntry.Node.MalId ?? -1),
                                ImgUrl = seasonSeasonEntry.Node.Picture.Medium,
                                Episodes = (seasonSeasonEntry.Node.Episodes ?? 0).ToString(),
                                Score = (float)(seasonSeasonEntry.Node.Score ?? 0),
                                Genres = (seasonSeasonEntry.Node.Genres ?? new List<JsonModels.MAL.Genre>()).Select(item => item.Name).ToList(),
                                Index = orderedData.FindIndex(seasonSeasonEntry)
                            });
                        }

                        break;
                    }
                    catch (HttpRequestException e)
                    {
                        if (e.Message.Contains("429"))
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        else
                            throw;
                    }
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