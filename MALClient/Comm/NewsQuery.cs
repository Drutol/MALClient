using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm
{
    class NewsQuery : Query
    {
        public NewsQuery()
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString("https://raw.githubusercontent.com/Mordonus/MALClient/master/NEWS.json"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}
