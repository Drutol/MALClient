using System;
using System.Net;

namespace MALClient.Comm
{
    internal class HtmlClassDefinitionsQuery : Query
    {
        public HtmlClassDefinitionsQuery()
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString(
                        "https://raw.githubusercontent.com/Mordonus/MALClient/master/MALClient/Comm/HtmlClassesDefinitions.json"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}