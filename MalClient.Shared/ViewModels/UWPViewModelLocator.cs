using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Adapters;
using MALClient.Adapters.Credentails;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MalClient.Shared.ViewModels
{
    public class UWPViewModelLocator
    {
        public static void RegisterDependencies()
        {
            SimpleIoc.Default.Register<PinTileDialogViewModel>();
            SimpleIoc.Default.Register<IPinTileService>(() => PinTileDialog);
        }

        public static PinTileDialogViewModel PinTileDialog => SimpleIoc.Default.GetInstance<PinTileDialogViewModel>();
    }
}
