using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace MALClient.Comm
{
    public enum ApiType
    {
        Mal,
        Hummingbird,
    }

    public abstract class Query
    {
        protected WebRequest Request;

        public static ApiType CurrentApiType { get; set; } = Settings.SelectedApiType;

        public async Task<string> GetRequestResponse(bool wantMsg = true,string statusBarMsg = null)
        {
            var responseString = "";
            try
            {
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
                        await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,async () =>
                        {
                            var msg = new MessageDialog(e.Message, "An error occured");
                            await msg.ShowAsync();
                        });
                    }
                    catch (Exception)
                    {
                        //window not yet loaded or something
                    }

                }
                if (statusBarMsg != null)
                {
                   Utils.GiveStatusBarFeedback(statusBarMsg);
                }
            }
            return responseString;
        }
      
    }
}