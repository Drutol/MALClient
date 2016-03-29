using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models
{

    public class AnimeHummingbirdDetailsData
    {
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Episodes { get; set; } = new List<string>();
        public string SourceId;

        public AnimeDetailsData ToAnimeDetailsData()
        {
            return new AnimeDetailsData
            {
                Episodes = Episodes,
                Genres = Genres,
                Source = DataSource.Hummingbird,
                SourceId = SourceId,
            };
        }
    }
}
