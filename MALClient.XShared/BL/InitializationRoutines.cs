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
        public static TaskCompletionSource<bool> AwaitableCompletion { get; } = new TaskCompletionSource<bool>();

        public static async Task InitApp()
        {
            ResourceLocator.ConnectionInfoProvider.Init();
            Credentials.Init();
            FavouritesManager.LoadData();
            AnimeImageQuery.Init();
            ViewModelLocator.ForumsMain.LoadPinnedTopics();
            await ResourceLocator.AiringInfoProvider.Init(false);
            if (Settings.NotificationCheckInRuntime && Credentials.Authenticated)
                ResourceLocator.SchdeuledJobsManger.StartJob(ScheduledJob.FetchNotifications, 5, () =>
                {
                    ResourceLocator.NotificationsTaskManager.CallTask(BgTasks.Notifications);
                });
            ResourceLocator.HandyDataStorage.Init();
            AwaitableCompletion.SetResult(true);
        }

        public static void InitPostUpdate()
        {
            var previousVersion = Settings.AppVersion;
            var currentVersion = ResourceLocator.ChangelogProvider.CurrentVersion;
            var isNewVersion = false;
            if (previousVersion != null)
            {
                if (previousVersion.Substring(0, previousVersion.LastIndexOf('.')) !=
                    currentVersion.Substring(0, currentVersion.LastIndexOf('.')))
                    isNewVersion = true;
            }

            ResourceLocator.ChangelogProvider.NewChangelog = isNewVersion;
            Settings.AppVersion = ResourceLocator.ChangelogProvider.CurrentVersion;
        }
    }
}
