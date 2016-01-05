using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm
{
    class AnimeSearchQuery : Query
    {
        public AnimeSearchQuery(string query)
        {
            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/anime/search.xml?q={query}"));
            Request.Credentials = Creditentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}
