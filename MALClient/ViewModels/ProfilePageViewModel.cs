using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Models;

namespace MALClient.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        public ProfileData CurrentData { get; set; } = new ProfileData();
        public ObservableCollection<AnimeItem> RecentAnime { get; set; } = new ObservableCollection<AnimeItem>(); 
        public ObservableCollection<AnimeItem> RecentManga { get; set; } = new ObservableCollection<AnimeItem>(); 
        
        private List<int> _animeChartValues = new List<int>();

        public List<int> AnimeChartValues
        {
            get { return _animeChartValues; }
            set
            {
                _animeChartValues = value;
                RaisePropertyChanged(() => AnimeChartValues);
            }
        }        

        private List<int> _mangaChartValues = new List<int>();

        public List<int> MangaChartValues
        {
            get { return _mangaChartValues; }
            set
            {
                _mangaChartValues = value;
                RaisePropertyChanged(() => MangaChartValues);
            }
        }

        public async void LoadProfileData()
        {
            CurrentData = await new MALProfileQuery().GetProfileData(false);
            RaisePropertyChanged(() => CurrentData);
            foreach (var id in CurrentData.RecentAnime)
            {
                var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);
                if (data != null)
                {
                    RecentAnime.Add((data as AnimeItemViewModel)._parentAbstraction.AnimeItem);
                }
            }
            foreach (var id in CurrentData.RecentManga)
            {
                var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id,false);
                if (data != null)
                {
                    RecentManga.Add((data as AnimeItemViewModel)._parentAbstraction.AnimeItem);
                }
            }
            AnimeChartValues = new List<int>
            {
                CurrentData.AnimeWatching,
                CurrentData.AnimeCompleted,
                CurrentData.AnimeOnHold,
                CurrentData.AnimeDropped,
                CurrentData.AnimePlanned
            };
            MangaChartValues = new List<int>
            {
                CurrentData.MangaReading,
                CurrentData.MangaCompleted,
                CurrentData.MangaOnHold,
                CurrentData.MangaDropped,
                CurrentData.MangaPlanned
            };
        }

    } 
}
