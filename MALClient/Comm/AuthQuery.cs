using System;
using System.Net;

namespace MALClient.Comm
{
    internal class AuthQuery : Query
    {
        public AuthQuery()
        {
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request = WebRequest.Create(Uri.EscapeUriString("http://myanimelist.net/api/account/verify_credentials.xml"));
                    Request.ContentType = "application/xml";
                    Request.Credentials = Credentials.GetHttpCreditentials();
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}