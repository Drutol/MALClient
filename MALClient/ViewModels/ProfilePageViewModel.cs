using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Models;
using MALClient.Models.Favourites;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public sealed class ProfilePageViewModel : ViewModelBase
    {
        private bool _loadedFavManga;
        private bool _loadedFavAnime;
        private bool _loadedRecent;
        private bool _loadedStats;

        //public ProfilePage View { get; set; }

        public ProfileData CurrentData { get; set; } = new ProfileData();
        public ObservableCollection<AnimeItem> RecentAnime { get; private set; }
        public ObservableCollection<AnimeItem> RecentManga { get; private set; }
        public ObservableCollection<AnimeItem> FavAnime { get; private set; } 
        public ObservableCollection<AnimeItem> FavManga { get; private set; }

        private PivotItem _currentlySelectedOuterPivotItem;

        public PivotItem CurrentlySelectedOuterPivotItem
        {
            get { return _currentlySelectedOuterPivotItem; }
            set
            {
                _currentlySelectedOuterPivotItem = value;
                //RaisePropertyChanged(() => CurrentlySelectedOuterPivotItem);
                OuterPivotItemChanged(value.Tag as string);
            }
        }

        private PivotItem _currentlySelectedInnerPivotItem;

        public PivotItem CurrentlySelectedInnerPivotItem
        {
            get { return _currentlySelectedInnerPivotItem; }
            set
            {
                _currentlySelectedInnerPivotItem = value;
                //RaisePropertyChanged(() => CurrentlySelectedInnerPivotItem);
                InnerPivotItemChanged(value.Tag as string);
            }
        }

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

        public static double MaxWidth { get; set; }

        private ICommand _navigateDetailsCommand;

        public ICommand NavigateDetailsCommand
            => _navigateDetailsCommand ?? (_navigateDetailsCommand = new RelayCommand<FavCharacter>(NavigateDetails));

        private ICommand _navigateCharPageCommand;

        public ICommand NavigateCharPageCommand
            => _navigateCharPageCommand ?? (_navigateCharPageCommand = new RelayCommand<FavCharacter>(NavigateCharacterWebPage));

        private ICommand _navigatePersonPageCommand;

        public ICommand NavigatePersonPageCommand
            => _navigatePersonPageCommand ?? (_navigatePersonPageCommand = new RelayCommand<FavPerson>(NavigatePersonWebPage));



        private bool _initialized;

        public ProfilePageViewModel()
        {
            FavAnime = new ObservableCollection<AnimeItem>();
            FavManga = new ObservableCollection<AnimeItem>();
            RecentAnime = new ObservableCollection<AnimeItem>();
            RecentManga = new ObservableCollection<AnimeItem>();
            RaisePropertyChanged(() => FavAnime);       
            RaisePropertyChanged(() => FavManga);       
            RaisePropertyChanged(() => RecentAnime);       
            RaisePropertyChanged(() => RecentManga);       
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            //var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            MaxWidth = bounds.Width / 2.2;
        }

        public async void LoadProfileData()
        {
            await Task.Run(async () => CurrentData = await new MalProfileQuery().GetProfileData(false));
            RaisePropertyChanged(() => CurrentData);
            _initialized = true;
            InnerPivotItemChanged(CurrentlySelectedInnerPivotItem.Tag as string);
        }

        private async void InnerPivotItemChanged(string tag)
        {
            if(!_initialized)
                return;
            switch (tag)
            {
                case "Chars":
                    break;
                case "Anime":
                    if (_loadedFavAnime)
                        break;
                    _loadedFavAnime = true;
                    foreach (var id in CurrentData.FavouriteAnime)
                    {
                        var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);
                        if (data != null)
                        {
                            FavAnime.Add((data as AnimeItemViewModel)._parentAbstraction.AnimeItem);
                        }
                    }
                    break;
                case "Manga":
                    if (_loadedFavManga)
                        break;
                    _loadedFavManga = true;
                    foreach (var id in CurrentData.FavouriteManga)
                    {
                        var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                        if (data != null)
                        {
                            FavManga.Add((data as AnimeItemViewModel)._parentAbstraction.AnimeItem);
                        }
                    }
                    break;
                case "Ppl":
                    break;
            }
        }

        private async void OuterPivotItemChanged(string tag)
        {
            if (!_initialized)
                return;
            switch (tag)
            {
                case "Recent":
                    if (_loadedRecent)
                        break;
                    _loadedRecent = true;
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
                        var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                        if (data != null)
                        {
                            RecentManga.Add((data as AnimeItemViewModel)._parentAbstraction.AnimeItem);
                        }
                    }
                    break;
                case "Stats":
                    if(_loadedStats)
                        return;
                    _loadedStats = true;
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
                    break;
            }
        }

        private async void NavigateDetails(FavCharacter character)
        {
            await ViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(int.Parse(character.ShowId), character.OriginatingShowName, null,
                    null, null) {Source = PageIndex.PageProfile , AnimeMode = character.FromAnime});
            
        }

        private async void NavigateCharacterWebPage(FavCharacter character)
        {
            await Launcher.LaunchUriAsync(new Uri($"http://myanimelist.net/character/{character.Id}"));
        }

        private async void NavigatePersonWebPage(FavPerson person)
        {
            await Launcher.LaunchUriAsync(new Uri($"http://myanimelist.net/people/{person.Id}"));
        }

        public override void Cleanup()
        {
            FavAnime = new ObservableCollection<AnimeItem>();
            FavManga = new ObservableCollection<AnimeItem>();
            RecentAnime = new ObservableCollection<AnimeItem>();
            RecentManga = new ObservableCollection<AnimeItem>();
            _loadedFavManga = false;
            _loadedFavAnime = false;
            _loadedRecent = false;
            _loadedStats = false;
            _initialized = false;
            base.Cleanup();
        }
    }
}
