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
using MALClient.Adapters.Credentials;
using MALClient.Android.Activities;
using MALClient.Android.Adapters;
using MALClient.Android.BackgroundTasks;
using MALClient.Android.DIalogs;
using MALClient.Android.Managers;
using MALClient.Android.ViewModels;
using MALClient.XShared.Interfaces;
using MALClient.XShared.ViewModels;

namespace MALClient.Android
{
    public static class AndroidViewModelLocator
    {
        class DialogProvider : IDialogsProvider
        {
            public void ShowScoreDialog(AnimeItemViewModel vm)
            {
                AnimeUpdateDialogBuilder.BuildScoreDialog(vm,f => vm.ChangeScore(f.ToString("N0")));
            }
        }

        public static void RegisterDependencies()
        {
            ViewModelLocator.Mobile = true;

            ViewModelLocator.RegisterBase();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<MainViewModelBase>(() => SimpleIoc.Default.GetInstance<MainViewModel>());
            SimpleIoc.Default.Register<SettingsViewModelBase>(() => SimpleIoc.Default.GetInstance<SettingsViewModel>());
            SimpleIoc.Default.Register<IHamburgerViewModel>(() => MainActivity.CurrentContext as IHamburgerViewModel);
            SimpleIoc.Default.Register<INavMgr,NavMgr>();
            SimpleIoc.Default.Register<Application>(() => App.Current);

            SimpleIoc.Default.Register<IDataCache, DataCache>();
            SimpleIoc.Default.Register<IPasswordVault, PasswordVaultProvider>();
            SimpleIoc.Default.Register<IApplicationDataService, ApplicationDataServiceService>();
            SimpleIoc.Default.Register<IClipboardProvider, ClipboardProvider>();
            SimpleIoc.Default.Register<ISystemControlsLauncherService, SystemControlLauncherService>();
            SimpleIoc.Default.Register<IMessageDialogProvider, MessageDialogProvider>();
            SimpleIoc.Default.Register<IImageDownloaderService, ImageDownloaderService>();
            SimpleIoc.Default.Register<ITelemetryProvider, TelemetryProvider>();
            SimpleIoc.Default.Register<INotificationsTaskManager, NotificationTaskManager>();
            SimpleIoc.Default.Register<ISchdeuledJobsManger, ScheduledJobsManager>();
            SimpleIoc.Default.Register<ICssManager, CssManager>();
            SimpleIoc.Default.Register<IChangeLogProvider, ChangelogProvider>();
            SimpleIoc.Default.Register<IMalHttpContextProvider, MalHttpContextProvider>();
            SimpleIoc.Default.Register<ISnackbarProvider, SnackbarProvider>();
            SimpleIoc.Default.Register<IConnectionInfoProvider, ConnectionInfoProvider>();
            SimpleIoc.Default.Register<IDispatcherAdapter, DispatcherAdapter>();
            SimpleIoc.Default.Register<IAiringNotificationsAdapter, AiringNotificationsAdapter>();
            SimpleIoc.Default.Register<IDialogsProvider, DialogProvider>();
            SimpleIoc.Default.Register<IShareProvider, ShareProvider>();

        }

        public static SettingsViewModel Settings => SimpleIoc.Default.GetInstance<SettingsViewModel>();
    }
}