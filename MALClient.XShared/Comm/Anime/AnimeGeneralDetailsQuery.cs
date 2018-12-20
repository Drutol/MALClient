using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Android.Runtime;
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
            try
            {
                switch (requestedApiType)
                {
                    case ApiType.Mal:

                        using (var client = new HttpClient())
                        {
                            var response = await client.GetStringAsync($"https://api.jikan.moe/{(animeMode ? "anime" : "manga")}/{id}");

                            if (animeMode)
                            {
                                var parsed = JsonConvert.DeserializeObject<RootObject>(response);

                                output = new AnimeGeneralDetailsData
                                {
                                    AllEpisodes = parsed.episodes ?? 0,
                                    Status = parsed.status,
                                    Type = parsed.type,
                                    AlternateTitle = parsed.title_japanese,
                                    StartDate = parsed.aired.from?.ToString("yyyy-MM-dd") ?? "N/A",
                                    EndDate = parsed.aired.to?.ToString("yyyy-MM-dd") ?? "N/A",
                                    ImgUrl = parsed.image_url,
                                    GlobalScore = (float)(parsed.score ?? 0),
                                    Id = parsed.mal_id,
                                    MalId = parsed.mal_id,
                                    Synopsis = WebUtility.HtmlDecode(parsed.synopsis),
                                    Title = WebUtility.HtmlDecode(parsed.title),
                                    Synonyms = parsed.title_synonyms?.Split(',').ToList() ?? new List<string>(),
                                };

                                if ((output.Type == "Movie" || output.AllEpisodes == 1) && output.EndDate == "N/A" &&
                                    output.Status == "Finished Airing")
                                {
                                    output.EndDate = output.StartDate;
                                }
                            }
                            else
                            {
                                var parsed = JsonConvert.DeserializeObject<MangaRootObject>(response);

                                int.TryParse(parsed.volumes, out var vols);
                                int.TryParse(parsed.chapters, out var chap);

                                output = new AnimeGeneralDetailsData
                                {
                                    AllEpisodes = chap,
                                    AllVolumes = vols,
                                    Status = parsed.status,
                                    Type = parsed.type,
                                    AlternateTitle = parsed.title_japanese,
                                    StartDate = parsed.published.from?.ToString("yyyy-MM-dd") ?? "N/A",
                                    EndDate = parsed.published.to?.ToString("yyyy-MM-dd") ?? "N/A",
                                    ImgUrl = parsed.image_url,
                                    GlobalScore = (float)(parsed.score ?? 0),
                                    Id = parsed.mal_id,
                                    MalId = parsed.mal_id,
                                    Synopsis = WebUtility.HtmlDecode(parsed.synopsis),
                                    Title = WebUtility.HtmlDecode(parsed.title),
                                    Synonyms = parsed.title_synonyms?.Split(',').ToList() ?? new List<string>(),
                                };
                            }
                        }


                        DataCache.SaveAnimeSearchResultsData(id, output, animeMode);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                // todo android notification nav bug
                // probably MAl garbled response
            }

            return output;
        }
        

        [Preserve(AllMembers = true)]
        class Aired
        {
            public DateTime? from { get; set; }
            public DateTime? to { get; set; }
        }
        [Preserve(AllMembers = true)]
        class Adaptation
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string url { get; set; }
            public string title { get; set; }
        }
        [Preserve(AllMembers = true)]
        class SideStory
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string url { get; set; }
            public string title { get; set; }
        }
        [Preserve(AllMembers = true)]
        class Summary
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string url { get; set; }
            public string title { get; set; }
        }
        [Preserve(AllMembers = true)]
        class Related
        {
            public List<Adaptation> Adaptation { get; set; }
            [JsonProperty("Side story")] public List<SideStory> SideStories { get; set; }
            public List<Summary> Summary { get; set; }
        }
        [Preserve(AllMembers = true)]
        class Producer
        {
            public string url { get; set; }
            public string name { get; set; }
        }
        [Preserve(AllMembers = true)]
        class Licensor
        {
            public string url { get; set; }
            public string name { get; set; }
        }
        [Preserve(AllMembers = true)]
        class Studio
        {
            public string url { get; set; }
            public string name { get; set; }
        }
        [Preserve(AllMembers = true)]
        class Genre
        {
            public string url { get; set; }
            public string name { get; set; }
        }
        [Preserve(AllMembers = true)]
        class RootObject
        {
            public int mal_id { get; set; }
            public string title { get; set; }
            public string title_japanese { get; set; }
            public string title_synonyms { get; set; }
            public string image_url { get; set; }
            public string type { get; set; }
            public int? episodes { get; set; }
            public string status { get; set; }
            public Aired aired { get; set; }
            public double? score { get; set; }
            public string synopsis { get; set; }
        }
        [Preserve(AllMembers = true)]
        class MangaRootObject
        {
            public int mal_id { get; set; }
            public string title { get; set; }
            public string title_synonyms { get; set; }
            public string title_japanese { get; set; }
            public string status { get; set; }
            public string image_url { get; set; }
            public string type { get; set; }
            public string volumes { get; set; }
            public string chapters { get; set; }
            public Aired published { get; set; }
            public double? score { get; set; }
            public string synopsis { get; set; }
        }
    }
}