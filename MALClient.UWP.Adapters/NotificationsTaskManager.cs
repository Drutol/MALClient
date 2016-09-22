using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Adapters;
using MALClient.Shared.Managers;

namespace MALClient.UWP.Adapters
{
    public class NotificationTaskManagerAdapter : INotificationsTaskManager
    {
        public void StartTask()
        {
            NotificationTaskManager.StartNotificationTask();
        }

        public void StopTask()
        {
            NotificationTaskManager.StopNotificationTask();
        }

        public void CallTask()
        {
            NotificationTaskManager.CallBackgroundTask();
        }
    }
}
