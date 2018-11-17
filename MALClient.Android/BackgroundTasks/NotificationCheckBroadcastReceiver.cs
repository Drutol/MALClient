using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using MALClient.Adapters;
using MALClient.Android.Activities;
using MALClient.Android.Adapters;
using MALClient.Models.Enums;
using MALClient.Models.Models.MalSpecific;
using MALClient.Models.Models.Notifications;
using MALClient.XShared.Comm.Forums;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Comm.MagicalRawQueries.Messages;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Uri = Android.Net.Uri;

namespace MALClient.Android.BackgroundTasks
{
    [global::Android.Runtime.Preserve(AllMembers = true)]
    [Service(Exported = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    public class NotificationCheckBroadcastReceiver : JobService
    {
        public override bool OnStartJob(JobParameters @params)
        {
            Log.Debug("MALClient-Notifications", "Starting job");
            var updater = new CalendarTask(ApplicationContext);
            updater.Execute();
            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            Log.Debug("MALClient-Notifications", "Job stopped");
            return true;
        }

        [global::Android.Runtime.Preserve(AllMembers = true)]
        public class CalendarTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, Java.Lang.Void>
        {
            private readonly Context _context;
            private readonly SemaphoreSlim _toastSemaphore = new SemaphoreSlim(1);

            public CalendarTask(Context context)
            {
                _context = context;
            }

            protected override Java.Lang.Void RunInBackground(params Java.Lang.Void[] @params)
            {
                Log.Debug("MalClient - Notifications", "Starting update in background.");
                RunUpdate().Wait();
                return default(Java.Lang.Void);
            }

            public async Task RunUpdate()
            {
                try
                {
                    ResourceLocator.RegisterBase();
                    ResourceLocator.RegisterAppDataServiceAdapter(new ApplicationDataServiceService());
                    ResourceLocator.RegisterPasswordVaultAdapter(new PasswordVaultProvider());
                    ResourceLocator.RegisterMessageDialogAdapter(new MessageDialogProvider());
                    ResourceLocator.RegisterHttpContextAdapter(new MalHttpContextProvider());
                    ResourceLocator.RegisterDataCacheAdapter(new Adapters.DataCache(null));
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
                        List<MalNotification> notifs = null;
                        await Task.Run(async () =>
                        {
                            notifs = await MalNotificationsQuery.GetNotifications();
                        });
                        notifications.AddRange(notifs);
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
                        List<MalMessageModel> msgs = null;
                        await Task.Run(async () =>
                        {
                            msgs = await AccountMessagesManager.GetMessagesAsync(1);
                        });
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


                if (!notifications.Any())
                    return;

                var dataService = (ResourceLocator.ApplicationDataService as ApplicationDataServiceService);

                dataService.OverridePreferenceManager(_context);

                var allTriggeredNotifications = (ResourceLocator.ApplicationDataService[nameof(RoamingDataTypes.ReadNotifications)] ?? string.Empty) as string;
                var triggeredNotifications = allTriggeredNotifications?.Split(';').ToList() ?? new List<string>();
                Log.Debug("MALClient",
                    $"Checking notifications: trig: {triggeredNotifications.Count} fetched: {notifications.Count}");
                //trigger new notifications
                foreach (var notification in notifications)
                {
                    if (triggeredNotifications.Contains(notification.Id))
                        continue;

                    triggeredNotifications.Add(notification.Id);
                    ScheduleToast(_context.ApplicationContext, notification);
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

                dataService.ResetPreferenceManagerOverride();
            }

            private async void ScheduleToast(Context context, MalNotification notification)
            {
                await _toastSemaphore.WaitAsync();
                var intent = new Intent(context, typeof(MainActivity));
                intent.SetAction(DateTime.Now.Ticks.ToString());
                intent.PutExtra("launchArgs", ((notification.Type == MalNotificationsTypes.UserMentions && !notification.IsSupported) ||
                                  notification.Type == MalNotificationsTypes.FriendRequest ||
                                  notification.Type == MalNotificationsTypes.ClubMessages
                                     ? "OpenUrl;"
                                     : "") + notification.LaunchArgs);
                var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot);

                var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                if (Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
                {
                    if (notificationManager.GetNotificationChannel("malclient_notifications") == null)
                    {
                        var notificationChannel = new NotificationChannel("malclient_notifications",
                            new Java.Lang.String("MyAnimeList.net notifications"), NotificationImportance.Default)
                        {
                            Name = "MALClient",
                            Description = "Notifications from MyAnimeList.net"
                        };

                        // Configure the notification channel.
                        
                        notificationManager.CreateNotificationChannel(notificationChannel);
                    }
                }

                var notificationBuilder = new NotificationCompat.Builder(context, "malclient_notifications")
                    .SetCategory("MALClient")
                    .SetSmallIcon(Resource.Drawable.ic_stat_name)
                    .SetStyle(new NotificationCompat.BigTextStyle().BigText(notification.Content))
                    .SetContentTitle(notification.Header)
                    .SetContentText(notification.Content)
                    .SetAutoCancel(true)
                    .SetGroup(notification.Type.GetDescription())
                    .SetContentIntent(pendingIntent);

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

               
                notificationManager.Notify("MALClient", notification.Id.GetHashCode(), notificationBuilder.Build());
                await Task.Delay(500);
                _toastSemaphore.Release();
            }
        }
    }
}