using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.BL
{
    /// <summary>
    /// Platfrom independent init chain
    /// </summary>
    public static class InitializationRoutines
    {
        public static void InitApp()
        {
            ResourceLocator.ConnectionInfoProvider.Init();
            Credentials.Init();
            FavouritesManager.LoadData();
            AnimeImageQuery.Init();
            ViewModelLocator.ForumsMain.LoadPinnedTopics();
            if (Settings.NotificationCheckInRuntime && Credentials.Authenticated)
                ResourceLocator.SchdeuledJobsManger.StartJob(ScheduledJob.FetchNotifications, 5, () =>
                {
                    ResourceLocator.NotificationsTaskManager.CallTask(BgTasks.Notifications);
                });
            ResourceLocator.HandyDataStorage.Init();
        }

        public static void InitPostUpdate()
        {
            var previousVersion = Settings.AppVersion;

            ResourceLocator.ChangelogProvider.NewChangelog = previousVersion != null && previousVersion !=
                                                             ResourceLocator.ChangelogProvider.CurrentVersion;

            Settings.AppVersion = ResourceLocator.ChangelogProvider.CurrentVersion;
        }
    }
}
