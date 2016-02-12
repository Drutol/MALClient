using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace MALClient.Comm
{
    public abstract class Query
    {
        protected WebRequest Request;

        public async Task<string> GetRequestResponse(bool wantMsg = true)
        {
            var responseString = "";
            try
            {
                WebResponse response = await Request.GetResponseAsync();

                using (Stream stream = response.GetResponseStream())
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
                    var msg = new MessageDialog(e.Message, "An error occured");
                    await msg.ShowAsync();
                }
            }
            return responseString;
        }
    }
}