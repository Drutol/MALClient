using System;
using MALClient.Models;
using MALClient.ViewModels;

// ReSharper disable InconsistentNaming

namespace MALClient.Items
{
    /// <summary>
    ///     This class serves as a container for actual UI AnimeItem element.
    ///     It contains all values required to sort and filter those items before loading them.
    /// </summary>
    public class AnimeItemAbstraction
    {
        private readonly bool _firstConstructor;
        private readonly SeasonalAnimeData _seasonalData;
        public readonly bool Auth;
        public readonly bool RepresentsAnime = true;

        private AnimeItem _animeItem;
        private AnimeGridItem _gridItem;
        private AnimeItemViewModel _viewModel;
        public int AirDay = -1;
        public string AirStartDate;
        public float GlobalScore;

        public int Index;
        public bool LoadedAnime;
        public bool LoadedModel;
        public bool LoadedVolatile;

        //three constructors depending on original init
        private AnimeItemAbstraction(ILibraryData entry, int? id = null)
        {
            if (entry != null)
                EntryData = entry;
            VolatileDataCache data;
            if (!DataCache.TryRetrieveDataForId(id ?? Id, out data)) return;
            LoadedVolatile = true;
            AirDay = data.DayOfAiring;
            GlobalScore = data.GlobalScore;
            AirStartDate = data.AirStartDate;
        }

        //three constructors depending on original init
        public AnimeItemAbstraction(SeasonalAnimeData data, bool anime) : this(null, data.Id)
        {
            _seasonalData = data;
            Index = data.Index;
            RepresentsAnime = anime;
        }

        public AnimeItemAbstraction(bool auth, MangaLibraryItemData data) : this(data)
        {
            RepresentsAnime = false;
            Auth = auth;
            EntryData = data;
            _firstConstructor = true;
        }

        public AnimeItemAbstraction(bool auth, AnimeLibraryItemData data) : this(data)
        {
            Auth = auth;
            MyStartDate = data.MyStartDate;
            MyEndDate = data.MyEndDate;
            _firstConstructor = true;
        }

        public ILibraryData EntryData { get; set; }

        public string Title => EntryData?.Title ?? _seasonalData.Title;
        public int Id => EntryData?.Id ?? _seasonalData.Id;
        public int MalId => EntryData?.MalId ?? _seasonalData.Id;
        public string ImgUrl => EntryData?.ImgUrl ?? _seasonalData.ImgUrl;
        private int AllEpisodes => EntryData?.AllEpisodes ?? 0;
        public int AllVolumes => (EntryData as MangaLibraryItemData)?.AllVolumes ?? 0;
        public int Type => EntryData?.Type ?? 0;

        public DateTime LastWatched
        {
            get { return EntryData?.LastWatched ?? DateTime.MinValue; }
            set
            {
                if (EntryData != null)
                    EntryData.LastWatched = value;
            }
        }

        public int MyEpisodes
        {
            get { return EntryData?.MyEpisodes ?? 0; }
            set { EntryData.MyEpisodes = value; }
        }

        public float MyScore
        {
            get { return EntryData?.MyScore ?? 0; }
            set { EntryData.MyScore = value; }
        }

        public int MyStatus
        {
            get { return (int) (EntryData?.MyStatus ?? AnimeStatus.AllOrAiring); }
            set { EntryData.MyStatus = (AnimeStatus) value; }
        }

        public int MyVolumes
        {
            get { return (EntryData as MangaLibraryItemData)?.MyVolumes ?? 0; }
            set
            {
                if (EntryData is MangaLibraryItemData)
                    ((MangaLibraryItemData) EntryData).MyVolumes = value;
            }
        }

        public string MyStartDate { get; set; }
        public string MyEndDate { get; set; }


        public AnimeItemViewModel ViewModel
        {
            get
            {
                if (LoadedAnime)
                    return _viewModel;
                ViewModel = LoadElementModel();
                return _viewModel;
            }
            private set { _viewModel = value; }
        }

        public bool TryRetrieveVolatileData(bool force = false)
        {
            if (GlobalScore != 0 && !force)
                return true;
            VolatileDataCache data;
            if (!DataCache.TryRetrieveDataForId(Id, out data)) return false;
            AirDay = data.DayOfAiring;
            GlobalScore = data.GlobalScore;
            return true;
        }

        //Load UIElement
        private AnimeItemViewModel LoadElementModel()
        {
            if (LoadedModel)
                return _viewModel;
            LoadedModel = true;
            if (RepresentsAnime)
                return _firstConstructor
                    ? new AnimeItemViewModel(Auth, Title, ImgUrl, Id, MyStatus, MyEpisodes, AllEpisodes, MyScore,
                        MyStartDate, MyEndDate, this,
                        false)
                    : new AnimeItemViewModel(_seasonalData, this);
            return
                _firstConstructor
                    ? new AnimeItemViewModel(Auth, Title, ImgUrl, Id, MyStatus, MyEpisodes, AllEpisodes, MyScore,
                        MyStartDate, MyEndDate, this,
                        false, MyVolumes, AllVolumes)
                    : new AnimeItemViewModel(_seasonalData, this);
        }
    }
}