using System;
using System.Collections.Generic;
using System.Linq;
using MalClient.Shared.Models.AnimeScrapped;
using MalClient.Shared.Models.Library;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;

// ReSharper disable InconsistentNaming

namespace MalClient.Shared.ViewModels
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
            {
                EntryData = entry;
                MyStartDate = entry.MyStartDate;
                MyEndDate = entry.MyEndDate;
            }

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
            _firstConstructor = true;
        }

        public AnimeItemAbstraction(bool auth, AnimeLibraryItemData data) : this(data)
        {
            Auth = auth;
            _firstConstructor = true;
        }

        public ILibraryData EntryData { get; set; }

        public string Title => EntryData?.Title ?? _seasonalData.Title;
        public int Id => EntryData?.Id ?? _seasonalData.Id;
        public int MalId => EntryData?.MalId ?? _seasonalData.Id;
        public string ImgUrl => EntryData?.ImgUrl ?? _seasonalData.ImgUrl;
        public int AllEpisodes => EntryData?.AllEpisodes ?? 0;
        public int AllVolumes => (EntryData as MangaLibraryItemData)?.AllVolumes ?? 0;
        public int Type => EntryData?.Type ?? 0;

        public string Notes
        {
            get { return EntryData?.Notes ?? ""; }
            set
            {
                if (EntryData != null)
                {
                    EntryData.Notes = value;
                    _tags = null;
                }
            }
        }

        private List<string> _tags;

        public List<string> Tags => _tags ?? (_tags = string.IsNullOrEmpty(Notes)
            ? new List<string>()
            : Notes.Contains(",")
                ? Notes.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.ToLower()).ToList()
                : new List<string> {Notes.ToLower()});

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
                    ? new AnimeItemViewModel(Auth, Title, ImgUrl, Id, AllEpisodes, this, false)
                    : new AnimeItemViewModel(_seasonalData, this);
            return
                _firstConstructor
                    ? new AnimeItemViewModel(Auth, Title, ImgUrl, Id, AllEpisodes, this, false, AllVolumes)
                    : new AnimeItemViewModel(_seasonalData, this);
        }
    }
}