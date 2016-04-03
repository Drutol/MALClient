using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.ViewManagement;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Models;
using MALClient.Models.Favourites;
using MALClient.Pages;

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

        public ProfilePageViewModel()
        {

            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            //var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            MaxWidth = bounds.Width / 2.2;
        }

        public static double MaxWidth { get; set; }

        private ICommand _navigateDetailsCommand;

        public ICommand NavigateDetailsCommand
            => _navigateDetailsCommand ?? (_navigateDetailsCommand = new RelayCommand<FavCharacter>(NavigateDetails));

        private ICommand _navigateCharPageCommand;

        public ICommand NavigateCharPageCommand
            => _navigateCharPageCommand ?? (_navigateCharPageCommand = new RelayCommand<FavCharacter>(NavigateCharacterWebPage));

        public async void LoadProfileData()
        {
            await Task.Run(async () => CurrentData = await new MalProfileQuery().GetProfileData(false));
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

        private async void NavigateDetails(FavCharacter character)
        {
            await ViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(int.Parse(character.ShowId), character.OriginatingShowName, null,
                    null, null) {Source = PageIndex.PageProfile , AnimeMode = character.FromAnime});
            
        }

        private void NavigateCharacterWebPage(FavCharacter character)
        {
            throw new NotImplementedException();
        }

    }
}
