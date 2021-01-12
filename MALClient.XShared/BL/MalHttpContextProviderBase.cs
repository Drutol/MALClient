using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Android.Runtime;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Interfaces;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.XShared.BL
{
    public abstract class MalHttpContextProviderBase : IMalHttpContextProvider
    {
        public const string MalBaseUrl = "https://myanimelist.net";
        protected CsrfHttpClient _httpClient;
        private HttpClient _apiHttpClient;
        private bool _skippedFirstError;
        protected DateTime? _contextExpirationTime;
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private DateTime _lastRefresh = DateTime.MinValue;
        private Exception _exc;

        public void SetCookies(string cookies)
        {
            if (_httpClient != null)
            {
                foreach (var cookie in cookies.Split(';'))
                {
                    var tokens = cookie.Split('=');
                    try
                    {
                        _httpClient.Handler.CookieContainer.Add(new Cookie(HttpUtility.UrlEncode(tokens[0].Trim()), HttpUtility.UrlEncode(tokens[1].Trim()), "/", "myanimelist.net"));
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }

        public async Task<HttpClient> GetApiHttpContextAsync()
        {
            if (_apiHttpClient != null && DateTime.UtcNow - _lastRefresh < TimeSpan.FromMinutes(50))
                return _apiHttpClient;
            
            if (_apiHttpClient != null)
            {
                _apiHttpClient.Dispose();
                _apiHttpClient = null;
            }

            using var tokenClient = new HttpClient();
            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new("client_id", "183063f74126e7551b00c3b4de66986c"),
                new("grant_type", "refresh_token"),
                new("refresh_token", Settings.RefreshToken)
            });

            try
            {
                var response = await tokenClient.PostAsync("https://myanimelist.net/v1/oauth2/token", content);
                var json = await response.Content.ReadAsStringAsync();
                var tokens = JsonConvert.DeserializeObject<TokenResponse>(json);

                Settings.ApiToken = tokens.access_token;
                Settings.RefreshToken = tokens.refresh_token;

                _apiHttpClient = new HttpClient();
                _apiHttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Settings.ApiToken);
            }
            catch (Exception e)
            {
                ResourceLocator.DispatcherAdapter.Run(() =>
                    ResourceLocator.MessageDialogProvider.ShowMessageDialog(
                        "Failed to authorize with MyAnimeList, please try signing in again.", "Error"));
                throw;
            }


            return _apiHttpClient;
        }

        protected abstract Task<CsrfHttpClient> ObtainContext();

        public void ErrorMessage(string what)
        {
            ResourceLocator.MessageDialogProvider.ShowMessageDialog($"Something went wrong... {what} implementation is pretty hacky so this stuff can happen from time to time, try again later or wait for next update. Sorry!", "Error");
        }

        public virtual async Task<CsrfHttpClient> GetHttpContextAsync(bool skipAtuhCheck = false)
        {
            if(!Credentials.Authenticated && !skipAtuhCheck)
                return new CsrfHttpClient(ResourceLocator.MalHttpContextProvider.GetHandler()) { Disabled = true };

            if (_httpClient != null)
                return _httpClient;
            
            await Task.Delay(100);
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (_httpClient == null)
                    _httpClient = await ObtainContext();
                
                return _httpClient;
            }
            catch (Exception e)
            {

                _exc = e;

                if (e.Message.Contains("Unable to authorize"))
                {
                    ResourceLocator.DispatcherAdapter.Run(async () =>
                    {
                        Credentials.Reset();
                        ResourceLocator.AnimeLibraryDataStorage.Reset();
                        ResourceLocator.MalHttpContextProvider.Invalidate();
                        await ResourceLocator.DataCacheService.ClearAnimeListData();
                    });
                }

                _skippedFirstError = false;
                return new CsrfHttpClient(ResourceLocator.MalHttpContextProvider.GetHandler()) {Disabled = true};               
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public void Invalidate()
        {
            _httpClient?.ExpiredDispose();
            _httpClient = null;
            _contextExpirationTime = null;
        }

        public abstract HttpClientHandler GetHandler();

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
                    new KeyValuePair<string, string>("csrf_token", _httpClient.Token),
                    new KeyValuePair<string, string>("g-recaptcha-response", "03AGdBq26L7XLbz4ZBud-qOsVbW7gMKggzg81EKyaxF3uaXuwNspa2kfr570Q2qpV9UQ3i6n-S6Mf738-5KROfCGohykKaOLIPZRLR6wBH15-8tk-1IzqW-TYArPe-03S7h2wptvKIlG9xZRonmFndkZrJ9O8gcWlVMDGIdOE8OJ6LfTRkc4H6nh5UWwQsK-iqObn3FGsvNxy2EFilVyZ1-susUAGqFzX5KkfuxXW3RlOdikP_3mFXxniTzZ6Z54WXHdGeJJXKL9AONpzOGWjh78NrPxTU_ZCgvQuBryc7ZEcajIb6aoE8p-8FdwAbbOKaPqCd6WlG3VnkLAGKKeo7vFqq5VJEdebIGIek1VJ4LUAyIRg-gEdt3WcdpcXWp8SwwPLVt_cKW-mtSySQn-G2a_y5mVhW02XsZiA7vWhfZUpDgvqy4A7Z1yjEYPe6rAdhuDDzyLqcZ5uj")
                };
                return new FormUrlEncodedContent(loginPostInfo);
            }
        }

        [Preserve(AllMembers = true)]
        public class TokenResponse
        {
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string access_token { get; set; }
            public string refresh_token { get; set; }
        }
    }
}
