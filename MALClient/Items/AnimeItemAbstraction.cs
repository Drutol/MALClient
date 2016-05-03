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
        private readonly int allEps;
        private readonly int allVolumes;

        public readonly bool auth;

        private readonly SeasonalAnimeData data;

        private AnimeItem _animeItem;

        private AnimeGridItem _gridItem;
        private AnimeItemViewModel _viewModel;
        public int AirDay = -1;
        private bool authSetEps;
        public float GlobalScore;
        public string AirStartDate;
        private readonly int id;
        public int Id;
        public string img;
        public int Index;
        public bool LoadedAnime;

        public bool LoadedGrid;
        public bool LoadedModel;
        private readonly int myVolumes;

        public int MyVolumes;
        private readonly string name;

        public bool RepresentsAnime = true;
        public string Title;

        private AnimeItemAbstraction(int id)
        {
            Id = id;
            VolatileDataCache data;
            if (!DataCache.TryRetrieveDataForId(Id, out data)) return;
            AirDay = data.DayOfAiring;
            GlobalScore = data.GlobalScore;
            AirStartDate = data.AirStartDate;
        }

        //three constructors depending on original init
        public AnimeItemAbstraction(bool auth, string name, string img, int type, int id, int myStatus, int myEps,
            int allEps, string startDate, string endDate,
            int myScore) : this(id)
        {
            this.auth = auth;
            this.name = name;
            this.img = img;
            this.id = id;
            MyStartDate = startDate;
            MyEndDate = endDate;
            MyStatus = myStatus;
            MyEpisodes = myEps;
            this.allEps = allEps;
            MyScore = MyScore = myScore;
            Type = type;
            _firstConstructor = true;

            Title = name;
        }

        public AnimeItemAbstraction(SeasonalAnimeData data, bool anime)
            : this(data.Id)
        {
            this.data = data;
            img = data.ImgUrl;
            Title = data.Title;
            GlobalScore = data.Score;
            Index = data.Index;
            RepresentsAnime = anime;
            MyStatus = (int)AnimeStatus.AllOrAiring;
        }

        public AnimeItemAbstraction(bool auth, string name, string img, int type, int id, int myStatus, int myEps,
            int allEps, string startDate, string endDate,
            int myScore, int volumes, int allVolumes)
            : this(auth, name, img, type, id, myStatus, myEps, allEps, startDate, endDate, myScore)
        {
            RepresentsAnime = false;
            myVolumes = MyVolumes = volumes;
            this.allVolumes = allVolumes;
        }


        public int MyEpisodes { get; set; }

        public int MyScore { get; set; }

        public int MyStatus { get; set; }

        public int Type { get; set; }

        public string MyStartDate { get; set; }
        public string MyEndDate { get; set; }

        public AnimeItem AnimeItem
        {
            get
            {
                if (LoadedAnime)
                    return _animeItem;

                ViewModel = LoadElementModel();
                _animeItem = LoadElement();
                return _animeItem;
            }
        }

        public AnimeGridItem AnimeGridItem
        {
            get
            {
                if (LoadedGrid)
                    return _gridItem;

                ViewModel = LoadElementModel();
                _gridItem = new AnimeGridItem(_viewModel);
                LoadedGrid = true;
                return _gridItem;
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
                    ? new AnimeItemViewModel(auth, name, img, id, MyStatus, MyEpisodes, allEps, MyScore, MyStartDate, MyEndDate, this,
                        authSetEps)
                    : new AnimeItemViewModel(data, this);
            return
                _firstConstructor
                    ? new AnimeItemViewModel(auth, name, img, id, MyStatus, MyEpisodes, allEps, MyScore, MyStartDate, MyEndDate, this,
                        authSetEps, myVolumes, allVolumes)
                    : new AnimeItemViewModel(data, this);
        }

        private AnimeItem LoadElement()
        {
            LoadedAnime = true;
            return new AnimeItem(ViewModel);
        }
    }
}