using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using MALClient.Models.Enums;
using MALClient.Models.Models.Notifications;
using MALClient.XShared.Interfaces;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Microsoft.Toolkit.Uwp.Notifications;

namespace MALClient.UWP.Adapters
{
    public class AiringNotificationsAdapter : IAiringNotificationsAdapter
    {
        private const string Logo = "ms-appx:///Assets/BadgeLogo.scale-400.png";
        private static readonly TimeZoneInfo _jstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

        public void ScheduleToast(AiringShowNotificationEntry entry)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();

            var date = DateTime.SpecifyKind(entry.StartAirTime, DateTimeKind.Unspecified);
            TimeZoneInfo.ConvertTime(date, _jstTimeZone, TimeZoneInfo.Utc);
            entry.StartAirTime = date.ToLocalTime().Add(TimeSpan.FromHours(Settings.AiringNotificationOffset));
            var airedEps = (DateTime.Now.Subtract(entry.StartAirTime).Days / 7) + 1;
            for (int i = airedEps; i < entry.EpisodeCount; i++)
            {
                var toast = new ScheduledToastNotification(new ToastContent
                        {
                            Launch = $"https://myanimelist.net/anime/{entry.Id}",

                            Scenario = ToastScenario.Default,

                            Visual = new ToastVisual
                            {
                                BindingGeneric = new ToastBindingGeneric
                                {
                                    AppLogoOverride = new ToastGenericAppLogo {Source = Logo},
                                    HeroImage = new ToastGenericHeroImage {Source = entry.ImageUrl},
                                    Children =
                                    {
                                        new AdaptiveText
                                        {
                                            Text = "New anime episode is on air!"
                                        },

                                        new AdaptiveText
                                        {
                                            Text = $"Episode {i} of {entry.Title} has just aired!",
                                            HintStyle = AdaptiveTextStyle.Subtitle,
                                        }
                                    },
                                },
                            },
                        }.GetXml(), new DateTimeOffset(entry.StartAirTime.Add(TimeSpan.FromDays(7 * i))),
                        TimeSpan.FromMinutes(5), 1)
                    {Tag = $"{entry.Id};ep{i}"};
                notifier.AddToSchedule(toast);
            }
        }

        public void RemoveToasts(string id)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();

            var notications = notifier.GetScheduledToastNotifications();
            foreach (var scheduledToastNotification in notications)
            {
                if(scheduledToastNotification.Tag.Contains(id))
                    notifier.RemoveFromSchedule(scheduledToastNotification);
            }
        }

        public bool AreNotificationRegistered(string id)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();

            var notications = notifier.GetScheduledToastNotifications();
            return notications.Any(notification => notification.Tag.Contains(id));
        }
    }
}
