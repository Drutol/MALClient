using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeAiringDataQuery : Query
    {
        public AnimeAiringDataQuery()
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString("http://iatgof.com/imal/airing.json"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}
