using System.Collections.Generic;

namespace MALClient.Models.Models
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
        string AirStartDate { get; set; }
    }
}