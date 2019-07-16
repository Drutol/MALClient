namespace MALClient.Models.Enums
{
    public enum AnimeListWorkModes
    {
        [EnumUtilities.AnimeListWorkModeEnumMember(AllowLoadingMore = false)]
        Anime,
        [EnumUtilities.AnimeListWorkModeEnumMember(AllowLoadingMore = false)]
        SeasonalAnime,
        [EnumUtilities.AnimeListWorkModeEnumMember(AllowLoadingMore = false)]
        Manga,
        [EnumUtilities.AnimeListWorkModeEnumMember(AllowLoadingMore = true)]
        TopAnime,
        [EnumUtilities.AnimeListWorkModeEnumMember(AllowLoadingMore = true)]
        TopManga,
        [EnumUtilities.AnimeListWorkModeEnumMember(AllowLoadingMore = true)]
        AnimeByGenre,
        [EnumUtilities.AnimeListWorkModeEnumMember(AllowLoadingMore = true)]
        AnimeByStudio
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
        [EnumUtilities.Description("Last updated")]
        SortLastWatched,
        [EnumUtilities.Description("Start date")]
        SortStartDate,
        [EnumUtilities.Description("End Date")]
        SortEndDate,
        [EnumUtilities.Description("None")]
        SortNothing,
        [EnumUtilities.Description("Season")]
        SortSeason,
        [EnumUtilities.Description("Priority")]
        SortPriority
    }
}