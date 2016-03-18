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
        //okay ... I know that it doesn't really fit here but 
        //I have to put it here in order to reuse some code
        //TODO : Rename this interface
        int Volumes { get; set; }
    }
}