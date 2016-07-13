using System;
using System.Net;
using MALClient.Utils;

namespace MALClient.Comm
{
    internal class MangaRemoveQuery : Query
    {
        public MangaRemoveQuery(string id)
        {
            MangaUpdateQuery.UpdatedSomething = true;
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/mangalist/delete/{id}.xml"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}