using System;
using MALClient.Models.Enums;

namespace MALClient.Models.Models.Library
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
        int Type { get; set; }
        DateTime LastWatched { get; set; }
        string MyStartDate { get; set; }
        string MyEndDate { get; set; }
        string Notes { get; set; }
        bool IsRewatching { get; set; }
        string AlternateTitle { get; set; }
    }
}