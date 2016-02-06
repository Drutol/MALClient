using System;
using System.Net;

namespace MALClient.Comm
{
    internal class MALProfileQuery : Query
    {
        public MALProfileQuery()
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/profile/{Creditentials.UserName}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}