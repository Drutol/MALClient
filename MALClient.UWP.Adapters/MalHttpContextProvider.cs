using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.XShared.BL;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Adapters
{
    public class MalHttpContextProvider : MalHttpContextProviderBase
    {
        protected override async Task<CsrfHttpClient> ObtainContext()
        {
            var httpHandler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _httpClient = new CsrfHttpClient(httpHandler) { BaseAddress = new Uri(MalBaseUrl) };
            await _httpClient.GetToken();
            //_httpClient.Handler.CookieContainer.Add(new Cookie("is_logged_in", "1", "/", "myanimelist.net"));
            //_httpClient.Handler.CookieContainer.Add(new Cookie("m_gdpr_mdl", "1", "/", "myanimelist.net"));
            //_httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
            //    "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            //_httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
            //_httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en",.9));

            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml",.9));
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/apng"));
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*",.8));

            //_httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            //_httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            //_httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));

            var response = await _httpClient.PostAsync("/login.php", LoginPostBody);
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Found ||
                response.StatusCode == HttpStatusCode.RedirectMethod)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (content.Contains("Too many failed login attempts. Please try to login again after several hours."))
                {
                    ResourceLocator.DispatcherAdapter.Run(async () =>
                    {
                        await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                            "Too many failed login attempts. Your account is locked according to MAL. You have to reset your password.",
                            "Reset your password");
                        if (ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
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
