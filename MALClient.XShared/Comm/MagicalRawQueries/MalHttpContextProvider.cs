using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using ModernHttpClient;
using HttpClient = System.Net.Http.HttpClient;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace MALClient.XShared.Comm.MagicalRawQueries
{
    /// <summary>
    ///     Client wrapped with token.
    /// </summary>
    public class CsrfHttpClient : HttpClient
    {
        private bool _loggedIn;

        public CsrfHttpClient(HttpClientHandler handler) : base(handler)
        {
            Handler = handler;
        }

        public string Token { get; set; }
#if ANDROID
        public string MalSessionIdCookie { get; set; } //logged in
        public string MalLogSessionIdCookie { get; set; } //basic

        public bool LoggedIn
        {
            get { return _loggedIn; }
            set
            {
                _loggedIn = value;
                if (value)
                {

                    DefaultRequestHeaders.Remove("Cookie");
                    DefaultRequestHeaders.Add("Cookie",
                        $"{MalSessionIdCookie}; {MalLogSessionIdCookie}; is_logged_in=1; ");
                }


            }
        }
#endif
        public bool Disabled { get; set; }

        public HttpClientHandler Handler { get; }



        protected new void Dispose(bool disposing)
        {
            //it's not disposable
        }

        public void ExpiredDispose()
        {
            base.Dispose();
        }

        public new async Task<HttpResponseMessage> GetAsync(string uri)
        {
//#if ANDROID
//            if (DefaultRequestHeaders.Contains("Cookie"))
//                DefaultRequestHeaders.Remove("Cookie");
//            if (MalLogSessionIdCookie == null)
//                content.Headers.Add("Cookie", $"{MalSessionIdCookie}");
//            else
//                content.Headers.Add("Cookie", $"{MalSessionIdCookie};{MalSessionIdCookie}");
//#endif


            if (!Disabled)
            {
                var resp = await base.GetAsync(uri);
#if ANDROID
                //if(!LoggedIn)
                //    ProcessCookies(resp);
#endif
                return resp;
            }
            return await new Task<HttpResponseMessage>(() => new HttpResponseMessage(HttpStatusCode.Forbidden));
        }

        public new async Task<HttpResponseMessage> PostAsync(string uri,HttpContent content)
        {
            if (!Disabled)
            {
                var resp = await base.PostAsync(uri,content);
#if ANDROID
              //  if (!LoggedIn)
              //      ProcessCookies(resp);
#endif
                return resp;
            }
            return await new Task<HttpResponseMessage>(() => new HttpResponseMessage(HttpStatusCode.Forbidden));
        }

        public async Task GetToken()
        {
#if ANDROID
            var raw = await GetAsync("/login.php");
#else
            var raw = await GetAsync("/pressroom"); //because it's lightweight and does not redirect
#endif
            ParseTokenFromHtml(await raw.Content.ReadAsStringAsync());

        }

        public void ParseTokenFromHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.Descendants("head").First().ChildNodes;
            var csfr =
                nodes.First(
                        htmlNode =>
                            htmlNode.Attributes.Contains("name") &&
                            htmlNode.Attributes["name"].Value == "csrf_token")
                    .Attributes["content"].Value;
            Token = csfr;
        }

        private void ProcessCookies(HttpResponseMessage raw)
        {
            bool changed = false;
            IEnumerable<string> values = new List<string>();
            if (raw.Headers.TryGetValues("set-cookie", out values))
            {
                foreach (var value in values)
                {
                    if (value.Contains("MALHLOGSESSID"))
                    {
                        MalLogSessionIdCookie = value.Substring(value.IndexOf("MALHLOGSESSID"), value.IndexOf(';'));
                        changed = true;
                    }
                    else if (value.Contains("MALSESSIONID"))
                    {
                        MalSessionIdCookie = value.Substring(value.IndexOf("MALSESSIONID"), value.IndexOf(';'));
                        changed = true;
                    }
                }
            }
            if (changed)
            {
                if (DefaultRequestHeaders.Contains("Cookie"))
                    DefaultRequestHeaders.Remove("Cookie");

                if (MalSessionIdCookie == null)
                    DefaultRequestHeaders.Add("Cookie", $"{MalLogSessionIdCookie};");
                else
                    DefaultRequestHeaders.Add("Cookie", $"{MalSessionIdCookie}; {MalLogSessionIdCookie}");
            }
        }
    }

    public static class MalHttpContextProvider
    {
        public const string MalBaseUrl = "https://myanimelist.net";
        private static CsrfHttpClient _httpClient;
        private static DateTime? _contextExpirationTime;
        private static bool _webViewsInitialized;
        private static bool _skippedFirstError;


        public static void ErrorMessage(string what)
        {
            ResourceLocator.MessageDialogProvider.ShowMessageDialog($"Something went wrong... {what} implementation is pretty hacky so this stuff can happen from time to time, try again later or wait for next update. Sorry!", "Error");
        }

        /// <summary>
        ///     Establishes connection with MAL, attempts to authenticate.
        /// </summary>
        /// <param name="updateToken">
        ///     Indicates whether created http client is meant to be used further or do we want to dispose it and return null.
        /// </param>
        /// <exception cref="WebException">
        ///     Unable to authorize.
        /// </exception>
        /// <returns>
        ///     Returns valid http client which can interact with website API.
        /// </returns>
        public static async Task<CsrfHttpClient> GetHttpContextAsync()
        {
            try
            {
                if (_contextExpirationTime == null || DateTime.Now.CompareTo(_contextExpirationTime.Value) > 0)
                {
                    _httpClient?.ExpiredDispose();

#if ANDROID
                    var httpHandler = new NativeMessageHandler(false,false,new NativeCookieHandler())
                    {
                        UseCookies = true,
                        AllowAutoRedirect = false,
                    };
                    //httpHandler.CookieContainer.Add(new Uri(MalBaseUrl),new Cookie("view","sp","/", "myanimelist.net"));
#else
                    var httpHandler = new HttpClientHandler()
                    {
                        CookieContainer = new CookieContainer(),
                        UseCookies = true,
                        AllowAutoRedirect = false,
                    };
#endif

                    //Utilities.GiveStatusBarFeedback("Performing http authentication...");
                    _httpClient = new CsrfHttpClient(httpHandler) { BaseAddress = new Uri(MalBaseUrl) };

                    await _httpClient.GetToken(); //gets token and sets cookies     


                    var loginPostInfo = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_name", Credentials.UserName),
                    new KeyValuePair<string, string>("password", Credentials.Password),
                    new KeyValuePair<string, string>("sublogin", "Login"),
                    new KeyValuePair<string, string>("cookie", "1"),
                    new KeyValuePair<string, string>("submit", "1"),
                    new KeyValuePair<string, string>("csrf_token", _httpClient.Token)
                };
                    var content = new FormUrlEncodedContent(loginPostInfo);

                    //we won't dispose it here because this instance gonna be passed further down to other queries

                    var response = await _httpClient.PostAsync("/login.php", content);
                    if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.RedirectMethod)
                    {
                        _contextExpirationTime = DateTime.Now.Add(TimeSpan.FromHours(.5));
                        return _httpClient; //else we are returning client that can be used for next queries
                    }
#if ANDROID
                    //retry after magiaclly obtaining magical cookie... kill me plz
                    else
                    {
                        //we do it here because cookies need to be set first, token changes otherwise
                        await _httpClient.GetToken();
                        loginPostInfo = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("user_name", Credentials.UserName),
                            new KeyValuePair<string, string>("password", Credentials.Password),
                            new KeyValuePair<string, string>("sublogin", "Login"),
                            new KeyValuePair<string, string>("cookie", "1"),
                            new KeyValuePair<string, string>("submit", "1"),
                            new KeyValuePair<string, string>("csrf_token", _httpClient.Token)
                        };
                        response = await _httpClient.PostAsync("/login.php", new FormUrlEncodedContent(loginPostInfo));
                        if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.RedirectMethod)
                        {
                            _contextExpirationTime = DateTime.Now.Add(TimeSpan.FromHours(.5));
                            _httpClient.ParseTokenFromHtml(await response.Content.ReadAsStringAsync());
                            //_httpClient.LoggedIn = true;
                            return _httpClient; //else we are returning client that can be used for next queries
                        }

                    }
#endif

                    throw new WebException("Unable to authorize");
                }
                return _httpClient;
            }
            catch (Exception e)
            {
                if (!_skippedFirstError && e.Message != "Unable to authorize")
                {
                    _skippedFirstError = true;
                    await Task.Delay(500);
                    return await GetHttpContextAsync(); //bug in android http client
                }
                if(!(e is TaskCanceledException) && !(e.InnerException is InvalidOperationException))
                ResourceLocator.MessageDialogProvider.ShowMessageDialog(
                    "Unable to connect to MyAnimeList, they have either changed something in html or your connection is down.",
                    "Something went wrong™");

                _skippedFirstError = false;
#if ANDROID
                return new CsrfHttpClient(new NativeMessageHandler()) {Disabled = true};
#else
                return new CsrfHttpClient(new HttpClientHandler()) { Disabled = true };
#endif
            }

        }

    }
}