using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalClient.Shared.Models.MalSpecific
{
    public class MalProfileHistoryEntry
    {     
        public int Id { get; set; }
        public int WatchedEpisode { get; set; }
        public string Date { get; set; }
        public bool IsAnime { get; set; }
    }
}
