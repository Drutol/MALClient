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
using MALClient.Models.Enums;
using MALClient.Models.Models.Notifications;
using MALClient.XShared.Interfaces;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Adapters
{
    public class AiringNotificationsAdapter : IAiringNotificationsAdapter
    {
        private static readonly TimeZoneInfo _jstTimeZone = TimeZoneInfo.CreateCustomTimeZone("JST", TimeSpan.FromHours(9), "JST", "JST");

        public void ScheduleToast(AiringShowNotificationEntry entry)
        {
            ResourceLocator.HandyDataStorage.RegisteredAiringNotifications.StoredItems.Add(entry);
            ResourceLocator.NotificationsTaskManager.StartTask(BgTasks.AiredNotification);
        }

        public void RemoveToasts(string id)
        {
            foreach (var entry in ResourceLocator.HandyDataStorage.RegisteredAiringNotifications.StoredItems)
            {
                if (entry.Id == id)
                {
                    ResourceLocator.HandyDataStorage.RegisteredAiringNotifications.StoredItems.Remove(entry);
                    break;
                }
            }
        }

        public bool AreNotificationRegistered(string id)
        {
            return ResourceLocator.HandyDataStorage.RegisteredAiringNotifications.StoredItems
                .Any(entry => entry.Id == id);
        }
    }
}