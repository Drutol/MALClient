using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeAiringDataQuery : Query
    {
        private HttpClient _httpClient = new HttpClient(ResourceLocator.MalHttpContextProvider.GetHandler());

        public override Task<string> GetRequestResponse()
        {
            return _httpClient.GetStringAsync("https://mylovelyvps.ml/malclient/airing.json");
        }
    }
}
