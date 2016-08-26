using System;
using System.Net;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Manga
{
    public class MangaSearchQuery : Query
    {
        public MangaSearchQuery(string query)
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/manga/search.xml?q={query}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}