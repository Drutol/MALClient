using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;
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
            if (Settings.NotificationCheckInRuntime)
                ResourceLocator.SchdeuledJobsManger.StartJob(ScheduledJob.FetchNotifications, 5, () =>
                {
                    ResourceLocator.NotificationsTaskManager.CallTask(BgTasks.Notifications);
                });
        }
    }
}
