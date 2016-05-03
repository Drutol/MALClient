using System;
using System.Net;

namespace MALClient.Comm
{
    internal class AuthQuery : Query
    {
        public AuthQuery()
        {
            Request = WebRequest.Create(Uri.EscapeUriString("http://myanimelist.net/api/account/verify_credentials.xml"));
            Request.ContentType = "application/xml";
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.Method = "GET";
        }
    }
}