using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MalClient.Shared.Comm.Anime;
using MalClient.Shared.Models.Anime;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.NavArgs
{
    public class AnimeListPageNavigationArgs
    {
        public int SelectedItemIndex = -1;
        public readonly int CurrPosition;
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

        public AnimeListPageNavigationArgs(SortOptions sort, int status, bool desc, int position,
            AnimeListWorkModes seasonal, string source, AnimeSeason season, AnimeListDisplayModes dispMode)
        {
            SortOption = sort;
            Status = status;
            Descending = desc;
            CurrPosition = position;
            WorkMode = seasonal;
            ListSource = source;
            NavArgs = true;
            CurrSeason = season;
            DisplayMode = dispMode;
        }

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

        public AnimeListPageNavigationArgs(int index, AnimeListWorkModes workMode)
        {
            WorkMode = workMode;
            StatusIndex = index;
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
