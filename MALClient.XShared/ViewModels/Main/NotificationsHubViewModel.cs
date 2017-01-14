using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Notifications;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Main
{
    public class NotificationsHubViewModel : ViewModelBase
    {
        private ObservableCollection<MalNotification> _notifications;
        private ICommand _markAsReadComand;
        private ICommand _markAllAsReadCommand;
        private MalNotificationsTypes? _currentNotificationType;
        private List<NotificationGroupViewModel> _notificationGroups;
        private ICommand _navigateNotificationCommand;
        private List<MalNotification> _allNotifications;
        private bool _loading;
        private bool _emptyNoticeVisibility;


        public async void Init(bool force = false)
        {
            if (AllNotifications == null || force)
            {
                Loading = true;
                AllNotifications = await MalNotificationsQuery.GetNotifications();
                Loading = false;
            }
        }

        public ICommand MarkAsReadComand => _markAsReadComand ?? (_markAsReadComand = new RelayCommand<MalNotification>(
                                                async notification =>
                                                {
                                                    if (await MalNotificationsQuery.MarkNotifiactionsAsRead(notification))
                                                    {
                                                        Notifications.Remove(notification);
                                                        AllNotifications.Remove(notification);
                                                        NotificationGroups.First(
                                                                model => model.NotificationType == notification.Type)
                                                            .NotificationsCount--;
                                                    }
                                                    else
                                                    {
                                                        ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to mark this notification as read.","Something went wrong...");
                                                        ResourceLocator.TelemetryProvider.LogEvent("Notification mark as read error.");
                                                    }
                                                }));

        public ICommand MarkAllAsReadCommand => _markAllAsReadCommand ?? (_markAllAsReadCommand = new RelayCommand(
                                                    async () =>
                                                    {
                                                        if (await MalNotificationsQuery.MarkNotifiactionsAsRead(Notifications))
                                                        {
                                                            Notifications.Clear();
                                                            foreach (var type in Notifications.Select(notification => notification.Type).Distinct())
                                                            {
                                                                NotificationGroups.First(
                                                                        model => model.NotificationType == type)
                                                                    .NotificationsCount = 0;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to mark notifications as read.", "Something went wrong...");
                                                            ResourceLocator.TelemetryProvider.LogEvent("Notification mark as read error.");
                                                        }
                                                    }));

        public ICommand NavigateNotificationCommand
            => _navigateNotificationCommand ?? (_navigateNotificationCommand = new RelayCommand<MalNotification>(
                   notification =>
                   {
                       if(!notification.IsSupported)
                           return;

                       var args = MalLinkParser.GetNavigationParametersForUrl(notification.LaunchArgs);
                       if (ViewModelLocator.Mobile)
                       {
                           if (!args.Item1.GetAttribute<EnumUtilities.PageIndexEnumMember>().OffPage)
                               ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageNotificationHub, null);
                       }
                       ViewModelLocator.GeneralMain.Navigate(args.Item1,args.Item2);
                   }));


        private List<MalNotification> AllNotifications
        {
            get { return _allNotifications; }
            set
            {
                _allNotifications = value;
                UpdateNotificationSet();
                NotificationGroups =
                    Enum.GetValues(typeof(MalNotificationsTypes))
                        .Cast<MalNotificationsTypes>()
                        .Except(new[]
                        {
                            MalNotificationsTypes.Generic, MalNotificationsTypes.Payment,
                            MalNotificationsTypes.BlogComment, MalNotificationsTypes.Messages, 
                        })
                        .Select(
                            type =>
                                new NotificationGroupViewModel
                                {
                                    NotificationType = type,
                                    NotificationsCount =
                                        AllNotifications.Count(notification => notification.Type == type)
                                }).ToList();
            }
        }

        public MalNotificationsTypes? CurrentNotificationType
        {
            get { return _currentNotificationType; }
            set
            {
                _currentNotificationType = value;
                RaisePropertyChanged(() => CurrentNotificationType);
                UpdateNotificationSet();
            }
        }

        public ObservableCollection<MalNotification> Notifications
        {
            get { return _notifications; }
            set
            {
                _notifications = value;
                RaisePropertyChanged(() => Notifications);
                EmptyNoticeVisibility = !value.Any();
            }
        }

        public List<NotificationGroupViewModel> NotificationGroups
        {
            get { return _notificationGroups; }
            set
            {
                _notificationGroups = value;
                RaisePropertyChanged(() => NotificationGroups);
            }
        }

        public class NotificationGroupViewModel : ViewModelBase
        {
            private int _notificationsCount;
            public MalNotificationsTypes NotificationType { get; set; }

            public int NotificationsCount
            {
                get { return _notificationsCount; }
                set
                {
                    _notificationsCount = value;
                    RaisePropertyChanged(() => NotificationsCount);
                }
            }

            public bool AnyNotifications => NotificationsCount != 0;
        }

        public bool EmptyNoticeVisibility
        {
            get { return _emptyNoticeVisibility; }
            set
            {
                _emptyNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyNoticeVisibility);
            }
        }

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        private void UpdateNotificationSet()
        {
            if (CurrentNotificationType != null)
                Notifications = new ObservableCollection<MalNotification>(AllNotifications.Where(notification => notification.Type == CurrentNotificationType));
            else
                Notifications = new ObservableCollection<MalNotification>(AllNotifications);
        }

    }
}
