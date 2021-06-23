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
            _client = new HttpClient(ResourceLocator.MalHttpContextProvider.GetHandler());
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
#if true
                var res = await _client.GetAsync(Request.RequestUri);
                if (res.StatusCode == HttpStatusCode.Forbidden && !Request.RequestUri.ToString()
                        .Contains("https://myanimelist.net/rss.php?type=rw&u=")) //workaround because I don't want to disturb the spaghetti gods sleeping around
                {
                    HandleMalBuggines();
                }

                await Task.Delay(150);
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
                    ResourceLocator.SnackbarProvider.ShowText(SnackbarMessageOnFail);
#endif
            }
            ResourceLocator.ConnectionInfoProvider.HasInternetConnection = true;
            return responseString;
        }

        public virtual string SnackbarMessageOnFail => "Operation failed, check your internet connection...";

        private static readonly SemaphoreSlim _buggedMalMessageSemaphore = new SemaphoreSlim(1);
        private async void HandleMalBuggines()
        {
            ResourceLocator.DispatcherAdapter.Run(async () =>
            {
                if (_buggedMalMessageSemaphore.CurrentCount == 0)
                    return;
                await _buggedMalMessageSemaphore.WaitAsync();

                await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                    "There was an error connecting to MAL Api, it tends to behave in unpredictable ways unfortunately and there's nothing I can do about it. Please try again later.", "Whoops!");
                _buggedMalMessageSemaphore.Release();
            });
            //Couldn't handle it :(
        }
    }
}