using MALClient.Models.Models.Anime;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils.Enums;

namespace MALClient.XShared.NavArgs
{
    public class AnimeListPageNavigationArgs
    {
        public int SelectedItemIndex = -1;
        public bool ResetBackNav = true;
        public readonly bool Descending;
        public readonly bool NavArgs;
        public readonly int Status;
        public readonly int? StatusIndex;
        public AnimeSeason CurrSeason;
        public AnimeListDisplayModes DisplayMode;
        public string ListSource;
        public SortOptions SortOption;
        public TopAnimeType TopWorkMode = TopAnimeType.General;
        public AnimeListWorkModes WorkMode = AnimeListWorkModes.Anime;

        public AnimeListPageNavigationArgs(SortOptions sort, int status, bool desc,
            AnimeListWorkModes seasonal, string source, AnimeSeason season, AnimeListDisplayModes dispMode)
        {
            SortOption = sort;
            Status = status;
            Descending = desc;
            WorkMode = seasonal;
            ListSource = source;
            NavArgs = true;
            CurrSeason = season;
            DisplayMode = dispMode;
        }

        private AnimeListPageNavigationArgs()
        {
        }

        public AnimeListPageNavigationArgs(int filterIndex, AnimeListWorkModes workMode)
        {
            WorkMode = workMode;
            StatusIndex = filterIndex;
        }

        public static AnimeListPageNavigationArgs Seasonal
            => new AnimeListPageNavigationArgs { WorkMode = AnimeListWorkModes.SeasonalAnime };

        public static AnimeListPageNavigationArgs Manga
            => new AnimeListPageNavigationArgs { WorkMode = AnimeListWorkModes.Manga };


        public static AnimeListPageNavigationArgs TopManga
            => new AnimeListPageNavigationArgs { WorkMode = AnimeListWorkModes.TopManga };

        public static AnimeListPageNavigationArgs TopAnime(TopAnimeType type) =>
            new AnimeListPageNavigationArgs { WorkMode = AnimeListWorkModes.TopAnime, TopWorkMode = type };
    }

    


}
