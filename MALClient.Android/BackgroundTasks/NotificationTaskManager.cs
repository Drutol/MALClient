using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.App.Job;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MALClient.Adapters;
using MALClient.Android.Activities;
using MALClient.Android.Widgets;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using Debug = System.Diagnostics.Debug;
using Exception = System.Exception;

namespace MALClient.Android.BackgroundTasks
{
    public class NotificationTaskManager : INotificationsTaskManager
    {
        internal void StartTask(BgTasks task, Context context)
        {
            Type listenerType;
            TimeSpan refreshTime;
            var id = 0;
            switch (task)
            {
                case BgTasks.Notifications:
                    if (!Settings.EnableNotifications || !Credentials.Authenticated ||
                        Settings.SelectedApiType == ApiType.Hummingbird)
                        return;
                    refreshTime = TimeSpan.FromMinutes(Settings.NotificationsRefreshTime);
                    listenerType = typeof(NotificationCheckBroadcastReceiver);
                    id = 3;
                    break;
                case BgTasks.Tiles:
                    return;
                case BgTasks.ToastActivation:
                    return;
                case BgTasks.AiredNotification:
                    listenerType = typeof(AiredNotificationCheckReceiver);
                    refreshTime = TimeSpan.FromHours(1);
                    id = 4;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(task), task, null);
            }

            var component = new ComponentName(context, Class.FromType(listenerType));
            var scheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
            var res = scheduler.Schedule(new JobInfo.Builder(id, component)
                .SetRequiredNetworkType(NetworkType.Any)
                .SetPersisted(true)
                .SetPeriodic((long) refreshTime.TotalMilliseconds + 1)
                .Build());
        }

        public void StartTask(BgTasks task)
        {
            StartTask(task,MainActivity.CurrentContext);
        }

        public void StopTask(BgTasks task)
        {
            var scheduler = (JobScheduler)MainActivity.CurrentContext.GetSystemService(Context.JobSchedulerService);
            switch (task)
            {
                case BgTasks.Notifications:
                    scheduler.Cancel(3);
                    break;
                case BgTasks.Tiles:
                    return;
                case BgTasks.ToastActivation:
                    return;
                case BgTasks.AiredNotification:
                    scheduler.Cancel(4);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(task), task, null);
            }               
        }

        public async void CallTask(BgTasks task)
        {
            try
            {
                await new NotificationCheckBroadcastReceiver.CalendarTask(MainActivity.CurrentContext).RunUpdate();
            }
            catch (Exception)
            {
                //something went wrong
            }
        }

    }
}