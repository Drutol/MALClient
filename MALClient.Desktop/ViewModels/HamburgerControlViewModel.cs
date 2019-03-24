using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.UWP.Shared;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using Microsoft.Services.Store.Engagement;

namespace MALClient.UWP.ViewModels
{
    public class HamburgerControlViewModel : ViewModelBase , IHamburgerViewModel
    {
        private Visibility _adLoadingSpinnerVisibility = Visibility.Collapsed;
        private ICommand _animeFiltersFlyoutCommand;


        private Thickness _bottomStackPanelMargin = new Thickness(0);

        private ICommand _buttonNavigationCommand;
        private ICommand _buttonNavigationTopAnimeCommand;


        private bool? _prevState;

        private bool _profileButtonVisibility;
        private ICommand _topCategoriesFiltersFlyoutCommand;
        //base value , we are either on log in page or list page (app bar on/off)

        private ImageSource _userImage;

        private Visibility _usrImgPlaceholderVisibility = Visibility.Collapsed;


        public HamburgerControlViewModel()
        {
            ResetActiveButton();
            ResetTopCategoryButtons();
            SetActiveButton(Credentials.Authenticated
                ? (Settings.DefaultMenuTab == "anime" ? HamburgerButtons.AnimeList : HamburgerButtons.MangaList)
                : HamburgerButtons.LogIn);
        }

        public bool HamburgerExpanded { get; set; }

        private Color RequestedFontColor
            => Application.Current.RequestedTheme == ApplicationTheme.Dark ? Colors.FloralWhite : Colors.Black;

        public Dictionary<string, Brush> TxtForegroundBrushes { get; } = new Dictionary<string, Brush>();
        public Dictionary<string, Brush> TopCategoriesForegorundBrushes { get; } = new Dictionary<string, Brush>();

        public Dictionary<string, Thickness> TxtBorderBrushThicknesses { get; } = new Dictionary<string, Thickness>();

        public ObservableCollection<Tuple<AnimeStatus, string>> AnimeListFilters { get; set; } =
            new ObservableCollection<Tuple<AnimeStatus, string>>
            {
                new Tuple<AnimeStatus, string>(AnimeStatus.Watching, "Watching"),
                new Tuple<AnimeStatus, string>(AnimeStatus.Completed, "Completed"),
                new Tuple<AnimeStatus, string>(AnimeStatus.OnHold, "On Hold"),
                new Tuple<AnimeStatus, string>(AnimeStatus.Dropped, "Dropped"),
                new Tuple<AnimeStatus, string>(AnimeStatus.PlanToWatch, "Plan to watch"),
                new Tuple<AnimeStatus, string>(AnimeStatus.AllOrAiring, "All")
            };

        public ObservableCollection<Tuple<AnimeStatus, string>> MangaListFilters { get; set; }
            = new ObservableCollection<Tuple<AnimeStatus, string>>
            {
                new Tuple<AnimeStatus, string>(AnimeStatus.Watching, "Reading"),
                new Tuple<AnimeStatus, string>(AnimeStatus.Completed, "Completed"),
                new Tuple<AnimeStatus, string>(AnimeStatus.OnHold, "On Hold"),
                new Tuple<AnimeStatus, string>(AnimeStatus.Dropped, "Dropped"),
                new Tuple<AnimeStatus, string>(AnimeStatus.PlanToWatch, "Plan to read"),
                new Tuple<AnimeStatus, string>(AnimeStatus.AllOrAiring, "All")
            };

        public int CurrentAnimeFiltersSelectedIndex
        {
            get
            {
                return DesktopViewModelLocator.Main.CurrentMainPage != PageIndex.PageAnimeList ||
                       ViewModelLocator.AnimeList.WorkMode == AnimeListWorkModes.Manga
                    ? -1
                    : ViewModelLocator.AnimeList.StatusSelectorSelectedIndex;
            }
            set
            {
                if (DesktopViewModelLocator.Main.CurrentMainPage != PageIndex.PageAnimeList ||
                    ViewModelLocator.AnimeList.WorkMode != AnimeListWorkModes.Anime)
                    DesktopViewModelLocator.Main.Navigate(PageIndex.PageAnimeList,
                        new AnimeListPageNavigationArgs(value, AnimeListWorkModes.Anime));
                ViewModelLocator.AnimeList.StatusSelectorSelectedIndex = value;
                SetActiveButton(HamburgerButtons.AnimeList);
                RaisePropertyChanged(() => CurrentAnimeFiltersSelectedIndex);
            }
        }


        public int CurrentMangaFiltersSelectedIndex
        {
            get
            {
                return DesktopViewModelLocator.Main.CurrentMainPage != PageIndex.PageAnimeList ||
                       ViewModelLocator.AnimeList.WorkMode == AnimeListWorkModes.Anime
                    ? -1
                    : ViewModelLocator.AnimeList.StatusSelectorSelectedIndex;
            }
            set
            {
                if (DesktopViewModelLocator.Main.CurrentMainPage != PageIndex.PageAnimeList ||
                    ViewModelLocator.AnimeList.WorkMode != AnimeListWorkModes.Manga)
                    DesktopViewModelLocator.Main.Navigate(PageIndex.PageAnimeList,
                        new AnimeListPageNavigationArgs(value, AnimeListWorkModes.Manga));
                ViewModelLocator.AnimeList.StatusSelectorSelectedIndex = value;
                SetActiveButton(HamburgerButtons.MangaList);
                RaisePropertyChanged(() => CurrentMangaFiltersSelectedIndex);
            }
        }

        public Visibility LogInButtonVisibility => Credentials.Authenticated ? Visibility.Collapsed : Visibility.Visible;


        public Visibility FeedbackHubButtonVisibility => StoreServicesFeedbackLauncher.IsSupported() ? Visibility.Visible : Visibility.Collapsed;

        public ImageSource UserImage
        {
            get { return _userImage; }
            set
            {
                _userImage = value;
                RaisePropertyChanged(() => UserImage);
            }
        }

        public bool ProfileButtonVisibility
        {
            get { return _profileButtonVisibility; }
            set
            {
                _profileButtonVisibility = value;
                RaisePropertyChanged(() => ProfileButtonVisibility);
            }
        }

        public ICommand ButtonNavigationCommand
            => _buttonNavigationCommand ?? (_buttonNavigationCommand = new RelayCommand<object>(ButtonClick));

        public ICommand ButtonNavigationTopAnimeCommand
            =>
                _buttonNavigationTopAnimeCommand ??
                (_buttonNavigationTopAnimeCommand = new RelayCommand<object>(ButtonClickTopCategory));

        public ICommand AnimeFiltersFlyoutCommand
            =>
                _animeFiltersFlyoutCommand ??
                (_animeFiltersFlyoutCommand =
                    new RelayCommand<string>(o => CurrentAnimeFiltersSelectedIndex = int.Parse(o)));

        public ICommand TopCategoriesFiltersFlyoutCommand
            =>
                _topCategoriesFiltersFlyoutCommand ??
                (_topCategoriesFiltersFlyoutCommand = new RelayCommand<object>(ButtonClickTopCategory));

        public Visibility UsrImgPlaceholderVisibility
        {
            get { return _usrImgPlaceholderVisibility; }
            set
            {
                _usrImgPlaceholderVisibility = value;
                RaisePropertyChanged(() => UsrImgPlaceholderVisibility);
            }
        }

        public Visibility AdLoadingSpinnerVisibility
        {
            get { return _adLoadingSpinnerVisibility; }
            set
            {
                _adLoadingSpinnerVisibility = value;
                RaisePropertyChanged(() => AdLoadingSpinnerVisibility);
            }
        }

        public Visibility MalApiSpecificButtonsVisibility
            => Settings.SelectedApiType == ApiType.Mal ? Visibility.Visible : Visibility.Collapsed;

        public Thickness BottomStackPanelMargin
        {
            get { return _bottomStackPanelMargin; }
            set
            {
                _bottomStackPanelMargin = value;
                RaisePropertyChanged(() => BottomStackPanelMargin);
            }
        }

        public List<string> PinnedProfiles => ResourceLocator.HandyDataStorage.PinnedUsers.StoredItems.Select(user => user.Name).ToList();

        public void UpdatePinnedProfiles()
        {
            RaisePropertyChanged(() => PinnedProfiles);
        }

        public void UpdateBottomMargin()
        {
            RaisePropertyChanged(() => BottomStackPanelMargin);
        }

        private void ButtonClick(object o)
        {
            if (o == null)
                return;
            PageIndex page;
            if (Enum.TryParse(o as string, out page))
            {
                DesktopViewModelLocator.Main.Navigate(page, GetAppropriateArgsForPage(page));
                SetActiveButton(Utilities.GetButtonForPage(page));
            }
        }

        private void ButtonClickTopCategory(object o)
        {
            if (o == null)
                return;
            var type = (TopAnimeType) int.Parse(o as string);
            DesktopViewModelLocator.Main.Navigate(PageIndex.PageTopAnime, AnimeListPageNavigationArgs.TopAnime(type));
            SetActiveButton(type);
            SetActiveButton(HamburgerButtons.TopAnime);
        }

        private object GetAppropriateArgsForPage(PageIndex page)
        {
            switch (page)
            {
                case PageIndex.PageSeasonal:
                    return AnimeListPageNavigationArgs.Seasonal;
                case PageIndex.PageMangaList:
                    return AnimeListPageNavigationArgs.Manga;
                case PageIndex.PageMangaSearch:
                    return new SearchPageNavigationArgs {Anime = false};
                case PageIndex.PageSearch:
                    return new SearchPageNavigationArgs();
                case PageIndex.PageTopAnime:
                    return AnimeListPageNavigationArgs.TopAnime(TopAnimeType.General);
                case PageIndex.PageTopManga:
                    return AnimeListPageNavigationArgs.TopManga;
                case PageIndex.PageProfile:
                    return new ProfilePageNavigationArgs {TargetUser = Credentials.UserName};
                case PageIndex.PageArticles:
                    return MalArticlesPageNavigationArgs.Articles;
                case PageIndex.PageNews:
                    return MalArticlesPageNavigationArgs.News;
                case PageIndex.PageWallpapers:
                    return new WallpaperPageNavigationArgs();
                default:
                    return null;
            }
        }

        public void ChangeBottomStackPanelMargin(bool up)
        {
            if (up == _prevState)
                return;

            _prevState = up;

            BottomStackPanelMargin = up ? new Thickness(0, 0, 0, 48) : new Thickness(0);
        }

        public void UpdateAnimeFiltersSelectedIndex()
        {
            RaisePropertyChanged(() => CurrentAnimeFiltersSelectedIndex);
            RaisePropertyChanged(() => CurrentMangaFiltersSelectedIndex);
        }

        public Task UpdateProfileImg(bool dl = true)
        {
            if (Credentials.Authenticated)
            {
                UserImage = new BitmapImage(new Uri($"https://cdn.myanimelist.net/images/userimages/{Credentials.Id}.jpg")); 
                ProfileButtonVisibility = true;
                UsrImgPlaceholderVisibility = Visibility.Collapsed;
            }
            else
            {
                UWPUtilities.RemoveProfileImg();
            }

            return Task.CompletedTask;
        }

        private void ResetActiveButton()
        {
            var color = RequestedFontColor;
            TxtForegroundBrushes["AnimeList"] = new SolidColorBrush(color);
            TxtForegroundBrushes["MangaList"] = new SolidColorBrush(color);
            TxtForegroundBrushes["AnimeSearch"] = new SolidColorBrush(color);
            TxtForegroundBrushes["MangaSearch"] = new SolidColorBrush(color);
            TxtForegroundBrushes["LogIn"] = new SolidColorBrush(color);
            TxtForegroundBrushes["Settings"] = new SolidColorBrush(color);
            TxtForegroundBrushes["Profile"] = new SolidColorBrush(color);
            TxtForegroundBrushes["Seasonal"] = new SolidColorBrush(color);
            TxtForegroundBrushes["About"] = new SolidColorBrush(color);
            TxtForegroundBrushes["Recommendations"] = new SolidColorBrush(color);
            TxtForegroundBrushes["TopAnime"] = new SolidColorBrush(color);
            TxtForegroundBrushes["TopManga"] = new SolidColorBrush(color);
            TxtForegroundBrushes["Calendar"] = new SolidColorBrush(color);
            TxtForegroundBrushes["Articles"] = new SolidColorBrush(color);
            TxtForegroundBrushes["News"] = new SolidColorBrush(color);
            TxtForegroundBrushes["Forums"] = new SolidColorBrush(color);
            TxtForegroundBrushes["CharacterSearch"] = new SolidColorBrush(color);
            TxtForegroundBrushes["Wallpapers"] = new SolidColorBrush(color);
            TxtForegroundBrushes["PopularVideos"] = new SolidColorBrush(color);

            TxtBorderBrushThicknesses["AnimeList"] = new Thickness(0);
            TxtBorderBrushThicknesses["MangaList"] = new Thickness(0);
            TxtBorderBrushThicknesses["AnimeSearch"] = new Thickness(0);
            TxtBorderBrushThicknesses["MangaSearch"] = new Thickness(0);
            TxtBorderBrushThicknesses["LogIn"] = new Thickness(0);
            TxtBorderBrushThicknesses["Settings"] = new Thickness(0);
            TxtBorderBrushThicknesses["Profile"] = new Thickness(0);
            TxtBorderBrushThicknesses["Seasonal"] = new Thickness(0);
            TxtBorderBrushThicknesses["About"] = new Thickness(0);
            TxtBorderBrushThicknesses["Recommendations"] = new Thickness(0);
            TxtBorderBrushThicknesses["TopAnime"] = new Thickness(0);
            TxtBorderBrushThicknesses["TopManga"] = new Thickness(0);
            TxtBorderBrushThicknesses["Calendar"] = new Thickness(0);
            TxtBorderBrushThicknesses["Articles"] = new Thickness(0);
            TxtBorderBrushThicknesses["News"] = new Thickness(0);
            TxtBorderBrushThicknesses["Forums"] = new Thickness(0);
            TxtBorderBrushThicknesses["CharacterSearch"] = new Thickness(0);
            TxtBorderBrushThicknesses["Wallpapers"] = new Thickness(0);
            TxtBorderBrushThicknesses["PopularVideos"] = new Thickness(0);
        }

        private void ResetTopCategoryButtons()
        {
            var color = RequestedFontColor;
            TopCategoriesForegorundBrushes["General"] = new SolidColorBrush(color);
            TopCategoriesForegorundBrushes["Airing"] = new SolidColorBrush(color);
            TopCategoriesForegorundBrushes["Upcoming"] = new SolidColorBrush(color);
            TopCategoriesForegorundBrushes["Tv"] = new SolidColorBrush(color);
            TopCategoriesForegorundBrushes["Movies"] = new SolidColorBrush(color);
            TopCategoriesForegorundBrushes["Ovas"] = new SolidColorBrush(color);
            TopCategoriesForegorundBrushes["Popular"] = new SolidColorBrush(color);
            TopCategoriesForegorundBrushes["Favourited"] = new SolidColorBrush(color);
        }

        public void SetActiveButton(TopAnimeType val)
        {
            ResetTopCategoryButtons();
            TopCategoriesForegorundBrushes[val.ToString()] =
                Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
            RaisePropertyChanged(() => TopCategoriesForegorundBrushes);
        }

        public void SetActiveButton(HamburgerButtons val)
        {
            ResetActiveButton();
            if (val != HamburgerButtons.TopAnime)
            {
                ResetTopCategoryButtons();
                RaisePropertyChanged(() => TopCategoriesForegorundBrushes);
            }
            if (TxtForegroundBrushes.ContainsKey(val.ToString()))
            {
                TxtForegroundBrushes[val.ToString()] =
                    Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                TxtBorderBrushThicknesses[val.ToString()] = new Thickness(4, 0, 0, 0);
            }
            RaisePropertyChanged(() => TxtForegroundBrushes);
            RaisePropertyChanged(() => TxtBorderBrushThicknesses);
        }

        public void HamburgerWidthChanged(bool wide)
        {
            HamburgerExpanded = wide;
        }

        public void UpdateLogInLabel()
        {
            ProfileButtonVisibility = Credentials.Authenticated;
            RaisePropertyChanged(() => LogInButtonVisibility);
        }

        private bool _mangaSectionVisbility = !Settings.HamburgerHideMangaSection;

        public bool MangaSectionVisbility
        {
            get { return _mangaSectionVisbility; }
            set
            {
                _mangaSectionVisbility = value;
                RaisePropertyChanged(() => MangaSectionVisbility);
            }
        }

        public void UpdateApiDependentButtons()
        {
            RaisePropertyChanged(() => MalApiSpecificButtonsVisibility);
        }
    }
}