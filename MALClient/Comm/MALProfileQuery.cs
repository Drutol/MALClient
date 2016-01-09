using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm
{
    class MALProfileQuery : Query
    {
        public MALProfileQuery()
        {
            Request = WebRequest.Create(Uri.EscapeUriString("http://myanimelist.net/profile/Drutol"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}
