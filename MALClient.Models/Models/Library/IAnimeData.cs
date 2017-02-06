using MALClient.Models.Enums;

namespace MALClient.Models.Models.Library
{
    public interface IAnimeData
    {
        int Id { get; }
        float MyScore { get; set; }
        int MyEpisodes { get; set; }
        string Title { get; }
        AnimeStatus MyStatus { get; set; }
        float GlobalScore { get; set; }
        int AllEpisodes { get; }
        string StartDate { get; set; }
        string EndDate { get; set; }
        string Notes { get; set; }
        bool IsRewatching { get; set; }
        //okay ... I know that it doesn't really fit here but 
        //I have to put it here in order to reuse some code
        //TODO : Rename this interface
        int MyVolumes { get; set; }
        int AllVolumes { get; }
    }
}