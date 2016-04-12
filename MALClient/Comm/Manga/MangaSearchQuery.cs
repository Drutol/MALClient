using System;
using System.Net;

namespace MALClient.Comm
{
    internal class MangaSearchQuery : Query
    {
        public MangaSearchQuery(string query)
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/manga/search.xml?q={query}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}