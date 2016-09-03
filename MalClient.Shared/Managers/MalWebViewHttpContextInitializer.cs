using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using MALClient.XShared.Comm.MagicalRawQueries;

namespace MALClient.Shared.Managers
{
    public static class MalWebViewHttpContextInitializer
    {
        private static bool _webViewsInitialized;

        /// <summary>
        /// Moves needed cookies to globaly used client by WebViews control, web view authentication in other words.
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeContextForWebViews(bool mobile)
        {
            if (_webViewsInitialized)
                return;
            _webViewsInitialized = true;

            var filter = new HttpBaseProtocolFilter();
            var httpContext = await MalHttpContextProvider.GetHttpContextAsync();
            var cookies = httpContext.Handler.CookieContainer.GetCookies(new Uri(MalHttpContextProvider.MalBaseUrl));
            if (mobile)
            {
                filter.CookieManager.SetCookie(new HttpCookie("view", "myanimelist.net", "/") { Value = "sp" });
            }
            foreach (var cookie in cookies.Cast<Cookie>())
            {
                try
                {
                    var newCookie = new HttpCookie(cookie.Name, cookie.Domain, cookie.Path) { Value = cookie.Value };
                    filter.CookieManager.SetCookie(newCookie);
                }
                catch (Exception)
                {
                    var msg = new MessageDialog("Something went wrong™", "Authorization failed while rewriting cookies, I don't know why this is happenning and after hours of debugging it fixed itself after reinstall. :(");
                    await msg.ShowAsync();
                }

            }

            filter.AllowAutoRedirect = true;
            Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient(filter); //use globaly by webviews
        }
    }
}
