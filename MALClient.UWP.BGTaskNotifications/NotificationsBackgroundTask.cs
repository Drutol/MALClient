using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using MALClient.XShared.Utils;

namespace MALClient.UWP.BGTaskNotifications
{
    public sealed class NotificationsBackgroundTask : IBackgroundTask
    {
        public BackgroundTaskDeferral Defferal { get; private set; }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Defferal = taskInstance.GetDeferral();
            Credentials.Init();


        }

    }
}
