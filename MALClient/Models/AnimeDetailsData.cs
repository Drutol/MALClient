using System.Collections.Generic;

namespace MALClient.Models
{
    public enum DataSource
    {
        Ann,
        Hummingbird
    }


    public class AnimeDetailsData
    {
        public string SourceId;
        public DataSource Source = DataSource.Ann;
        public List<string> EDs;
        public List<string> Episodes;
        public List<string> Genres;
        public List<string> OPs;
    }
}