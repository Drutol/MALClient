using System;
using System.Net;

namespace MALClient.Comm
{
    class AnimeRemoveQuery : Query
    {
        public AnimeRemoveQuery(string id)
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/animelist/delete/{id}.xml"));
            Request.Credentials = Creditentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}
