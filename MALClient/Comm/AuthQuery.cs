using System;
using System.Net;

namespace MALClient.Comm
{
    class AuthQuery : Query
    {
        public AuthQuery()
        {
            Request = WebRequest.Create(Uri.EscapeUriString("http://myanimelist.net/api/account/verify_credentials.xml"));
            Request.ContentType = "application/xml";
            Request.Credentials = Creditentials.GetHttpCreditentials();
            Request.Method = "GET";
        }
    }
}
