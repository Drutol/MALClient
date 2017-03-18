using MALClient.Models.Enums;
using SQLite;

namespace MALClient.Models.Models.AnimeScrapped
{
    public class TopAnimeData : SeasonalAnimeData
    {
        [PrimaryKey]
        public int CompositeKey
        {
            get
            {
                unchecked
                {
                    return (Id * 397) ^ (int) Type;
                }
            }
            set { }
        }

        public TopAnimeType Type { get; set; }
    }
}