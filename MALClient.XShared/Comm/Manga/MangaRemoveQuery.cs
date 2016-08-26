using System;
using System.Net;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Manga
{
    public class MangaRemoveQuery : Query
    {
        public MangaRemoveQuery(string id)
        {
            MangaUpdateQuery.UpdatedSomething = true;
            Request = WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/mangalist/delete/{id}.xml"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}