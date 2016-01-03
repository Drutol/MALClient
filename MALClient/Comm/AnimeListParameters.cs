using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm
{
    class AnimeListParameters : Parameters
    {
        public string user { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }
}
