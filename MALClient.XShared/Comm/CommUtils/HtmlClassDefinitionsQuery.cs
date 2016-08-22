using System;
using System.Net;

namespace MalClient.Shared.Comm.CommUtils
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