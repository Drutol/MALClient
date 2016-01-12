using System;
using System.Net;

namespace MALClient.Comm
{
    class AnimeSearchQuery : Query
    {
        public AnimeSearchQuery(string query)
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/anime/search.xml?q={query}"));
            Request.Credentials = Creditentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}
