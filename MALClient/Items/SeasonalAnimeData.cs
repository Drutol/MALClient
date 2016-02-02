using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Items
{
    public class SeasonalAnimeData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public List<string> Genres { get; set; }
        public float Score { get; set; }
        public int Members { get; set; }
        public string ImgUrl { get; set; }
        public string MalLink { get; set; }
        public string Episodes { get; set; }
        public int Index { get; set; }
        public int AirDay { get; set; }
    }
}
