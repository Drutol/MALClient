using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models
{
    public interface ISeasonalAnimeBaseData
    {
         int Id { get; set; }
         string Title { get; set; }
         List<string> Genres { get; set; }
         float Score { get; set; }
         string ImgUrl { get; set; }
         string Episodes { get; set; }
         int Index { get; set; }
         int AirDay { get; set; }
    }
}
