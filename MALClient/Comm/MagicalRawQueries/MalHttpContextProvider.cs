using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Popups;
using HtmlAgilityPack;
using MALClient.Utils;

namespace MALClient.Comm.MagicalRawQueries
{
    /// <summary>
    ///     Client wrapped with token.
    /// </summary>
    public class CsrfHttpClient : HttpClient
    {
        public CsrfHttpClient(HttpClientHandler hander) : base(hander)
        {
        }

        public string Token { get; set; }

        protected new void Dispose(bool disposing)
        {
            //it's not disposable
        }

        public void ExpiredDispose()
        {
            base.Dispose();
        }

        public async Task GetToken()
        {
            var raw = await GetAsync("/login.php");
            var doc = new HtmlDocument();
            doc.LoadHtml(await raw.Content.ReadAsStringAsync());

            var nodes = doc.DocumentNode.Descendants("head").First().ChildNodes;
            var csfr =
                nodes.First(
                    htmlNode =>
                        htmlNode.Attributes.Contains("name") && htmlNode.Attributes["name"].Value == "csrf_token")
                    .Attributes["content"].Value;

            Token = csfr;
        }
    }

    public static class MalHttpContextProvider
    {
        private const string MalBaseUrl = "http://myanimelist.net";
        private static CsrfHttpClient _httpClient;
        private static DateTime? _contextExpirationTime;

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
        public static async Task<CsrfHttpClient> GetHttpContextAsync(bool updateToken = false)
        {
            if (_contextExpirationTime == null || DateTime.Now.CompareTo(_contextExpirationTime.Value) > 0)
            {
                _httpClient?.ExpiredDispose();

                var httpHandler = new HttpClientHandler
                {
                    CookieContainer = new CookieContainer(),
                    UseCookies = true,
                    AllowAutoRedirect = false,
                };
                Utilities.GiveStatusBarFeedback("Performing http authentication...");
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
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Found)
                {
                    _contextExpirationTime = DateTime.Now.Add(TimeSpan.FromHours(.5));
                    if (updateToken) //we are here just to update this thing
                        return null;
                    return _httpClient; //else we are returning client that can be used for next queries
                }

                throw new WebException("Unable to authorize");
            }
            return _httpClient;
        }

        public static async void ErrorMessage(string what)
        {
            var msg = new MessageDialog($"Something went wrong... {what} implementation is pretty hacky so this stuff can happen from time to time, try again later or wait for next update. Sorry!", "Error");
            await msg.ShowAsync();
        }
    }
}