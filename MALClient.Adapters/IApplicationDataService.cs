using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Adapters
{
    public interface IApplicationDataService
    {
        object this[string key] { get; set; }
    }
}
