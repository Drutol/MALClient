using System.Collections.Generic;
using MALClient.Models;

namespace MALClient.Items
{
    public class SeasonalAnimeData : ISeasonalAnimeBaseData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<string> Genres { get; set; }
        public float Score { get; set; }
        public string ImgUrl { get; set; }
        public string Episodes { get; set; }
        public int Index { get; set; }
        public int AirDay { get; set; }
        public string AirStartDate { get; set; }
    }
}