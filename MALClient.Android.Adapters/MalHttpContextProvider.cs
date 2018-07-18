using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Models.Enums;
using MALClient.XShared.BL;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.ViewModels;
using ModernHttpClient;
using Xamarin.Android.Net;

namespace MALClient.Android.Adapters
{
    [Preserve(AllMembers = true)]
    public class MalHttpContextProvider : MalHttpContextProviderBase
    {
        protected override async Task<CsrfHttpClient> ObtainContext()
        {
            var httpHandler = new NativeMessageHandler(false,false,new NativeCookieHandler())
            {
                AllowAutoRedirect = false,
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = new CookieContainer(),
            };

            _httpClient = new CsrfHttpClient(httpHandler) { BaseAddress = new Uri(MalBaseUrl) };
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authority", "myanimelist.net");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Host", "myanimelist.net");
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", new[] { "XMLHttpRequest" });
            _httpClient.Handler.CookieContainer.Add(new Cookie("anime_update_advanced", "0", "/", "myanimelist.net"));

            await _httpClient.GetToken();

            var response = await _httpClient.PostAsync("/login.php", LoginPostBody);
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Found ||
                response.StatusCode == HttpStatusCode.RedirectMethod)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (content.Contains("Too many failed login attempts. Please try to login again after several hours."))
                {
                    ResourceLocator.DispatcherAdapter.Run( async () =>
                    {
                        await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                            "Too many failed login attempts. Your account is locked according to MAL. You have to reset your password.",
                            "Reset your password");
                        if(ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
                    });
 
                    throw new WebException("Unable to authorize");
                }

                if (content.Contains("Your username or password is incorrect.") ||
                    content.Contains("badresult badresult--is-reset-password"))
                    throw new WebException("Unable to authorize");

                _contextExpirationTime = DateTime.Now.Add(TimeSpan.FromHours(.5));
                return _httpClient; //else we are returning client that can be used for next queries
            }

            throw new WebException("Unable to authorize");
        }
    }
}