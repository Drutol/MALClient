using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
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
//#if ANDROID
//        public string MalSessionIdCookie { get; set; } //logged in
//        public string MalLogSessionIdCookie { get; set; } //basic

//        public bool LoggedIn
//        {
//            get { return _loggedIn; }
//            set
//            {
//                _loggedIn = value;
//                if (value)
//                {

//                    DefaultRequestHeaders.Remove("Cookie");
//                    DefaultRequestHeaders.Add("Cookie",
//                        $"{MalSessionIdCookie}; {MalLogSessionIdCookie}; is_logged_in=1; ");
//                }


//            }
//        }
//#endif
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

        public new Task<HttpResponseMessage> GetAsync(string uri)
        {
            if (!Disabled)
            {
                return base.GetAsync(uri);
            }
            return  new Task<HttpResponseMessage>(() => new HttpResponseMessage(HttpStatusCode.Forbidden));
        }

        public new Task<HttpResponseMessage> PostAsync(string uri,HttpContent content)
        {
            if (!Disabled)
            {
                return base.PostAsync(uri,content);
            }
            return new Task<HttpResponseMessage>(() => new HttpResponseMessage(HttpStatusCode.Forbidden));
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

        //private void ProcessCookies(HttpResponseMessage raw)
        //{
        //    bool changed = false;
        //    IEnumerable<string> values = new List<string>();
        //    if (raw.Headers.TryGetValues("set-cookie", out values))
        //    {
        //        foreach (var value in values)
        //        {
        //            if (value.Contains("MALHLOGSESSID"))
        //            {
        //                MalLogSessionIdCookie = value.Substring(value.IndexOf("MALHLOGSESSID"), value.IndexOf(';'));
        //                changed = true;
        //            }
        //            else if (value.Contains("MALSESSIONID"))
        //            {
        //                MalSessionIdCookie = value.Substring(value.IndexOf("MALSESSIONID"), value.IndexOf(';'));
        //                changed = true;
        //            }
        //        }
        //    }
        //    if (changed)
        //    {
        //        if (DefaultRequestHeaders.Contains("Cookie"))
        //            DefaultRequestHeaders.Remove("Cookie");

        //        if (MalSessionIdCookie == null)
        //            DefaultRequestHeaders.Add("Cookie", $"{MalLogSessionIdCookie};");
        //        else
        //            DefaultRequestHeaders.Add("Cookie", $"{MalSessionIdCookie}; {MalLogSessionIdCookie}");
        //    }
        //}
    }
}