using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using ModernHttpClient;

namespace MALClient.XShared.Comm
{
    public abstract class Query
    {

        protected WebRequest Request;
        private bool _retry = true;
        public static ApiType CurrentApiType { get; set; } = Settings.SelectedApiType;

#if ANDROID
        protected static HttpClient _client;

        static Query()
        {
            _client = new HttpClient(new NativeMessageHandler());
            RefreshClientAuthHeader();
        }

        public static void RefreshClientAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Credentials.UserName}:{Credentials.Password}")));
        }

#endif


        public virtual async Task<string> GetRequestResponse(bool wantMsg = true, string statusBarMsg = null)
        {
            var responseString = "";
            try
            {
#if ANDROID
                var res = await _client.GetAsync(Request.RequestUri);
                if (res.StatusCode == HttpStatusCode.Forbidden)
                {
                    HandleMalBuggines();
                }
                var content = await res.Content.ReadAsStringAsync();
                return content;

#else
                var response = await Request.GetResponseAsync();

                using (var stream = response.GetResponseStream())
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                    reader.Dispose();
                }
                return responseString;
#endif
            }
            catch (Exception e)
            {
                ResourceLocator.ConnectionInfoProvider.HasInternetConnection = false;

#if !ANDROID
                if (e is WebException exc)
                {
                    if (((HttpWebResponse)exc.Response).StatusCode == HttpStatusCode.Forbidden)
                    {
                        HandleMalBuggines();   
                    }
                }
#endif

#if ANDROID
                if(Credentials.Authenticated)
                    ResourceLocator.SnackbarProvider.ShowText("Operation failed, check your internet connection...");
#endif
            }
            ResourceLocator.ConnectionInfoProvider.HasInternetConnection = true;
            return responseString;
        }

        private static readonly SemaphoreSlim _buggedMalMessageSemaphore = new SemaphoreSlim(1);
        private async void HandleMalBuggines()
        {
            ResourceLocator.DispatcherAdapter.Run(async () =>
            {
                if (_buggedMalMessageSemaphore.CurrentCount == 0)
                    return;
                await _buggedMalMessageSemaphore.WaitAsync();

                await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                    "Looks like MAL has banned your IP supposedly for 10 failed log-in attempts... Truth be told they have this system bugged as reported on forums and it triggers on false-positives from time to time.\n\nYou will now have to either obtain new IP or wait for 2 hours without making any requests to MAL via apps. Sorry for inconvenince but I cannot do much about it :(", "Whoops!");
                _buggedMalMessageSemaphore.Release();
            });
            //Couldn't handle it :(
        }
    }
}