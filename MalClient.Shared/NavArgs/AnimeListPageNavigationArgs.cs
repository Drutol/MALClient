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
        public readonly bool Descending;
        public string ListSource;
        public readonly bool NavArgs;
        public int Status;
        public AnimeSeason CurrSeason;
        public AnimeListDisplayModes DisplayMode;
        public SortOptions SortOption;
        public TopAnimeType TopWorkMode;
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

        public AnimeListPageNavigationArgs(int status, AnimeListWorkModes mode)
        {
            WorkMode = mode;
            Status = status;
        }

        public static AnimeListPageNavigationArgs Seasonal
            => new AnimeListPageNavigationArgs { WorkMode = AnimeListWorkModes.SeasonalAnime };

        public static AnimeListPageNavigationArgs Manga
            => new AnimeListPageNavigationArgs { WorkMode = AnimeListWorkModes.Manga };

        public static AnimeListPageNavigationArgs TopAnime(TopAnimeType type) =>
         new AnimeListPageNavigationArgs { WorkMode = AnimeListWorkModes.TopAnime, TopWorkMode = type };



        public static AnimeListPageNavigationArgs TopManga
            => new AnimeListPageNavigationArgs { WorkMode = AnimeListWorkModes.TopManga };
    }
}
