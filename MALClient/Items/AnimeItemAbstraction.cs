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
        private readonly int AllEpisodes;
        public readonly int AllVolumes;

        public readonly bool auth;

        private readonly SeasonalAnimeData data;

        private AnimeItem _animeItem;
        private AnimeGridItem _gridItem;
        private AnimeItemViewModel _viewModel;
        private AnimeCompactItem _compactItem;

        public string Title;
        public int MyVolumes;
        public int AirDay = -1;
        public float GlobalScore;
        public string AirStartDate;
        private int _id = -1; 
        public int Id { get { return _id == -1 ? MalId : _id; } set { _id = value; } }
        public int MalId;
        public string ImgUrl;
        public int Index;
        public bool LoadedAnime;
        public bool LoadedGrid;
        public bool LoadedCompact;
        public bool LoadedModel;
        private readonly int myVolumes;


        public bool RepresentsAnime = true;


        private AnimeItemAbstraction(int id,int malId)
        {
            Id = id;
            MalId = malId;
            VolatileDataCache data;
            if (!DataCache.TryRetrieveDataForId(Id, out data)) return;
            AirDay = data.DayOfAiring;
            GlobalScore = data.GlobalScore;
            AirStartDate = data.AirStartDate;
        }

        //three constructors depending on original init

        public AnimeItemAbstraction(SeasonalAnimeData data, bool anime)
            : this(-1,data.Id)
        {
            this.data = data;
            ImgUrl = data.ImgUrl;
            Title = data.Title;
            GlobalScore = data.Score;
            Index = data.Index;
            RepresentsAnime = anime;
            MyStatus = (int) AnimeStatus.AllOrAiring;
        }

        public AnimeItemAbstraction(bool auth, MangaLibraryItemData data) : this(auth,data as AnimeLibraryItemData)
        {
            RepresentsAnime = false;
            MyVolumes = data.MyVolumes;
            AllVolumes = data.AllVolumes;
        }

        public AnimeItemAbstraction(bool auth, AnimeLibraryItemData data) : this(data.Id,data.MalId)
        {
            this.auth = auth;
            Title = data.Title;
            ImgUrl = data.ImgUrl;
            MyStartDate = data.MyStartDate;
            MyEndDate = data.MyEndDate;
            MyStatus = (int)data.MyStatus;
            MyEpisodes = data.MyEpisodes;
            AllEpisodes = data.AllEpisodes;
            MyScore = data.MyScore;
            Type = data.Type;
            _firstConstructor = true;           
        }


        public int MyEpisodes { get; set; }

        public float MyScore { get; set; }

        public int MyStatus { get; set; }

        public int Type { get; set; }

        public string MyStartDate { get; set; }
        public string MyEndDate { get; set; }

        public AnimeItem AnimeItem
        {
            get
            {
                if(LoadedGrid)
                    _gridItem.ClearImage();
                //if(LoadedCompact)

                if (LoadedAnime)
                {
                    _animeItem.BindImage();
                    return _animeItem;
                }

                ViewModel = LoadElementModel();
                _animeItem = LoadElement(); // sets loaded flag
                _animeItem.BindImage();
                return _animeItem;
            }
        }

        public AnimeGridItem AnimeGridItem
        {
            get
            {
                if(LoadedAnime)
                    _animeItem.ClearImage();

                if (LoadedGrid)
                {
                    _gridItem.BindImage();
                    return _gridItem;
                }

                ViewModel = LoadElementModel();
                _gridItem = new AnimeGridItem(_viewModel);
                _gridItem.BindImage();
                LoadedGrid = true;
                return _gridItem;
            }
        }

        public AnimeCompactItem AnimeCompactItem
        {
            get
            {
                if (LoadedCompact)
                    return _compactItem;

                ViewModel = LoadElementModel();
                _compactItem = new AnimeCompactItem(_viewModel);
                LoadedCompact = true;
                return _compactItem;
            }
        }

        public AnimeItemViewModel ViewModel
        {
            get
            {
                if (LoadedAnime)
                    return _viewModel;
                ViewModel = LoadElementModel();
                _animeItem = LoadElement();
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
                    ? new AnimeItemViewModel(auth, Title, ImgUrl, Id, MyStatus, MyEpisodes, AllEpisodes, MyScore, MyStartDate, MyEndDate, this,
                        false)
                    : new AnimeItemViewModel(data, this);
            return
                _firstConstructor
                    ? new AnimeItemViewModel(auth, Title, ImgUrl, Id, MyStatus, MyEpisodes, AllEpisodes, MyScore, MyStartDate, MyEndDate, this,
                        false, myVolumes, AllVolumes)
                    : new AnimeItemViewModel(data, this);
        }

        private AnimeItem LoadElement()
        {
            LoadedAnime = true;
            return new AnimeItem(ViewModel);
        }

        public static AnimeLibraryItemData ToLibraryItem(AnimeItemAbstraction source)
        {
            return source.RepresentsAnime ? 
                new AnimeLibraryItemData
                {
                    Id = source.Id,
                    MalId = source.MalId,
                    Title = source.Title,
                    MyStatus = (AnimeStatus)source.MyStatus,
                    MyEpisodes = source.MyEpisodes,
                    AllEpisodes = source.AllEpisodes,
                    ImgUrl = source.ImgUrl,
                    Type = source.Type,
                    MyStartDate = source.MyStartDate,
                    MyEndDate = source.MyEndDate,
                    MyScore = source.MyScore                   
                } : 
                new MangaLibraryItemData();
        }
    }
}