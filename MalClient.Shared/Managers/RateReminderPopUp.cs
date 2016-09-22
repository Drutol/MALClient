using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Popups;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Shared.Managers
{
    public static class RateReminderPopUp
    {
        public const int LaunchThresholdValue = 7;
        public const int DonateLaunchThresholdValue = 80;

        public static async void ProcessRatePopUp()
        {
            if (!Settings.RatePopUpEnable)
                return;
            Settings.RatePopUpStartupCounter++;
            if (Settings.RatePopUpStartupCounter <= LaunchThresholdValue)
                return;
            var msg =
                new MessageDialog(
                    "Your feedback helps improve this app!\n\nPlease take a minute to review this application , if you want to fill in bug report check out the about page. :) \n\nYou can disable this pop-up entirely in settings.",
                    "Rate MALClient!");
            msg.Commands.Add(new UICommand("To the store!", async command =>
            {
                Settings.RatePopUpStartupCounter = 0;
                Settings.RatePopUpEnable = false;
                await
                    Launcher.LaunchUriAsync(
                        new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
            }));
            msg.Commands.Add(new UICommand("Not now...", command => Settings.RatePopUpStartupCounter = 0));
            msg.CancelCommandIndex = 1;
            try
            {
                await msg.ShowAsync();
            }
            catch (Exception)
            {
                //
            }

        }

        /// <summary>
        /// This will showup once every 80 app launches.
        /// </summary>
        public static async void ProcessDonatePopUp()
        {
            if(Settings.Donated)
                return;
            Settings.DonatePopUpStartupCounter++;
            if (Settings.DonatePopUpStartupCounter <= DonateLaunchThresholdValue)
                return;
            try
            {
                Settings.DonatePopUpStartupCounter = 0;
                var consumables = await CurrentApp.GetUnfulfilledConsumablesAsync();
                if (consumables.Count > 0)
                {
                    Settings.Donated = true;
                    return; //user has already donated
                }
                var msg =
                    new MessageDialog(
                        "Did you consider making a donation?\nI've devoted hundreds of hours worth of time so I would be grateful for a little tip if you don't mind :)",
                        "You seem to use this app quite a bit!");
                msg.Commands.Add(new UICommand("Okay..."));
                msg.CancelCommandIndex = 1;

                await msg.ShowAsync();
            }
            catch (Exception)
            {
                //
            }
            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.DonatePopUpAppeared);
        }
    }
}