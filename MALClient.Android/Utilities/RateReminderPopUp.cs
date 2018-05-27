using System;
using Android.Content;
using MALClient.Android.Activities;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android
{
    public static class RateReminderPopUp
    {
        public const int LaunchThresholdValue = 10;

        public static void ProcessRatePopUp()
        {
            //if (!Settings.RatePopUpEnable)
            //    return;
            //Settings.RatePopUpStartupCounter++;
            //if (Settings.RatePopUpStartupCounter <= LaunchThresholdValue)
            //    return;
            //ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.RatePopUpAppeared);
            //ResourceLocator.MessageDialogProvider.ShowMessageDialogWithInput("Your feedback helps improve MALClient!\n\nPlease take a minute to review this app, for bug reports check out the about page. :) \n\nYou can disable this pop-up entirely in settings.","Rate MALClient!","To the store!","Not now...",CallbackOnTrue,CallBackOnFalse);       
        }

        private static void CallBackOnFalse()
        {
            Settings.RatePopUpStartupCounter = 0;
        }

        private static void CallbackOnTrue()
        {
            Settings.RatePopUpStartupCounter = 0;
            Settings.RatePopUpEnable = false;
            //ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://github.com/Drutol/MALClient/issues"));
            MainActivity.CurrentContext.StartActivity(new Intent(Intent.ActionView, global::Android.Net.Uri.Parse($"market://details?id={MainActivity.CurrentContext.PackageName}")));
        }
    }
}