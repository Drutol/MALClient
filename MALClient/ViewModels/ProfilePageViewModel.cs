using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Models;
using MALClient.Models.Favourites;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public class ProfilePageNavigationArgs
    {
        public int OuterPivotSelectedIndex { get; set; }
        public int InnerPivotSelectedIndex { get; set; }
        public string TargetName { get; set; }
    }

    public sealed class ProfilePageViewModel : ViewModelBase
    {
        private List<int> _animeChartValues = new List<int>();

        private int _currentlySelectedInnerPivotIndex;

        private PivotItem _currentlySelectedInnerPivotItem;

        private int _currentlySelectedOuterPivotIndex;


        private PivotItem _currentlySelectedOuterPivotItem;
        private bool _dataLoaded;


        private Visibility _emptyFavAnimeNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavCharactersNoticeVisibility = Visibility.Collapsed;
        private Visibility _emptyFavMangaNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyFavPeopleNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyRecentAnimeNoticeVisibility = Visibility.Collapsed;

        private Visibility _emptyRecentMangaNoticeVisibility = Visibility.Collapsed;
        private List<AnimeItemViewModel> _favAnime = new List<AnimeItemViewModel>();
        private List<AnimeItemViewModel> _favManga = new List<AnimeItemViewModel>();


        private bool _initialized;
        private bool _loadedFavAnime;
        private bool _loadedFavManga;
        private bool _loadedRecent;
        private bool _loadedStats;

        private Visibility _loadingVisibility = Visibility.Collapsed;

        private List<int> _mangaChartValues = new List<int>();

        private ICommand _navigateCharPageCommand;

        private ICommand _navigateDetailsCommand;

        private ICommand _navigatePersonPageCommand;

        private List<AnimeItemViewModel> _recentAnime;
        private List<AnimeItemViewModel> _recentManga;

        public ProfilePageViewModel()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            MaxWidth = bounds.Width/2.2;
        }

        //public ProfilePage View { get; set; }

        public ProfileData CurrentData { get; set; } = new ProfileData();

        public List<AnimeItemViewModel> RecentAnime
        {
            get { return _recentAnime; }
            private set
            {
                _recentAnime = value;
                RaisePropertyChanged(() => RecentAnime);
            }
        }

        public List<AnimeItemViewModel> RecentManga
        {
            get { return _recentManga; }
            private set
            {
                _recentManga = value;
                RaisePropertyChanged(() => RecentManga);
            }
        }

        public List<AnimeItemViewModel> FavAnime
        {
            get { return _favAnime; }
            private set
            {
                _favAnime = value;
                RaisePropertyChanged(() => FavAnime);
            }
        }

        public List<AnimeItemViewModel> FavManga
        {
            get { return _favManga; }
            private set
            {
                _favManga = value;
                RaisePropertyChanged(() => FavManga);
            }
        }


        public PivotItem CurrentlySelectedOuterPivotItem
        {
            get { return _currentlySelectedOuterPivotItem; }
            set
            {
                _currentlySelectedOuterPivotItem = value;
                RaisePropertyChanged(() => CurrentlySelectedOuterPivotItem);
                OuterPivotItemChanged(value.Tag as string);
            }
        }

        public PivotItem CurrentlySelectedInnerPivotItem
        {
            get { return _currentlySelectedInnerPivotItem; }
            set
            {
                _currentlySelectedInnerPivotItem = value;
                RaisePropertyChanged(() => CurrentlySelectedInnerPivotItem);
                InnerPivotItemChanged(value.Tag as string);
            }
        }

        public AnimeItemViewModel TemporarilySelectedAnimeItem
        {
            get { return null; }
            set
            {
                value?.NavigateDetails(PageIndex.PageProfile,
                    new ProfilePageNavigationArgs
                    {
                        InnerPivotSelectedIndex = CurrentlySelectedInnerPivotIndex,
                        OuterPivotSelectedIndex = CurrentlySelectedOuterPivotIndex
                    });
            }
        }

        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        public int CurrentlySelectedOuterPivotIndex
        {
            get { return _currentlySelectedOuterPivotIndex; }
            set
            {
                _currentlySelectedOuterPivotIndex = value;
                RaisePropertyChanged(() => CurrentlySelectedOuterPivotIndex);
            }
        }

        public int CurrentlySelectedInnerPivotIndex
        {
            get { return _currentlySelectedInnerPivotIndex; }
            set
            {
                _currentlySelectedInnerPivotIndex = value;
                RaisePropertyChanged(() => CurrentlySelectedInnerPivotIndex);
            }
        }

        public List<int> AnimeChartValues
        {
            get { return _animeChartValues; }
            set
            {
                _animeChartValues = value;
                RaisePropertyChanged(() => AnimeChartValues);
            }
        }

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

        public ICommand NavigateDetailsCommand
            => _navigateDetailsCommand ?? (_navigateDetailsCommand = new RelayCommand<FavCharacter>(NavigateDetails));

        public ICommand NavigateCharPageCommand
            =>
                _navigateCharPageCommand ??
                (_navigateCharPageCommand = new RelayCommand<FavCharacter>(NavigateCharacterWebPage));

        public ICommand NavigatePersonPageCommand
            =>
                _navigatePersonPageCommand ??
                (_navigatePersonPageCommand = new RelayCommand<FavPerson>(NavigatePersonWebPage));

        public Visibility EmptyFavAnimeNoticeVisibility
        {
            get { return _emptyFavAnimeNoticeVisibility; }
            set
            {
                _emptyFavAnimeNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavAnimeNoticeVisibility);
            }
        }

        public Visibility EmptyFavCharactersNoticeVisibility
        {
            get { return _emptyFavCharactersNoticeVisibility; }
            set
            {
                _emptyFavCharactersNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavCharactersNoticeVisibility);
            }
        }

        public Visibility EmptyFavMangaNoticeVisibility
        {
            get { return _emptyFavMangaNoticeVisibility; }
            set
            {
                _emptyFavMangaNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavMangaNoticeVisibility);
            }
        }

        public Visibility EmptyRecentMangaNoticeVisibility
        {
            get { return _emptyRecentMangaNoticeVisibility; }
            set
            {
                _emptyRecentMangaNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyRecentMangaNoticeVisibility);
            }
        }

        public Visibility EmptyRecentAnimeNoticeVisibility
        {
            get { return _emptyRecentAnimeNoticeVisibility; }
            set
            {
                _emptyRecentAnimeNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyRecentAnimeNoticeVisibility);
            }
        }

        public Visibility EmptyFavPeopleNoticeVisibility
        {
            get { return _emptyFavPeopleNoticeVisibility; }
            set
            {
                _emptyFavPeopleNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyFavPeopleNoticeVisibility);
            }
        }

        public async void LoadProfileData(ProfilePageNavigationArgs args, bool force = false)
        {
            if (!_dataLoaded || force)
            {
                LoadingVisibility = Visibility.Visible;
                Cleanup();
                await Task.Run(async () => CurrentData = await new ProfileQuery().GetProfileData(force));
                _dataLoaded = true;
            }
            RaisePropertyChanged(() => CurrentData);
            _initialized = true;
            CurrentlySelectedInnerPivotIndex = args?.InnerPivotSelectedIndex ?? 0;
            CurrentlySelectedOuterPivotIndex = args?.OuterPivotSelectedIndex ?? 1;
            OuterPivotItemChanged(CurrentlySelectedOuterPivotItem.Tag as string);
            LoadingVisibility = Visibility.Collapsed;
        }

        private async void InnerPivotItemChanged(string tag)
        {
            if (!_initialized)
                return;
            var list = new List<AnimeItemViewModel>();
            switch (tag)
            {
                case "Anime":
                    if (_loadedFavAnime)
                        break;
                    _loadedFavAnime = true;
                    foreach (var id in CurrentData.FavouriteAnime)
                    {
                        var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);
                        if (data != null)
                        {
                            FavAnime.Add(data as AnimeItemViewModel);
                        }
                    }
                    FavAnime = list;
                    EmptyFavAnimeNoticeVisibility = FavAnime.Count == 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
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
                            FavManga.Add(data as AnimeItemViewModel);
                        }
                    }
                    FavManga = list;
                    EmptyFavMangaNoticeVisibility = FavManga.Count == 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;
            }
        }

        private async void OuterPivotItemChanged(string tag)
        {
            if (!_initialized)
                return;
            var list = new List<AnimeItemViewModel>();
            switch (tag)
            {
                case "Favs":
                    InnerPivotItemChanged(CurrentlySelectedInnerPivotItem.Tag as string);
                    _loadedRecent = false;
                    break;
                case "Recent":
                    if (_loadedRecent)
                        break;
                    _loadedRecent = true;


                    foreach (var id in CurrentData.RecentAnime)
                    {
                        var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id);
                        if (data != null)
                        {
                            RecentAnime.Add(data as AnimeItemViewModel);
                        }
                    }
                    foreach (var id in CurrentData.RecentManga)
                    {
                        var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(id, false);
                        if (data != null)
                        {
                            RecentManga.Add(data as AnimeItemViewModel);
                        }
                    }
                    EmptyRecentAnimeNoticeVisibility = RecentAnime.Count == 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    EmptyRecentMangaNoticeVisibility = RecentManga.Count == 0
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;
                case "Stats":
                    if (_loadedStats)
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

        private void NavigateDetails(FavCharacter character)
        {
            ViewModelLocator.Main.Navigate(PageIndex.PageAnimeDetails,
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

    }
}