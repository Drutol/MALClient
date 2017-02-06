using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using MALClient.Models.Models.Notifications;
using MALClient.UWP.Adapters;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.BGTaskToastActivation
{
    public sealed class ToastActivationTask : IBackgroundTask
    {
        private const string MarkReadAction = "MarkRead";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;

            if (details != null)
            {
                string arguments = details.Argument;

                var args = arguments.Split(';');

                if (args[0] == MarkReadAction)
                {
                    try
                    {
                        ResourceLocator.RegisterAppDataServiceAdapter(new ApplicationDataServiceService());
                        ResourceLocator.RegisterPasswordVaultAdapter(new PasswordVaultProvider());
                        ResourceLocator.RegisterMessageDialogAdapter(new MessageDialogProvider());
                        Credentials.Init();                    
                    }
                    catch (Exception)
                    {
                        //we may be already up and running, pure voodoo
                    }
                    var deff = taskInstance.GetDeferral();
                    await MalNotificationsQuery.MarkNotifiactionsAsRead(new MalNotification(args[1]));
                    deff.Complete();
                }
            }
        }
    }
}
