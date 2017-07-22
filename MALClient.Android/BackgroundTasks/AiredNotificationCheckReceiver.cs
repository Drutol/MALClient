using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using MALClient.Android.Activities;
using MALClient.Android.Adapters;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.BackgroundTasks
{
    [BroadcastReceiver]
    public class AiredNotificationCheckReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Context context, Intent intent)
        {
            try
            {
                ResourceLocator.RegisterAppDataServiceAdapter(new ApplicationDataServiceService());
                ResourceLocator.RegisterBase();

            }
            catch (Exception)
            {
                //may be already registered... voodoo I guess
            }

            bool updated = false;
            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            foreach (var entry in ResourceLocator.HandyDataStorage.RegisteredAiringNotifications.StoredItems)
            {
                var notificationToTrigger = (DateTime.Now.Subtract(entry.StartAirTime).Days / 7) + 1;
                if(entry.TriggeredNotifications == notificationToTrigger)
                    continue;

                var notificationIntent = new Intent(context, typeof(MainActivity));
                notificationIntent.SetAction(DateTime.Now.Ticks.ToString());
                notificationIntent.PutExtra("launchArgs", $"https://myanimelist.net/anime/{entry.Id}");
                var pendingIntent = PendingIntent.GetActivity(context, 0, notificationIntent, PendingIntentFlags.OneShot);
                var notificationBuilder = new NotificationCompat.Builder(context)
                    .SetSmallIcon(Resource.Drawable.badge_icon)
                    .SetContentTitle("New anime episode is on air!")
                    .SetContentText($"Episode {notificationToTrigger} of {entry.Title} has just aired!")
                    .SetAutoCancel(true)
                    .SetLargeIcon((await ImageService.Instance.LoadUrl(entry.ImageUrl).AsBitmapDrawableAsync()).Bitmap)
                    .SetGroup("airing")
                    .SetContentIntent(pendingIntent)
                    .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification));

                entry.TriggeredNotifications = notificationToTrigger;
                
                notificationManager.Notify($"{entry.Id};ep{notificationToTrigger}".GetHashCode(), notificationBuilder.Build());
                updated = true;
            }

            if(updated)
                ResourceLocator.HandyDataStorage.RegisteredAiringNotifications.SaveData();
        }
    }
}