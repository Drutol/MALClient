using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
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
    public class ProfilePageNavigationArgs
    {
        public int OuterPivotSelectedIndex { get; set; }
        public int InnerPivotSelectedIndex { get; set; }
    }

    public sealed class ProfilePageViewModel : ViewModelBase
    {
        private bool _loadedFavManga;
        private bool _loadedFavAnime;
        private bool _loadedRecent;
        private bool _loadedStats;
        private bool _loadedChars;
        private bool _loadedPpl;

        //public ProfilePage View { get; set; }

        public ProfileData CurrentData { get; set; } = new ProfileData();
        private bool _dataLoaded;

        private ObservableCollection<AnimeItem> _recentAnime;
        private ObservableCollection<AnimeItem> _recentManga;
        private ObservableCollection<AnimeItem> _favManga;
        private ObservableCollection<AnimeItem> _favAnime;
        private ObservableCollection<FavCharacter> _favChars;
        private ObservableCollection<FavPerson> _favPpl;

        public ObservableCollection<AnimeItem> RecentAnime
        {
            get { return _recentAnime; }
            private set
            {
                _recentAnime = value;
                RaisePropertyChanged(() => RecentAnime);
            }
        }

        public ObservableCollection<AnimeItem> RecentManga
        {
            get { return _recentManga; }
            private set
            {
                _recentManga = value;
                RaisePropertyChanged(() => RecentManga);
            }
        }

        public ObservableCollection<AnimeItem> FavAnime
        {
            get { return _favAnime; }
            private set
            {
                _favAnime = value;
                RaisePropertyChanged(() => FavAnime);
            }
        }

        public ObservableCollection<AnimeItem> FavManga
        {
            get { return _favManga; }
            private set
            {
                _favManga = value;
                RaisePropertyChanged(() => FavManga);
            }
        }

        public ObservableCollection<FavCharacter> FavCharacters
        {
            get { return _favChars; }
            private set
            {
                _favChars = value;
                RaisePropertyChanged(() => FavCharacters);
            }
        }

        public ObservableCollection<FavPerson> FavPeople
        {
            get { return _favPpl; }
            private set
            {
                _favPpl = value;
                RaisePropertyChanged(() => FavPeople);
            }
        }



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

        public AnimeItem TemporarilySelectedAnimeItem
        {
            get { return null; }
            set
            {
                value?.ViewModel.NavigateDetails(PageIndex.PageProfile,
                    new ProfilePageNavigationArgs
                    {
                        InnerPivotSelectedIndex = CurrentlySelectedInnerPivotIndex,
                        OuterPivotSelectedIndex = CurrentlySelectedOuterPivotIndex
                    });
            }
        }

        private Visibility _loadingVisibility = Visibility.Collapsed;

        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        private int _currentlySelectedOuterPivotIndex = 0;

        public int CurrentlySelectedOuterPivotIndex
        {
            get { return _currentlySelectedOuterPivotIndex; }
            set
            {
                _currentlySelectedOuterPivotIndex = value;
                RaisePropertyChanged(() => CurrentlySelectedOuterPivotIndex);
            }
        }

        private int _currentlySelectedInnerPivotIndex = 0;

        public int CurrentlySelectedInnerPivotIndex
        {
            get { return _currentlySelectedInnerPivotIndex; }
            set
            {
                _currentlySelectedInnerPivotIndex = value;
                RaisePropertyChanged(() => CurrentlySelectedInnerPivotIndex);
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
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            MaxWidth = bounds.Width / 2.2;
        }

        public async void LoadProfileData(ProfilePageNavigationArgs args,bool force = false)
        {        
            if (!_dataLoaded || force)
            {
                LoadingVisibility = Visibility.Visible;
                Cleanup();
                await Task.Run(async () => CurrentData = await new MalProfileQuery().GetProfileData(force));
                _dataLoaded = true;
            }
            RaisePropertyChanged(() => CurrentData);
            _initialized = true;
            CurrentlySelectedInnerPivotIndex = args?.InnerPivotSelectedIndex ?? 0;
            CurrentlySelectedOuterPivotIndex = args?.OuterPivotSelectedIndex ?? 0;
            OuterPivotItemChanged(CurrentlySelectedOuterPivotItem.Tag as string);
            InnerPivotItemChanged(CurrentlySelectedInnerPivotItem.Tag as string);
            LoadingVisibility = Visibility.Collapsed;
        }

        private async void InnerPivotItemChanged(string tag)
        {
            if(!_initialized)
                return;
            switch (tag)
            {
                case "Chars":
                    if(_loadedChars)
                        return;
                    _loadedChars = true;
                    FavCharacters = new ObservableCollection<FavCharacter>();
                    await Task.Delay(10);
                    foreach (var favCharacter in CurrentData.FavouriteCharacters)
                    {
                        favCharacter.LoadBitmap();
                        FavCharacters.Add(favCharacter);
                    }
                    break;
                case "Anime":
                    if (_loadedFavAnime)
                        break;
                    _loadedFavAnime = true;
                    FavAnime = new ObservableCollection<AnimeItem>();
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
                    FavManga = new ObservableCollection<AnimeItem>();
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
                    if (_loadedPpl)
                        return;
                    _loadedPpl = true;                 
                    FavPeople = new ObservableCollection<FavPerson>();
                    await Task.Delay(10);
                    foreach (var favPerson in CurrentData.FavouritePeople)
                    {
                        favPerson.LoadBitmap();
                        FavPeople.Add(favPerson);
                    }
                    break;
            }
        }

        private async void OuterPivotItemChanged(string tag)
        {
            if (!_initialized)
                return;
            switch (tag)
            {
                case "Favs":
                    RecentManga = new ObservableCollection<AnimeItem>();
                    RecentAnime = new ObservableCollection<AnimeItem>();
                    InnerPivotItemChanged(CurrentlySelectedInnerPivotItem.Tag as string);
                    _loadedRecent = false;
                    break;
                case "Recent":
                    if (_loadedRecent)
                        break;
                    _loadedRecent = true;
                    //in case of duplicate we have to clear this
                    FavAnime = new ObservableCollection<AnimeItem>();
                    FavManga = new ObservableCollection<AnimeItem>();
                    _loadedFavManga = false;
                    _loadedFavAnime = false;
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
            FavAnime = new ObservableCollection<AnimeItem>();
            FavManga = new ObservableCollection<AnimeItem>();
            RecentAnime = new ObservableCollection<AnimeItem>();
            RecentManga = new ObservableCollection<AnimeItem>();
            FavCharacters = new ObservableCollection<FavCharacter>();
            FavPeople = new ObservableCollection<FavPerson>();
            _loadedFavManga = false;
            _loadedFavAnime = false;
            _loadedRecent = false;
            _loadedStats = false;
            _loadedChars = false;
            await ViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails,
                new AnimeDetailsPageNavigationArgs(int.Parse(character.ShowId), character.OriginatingShowName, null,
                    null,
                    new ProfilePageNavigationArgs
                    {
                        InnerPivotSelectedIndex = CurrentlySelectedInnerPivotIndex,
                        OuterPivotSelectedIndex = CurrentlySelectedOuterPivotIndex
                    })
                {
                    Source = PageIndex.PageProfile,
                    AnimeMode = character.FromAnime
                });

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
            FavCharacters = new ObservableCollection<FavCharacter>();
            FavPeople = new ObservableCollection<FavPerson>();
            _loadedFavManga = false;
            _loadedFavAnime = false;
            _loadedRecent = false;
            _loadedStats = false;
            _loadedChars = false;
            _loadedPpl = false;
            _initialized = false;
            base.Cleanup();
        }
    }
}
