using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm
{
    class HttpClassDefinitionsQuery : Query
    {
        public HttpClassDefinitionsQuery()
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString("https://raw.githubusercontent.com/Mordonus/MALClient/master/MALClient/Comm/HtmlClassesDefinitions.json"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}
