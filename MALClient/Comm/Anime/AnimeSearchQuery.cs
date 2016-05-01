using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MALClient.Models;

namespace MALClient.Comm
{
    internal class AnimeSearchQuery : Query
    {
        public AnimeSearchQuery(string query)
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/anime/search.xml?q={query}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        //public async Task<List<AnimeGeneralDetailsData>> GetSearchResults()
        //{
        //    var output = new List<AnimeGeneralDetailsData>();

        //    var raw = await GetRequestResponse();
        //    if (string.IsNullOrEmpty(raw))
        //        return output;




        //}
    }
}