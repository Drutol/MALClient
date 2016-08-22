using System.Collections.Generic;

namespace MalClient.Shared.Models.Anime
{
    public class AnimeHummingbirdDetailsData
    {
        public string AlternateCoverImgUrl;
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
                SourceId = SourceId,
                AlternateCoverImgUrl = AlternateCoverImgUrl
            };
        }
    }
}