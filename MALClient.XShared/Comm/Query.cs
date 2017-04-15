using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
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

            }
            return responseString;
        }
    }
}