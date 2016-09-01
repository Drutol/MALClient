using MALClient.Models.Enums.Enums;

namespace MALClient.XShared.Utils.Enums
{
    public enum ForumBoards
    {
        [EnumUtilities.Description("Updates & Announcements")]
        Updates = 5,
        [EnumUtilities.Description("MAL Guidelines & FAQ")]
        Guidelines = 14,
        [EnumUtilities.Description("Support")]
        Support = 3,
        [EnumUtilities.Description("Suggestions")]
        Suggestions = 4,
        [EnumUtilities.Description("MAL Contests")]
        Contests = 13,
        [EnumUtilities.Description("News Discussion")]
        NewsDisc = 15,
        [EnumUtilities.Description("Anime & Manga Recommendations")]
        Recomms = 16,
        [EnumUtilities.Description("Anime Series Discussion")]
        AnimeSeriesDisc = 101, //sub
        [EnumUtilities.Description("Manga Series Discussion")]
        MangaSeriesDisc = 104, //sub
        [EnumUtilities.Description("Anime Discussion")]
        AnimeDisc = 1,
        [EnumUtilities.Description("Manga Discussion")]
        MangaDisc = 2,
        [EnumUtilities.Description("Introductions")]
        Intro = 8,
        [EnumUtilities.Description("Games, Computers & Tech Support")]
        GamesTech = 7,
        [EnumUtilities.Description("Music & Entertainment")]
        Music = 10,
        [EnumUtilities.Description("Current Events")]
        Events = 6,
        [EnumUtilities.Description("Casual Discussion")]
        CasualDisc = 11,
        [EnumUtilities.Description("Creative Corner")]
        Creative = 12,
        [EnumUtilities.Description("Forum Games")]
        ForumsGames = 9,
    }
}     