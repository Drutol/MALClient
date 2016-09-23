using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.System;
using Windows.UI.Notifications;
using MALClient.Models.Enums;
using MALClient.Models.Models.Notifications;
using MALClient.UWP.Adapters;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Comm.MagicalRawQueries.Messages;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Microsoft.Toolkit.Uwp.Notifications;

namespace MALClient.UWP.BGTaskNotifications
{
    public sealed class NotificationsBackgroundTask : IBackgroundTask
    {
        private BackgroundTaskDeferral Defferal { get; set; }
        private readonly SemaphoreSlim _toastSemaphore = new SemaphoreSlim(1);
        private const string Logo = "ms-appx:///Assets/BadgeLogo.scale-400.png";

        private int _waitCount = 0;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            Defferal = taskInstance?.GetDeferral();

            //var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
            //if (details != null)
            //{
            //    await Launcher.LaunchUriAsync(new Uri(details.Argument));
            //    Defferal.Complete();
            //    return;
            //}

            if (taskInstance != null) //we are already running -> started on demand
            {
                ResourceLocator.RegisterAppDataServiceAdapter(new ApplicationDataServiceService());
                ResourceLocator.RegisterPasswordVaultAdapter(new PasswordVaultProvider());
                ResourceLocator.RegisterMessageDialogAdapter(new MessageDialogProvider());
                Credentials.Init();
            }

            List<MalNotification> notifications = new List<MalNotification>();

            if (
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.FriendRequestAcceptDeny) ||
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.NewRelatedAnime) ||
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.BlogComment) ||
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.ClubMessages) ||
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.ForumQuoute) ||
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.FriendRequest) ||
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.NowAiring) ||
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.ProfileComment) ||
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.Payment) ||
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.UserMentions) ||
                Settings.EnabledNotificationTypes.HasFlag(MalNotificationsTypes.WatchedTopics))
            {
                notifications.AddRange(await MalNotificationsQuery.GetNotifications());
                notifications =
                    notifications.Where(
                        notification =>
                            !notification.IsRead &&
                            (Settings.EnabledNotificationTypes & notification.Type) == notification.Type).ToList();
            }

            if ((Settings.EnabledNotificationTypes & MalNotificationsTypes.Messages) == MalNotificationsTypes.Messages)
            {
                var msgs = await AccountMessagesManager.GetMessagesAsync(1);
                foreach (var malMessageModel in msgs)
                {
                    if (!malMessageModel.IsRead)
                    {
                        notifications.Add(new MalNotification(malMessageModel)); //I'm assuming that Ids are unique
                    }
                }
            }

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
            _waitCount++;
            var toastContent = BuildToast(notification);
            await _toastSemaphore.WaitAsync();
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastContent.GetXml()));
            await Task.Delay(500);
            _toastSemaphore.Release();
            _waitCount--;
            if(_waitCount == 0)
                Defferal?.Complete();
        }

        private ToastContent BuildToast(MalNotification notification)
        {
            return new ToastContent()
            {
                Launch =
                ((notification.Type == MalNotificationsTypes.UserMentions && !notification.IsSupported) ||
                 notification.Type == MalNotificationsTypes.FriendRequest ||
                 notification.Type == MalNotificationsTypes.ClubMessages
                    ? "OpenUrl;"
                    : "") + notification.LaunchArgs,
                Scenario = ToastScenario.Default,

                Visual = new ToastVisual()
                {

                    BindingGeneric = new ToastBindingGeneric()
                    {
                        AppLogoOverride = new ToastGenericAppLogo {Source = Logo},
                        HeroImage =
                            string.IsNullOrEmpty(notification.ImgUrl)
                                ? null
                                : new ToastGenericHeroImage {Source = notification.ImgUrl},
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = notification.Header
                            },

                            new AdaptiveText()
                            {
                                Text = notification.Content
                            },
                            new AdaptiveText()
                            {
                                Text = notification.Date,
                                HintStyle = AdaptiveTextStyle.Subtitle
                            },
                        },

                    },


                },
                Actions = GetActions(notification)
            };
        }

        private ToastActionsCustom GetActions(MalNotification notification)
        {
            switch (notification.Type)
            {
                case MalNotificationsTypes.Generic:
                case MalNotificationsTypes.FriendRequestAcceptDeny:
                case MalNotificationsTypes.BlogComment:
                case MalNotificationsTypes.Payment:
                    return new ToastActionsCustom();
                case MalNotificationsTypes.FriendRequest:
                    return new ToastActionsCustom
                    {
                        Buttons =
                        {
                            new ToastButton("Open website","OpenUrl;" + notification.LaunchArgs)
                            {
                                ActivationType = ToastActivationType.Foreground,
                            },
                            new ToastButtonDismiss()
                        }
                    };
                case MalNotificationsTypes.ProfileComment:
                    return new ToastActionsCustom
                    {
                        Buttons =
                        {
                            new ToastButton("Open conversation", notification.LaunchArgs)
                            {
                                ActivationType = ToastActivationType.Foreground,
                            },
                            new ToastButtonDismiss()
                        }
                    };
                case MalNotificationsTypes.ForumQuoute:
                    return new ToastActionsCustom
                    {
                        Buttons =
                        {
                            new ToastButton("Open topic", notification.LaunchArgs)
                            {
                                ActivationType = ToastActivationType.Foreground,
                            },
                            new ToastButtonDismiss()
                        }
                    };
                case MalNotificationsTypes.UserMentions:
                    return new ToastActionsCustom
                    {
                        Buttons =
                        {
                            new ToastButton("Open", (!notification.IsSupported ? "OpenUrl;" : "") + notification.LaunchArgs)
                            {
                                ActivationType = ToastActivationType.Foreground,
                            },
                            new ToastButtonDismiss()
                        }
                    };
                case MalNotificationsTypes.WatchedTopics:
                    return new ToastActionsCustom
                    {
                        Buttons =
                        {
                            new ToastButton("Open topic", notification.LaunchArgs)
                            {
                                ActivationType = ToastActivationType.Foreground,
                            },
                            new ToastButtonDismiss()
                        }
                    };
                case MalNotificationsTypes.ClubMessages:
                    return new ToastActionsCustom
                    {
                        Buttons =
                        {
                            new ToastButton("Open website", "OpenUrl;" + notification.LaunchArgs)
                            {
                                ActivationType = ToastActivationType.Foreground,
                            },
                            new ToastButtonDismiss()
                        }
                    };
                case MalNotificationsTypes.NewRelatedAnime:
                    return new ToastActionsCustom
                    {
                        Buttons =
                        {
                            new ToastButton("Open details", notification.LaunchArgs)
                            {
                                ActivationType = ToastActivationType.Foreground,
                            },
                            new ToastButtonDismiss()
                        }
                    };
                case MalNotificationsTypes.NowAiring:
                    return new ToastActionsCustom
                    {
                        Buttons =
                        {
                            new ToastButton("Open details", notification.LaunchArgs)
                            {
                                ActivationType = ToastActivationType.Foreground,
                            },
                            new ToastButtonDismiss()
                        }
                    };

                case MalNotificationsTypes.Messages:
                    return new ToastActionsCustom
                    {
                        Buttons =
                        {
                            new ToastButton("Open conversation", notification.LaunchArgs)
                            {
                                ActivationType = ToastActivationType.Foreground,
                            },
                            new ToastButtonDismiss()
                        }
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

}
