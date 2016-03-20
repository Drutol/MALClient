using System;
using System.Net;

namespace MALClient.Comm
{
    public class MalListQuery : Query
    {
        public MalListQuery(MalListParameters args)
        {
            Request = WebRequest.Create(Uri.EscapeUriString(UriBuilder.GetUri(UriType.MalListQuery, args)));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}