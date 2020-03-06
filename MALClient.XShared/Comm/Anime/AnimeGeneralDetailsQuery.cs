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
            try
            {
                switch (requestedApiType)
                {
                    case ApiType.Mal:

                        using (var client = new HttpClient(ResourceLocator.MalHttpContextProvider.GetHandler()))
                        {
                            response = await client.GetStringAsync(
                                    $"https://api.jikan.moe/v3/{(animeMode ? "anime" : "manga")}/{id}");

                            if (animeMode)
                            {
                                var parsed = JsonConvert.DeserializeObject<RootObject>(response);

                                output = new AnimeGeneralDetailsData
                                {
                                    AllEpisodes = parsed.episodes ?? 0,
                                    Status = parsed.status,
                                    Type = parsed.type,
                                    AlternateTitle = parsed.title_japanese,
                                    StartDate = parsed.aired.from?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "N/A",
                                    EndDate = parsed.aired.to?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "N/A",
                                    ImgUrl = parsed.image_url,
                                    GlobalScore = (float) (parsed.score ?? 0),
                                    Id = parsed.mal_id,
                                    MalId = parsed.mal_id,
                                    Synopsis = WebUtility.HtmlDecode(parsed.synopsis),
                                    Title = WebUtility.HtmlDecode(parsed.title),
                                    Synonyms = parsed.title_synonyms ?? new List<string>(),
                                };

                                if ((output.Type == "Movie" || output.AllEpisodes == 1) && output.EndDate == "N/A" &&
                                    output.Status == "Finished Airing")
                                {
                                    output.EndDate = output.StartDate;
                                }

                                ResourceLocator.EnglishTitlesProvider.AddOrUpdate(int.Parse(id), true,
                                    parsed.title_english);
                            }
                            else
                            {
                                var parsed = JsonConvert.DeserializeObject<MangaRootObject>(response);

                                var vols = parsed.volumes ?? 0;
                                var chap = parsed.chapters ?? 0;

                                output = new AnimeGeneralDetailsData
                                {
                                    AllEpisodes = chap,
                                    AllVolumes = vols,
                                    Status = parsed.status,
                                    Type = parsed.type,
                                    AlternateTitle = parsed.title_japanese,
                                    StartDate = parsed.published.from?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "N/A",
                                    EndDate = parsed.published.to?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "N/A",
                                    ImgUrl = parsed.image_url,
                                    GlobalScore = (float) (parsed.score ?? 0),
                                    Id = parsed.mal_id,
                                    MalId = parsed.mal_id,
                                    Synopsis = WebUtility.HtmlDecode(parsed.synopsis),
                                    Title = WebUtility.HtmlDecode(parsed.title),
                                    Synonyms = parsed.title_synonyms ?? new List<string>(),
                                };

                                ResourceLocator.EnglishTitlesProvider.AddOrUpdate(int.Parse(id), false,
                                    parsed.title_english);
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
                //ResourceLocator.ClipboardProvider.SetText($"{e}\n{response}");
                //ResourceLocator.SnackbarProvider.ShowText("Error copied to clipboard.");
                // todo android notification nav bug
                // probably MAl garbled response
            }

            return output;
        }


        public class From
        {
            public int day { get; set; }
            public int month { get; set; }
            public int year { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class To
        {
            public int day { get; set; }
            public int month { get; set; }
            public int year { get; set; }
        }

        [Preserve(AllMembers = true)]
        public class Aired
        {
            public DateTime? from { get; set; }
            public DateTime? to { get; set; }
            public string @string { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Adaptation
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class SideStory
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Related
        {
            public List<Adaptation> Adaptation { get; set; }
            [JsonProperty("Side story")] public List<SideStory> SideStories { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Producer
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Licensor
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Studio
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Genre
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Published
        {
            public DateTime? from { get; set; }
            public DateTime? to { get; set; }
            public string @string { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Author
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
        [Preserve(AllMembers = true)]
        public class Serialization
        {
            public int mal_id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }

        [Preserve(AllMembers = true)]
        public class RootObject
        {
            public int mal_id { get; set; }
            public string image_url { get; set; }
            public string title { get; set; }
            public string title_english { get; set; }
            public string title_japanese { get; set; }
            public List<string> title_synonyms { get; set; }
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
            public string title_english { get; set; }
            public List<string> title_synonyms { get; set; }
            public string title_japanese { get; set; }
            public string status { get; set; }
            public string image_url { get; set; }
            public string type { get; set; }
            public int? volumes { get; set; }
            public int? chapters { get; set; }
            public Published published { get; set; }
            public double? score { get; set; }

            public string synopsis { get; set; }
        }
    }
}