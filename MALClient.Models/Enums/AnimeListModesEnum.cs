using MALClient.Models.Enums.Enums;

namespace MALClient.XShared.Utils.Enums
{
    public enum AnimeListWorkModes
    {
        Anime,
        SeasonalAnime,
        Manga,
        TopAnime,
        TopManga
    }

    public enum AnimeListDisplayModes
    {      
        IndefiniteList,
        IndefiniteGrid,
        IndefiniteCompactList,
    }

    public enum SortOptions
    {
        [EnumUtilities.Description("Title")]
        SortTitle,
        [EnumUtilities.Description("Score")]
        SortScore,
        [EnumUtilities.Description("Watched")]
        SortWatched,
        [EnumUtilities.Description("Air day")]
        SortAirDay,
        [EnumUtilities.Description("Last watched")]
        SortLastWatched,
        [EnumUtilities.Description("Start date")]
        SortStartDate,
        [EnumUtilities.Description("End Date")]
        SortEndDate,
        [EnumUtilities.Description("")]
        SortNothing
    }
}