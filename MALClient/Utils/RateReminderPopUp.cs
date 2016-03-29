﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Popups;

namespace MALClient
{
    public static class RateReminderPopUp
    {
        public const int LaunchThresholdValue = 7;

        public static async void ProcessRatePopUp()
        {
            if(!Settings.RatePopUpEnable)
                return;
            Settings.RatePopUpStartupCounter++;
            if(Settings.RatePopUpStartupCounter <= LaunchThresholdValue)
                return;
            var msg = new MessageDialog("Your feedback helps improve this app!\n\nPlease take a minute to review this application , if you want to fill in bug report check out the about page. :) \n\nYou can disable this pop-up entirely in settings.", "Rate MALClient!");
            msg.Commands.Add(new UICommand("To the store!", async command =>
            {
                Settings.RatePopUpStartupCounter = 0;
                Settings.RatePopUpEnable = false;
                await
                    Windows.System.Launcher.LaunchUriAsync(
                        new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
            }));
            msg.Commands.Add(new UICommand("Not now...", command => Settings.RatePopUpStartupCounter = 0));
            msg.CancelCommandIndex = 1;
            await msg.ShowAsync();
        }
    }
}
