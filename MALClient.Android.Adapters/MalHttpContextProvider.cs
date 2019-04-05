﻿using System;
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
                            "Too many failed login attempts. Your account is locked according to MAL. Please try signing in on website.",
                            "Failed to authorize.");
                        if(ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
                    });
 
                    throw new WebException("Unable to authorize");
                }
                if(content.Contains("This account has not yet authorized their e-mail."))
                {
                    ResourceLocator.DispatcherAdapter.Run(async () =>
                    {
                        await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                            "You didn't confirm your email address. Please confirm it before signing in.",
                            "Confirm your email.");
                        if (ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
                    });

                    throw new WebException("Unable to authorize");
                }
                if (content.Contains("It has been a while since your last login, for security reasons we require you to also provide a captcha code."))
                {
                    ResourceLocator.DispatcherAdapter.Run(async () =>
                    {
                        await ResourceLocator.MessageDialogProvider.ShowMessageDialogAsync(
                            "It looks like a captcha is waiting for you. Please sign in on website before using this app.",
                            "Website sign in required");
                        if (ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageLogIn)
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
                    });

                    throw new WebException("Unable to authorize");
                }

                if (content.Contains("Your username or password is incorrect.") ||
                    content.Contains("badresult badresult--is-reset-password"))
                    throw new WebException("Unable to authorize");

                var matches = Regex.Match(content, "\\/images\\/userimages\\/(\\d+)\\..*");
                if (matches.Success)
                {
                    Credentials.SetId(int.Parse(matches.Groups[1].Captures[0].Value));
                }

                _contextExpirationTime = DateTime.Now.Add(TimeSpan.FromHours(.5));
                return _httpClient; //else we are returning client that can be used for next queries
            }

            throw new WebException("Unable to authorize");
        }
    }
}