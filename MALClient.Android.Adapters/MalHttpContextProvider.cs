using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ModernHttpClient;
using Xamarin.Android.Net;

namespace MALClient.Android.Adapters
{
    [Preserve(AllMembers = true)]
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

            await _httpClient.GetToken();
            await Task.Delay(300); //too may requests from MAL
            var response = await _httpClient.PostAsync("https://myanimelist.net/login.php", LoginPostBody);
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Found ||
                response.StatusCode == HttpStatusCode.RedirectMethod)
                {
                    if (content.Contains("Too many failed login attempts"))
                    {
                        ResourceLocator.DispatcherAdapter.Run(async () =>
                        {
                            await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                                "Too many failed login attempts. Please try signing in on website. Resetting password also sometimes help.",
                                "Failed to authorize.");
                            if (ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
                                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
                        });

                        ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Too many failed login attempts."));
                        throw new WebException($"Unable to authorize, failed logins");
                    }
                    if (content.Contains("This account has not yet authorized their e-mail."))
                    {
                        ResourceLocator.DispatcherAdapter.Run(async () =>
                        {
                            await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                                "You didn't confirm your email address. Please confirm it before signing in.",
                                "Confirm your email.");
                            if (ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
                                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
                        });
                        ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Not verified email."));
                        throw new WebException($"Unable to authorize, email auth");
                    }
                    if (content.Contains("provide a captcha code"))
                    {
                        ResourceLocator.DispatcherAdapter.Run(async () =>
                        {
                            await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                                "It looks like a captcha is waiting for you. Please sign in on website before using this app.",
                                "Website sign in required");
                            if (ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
                                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
                            ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://myanimelist.net/login.php"));
                        });

                        ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Captcha."));
                        throw new WebException($"Unable to authorize, captcha");
                    }

                    if (content.Contains("Your username or password is incorrect."))
                    {
                        await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                            "App got response that your username or password is incorrect.",
                            "Check your credentials");
                        ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Invalid credentials."));
                        throw new WebException($"Unable to authorize, incorrect creds.");
                    }

                    if (content.Contains("badresult badresult--is-reset-password"))
                    {
                        await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                            "App got response that there's need for a password reset.",
                            "Website sign in required");
                        ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Password reset."));
                        throw new WebException($"Unable to authorize, password reset");
                    }

                    var matches = Regex.Match(content, "\\/images\\/userimages\\/(\\d+)\\..*");
                    if (matches.Success)
                    {
                        Credentials.SetId(int.Parse(matches.Groups[1].Captures[0].Value));
                    }

                    _contextExpirationTime = DateTime.Now.Add(TimeSpan.FromHours(.5));
                    return _httpClient; //else we are returning client that can be used for next queries
                }

                ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FailedLogin, ("Reason", "Too many failed login attempts."));
                throw new WebException($"Unable to authorize");
            }
            catch (Exception e)
            {
                Crashes.GetErrorAttachments = report => new List<ErrorAttachmentLog>
                {
                    ErrorAttachmentLog.AttachmentWithText(content, "login.html")
                };
                //ResourceLocator.ClipboardProvider.SetText($"{e}\n{response}\n{content}");
                //ResourceLocator.SnackbarProvider.ShowText("Error copied to clipboard.");
                if(!e.Message.Contains("captcha") && !e.Message.Contains("failed logins"))
                    ResourceLocator.TelemetryProvider.TrackException(e, content);

                throw;
            }
        }

        public override HttpClientHandler GetHandler()
        {
            return new NativeMessageHandler(false, new TLSConfig(), new NativeCookieHandler())
            { 
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
            };
        }
    }
}