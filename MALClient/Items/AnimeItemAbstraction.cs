using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
// ReSharper disable InconsistentNaming

namespace MALClient.Items
{
    /// <summary>
    /// This class serves as a container for actual UI AnimeItem element.
    /// It contains all values required to sort and filter those items before loading them.
    /// </summary>
    public class AnimeItemAbstraction
    {
        private bool _loaded = false;
        public int MyStatus;
        public int MyScore;
        public int MyEpisodes;
        public string Title;
        public float GlobalScore;
        public int Id;
        public int _allEpisodes;
        public int AirDay = -1;
        public int Index = 0;

        public AnimeItem AnimeItem
        {
            get
            {
                if (_loaded)
                    return _animeItem;

                _animeItem = LoadElement();
                return _animeItem;
            }
        }

        private AnimeItem _animeItem = null;

        //Data from constructors
        private bool _firstConstructor = false;
        //1st
        private bool auth;
        private string name,img;
        private int id,myStatus,myEps,allEps,myScore;
        //2nd
        private SeasonalAnimeData data;
        private Dictionary<int, XElement> dl;
        private Dictionary<int, AnimeItemAbstraction> loaded;

        private AnimeItemAbstraction(int id)
        {
            Id = id;
            VolatileDataCache data;
            if (!DataCache.TryRetrieveDataForId(Id, out data)) return;
            AirDay = data.DayOfAiring;
            GlobalScore = data.GlobalScore;
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

        //Two constructors depending on original init
        public AnimeItemAbstraction(bool auth, string name, string img, int id, int myStatus, int myEps, int allEps, int myScore) : this(id)
        {
            this.auth = auth;
            this.name = name;
            this.img = img;
            this.id = id;
            this.myStatus =  MyStatus = myStatus;
            this.myEps = MyEpisodes = myEps;
            this.allEps = allEps;
            this.myScore = MyScore = myScore;
            _firstConstructor = true;

            Title = name;

        }

        public AnimeItemAbstraction(SeasonalAnimeData data, Dictionary<int, XElement> dl,Dictionary<int, AnimeItemAbstraction> loaded) : this(data.Id)
        {
            this.data = data;
            this.dl = dl;
            this.loaded = loaded;

            Title = data.Title;
            GlobalScore = data.Score;
            Index = data.Index;

            MyStatus = (int) AnimeStatus.AllOrAiring;
            
        }
        //Load UIElement
        private AnimeItem LoadElement()
        {
            _loaded = true;
            if(_firstConstructor)
                return new AnimeItem(auth,name,img,id,myStatus,myEps,allEps,myScore,this);

            return new AnimeItem(data,dl,loaded,this);
        }
    }
}
