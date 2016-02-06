namespace MALClient.Items
{
    public interface IAnimeData
    {
        int Id { get; set; }
        int MyScore { get; set; }
        int MyEpisodes { get; set; }
        string Title { get; set; }
        int MyStatus { get; set; }
        float GlobalScore { get; set; }
        int AllEpisodes { get; }
    }
}