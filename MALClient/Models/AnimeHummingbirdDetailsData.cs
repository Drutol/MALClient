using System.Collections.Generic;

namespace MALClient.Models
{
    public class AnimeHummingbirdDetailsData
    {
        public string SourceId;
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Episodes { get; set; } = new List<string>();

        public AnimeDetailsData ToAnimeDetailsData()
        {
            return new AnimeDetailsData
            {
                Episodes = Episodes,
                Genres = Genres,
                Source = DataSource.Hummingbird,
                SourceId = SourceId
            };
        }
    }
}