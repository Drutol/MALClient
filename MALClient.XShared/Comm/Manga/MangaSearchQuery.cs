using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;
using ModernHttpClient;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.Manga
{
    public class MangaSearchQuery : Query
    {
        private readonly string _query;

        public MangaSearchQuery(string query)
        {
            _query = query;
            Request = WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/manga/search.xml?q={query}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<AnimeGeneralDetailsData>> GetSearchResults()
        {
            var output = new List<AnimeGeneralDetailsData>();

            try
            {
                var client = new HttpClient(new NativeMessageHandler());
                var response =
                    await client.GetAsync(
                        $"https://api.jikan.moe/search/anime/{_query}/1");

                if (!response.IsSuccessStatusCode)
                    return output;

                var results =
                    JsonConvert.DeserializeObject<RootObject>(
                        await response.Content.ReadAsStringAsync());

                foreach (var result in results.result)
                {
                    result.image_url =
                        Regex.Replace(result.image_url, @"\/r\/\d+x\d+", "");
                    result.image_url =
                        result.image_url.Substring(0, result.image_url.IndexOf('?'));

                    output.Add(new AnimeGeneralDetailsData
                    {
                        Id = result.mal_id,
                        AllEpisodes = result.volumes,
                        Title = result.title,
                        ImgUrl = result.image_url,
                        Type = result.type,
                        Synopsis = result.description,
                        MalId = result.mal_id,
                        GlobalScore = (float) result.score,
                        Status = "Unknown"
                    });
                }

            }
            catch (Exception e)
            {
                return output;
            }

            return output;
        }

        class Result
        {
            public int mal_id { get; set; }
            public string url { get; set; }
            public string image_url { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string type { get; set; }
            public double score { get; set; }
            public int volumes { get; set; }
            public int members { get; set; }
        }

        class RootObject
        {
            public string request_hash { get; set; }
            public bool request_cached { get; set; }
            public List<Result> result { get; set; }
            public int result_last_page { get; set; }
        }
    }
}