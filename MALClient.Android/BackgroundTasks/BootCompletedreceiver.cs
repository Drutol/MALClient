using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MALClient.Android.Adapters;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.BackgroundTasks
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootCompletedreceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                try
                {
                    ResourceLocator.RegisterAppDataServiceAdapter(new ApplicationDataServiceService());
                }
                catch (Exception)
                {
                    //Log.Debug("MALCLient", "AppData present.");
                }
                try
                {
                    ResourceLocator.RegisterPasswordVaultAdapter(new PasswordVaultProvider());
                }
                catch (Exception)
                {
                    //Log.Debug("MALCLient", "Vault present.");
                }


                Credentials.Init();
                //Log.Debug("MALCLient", "Setting up notification fetching.");
                new NotificationTaskManager().StartTask(BgTasks.Notifications, context);
            }
            catch (Exception)
            {
                //I can throw error on startup of OS I really don't want that no matter what.
            }
        }
    }
}