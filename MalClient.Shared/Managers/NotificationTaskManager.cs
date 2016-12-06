using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using MALClient.Models.Enums;
using MALClient.XShared.Delegates;
using MALClient.XShared.Utils;

namespace MALClient.Shared.Managers
{
    public static class NotificationTaskManager
    {
        private const string NotificationsTaskName = "NotificationsBackgroundTask";
        private const string NotificationsTaskNamespace = "MALClient.UWP.BGTaskNotifications.NotificationsBackgroundTask";

        private const string TilesTaskName = "LiveTilesBackgroundTask";
        private const string TilesTaskNamespace = "MALClient.UWP.BGTaskLiveTilesNotifications.LiveTilesBackgroundTask";

        public static event BackgroundTaskCall OnNotificationTaskRequested;

        private static Dictionary<BgTasks, BackgroundTaskRegistration> TaskRegistration { get; } =
            new Dictionary<BgTasks, BackgroundTaskRegistration>();


        public static async void StartNotificationTask(BgTasks targetTask,bool restart = true)
        {
            string taskName, taskEntryPoint;
            int refreshTime;
            if (targetTask == BgTasks.Notifications)
            {
                if (!Settings.EnableNotifications || !Credentials.Authenticated ||
                    Settings.SelectedApiType == ApiType.Hummingbird)
                    return;

                refreshTime = Settings.NotificationsRefreshTime;
                taskName = NotificationsTaskName;
                taskEntryPoint = NotificationsTaskNamespace;
            }
            else
            {
                refreshTime = 720; //half a day 
                taskName = TilesTaskName;
                taskEntryPoint = TilesTaskNamespace;
            }

            var taskRegistered = TaskRegistration.ContainsKey(targetTask);

            if (!taskRegistered)
                taskRegistered = TryFindTask(targetTask);
            
            if (taskRegistered && !restart)
                return;
            try
            {
                if (taskRegistered)
                    TaskRegistration[targetTask].Unregister(true);

                var access = await BackgroundExecutionManager.RequestAccessAsync();
                if (access == BackgroundAccessStatus.DeniedBySystemPolicy ||
                    access == BackgroundAccessStatus.DeniedByUser)
                    return;

                var builder = new BackgroundTaskBuilder
                {
                    Name = taskName,
                    TaskEntryPoint = taskEntryPoint
                };

                builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                builder.SetTrigger(new TimeTrigger((uint)refreshTime, false));

                if (TaskRegistration.ContainsKey(targetTask))
                    TaskRegistration[targetTask] = builder.Register();
                else
                    TaskRegistration.Add(targetTask, builder.Register());
            }
            catch (Exception)
            {
                //unable to register this task
            }
        }

        private static bool TryFindTask(BgTasks targetTask)
        {
            var taskName = targetTask == BgTasks.Notifications ? NotificationsTaskName : TilesTaskName;
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    TaskRegistration.Add(targetTask, task.Value as BackgroundTaskRegistration);
                    return true;
                }
            }
            return false;
        }


        public static void StopTask(BgTasks task)
        {
            if (!TaskRegistration.ContainsKey(task))
            {
                if(!TryFindTask(task))
                    return;
            }
            TaskRegistration[task].Unregister(false);
            TaskRegistration.Remove(task);
        }

        public static void CallBackgroundTask(BgTasks task)
        {
            OnNotificationTaskRequested?.Invoke(task);
        }
    }
}
