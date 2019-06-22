using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm
{
    public class AppUpdateQuery 
    {
        private HttpClient _httpClient = new HttpClient(ResourceLocator.MalHttpContextProvider.GetHandler());

        public Task<string> GetRequestResponse()
        {
            return _httpClient.GetStringAsync("https://mylovelyvps.ml/malclient/appVersion.json");
        }
    }
}
