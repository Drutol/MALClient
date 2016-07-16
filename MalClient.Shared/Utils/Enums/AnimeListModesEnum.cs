namespace MalClient.Shared.Utils.Enums
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
        IndefiniteCompactList,
        IndefiniteList,
        IndefiniteGrid
    }

    public enum SortOptions
    {
        [Utilities.Description("Title")]
        SortTitle,
        [Utilities.Description("Score")]
        SortScore,
        [Utilities.Description("Watched")]
        SortWatched,
        [Utilities.Description("Air day")]
        SortAirDay,
        [Utilities.Description("Last watched")]
        SortLastWatched,
        [Utilities.Description("Start date")]
        SortStartDate,
        [Utilities.Description("End Date")]
        SortEndDate,
        [Utilities.Description("")]
        SortNothing
    }
}