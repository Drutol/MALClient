using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Android.Adapters;
using MALClient.Models.Models.Notifications;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.BackgroundTasks
{
    [BroadcastReceiver]
    public class NotificationClickBroadcastReceiver : BroadcastReceiver
    {
        public const string NotificationReadKey = "NotificationMarkRead";

        public override async void OnReceive(Context context, Intent intent)
        {
            var details = intent.Extras?.GetString(NotificationReadKey, null);

            if (details != null)
            {
                try
                {
                    ResourceLocator.RegisterAppDataServiceAdapter(new ApplicationDataServiceService());
                    ResourceLocator.RegisterPasswordVaultAdapter(new PasswordVaultProvider());
                    ResourceLocator.RegisterMessageDialogAdapter(new MessageDialogProvider());
                    Credentials.Init();
                }
                catch (Exception)
                {
                    //we may be already up and running, pure voodoo
                }

                var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                notificationManager.Cancel(details.GetHashCode());

                await MalNotificationsQuery.MarkNotifiactionsAsRead(new MalNotification(details));
            }
        }
    }
}