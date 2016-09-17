using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;
using MALClient.Models.Models.Notifications;
using MALClient.UWP.Adapters;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Microsoft.Toolkit.Uwp.Notifications;

namespace MALClient.UWP.BGTaskNotifications
{
    public sealed class NotificationsBackgroundTask : IBackgroundTask
    {
        public BackgroundTaskDeferral Defferal { get; private set; }
        private SemaphoreSlim ToastSemaphore = new SemaphoreSlim(1);

        private int waitCount = 0;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Defferal = taskInstance.GetDeferral();
            ResourceLocator.RegisterAppDataServiceAdapter(new ApplicationDataServiceService());
            ResourceLocator.RegisterPasswordVaultAdapter(new PasswordVaultProvider());
            ResourceLocator.RegisterMessageDialogAdapter(new MessageDialogProvider());
            Credentials.Init();

            var notifications = await MalNotificationsQuery.GetNotifications();

            var allTriggeredNotifications = (string)(ApplicationData.Current.LocalSettings.Values["TriggeredNotifications"] ?? string.Empty);

            var triggeredNotifications = allTriggeredNotifications.Split(';').ToList();

            //trigger new notifications
            foreach (var notification in notifications)
            {
                if(triggeredNotifications.Contains(notification.Id))
                    continue;

                triggeredNotifications.Add(notification.Id);
                ScheduleToast(notification);
            }

            //remove old triggered entries
            var presentNotifications = new List<string>();

            foreach (var triggeredNotification in triggeredNotifications)
            {
                if (notifications.Any(notif => notif.Id == triggeredNotification))
                    presentNotifications.Add(triggeredNotification);
            }

            ApplicationData.Current.LocalSettings.Values["TriggeredNotifications"] = string.Join(";",presentNotifications);
        }

        private async void ScheduleToast(MalNotification notification)
        {
            waitCount++;
            var toastContent = BuildToast(notification);
            await ToastSemaphore.WaitAsync();
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastContent.GetXml()));
            await Task.Delay(500);
            ToastSemaphore.Release();
            waitCount--;
            if(waitCount == 0)
                Defferal.Complete();
        }

        private ToastContent BuildToast(MalNotification notification)
        {
            return new ToastContent()
            {
                Launch = $"action=viewEvent&eventId={notification.Id}",
                Scenario = ToastScenario.Reminder,

                Visual = new ToastVisual()
                {
                   

                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = notification.Type.ToString()
                            },

                            new AdaptiveText()
                            {
                                Text = notification.Content
                            },
                        }
                    },
                              
                },
                Actions = new ToastActionsCustom
                {
                    Buttons =
                    {
                        new ToastButtonDismiss()
                    }
                }

                
            };
        }

    }
}
