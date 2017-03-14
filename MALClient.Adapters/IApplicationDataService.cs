using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Adapters
{
    public enum RoamingDataTypes
    {
        ReadNotifications,
        LastLibraryUpdate,
    }

    public interface IApplicationDataService
    {
        object this[string key] { get; set; }

        object this[RoamingDataTypes key] { get; set; }
    }
}
