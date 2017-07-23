namespace MALClient.Models.Enums
{
    public enum PageIndex
    {
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageAnimeList,
        [EnumUtilities.PageIndexEnumMember(OffPage = true,RequiresSyncBlock = true)]
        PageAnimeDetails,
        [EnumUtilities.PageIndexEnumMember(OffPage = true)]
        PageSettings,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageSearch,
        [EnumUtilities.PageIndexEnumMember(OffPage = true)]
        PageLogIn,
        [EnumUtilities.PageIndexEnumMember(OffPage = false, RequiresSyncBlock = true)]
        PageProfile,
        [EnumUtilities.PageIndexEnumMember(OffPage = true)]
        PageAbout,
        [EnumUtilities.PageIndexEnumMember(OffPage = false,RequiresSyncBlock = true)]
        PageRecomendations,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageSeasonal,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]// it's the same as AnimeList
        PageMangaList,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageMangaSearch,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageTopAnime,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageTopManga,
        [EnumUtilities.PageIndexEnumMember(OffPage = false,RequiresSyncBlock = true)]
        PageCalendar,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageArticles,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageNews,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageMessanging,
        [EnumUtilities.PageIndexEnumMember(OffPage = true)]
        PageMessageDetails,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageForumIndex,
        [EnumUtilities.PageIndexEnumMember(OffPage = false, RequiresSyncBlock = true)]
        PageHistory,
        [EnumUtilities.PageIndexEnumMember(OffPage = true)]
        PageCharacterDetails,
        [EnumUtilities.PageIndexEnumMember(OffPage = true)]
        PageStaffDetails,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageCharacterSearch,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageWallpapers,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PagePopularVideos,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageFeeds,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageNotificationHub,     
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageListComparison,
        [EnumUtilities.PageIndexEnumMember(OffPage = false)]
        PageFriends
    }
}