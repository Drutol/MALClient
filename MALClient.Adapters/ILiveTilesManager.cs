using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Models.Library;

namespace MALClient.Adapters
{
    public interface ILiveTilesManager
    {
        void UpdateTile(IAnimeData item);
    }
}
