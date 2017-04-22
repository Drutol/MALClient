using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Adapters;
using MALClient.Adapters.Credentials;
using MALClient.WPF.Adapters;
using MALClient.XShared.ViewModels;

namespace MALClient.WPF
{
    public class WPFViewModelLocator
    {
        public static void RegisterBase()
        {
            ViewModelLocator.RegisterBase();

            SimpleIoc.Default.Register<IDataCache, DataCache>();
            SimpleIoc.Default.Register<IPasswordVault, PasswordVaultProvider>();
            SimpleIoc.Default.Register<IApplicationDataService, ApplicationDataServiceService>();
            SimpleIoc.Default.Register<IDatabaseContextProvider, DatabaseContextProvider>();
        }

    }
}
