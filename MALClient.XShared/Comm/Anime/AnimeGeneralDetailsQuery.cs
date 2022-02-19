using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Android.Runtime;
using JikanDotNet;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Comm.Manga;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeGeneralDetailsQuery : Query
    {
        public async Task<AnimeGeneralDetailsData> GetAnimeDetails(bool force, string id, string title, bool animeMode,
            ApiType? apiOverride = null)
        {
            var output = force ? null : await DataCache.RetrieveAnimeSearchResultsData(id, animeMode);
            if (output != null)
                return output;

            var requestedApiType = apiOverride ?? CurrentApiType;
            var response = string.Empty;
            var jikan = new Jikan();
            try
            {
                switch (requestedApiType)
                {
                    case ApiType.Mal:

                            if (animeMode)
                            {
                                var resultRequest = await jikan.GetAnimeAsync(long.Parse(id));
                                var result = resultRequest.Data;
                                output = new AnimeGeneralDetailsData
                                {
                                    AllEpisodes = result.Episodes ?? 0,
                                    Status = result.Status,
                                    Type = result.Type,
                                    AlternateTitle = result.TitleJapanese,
                                    StartDate = result.Aired.From?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "N/A",
                                    EndDate = result.Aired.To?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "N/A",
                                    ImgUrl = result.Images.JPG.ImageUrl,
                                    GlobalScore = (float) (result.Score ?? 0),
                                    Id = (int)result.MalId,
                                    MalId = (int)result.MalId,
                                    Synopsis = WebUtility.HtmlDecode(result.Synopsis),
                                    Title = WebUtility.HtmlDecode(result.Title),
                                    Synonyms = result.TitleSynonyms?.ToList() ?? new List<string>(),
                                };

                                if ((output.Type == "Movie" || output.AllEpisodes == 1) && output.EndDate == "N/A" &&
                                    output.Status == "Finished Airing")
                                {
                                    output.EndDate = output.StartDate;
                                }

                                ResourceLocator.EnglishTitlesProvider.AddOrUpdate(int.Parse(id), true,
                                    result.TitleEnglish);
                            }
                            else
                            {
                                var resultRequest = await jikan.GetMangaAsync(long.Parse(id));
                                var result = resultRequest.Data;

                                var vols = result.Volumes ?? 0;
                                var chap = result.Chapters ?? 0;

                                output = new AnimeGeneralDetailsData
                                {
                                    AllEpisodes = chap,
                                    AllVolumes = vols,
                                    Status = result.Status,
                                    Type = result.Type,
                                    AlternateTitle = result.TitleJapanese,
                                    StartDate = result.Published.From?.ToString("yyyy-MM-dd",
                                        CultureInfo.InvariantCulture) ?? "N/A",
                                    EndDate =
                                        result.Published.To?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ??
                                        "N/A",
                                    ImgUrl = result.Images.JPG.ImageUrl,
                                    GlobalScore = (float)(result.Score ?? 0),
                                    Id = (int)result.MalId,
                                    MalId = (int)result.MalId,
                                    Synopsis = WebUtility.HtmlDecode(result.Synopsis),
                                    Title = WebUtility.HtmlDecode(result.Title),
                                    Synonyms = result.TitleSynonyms?.ToList() ?? new List<string>(),
                                };

                                ResourceLocator.EnglishTitlesProvider.AddOrUpdate(int.Parse(id), false,
                                    result.TitleEnglish);
                            }



                            DataCache.SaveAnimeSearchResultsData(id, output, animeMode);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                //ResourceLocator.ClipboardProvider.SetText($"{e}\n{response}");
                //ResourceLocator.SnackbarProvider.ShowText("Error copied to clipboard.");
                // todo android notification nav bug
                // probably MAl garbled response
            }

            return output;
        }
    }
}