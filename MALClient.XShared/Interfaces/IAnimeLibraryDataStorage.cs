using System;
using System.Collections.Generic;
using MALClient.XShared.Delegates;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Interfaces
{
    public interface IAnimeLibraryDataStorage
    {
        event AnimeAbstractionTransaction AnimeRemoved;

        /// <summary>
        /// All loaded anime not necessarily bound to any user
        /// </summary>
        List<AnimeItemAbstraction> AllLoadedAnimeItemAbstractions { get; set; }

        /// <summary>
        /// All loaded manga not necessarily bound to any user
        /// </summary>
        List<AnimeItemAbstraction> AllLoadedMangaItemAbstractions { get; set; }

        /// <summary>
        /// Per user set of anime and manga
        /// </summary>
        Dictionary<string, Tuple<List<AnimeItemAbstraction>, List<AnimeItemAbstraction>>> OthersAbstractions { get; }

        List<AnimeItemAbstraction> AllLoadedAuthAnimeItems { get; set; }
        List<AnimeItemAbstraction> AllLoadedAuthMangaItems { get; set; }
        List<AnimeItemAbstraction> AllLoadedSeasonalAnimeItems { get; set; }
        List<AnimeItemAbstraction> AllLoadedSeasonalMangaItems { get; set; }

        /// <summary>
        /// Add new anime to auth storage
        /// </summary>
        /// <param name="parentAbstraction"></param>
        void AddAnimeEntry(AnimeItemAbstraction parentAbstraction);

        /// <summary>
        /// Remove anime entry from auth storage
        /// </summary>
        /// <param name="parentAbstraction"></param>
        void RemoveAnimeEntry(AnimeItemAbstraction parentAbstraction);


    }
}
