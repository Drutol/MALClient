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
using MALClient.Adapters;
using MALClient.Adapters.Credentails;
using MALClient.Android.Adapters;
using MALClient.Android.Managers;
using MALClient.Android.ViewModels;
using MALClient.XShared.ViewModels;

namespace MALClient.Android
{
    public static class AndroidViewModelLocator
    {
        public static void RegisterDependencies()
        {
            ViewModelLocator.RegisterBase();

            SimpleIoc.Default.Register<IHamburgerViewModel,HamburgerControlViewModel>();
            SimpleIoc.Default.Register<INavMgr,NavMgr>();
            SimpleIoc.Default.Register<IMainViewModel,MainViewModel>();

            SimpleIoc.Default.Register<IDataCache, DataCache>();
            SimpleIoc.Default.Register<IPasswordVault, PasswordVaultProvider>();
            SimpleIoc.Default.Register<IApplicationDataService, ApplicationDataServiceService>();
            SimpleIoc.Default.Register<IClipboardProvider, ClipboardProvider>();
            SimpleIoc.Default.Register<ISystemControlsLauncherService, SystemControlLauncherService>();
            SimpleIoc.Default.Register<IMessageDialogProvider, MessageDialogProvider>();
            SimpleIoc.Default.Register<IImageDownloaderService, ImageDownloaderService>();
        }
    }
}