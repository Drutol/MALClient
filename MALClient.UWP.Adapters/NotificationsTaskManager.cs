using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.Shared.Managers;
using MALClient.XShared.Delegates;

namespace MALClient.UWP.Adapters
{
    public class NotificationTaskManagerAdapter : INotificationsTaskManager
    {
        public void StartTask(BgTasks task)
        {
            NotificationTaskManager.StartNotificationTask(task);
        }

        public void StopTask(BgTasks task)
        {
            NotificationTaskManager.StopTask(task);
        }

        public void CallTask(BgTasks task)
        {
            NotificationTaskManager.CallBackgroundTask(task);
        }
    }
}
