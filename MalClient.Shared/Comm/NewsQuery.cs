using System;
using System.Net;

namespace MALClient.Comm
{
    public class NewsQuery : Query
    {
        public NewsQuery()
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString("https://raw.githubusercontent.com/Mordonus/MALClient/master/NEWS.json"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}