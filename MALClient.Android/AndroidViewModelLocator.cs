using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using MALClient.Adapters;
using MALClient.Adapters.Credentails;
using MALClient.Android.Activities;
using MALClient.Android.Adapters;
using MALClient.Android.Managers;
using MALClient.Android.ViewModels;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace MALClient.Android
{
    public static class AndroidViewModelLocator
    {
        public static void RegisterDependencies()
        {
            ViewModelLocator.RegisterBase();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MainViewModelBase>(() => SimpleIoc.Default.GetInstance<MainViewModel>());
            SimpleIoc.Default.Register<IHamburgerViewModel,HamburgerControlViewModel>();
            SimpleIoc.Default.Register<INavMgr,NavMgr>();

            SimpleIoc.Default.Register<IDataCache, DataCache>();
            SimpleIoc.Default.Register<IPasswordVault, PasswordVaultProvider>();
            SimpleIoc.Default.Register<IApplicationDataService, ApplicationDataServiceService>();
            SimpleIoc.Default.Register<IClipboardProvider, ClipboardProvider>();
            SimpleIoc.Default.Register<ISystemControlsLauncherService, SystemControlLauncherService>();
            SimpleIoc.Default.Register<IMessageDialogProvider, MessageDialogProvider>();
            SimpleIoc.Default.Register<IImageDownloaderService, ImageDownloaderService>();
            SimpleIoc.Default.Register<ITelemetryProvider, TelemetryProvider>();
            SimpleIoc.Default.Register<INotificationsTaskManager, NotificationTaskManagerAdapter>();
        }

        public static INavigationService NavigationService => ServiceLocator.Current.GetInstance<INavigationService>();
    }
}