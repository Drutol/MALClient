using System;
using System.Net;

namespace MALClient.Comm
{
    internal class AnimeListQuery : Query
    {
        public AnimeListQuery(AnimeListParameters args)
        {
            Request = WebRequest.Create(Uri.EscapeUriString(UriBuilder.GetUri(UriType.AnimeListUpdate, args)));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}