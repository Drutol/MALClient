using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeAiringDataQuery : Query
    {
        private HttpClient _httpClient = new HttpClient();

        public override Task<string> GetRequestResponse()
        {
            return _httpClient.GetStringAsync("https://mylovelyvps.ml/malclient/airing.json");
        }
    }
}
