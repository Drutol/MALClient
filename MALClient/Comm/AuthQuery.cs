using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
