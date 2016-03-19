using System;
using System.Net;

namespace MALClient.Comm
{
    internal class MangaRemoveQuery : Query
    {
        public MangaRemoveQuery(string id)
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/mangalist/delete/{id}.xml"));
            Request.Credentials = Creditentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}