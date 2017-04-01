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
using MALClient.Adapters;
using MALClient.Android.Activities;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;

namespace MALClient.Android.BackgroundTasks
{
    public class NotificationTaskManager : INotificationsTaskManager
    {
        public void StartTask(BgTasks task)
        {
    
            Type listenerType;
            TimeSpan refreshTime;
            switch (task)
            {
                case BgTasks.Notifications:
                    if (!Settings.EnableNotifications || !Credentials.Authenticated ||
                        Settings.SelectedApiType == ApiType.Hummingbird)
                        return;

                    refreshTime = TimeSpan.FromMinutes(Settings.NotificationsRefreshTime);
                    listenerType = typeof(NotificationCheckBroadcastReceiver);
                    break;
                case BgTasks.Tiles:
                    return;
                case BgTasks.ToastActivation:
                    return; 
                default:
                    throw new ArgumentOutOfRangeException(nameof(task), task, null);
            }
            StopTask(task);
            long now = SystemClock.CurrentThreadTimeMillis();
            var am = (AlarmManager)MainActivity.CurrentContext.GetSystemService(Context.AlarmService);
            var intent = new Intent(MainActivity.CurrentContext,listenerType);
            var pi = PendingIntent.GetBroadcast(MainActivity.CurrentContext, 0, intent, PendingIntentFlags.CancelCurrent);
            am.SetRepeating(AlarmType.RtcWakeup, now, (long)refreshTime.TotalMilliseconds, pi);
        }

        public void StopTask(BgTasks task)
        {
            Type listenerType;
            switch (task)
            {
                case BgTasks.Notifications:
                    listenerType = typeof(NotificationCheckBroadcastReceiver);
                    break;
                case BgTasks.Tiles:
                    return;
                case BgTasks.ToastActivation:
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(task), task, null);
            }
            var am = (AlarmManager)MainActivity.CurrentContext.GetSystemService(Context.AlarmService);
            var intent = new Intent(MainActivity.CurrentContext, listenerType);
            var broadcast = PendingIntent.GetBroadcast(MainActivity.CurrentContext, 0, intent, PendingIntentFlags.CancelCurrent);
            if (broadcast != null)
                am.Cancel(broadcast);
        }

        public void CallTask(BgTasks task)
        {
            try
            {
                new NotificationCheckBroadcastReceiver().OnReceive(MainActivity.CurrentContext,null);
            }
            catch (Exception)
            {
                //something went wrong
            }
        }

    }
}