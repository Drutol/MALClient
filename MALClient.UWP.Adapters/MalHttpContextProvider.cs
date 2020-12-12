using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Enums;
using MALClient.XShared.BL;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Adapters
{
    public class MalHttpContextProvider : MalHttpContextProviderBase
    {
        protected override async Task<CsrfHttpClient> ObtainContext()
        {
            var httpHandler = ResourceLocator.MalHttpContextProvider.GetHandler();
            _httpClient = new CsrfHttpClient(httpHandler) { BaseAddress = new Uri(MalBaseUrl) };

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authority", "myanimelist.net");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Host", "myanimelist.net");
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", new[] { "XMLHttpRequest" });
            _httpClient.Handler.CookieContainer.Add(new Cookie("anime_update_advanced", "0", "/", "myanimelist.net"));

            var existingCookies = Credentials.Password;

            if (!string.IsNullOrEmpty(existingCookies))
            {
                SetCookies(existingCookies);

                try
                {
                    var req = await _httpClient.GetAsync("https://myanimelist.net/editprofile.php?go=myoptions");
                    req.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                        "Failed to sign in.",
                        "Error.");
                    throw new WebException($"Unable to authorize,");
                }
            }

            await _httpClient.GetToken();

            if (string.IsNullOrEmpty(Credentials.UserName))
            {
                var raw = await _httpClient.GetStringAsync("https://myanimelist.net");
                var doc = new HtmlDocument();
                doc.LoadHtml(raw);

                var username = doc.FirstOfDescendantsWithClass("a", "header-profile-link").InnerText.Trim();

                Credentials.UserName = username;

                var matches = Regex.Match(raw, "\\/images\\/userimages\\/(\\d+)\\..*");
                if (matches.Success)
                {
                    Credentials.SetId(int.Parse(matches.Groups[1].Captures[0].Value));
                }
            }

            return _httpClient;
        }

        public override HttpClientHandler GetHandler()
        {
            return new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
            };
        }
    }
}
