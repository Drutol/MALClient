namespace MALClient.Models.Models.MalSpecific
{
    public class MalProfileHistoryEntry
    {     
        public int Id { get; set; }
        public int WatchedEpisode { get; set; }
        public string Date { get; set; }
        public bool IsAnime { get; set; }
        public string ShowUnit => IsAnime ? "Episode" : "Chapter";
    }
}
