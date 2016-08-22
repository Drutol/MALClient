namespace MalClient.Shared.Models.MalSpecific
{
    public enum MalNewsType
    {
        Article,
        News,
    }

    public class MalNewsUnitModel
    {
        public string ImgUrl { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Highlight { get; set; }
        public string Author { get; set; }
        public string Views { get; set; }
        public string Tags { get; set; }
        public MalNewsType Type { get; set; }
    }
}
