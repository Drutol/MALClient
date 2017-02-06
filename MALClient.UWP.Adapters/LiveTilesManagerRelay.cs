using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Adapters;
using MALClient.Models.Models.Library;
using MALClient.UWP.Shared.Managers;
using MALClient.XShared.Utils.Managers;

namespace MALClient.UWP.Adapters
{
    public class LiveTilesManagerRelay : ILiveTilesManager
    {
        public void UpdateTile(IAnimeData item)
        {
            LiveTilesManager.UpdateTile(item);
        }
    }
}
