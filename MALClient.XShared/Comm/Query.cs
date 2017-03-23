using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm
{
    public abstract class Query
    {
        protected WebRequest Request;

        public static ApiType CurrentApiType { get; set; } = Settings.SelectedApiType;

        public virtual async Task<string> GetRequestResponse(bool wantMsg = true, string statusBarMsg = null)
        {
            var responseString = "";
            try
            {
                //using (var client = new HttpClient())
                //{
                //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",Convert.ToBase64String(Encoding.UTF8.GetBytes("MALClientTestAcc:passwd")));
                //    var res = await client.GetAsync(Request.RequestUri);
                //    var content = await res.Content.ReadAsStringAsync();
                //    return content;
                //}
                var response = await Request.GetResponseAsync();

                using (var stream = response.GetResponseStream())
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                    reader.Dispose();
                }
                return responseString;
            }
            catch (Exception e)
            {
                if (wantMsg)
                {
                    try
                    {
                        //await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        //{
                        //    var msg = new MessageDialog(e.Message, "An error occured");
                        //    await msg.ShowAsync();
                        //});
                    }
                    catch (Exception)
                    {
                        //window not yet loaded or something
                    }
                }
                if (statusBarMsg != null)
                {
                    //Utilities.GiveStatusBarFeedback(statusBarMsg);
                }
            }
            return responseString;
        }
    }
}