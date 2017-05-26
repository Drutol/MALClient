using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using MALClient.UWP.Adapters;
using MALClient.UWP.Shared.Managers;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.BGTaskLiveTilesNotifications
{
    public sealed class LiveTilesBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferall = taskInstance.GetDeferral();

            try
            {
                ResourceLocator.RegisterAppDataServiceAdapter(new ApplicationDataServiceService());
                ResourceLocator.RegisterDataCacheAdapter(new DataCache(null));
                ResourceLocator.RegisterMessageDialogAdapter(new MessageDialogProvider());
            }
            catch (Exception)
            {
                //may be already registered... voodoo I guess
            }

            await LiveTilesManager.UpdateNewsTilesAsync();
            deferall.Complete();
        }
    }
}
