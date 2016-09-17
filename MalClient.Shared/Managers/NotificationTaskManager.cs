using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace MALClient.Shared.Managers
{
    public static class NotificationTaskManager
    {
        private static bool _taskRegistered;
        private const string TaskName = "NotificationsBackgroundTask";
        private const string TaskNamespace = "MALClient.UWP.BGTaskNotifications.NotificationsBackgroundTask";

        private static IBackgroundTaskRegistration TaskRegistration { get; set; }

        public static async void StartNotificationTask()
        {
            if(_taskRegistered)
                return;

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == TaskName)
                {
                    _taskRegistered = true;
                    TaskRegistration = task.Value;
                    return;
                }
            }
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            if(access == BackgroundAccessStatus.DeniedBySystemPolicy || 
               access == BackgroundAccessStatus.DeniedByUser)
                return;

            TaskRegistration?.Unregister(true);

            var builder = new BackgroundTaskBuilder();

            builder.Name = TaskName;
            builder.TaskEntryPoint = TaskNamespace;
            builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
            builder.SetTrigger(new SystemTrigger(SystemTriggerType.InternetAvailable, false));
            //builder.SetTrigger(new TimeTrigger(30,false));

            TaskRegistration = builder.Register();

            _taskRegistered = true;
        }


        public static void StopNotificationTask()
        {
            TaskRegistration.Unregister(false);
        }
    }
}
