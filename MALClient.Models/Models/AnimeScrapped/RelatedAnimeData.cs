namespace MALClient.Models.Models.AnimeScrapped
{
    public class RelatedAnimeData : IDetailsPageArgs
    {
        public string WholeRelation { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public RelatedItemType Type { get; set; }
    }
}