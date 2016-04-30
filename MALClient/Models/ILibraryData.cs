namespace MALClient.Models
{
    public interface ILibraryData
    {
        int Id { get; set; }
        int MalId { get; set; }
        string Title { get; set; }
        AnimeStatus MyStatus { get; set; }
        float MyScore { get; set; }
        int MyEpisodes { get; set; }
        int AllEpisodes { get; set; }
        string ImgUrl { get; set; }
        AnimeType Type { get; set; }
    }
}