using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Interfaces;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.BL
{
    public abstract class MalHttpContextProviderBase : IMalHttpContextProvider
    {
        public const string MalBaseUrl = "https://myanimelist.net";
        protected CsrfHttpClient _httpClient;
        private bool _skippedFirstError;
        protected DateTime? _contextExpirationTime;

        protected abstract Task<CsrfHttpClient> ObtainContext();

        public void ErrorMessage(string what)
        {
            ResourceLocator.MessageDialogProvider.ShowMessageDialog($"Something went wrong... {what} implementation is pretty hacky so this stuff can happen from time to time, try again later or wait for next update. Sorry!", "Error");
        }

        public virtual async Task<CsrfHttpClient> GetHttpContextAsync()
        {
            try
            {
                if (_contextExpirationTime == null || DateTime.Now.CompareTo(_contextExpirationTime.Value) > 0)
                {
                    _httpClient?.ExpiredDispose();

                    return await ObtainContext();
                }
                return _httpClient;
            }
            catch (Exception e)
            {
                if (!_skippedFirstError)
                {
                    _skippedFirstError = true;
                    await Task.Delay(500);
                    return await GetHttpContextAsync(); //bug in android http client
                }
                if (!(e is TaskCanceledException) && !(e.InnerException is InvalidOperationException))
                    ResourceLocator.MessageDialogProvider.ShowMessageDialog(
                        "Unable to connect to MyAnimeList, they have either changed something in html or your connection is down.",
                        "Something went wrong™");

                _skippedFirstError = false;
                return new CsrfHttpClient(new HttpClientHandler()) {Disabled = true};

            }
        }

        protected FormUrlEncodedContent LoginPostBody 
        {
            get
            {
                var loginPostInfo = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_name", Credentials.UserName),
                    new KeyValuePair<string, string>("password", Credentials.Password),
                    new KeyValuePair<string, string>("sublogin", "Login"),
                    new KeyValuePair<string, string>("cookie", "1"),
                    new KeyValuePair<string, string>("submit", "1"),
                    new KeyValuePair<string, string>("csrf_token", _httpClient.Token)
                };
                return new FormUrlEncodedContent(loginPostInfo);
            }
        }
    }
}
