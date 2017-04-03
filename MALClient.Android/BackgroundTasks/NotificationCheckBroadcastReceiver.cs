using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MALClient.Adapters;
using MALClient.Android.Activities;
using MALClient.Android.Adapters;
using MALClient.Models.Enums;
using MALClient.Models.Models.Notifications;
using MALClient.XShared.Comm.Forums;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Comm.MagicalRawQueries.Messages;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.BackgroundTasks
{
    [BroadcastReceiver]
    public class NotificationCheckBroadcastReceiver : BroadcastReceiver
    {
        private readonly SemaphoreSlim _toastSemaphore = new SemaphoreSlim(1);

        public override async void OnReceive(Context context, Intent sourceIntent)
        {
            try
            {
                ResourceLocator.RegisterBase();
                ResourceLocator.RegisterAppDataServiceAdapter(new ApplicationDataServiceService());
                ResourceLocator.RegisterPasswordVaultAdapter(new PasswordVaultProvider());
                ResourceLocator.RegisterMessageDialogAdapter(new MessageDialogProvider());
                ResourceLocator.RegisterDataCacheAdapter(new Adapters.DataCache());
                Credentials.Init();
            }
            catch (Exception)
            {
                //may be already registered... voodoo I guess
            }


            List<MalNotification> notifications = new List<MalNotification>();
            try
            {
                if (
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.FriendRequestAcceptDeny) ||
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.NewRelatedAnime) ||
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.BlogComment) ||
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.ClubMessages) ||
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.ForumQuoute) ||
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.FriendRequest) ||
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.NowAiring) ||
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.ProfileComment) ||
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.Payment) ||
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.UserMentions) ||
                    Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.WatchedTopics))
                {
                    notifications.AddRange(await MalNotificationsQuery.GetNotifications());
                    notifications =
                        notifications.Where(
                            notification =>
                                !notification.IsRead &&
                                (Settings.EnabledNotificationTypes & notification.Type) == notification.Type).ToList();
                }
            }
            catch (Exception e)
            {
                //http exec error
            }


            if ((Settings.EnabledNotificationTypes & MalNotificationsTypes.Messages) == MalNotificationsTypes.Messages)
            {
                try
                {
                    var msgs = await AccountMessagesManager.GetMessagesAsync(1);
                    foreach (var malMessageModel in msgs)
                    {
                        if (!malMessageModel.IsRead)
                        {
                            notifications.Add(new MalNotification(malMessageModel)); //I'm assuming that Ids are unique
                        }
                    }
                }
                catch (Exception)
                {
                    //no messages
                }

            }

            bool watchedTopicsUpdated = false;
            foreach (var watchedTopic in ResourceLocator.HandyDataStorage.WatchedTopics.StoredItems)
            {
                if (!watchedTopic.OnCooldown)
                {
                    var count = await new ForumTopicMessageCountQuery(watchedTopic.Id).GetMessageCount(true);
                    if (count == null)
                        continue;

                    if (count > watchedTopic.LastCheckedReplyCount)
                    {

                        var notif = new MalNotification(watchedTopic);
                        var diff = count - watchedTopic.LastCheckedReplyCount;
                        if (diff == 1)
                            notif.Content += " There is one new reply.";
                        else
                            notif.Content += $" There are {diff} new replies.";

                        notifications.Add(notif);

                        watchedTopic.OnCooldown = true;
                        watchedTopic.LastCheckedReplyCount = count.Value;
                        watchedTopicsUpdated = true;
                    }
                }
            }
            if (watchedTopicsUpdated)
                ResourceLocator.HandyDataStorage.WatchedTopics.SaveData();

            var allTriggeredNotifications = (ResourceLocator.ApplicationDataService[nameof(RoamingDataTypes.ReadNotifications)] ?? string.Empty) as string;
            var triggeredNotifications = allTriggeredNotifications?.Split(';').ToList() ?? new List<string>();

            //trigger new notifications
            foreach (var notification in notifications)
            {
                if (triggeredNotifications.Contains(notification.Id))
                    continue;

                triggeredNotifications.Add(notification.Id);
                ScheduleToast(context, notification);
            }

            //remove old triggered entries
            var presentNotifications = new List<string>();

            foreach (var triggeredNotification in triggeredNotifications)
            {
                if (notifications.Any(notif => notif.Id == triggeredNotification))
                    presentNotifications.Add(triggeredNotification);
            }

            ResourceLocator.ApplicationDataService[nameof(RoamingDataTypes.ReadNotifications)] = string.Join(";",
                presentNotifications);
        }

        private async void ScheduleToast(Context context, MalNotification notification)
        {
            await _toastSemaphore.WaitAsync();
            var intent = new Intent(context,  typeof(MainActivity));
            intent.SetAction(DateTime.Now.Ticks.ToString());
            intent.PutExtra("launchArgs",((notification.Type == MalNotificationsTypes.UserMentions && !notification.IsSupported) ||
                              notification.Type == MalNotificationsTypes.FriendRequest ||
                              notification.Type == MalNotificationsTypes.ClubMessages
                                 ? "OpenUrl;"
                                 : "") + notification.LaunchArgs);
            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot);
            var notificationBuilder = new NotificationCompat.Builder(context)
                .SetSmallIcon(Resource.Drawable.badge_icon)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(notification.Content))
                .SetContentTitle(notification.Header)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification));

            if (notification.Type != MalNotificationsTypes.Messages &&
                notification.Type != MalNotificationsTypes.WatchedTopic)
            {
                var readIntent = new Intent(context, typeof(NotificationClickBroadcastReceiver));
                readIntent.SetAction(DateTime.Now.Ticks.ToString());
                readIntent.PutExtra(NotificationClickBroadcastReceiver.NotificationReadKey, notification.Id);
                var pendingReadIntent = PendingIntent.GetBroadcast(context, 23, readIntent, PendingIntentFlags.OneShot);
                notificationBuilder.AddAction(new NotificationCompat.Action(Resource.Drawable.icon_eye_notification, "Mark as Read",
                    pendingReadIntent));
            }

            var notificationManager = (NotificationManager) context.GetSystemService(Context.NotificationService);
            notificationManager.Notify(notification.Id.GetHashCode(), notificationBuilder.Build());
            await Task.Delay(500);
            _toastSemaphore.Release();
        }

        private bool CheckAppInForeground(Context context)
        {
            var activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            return activityManager.RunningAppProcesses.First(p => p.Pid == Process.MyPid())?.Importance ==
                       Importance.Foreground;
        }
    }
}