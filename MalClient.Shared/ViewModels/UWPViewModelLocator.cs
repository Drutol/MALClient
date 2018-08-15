using GalaSoft.MvvmLight.Ioc;
using MALClient.Adapters;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Shared.ViewModels
{
    public class UWPViewModelLocator
    {
        public static void RegisterDependencies()
        {
            SimpleIoc.Default.Register<PinTileDialogViewModel>();
            SimpleIoc.Default.Register<SettingsViewModelBase,SettingsViewModel>();
            SimpleIoc.Default.Register<ISnackbarProvider, SnackbarProvider>();
            SimpleIoc.Default.Register<IPinTileService>(() => PinTileDialog);
        }

        public static PinTileDialogViewModel PinTileDialog => SimpleIoc.Default.GetInstance<PinTileDialogViewModel>();

        class SnackbarProvider : ISnackbarProvider
        {
            public void ShowText(string text)
            {
                
            }
        }
    }
}
