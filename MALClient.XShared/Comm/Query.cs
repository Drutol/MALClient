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
using MALClient.XShared.Comm.MalSpecific;
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

#if true
        protected static HttpClient _client;

        static Query()
        {
            _client = new HttpClient(new NativeMessageHandler());
            _client.ConfigureToAcceptCompressedContent();

            RefreshClientAuthHeader();
        }

        public static void RefreshClientAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Credentials.UserName}:{Credentials.Password}")));
        }

#endif


        public virtual async Task<string> GetRequestResponse()
        {
            var responseString = "";
            try
            {
                using (var res = await _client.GetAsync(Request.RequestUri)
                                              .ConfigureAwait(false))
                {
                    if (res.StatusCode == HttpStatusCode.Forbidden && !Request.RequestUri.ToString()
                            .Contains("https://myanimelist.net/rss.php?type=rw&u=")) //workaround because I don't want to disturb the spaghetti gods sleeping around
                    {
                        HandleMalBuggines();
                    }

                    using (var responseStream = await res.GetDecompressionStreamAsync()
                                                         .ConfigureAwait(false))
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            return await reader.ReadToEndAsync()
                                               .ConfigureAwait(false);
                        }
                    
                    }
                }
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
                    ResourceLocator.SnackbarProvider.ShowText(SnackbarMessageOnFail);
#endif
            }
            ResourceLocator.ConnectionInfoProvider.HasInternetConnection = true;
            return responseString;
        }

        public virtual string SnackbarMessageOnFail => "Operation failed, check your internet connection...";

        private static readonly SemaphoreSlim _buggedMalMessageSemaphore = new SemaphoreSlim(1);

        private void HandleMalBuggines()
        {
            ResourceLocator.DispatcherAdapter.Run(async () =>
            {
                if (_buggedMalMessageSemaphore.CurrentCount == 0)
                {
                    return;
                }

                await _buggedMalMessageSemaphore.WaitAsync()
                                                .ConfigureAwait(false);

                await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                    "Looks like MAL has banned your IP supposedly for 10 failed log-in attempts... Truth be told they have this system bugged as reported on forums and it triggers on false-positives from time to time.\n\nYou will now have to either obtain new IP or wait for 2 hours without making any requests to MAL via apps. Sorry for inconvenince but I cannot do much about it :(", "Whoops!");
                _buggedMalMessageSemaphore.Release();
            });
            //Couldn't handle it :(
        }
    }
}