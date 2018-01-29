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
using MALClient.Models.Models.Notifications;
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
            var items = ResourceLocator.HandyDataStorage.RegisteredAiringNotifications;
            var expiredItems = new List<AiringShowNotificationEntry>();
            await ResourceLocator.AiringInfoProvider.Init(false);
            bool updated = false;
            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            foreach (var entry in items.StoredItems)
            {
                var intId = int.Parse(entry.Id);
                if (!ResourceLocator.AiringInfoProvider.TryGetCurrentEpisode(intId, out var notificationToTrigger,
                    DateTime.Today))
                {
                    expiredItems.Add(entry);
                    continue;
                }
                if (notificationToTrigger == entry.TriggeredNotifications)
                    continue;

                var notificationIntent = new Intent(context, typeof(MainActivity));
                notificationIntent.SetAction(DateTime.Now.Ticks.ToString());
                notificationIntent.PutExtra("launchArgs", $"https://myanimelist.net/anime/{entry.Id}");
                var pendingIntent =
                    PendingIntent.GetActivity(context, 0, notificationIntent, PendingIntentFlags.OneShot);
                var notificationBuilder = new NotificationCompat.Builder(context)
                    .SetSmallIcon(Resource.Drawable.badge_icon)
                    .SetContentTitle("New anime episode is on air!")
                    .SetContentText($"Episode {notificationToTrigger} of {entry.Title} has just aired!")
                    .SetAutoCancel(true)
                    .SetGroup("airing")
                    .SetContentIntent(pendingIntent)
                    .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification));

                try
                {
                    notificationBuilder.SetLargeIcon(
                        (await ImageService.Instance.LoadUrl(entry.ImageUrl).AsBitmapDrawableAsync()).Bitmap);
                }
                catch (Exception e)
                {
                    //no image
                }


                entry.TriggeredNotifications = notificationToTrigger;
                if (entry.EpisodeCount == entry.TriggeredNotifications)
                    expiredItems.Add(entry);
                else
                {
                    if (!ResourceLocator.AiringInfoProvider.TryGetNextAirDate(intId,
                        DateTime.Today.Add(TimeSpan.FromDays(.9)), out var nextAir))
                        expiredItems.Add(entry);
                }

                notificationManager.Notify($"{entry.Id};ep{notificationToTrigger}".GetHashCode(),
                    notificationBuilder.Build());
                updated = true;
            }


            items.StartBatch();
            foreach (var expiredItem in expiredItems)
                items.StoredItems.Remove(expiredItem);
            items.CommitBatch();

            if(updated)
                ResourceLocator.HandyDataStorage.RegisteredAiringNotifications.SaveData();
        }
    }
}