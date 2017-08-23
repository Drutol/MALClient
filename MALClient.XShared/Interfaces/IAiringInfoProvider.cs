using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Interfaces
{
    public interface IAiringInfoProvider
    {
        void Init();
        bool TryGetCurrentEpisode(int id, ref int episode);
    }
}
