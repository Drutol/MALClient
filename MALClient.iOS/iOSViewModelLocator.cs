using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Adapters;
using MALClient.Adapters.Credentails;
using MALClient.iOS.Adapters;
using MALClient.iOS.Managers;
using MALClient.XShared.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace MALClient.iOS.ViewModel
{
	public static class iOSViewModelLocator
	{
		public static void RegisterDependencies()
		{
			ViewModelLocator.RegisterBase();

			//SimpleIoc.Default.Register<IHamburgerViewModel, HamburgerControlViewModel>();
			SimpleIoc.Default.Register<INavMgr, NavigationManager>();
			//SimpleIoc.Default.Register<IMainViewModel, MainViewModel>();

			SimpleIoc.Default.Register<IDataCache, DataCache>();
			SimpleIoc.Default.Register<IPasswordVault, PasswordVaultProvider>();
			SimpleIoc.Default.Register<IApplicationDataService, ApplicationDataService>();
			SimpleIoc.Default.Register<IClipboardProvider, ClipboardProvider>();
			SimpleIoc.Default.Register<ISystemControlsLauncherService, SystemControlLauncherService>();
			SimpleIoc.Default.Register<IMessageDialogProvider, MessageDialogProvider>();
			SimpleIoc.Default.Register<IImageDownloaderService, ImageDownloaderService>();
		}
	}
}