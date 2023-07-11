using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Runtime;
using JikanDotNet;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
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
                var jikan = JikanClient.Jikan;
                var searchResult = await jikan.SearchMangaAsync(_query);

                foreach (var result in searchResult.Data)
                {

                    output.Add(new AnimeGeneralDetailsData
                    {
                        Id = (int)result.MalId,
                        AllVolumes = result.Volumes ?? 0,
                        Title = WebUtility.HtmlDecode(result.Title),
                        ImgUrl = result.Images.JPG.ImageUrl,
                        Type = result.Type,
                        Synopsis = WebUtility.HtmlDecode(result.Synopsis),
                        MalId = (int)result.MalId,
                        GlobalScore = (float) (result.Score ?? 0),
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
    }
}