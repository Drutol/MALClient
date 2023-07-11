using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Android.Runtime;
using JikanDotNet;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeSearchQuery : Query
    {
        private readonly string _query;

        public AnimeSearchQuery(string query, ApiType? apiOverride = null)
        {
            _query = query;
            var targettedApi = apiOverride ?? CurrentApiType;
            switch (targettedApi)
            {
                case ApiType.Mal:
                    Request =
                        WebRequest.Create(new Uri($"https://myanimelist.net/api/anime/search.xml?q={query}"));
                    Request.Credentials = Credentials.GetHttpCreditentials();
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:
                    Request =
                        WebRequest.Create(Uri.EscapeUriString($"http://hummingbird.me/api/v1/search/anime?query={query}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<List<AnimeGeneralDetailsData>> GetSearchResults()
        {
            var output = new List<AnimeGeneralDetailsData>();

            try
            {
                var jikan = JikanClient.Jikan;
                var searchResult = await jikan.SearchAnimeAsync(_query);

                foreach (var result in searchResult.Data)
                {
                    output.Add(new AnimeGeneralDetailsData
                    {
                        Id = (int)result.MalId,
                        AllEpisodes = (result.Episodes ?? 0),
                        Title = WebUtility.HtmlDecode(result.Title),
                        ImgUrl = result.Images.JPG.ImageUrl,
                        Type = result.Type,
                        Synopsis = WebUtility.HtmlDecode(result.Synopsis),
                        MalId = (int)result.MalId,
                        GlobalScore = (float)(result.Score ?? 0f),      
                        Status =  result.Airing ? "Currently Airing" : "Unknown"                       
                    });
                }

            }
            catch (Exception e)
            {
                return output;
            }

            return output;
            //switch (CurrentApiType)
            //{
            //    case ApiType.Mal:
            //        try
            //        {
            //            var parsed = XElement.Parse(raw.Replace("&", "")); //due to unparasable stuff returned by mal
            //            foreach (var element in parsed.Elements("entry"))
            //            {
            //                var item = new AnimeGeneralDetailsData();
            //                item.ParseXElement(element, true, Settings.PreferEnglishTitles);
            //                output.Add(item);
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            //mal can throw html in synopisis and xml cannot do much
            //        }

            //        break;
            //    case ApiType.Hummingbird:
            //        dynamic jsonObj = JsonConvert.DeserializeObject(raw);
            //        foreach (var entry in jsonObj)
            //        {
            //            try
            //            {
            //                var allEps = 0;
            //                if (entry.episode_count != null)
            //                    allEps = Convert.ToInt32(entry.episode_count.ToString());
            //                output.Add(new AnimeGeneralDetailsData
            //                {
            //                    Title = entry.title.ToString(),
            //                    ImgUrl = entry.cover_image.ToString(),
            //                    Type = entry.show_type.ToString(),
            //                    Id = Convert.ToInt32(entry.id.ToString()),
            //                    MalId = Convert.ToInt32(entry.mal_id.ToString()),
            //                    AllEpisodes = allEps,
            //                    StartDate = "0000-00-00", //TODO : Do sth
            //                    EndDate = "0000-00-00",
            //                    Status = entry.status.ToString(),
            //                    Synopsis = entry.synopsis.ToString(),
            //                    GlobalScore = float.Parse(entry.community_rating.ToString()),
            //                    Synonyms = new List<string>()
            //                });
            //            }
            //            catch (Exception e)
            //            {
            //            }
            //        }

            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}

            //return output;
        }
    }
}