using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using HtmlAgilityPack;
using MALClient.Android.Web;
using MALClient.Models.Enums;
using MALClient.XShared.BL;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ModernHttpClient;
using Xamarin.Android.Net;
using Object = Java.Lang.Object;

namespace MALClient.Android.Adapters
{
    [Preserve(AllMembers = true)]
    public class MalHttpContextProvider : MalHttpContextProviderBase
    {
        private TaskCompletionSource<bool> _tcs;
        private WebView _authWebView;

        public MalHttpContextProvider()
        {

        }

        protected override async Task<CsrfHttpClient> ObtainContext()
        {
            _tcs = new TaskCompletionSource<bool>();
            var httpHandler = ResourceLocator.MalHttpContextProvider.GetHandler();
            _httpClient = new CsrfHttpClient(httpHandler) { BaseAddress = new Uri(MalBaseUrl) };

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authority", "myanimelist.net");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Host", "myanimelist.net");
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", new[] { "XMLHttpRequest" });
            _httpClient.Handler.CookieContainer.Add(new Cookie("anime_update_advanced", "0", "/", "myanimelist.net"));

            var existingCookies = Credentials.Password;
            var success = false;

            if (!string.IsNullOrEmpty(existingCookies))
            {
                SetCookies(existingCookies);

                try
                {
                    var req = await _httpClient.GetAsync("https://myanimelist.net/editprofile.php?go=myoptions");

                    req.EnsureSuccessStatusCode();
                    success = true;

                    //if((int)req.StatusCode > 300 && (int)req.StatusCode < 400)
                    //    success = true;
                    //else
                    //    success = false;
                }
                catch (Exception e)
                {
                    success = false;
                }
            }

            if (!success)
            {
                ResourceLocator.DispatcherAdapter.Run(async () =>
                {
                    await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                        "Failed to sign in. Please sign in again, MAL seems to have invalidated your current session :(",
                        "Error.");
                });

                throw new WebException($"Unable to authorize,");
            }

            await _httpClient.GetToken();

            return _httpClient;

            //await _httpClient.GetToken();
            //await Task.Delay(300); //too may requests from MAL
            //var response = await _httpClient.PostAsync("https://myanimelist.net/login.php", LoginPostBody);
            //var content = await response.Content.ReadAsStringAsync();
            //try
            //{
            //    if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Found ||
            //    response.StatusCode == HttpStatusCode.RedirectMethod)
            //    {
            //        if (content.Contains("Too many failed login attempts"))
            //        {
            //            ResourceLocator.DispatcherAdapter.Run(async () =>
            //            {
            //                await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
            //                    "Too many failed login attempts. Please try signing in on website. Resetting password also sometimes help.",
            //                    "Failed to authorize.");
            //                if (ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
            //                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
            //            });

            //            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Too many failed login attempts."));
            //            throw new WebException($"Unable to authorize, failed logins");
            //        }
            //        if (content.Contains("This account has not yet authorized their e-mail."))
            //        {
            //            ResourceLocator.DispatcherAdapter.Run(async () =>
            //            {
            //                await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
            //                    "You didn't confirm your email address. Please confirm it before signing in.",
            //                    "Confirm your email.");
            //                if (ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
            //                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
            //            });
            //            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Not verified email."));
            //            throw new WebException($"Unable to authorize, email auth");
            //        }
            //        if (content.Contains("provide a captcha code"))
            //        {
            //            ResourceLocator.DispatcherAdapter.Run(async () =>
            //            {
            //                await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
            //                    "It looks like a captcha is waiting for you. Please sign in on website before using this app.",
            //                    "Website sign in required");
            //                if (ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
            //                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
            //                ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://myanimelist.net/login.php"));
            //            });

            //            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Captcha."));
            //            throw new WebException($"Unable to authorize, captcha");
            //        }

            //        if (content.Contains("Your username or password is incorrect."))
            //        {
            //            await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
            //                "App got response that your username or password is incorrect.",
            //                "Check your credentials");
            //            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Invalid credentials."));
            //            throw new WebException($"Unable to authorize, incorrect creds.");
            //        }

            //        if (content.Contains("badresult badresult--is-reset-password"))
            //        {
            //            await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
            //                "App got response that there's need for a password reset.",
            //                "Website sign in required");
            //            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Password reset."));
            //            throw new WebException($"Unable to authorize, password reset");
            //        }

            //        var matches = Regex.Match(content, "\\/images\\/userimages\\/(\\d+)\\..*");
            //        if (matches.Success)
            //        {
            //            Credentials.SetId(int.Parse(matches.Groups[1].Captures[0].Value));
            //        }

            //        _contextExpirationTime = DateTime.Now.Add(TimeSpan.FromHours(.5));
            //        return _httpClient; //else we are returning client that can be used for next queries
            //    }

            //    ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Too many failed login attempts."));
            //    throw new WebException($"Unable to authorize");
            //}
            //catch (Exception e)
            //{
            //    Crashes.GetErrorAttachments = report => new List<ErrorAttachmentLog>
            //    {
            //        ErrorAttachmentLog.AttachmentWithText(content, "login.html")
            //    };
            //    //ResourceLocator.ClipboardProvider.SetText($"{e}\n{response}\n{content}");
            //    //ResourceLocator.SnackbarProvider.ShowText("Error copied to clipboard.");
            //    if(!e.Message.Contains("captcha") && !e.Message.Contains("failed logins"))
            //        ResourceLocator.TelemetryProvider.TrackExceptionWithMessage(e, content);

            //    throw;
            //}
        }

        private async Task<string> NavigationInterceptOpportunity(string targeturl)
        {
            if (targeturl == "https://myanimelist.net")
            {
                var cookies = CookieManager.Instance.GetCookie("https://myanimelist.net");
                ///var callback = new ValueCallback();

                //_authWebView.EvaluateJavascript(
                //    "(function() { return ('<html>'+document.getElementsByTagName('html')[0].innerHTML+'</html>'); })();",
                //    callback);

                //var token = await callback.CompletionSource.Task;

                if (cookies.Contains("is_signed_in=1"))
                {
                    SetCookies(cookies);
                    _tcs.SetResult(true);
                }
                else
                {
                    _tcs.SetResult(false);
                }
            }

            _tcs.SetResult(false);
            return null;
        }

        public override HttpClientHandler GetHandler()
        {
            return new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
                //TODO, let's encrypt cert expired and caused issues
                ServerCertificateCustomValidationCallback = (message, certificate2, chain, errors) =>
                {
                    if (message.RequestUri.ToString().StartsWith("https://api.jikan.moe"))
                        return true;

                    if (errors == SslPolicyErrors.None)
                        return true;

                    return false;
                }
            };


            return new NativeMessageHandler(false, new TLSConfig(), new NativeCookieHandler())
            { 
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
            };
        }

        
    }

    class ValueCallback : Java.Lang.Object, IValueCallback
    {
        public TaskCompletionSource<string> CompletionSource { get; } = new TaskCompletionSource<string>();

        public void OnReceiveValue(Object? value)
        {
            var s = (Java.Lang.String) value;
            var str = (string) s;

            CompletionSource.SetResult(str);
        }
    }
}