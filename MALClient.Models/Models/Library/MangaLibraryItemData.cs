namespace MALClient.Models.Models.Library
{
    public class MangaLibraryItemData : AnimeLibraryItemData
    {
        public int AllVolumes;
        public int MyVolumes;
        public string SlugId { get; set; } //manga on hummingbird does not have integer id
    }
}