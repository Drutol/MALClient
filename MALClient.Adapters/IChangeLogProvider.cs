using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Adapters
{
    public interface IChangeLogProvider
    {
        string DateWithVersion { get; }
        List<string> Changelog { get; }
    }
}
