using System.Collections.Generic;

namespace MALClient.Models
{
    public enum DataSource
    {
        Ann,
        Hummingbird,
        AnnHum
    }

    public class AnimeDetailsData
    {
        public string AlternateCoverImgUrl;
        public List<string> EDs;
        public List<string> Episodes;
        public List<string> Genres;
        public List<string> OPs;
        public DataSource Source = DataSource.Ann;
        public string SourceId;
    }
}