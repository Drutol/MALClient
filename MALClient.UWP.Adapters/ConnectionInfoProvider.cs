using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using MALClient.Adapters;

namespace MALClient.UWP.Adapters
{
    public class ConnectionInfoProvider : IConnectionInfoProvider
    {
        public void Init()
        {
            HasInternetConnection = NetworkInterface.GetIsNetworkAvailable();
        }

        public bool HasInternetConnection { get; set; }
    }
}
