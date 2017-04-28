using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.XShared.BL;
using MALClient.XShared.Comm.MagicalRawQueries;
using ModernHttpClient;
using Xamarin.Android.Net;

namespace MALClient.Android.Adapters
{
    public class MalHttpContextProvider : MalHttpContextProviderBase
    {
        protected override async Task<CsrfHttpClient> ObtainContext()
        {
            var httpHandler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                AllowAutoRedirect = false,
            };
            _httpClient = new CsrfHttpClient(httpHandler) { BaseAddress = new Uri(MalBaseUrl) };
            await _httpClient.GetToken();

            var response = await _httpClient.PostAsync("/login.php", LoginPostBody);
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.RedirectMethod)
            {
                _contextExpirationTime = DateTime.Now.Add(TimeSpan.FromHours(.5));
                return _httpClient; //else we are returning client that can be used for next queries
            }

            throw new WebException("Unable to authorize");
        }
    }
}