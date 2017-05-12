using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.CommUtils;
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
            Credentials.Init();
            HtmlClassMgr.Init();
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

        public static async void InitPostUpdate()
        {
            var previousVersion = Settings.AppVersion;

            ResourceLocator.ChangelogProvider.NewChangelog = previousVersion != null && previousVersion !=
                                                             ResourceLocator.ChangelogProvider.CurrentVersion;

            if (await new NewsQuery().GetRequestResponse() == "Android Tajm!")
            {
                ResourceLocator.MessageDialogProvider.ShowMessageDialogWithInput(
                    "I've just released open beta of Android version and I'd like to invite you to try it out and help me test it :)\nAndroid has almost all features of Windows versions and UI is similar to what you are familiar with!",
                    "MALClient on Android!", "To the PlayStore!", "Nah, thanks",
                    () => ResourceLocator.SystemControlsLauncherService.LaunchUri(
                        new Uri("https://play.google.com/store/apps/details?id=com.drutol.malclient")));
            }

            Settings.AppVersion = ResourceLocator.ChangelogProvider.CurrentVersion;
        }
    }
}
