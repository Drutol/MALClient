using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Pages;
using MALClient.UserControls;
using Microsoft.Advertising.WinRT.UI;

namespace MALClient.ViewModels
{
    public interface IHamburgerInteraction
    {
        AlternatingListView AnimeFilters { get; }
        AlternatingListView MangaFilters { get; }
    }

    public class HamburgerControlViewModel : ViewModelBase
    {
        private Visibility _adLoadingSpinnerVisibility = Visibility.Collapsed;


        private ICommand _buttonAdCommand;
        private ICommand _buttonNavigationCommand;

        public IHamburgerInteraction View { get; set; }

        private bool? _prevState;

        private bool _profileButtonVisibility;
        //base value , we are either on log in page or list page (app bar on/off)

        private BitmapImage _userImage;

        private Visibility _usrImgPlaceholderVisibility = Visibility.Collapsed;


        public HamburgerControlViewModel()
        {
            ResetActiveButton();
            SetActiveButton(Credentials.Authenticated ? (Settings.DefaultMenuTab == "anime" ? HamburgerButtons.AnimeList : HamburgerButtons.MangaList) : HamburgerButtons.LogIn);
        }

        private Color RequestedFontColor
            => Application.Current.RequestedTheme == ApplicationTheme.Dark ? Colors.FloralWhite : Colors.Black;

        public Dictionary<string, Brush> TxtForegroundBrushes { get; } = new Dictionary<string, Brush>();

        public Dictionary<string, Thickness> TxtBorderBrushThicknesses { get; } = new Dictionary<string, Thickness>();


        private bool _allowFilterNavigation = true;
        public ObservableCollection<Tuple<AnimeStatus, string>> AnimeListFilters { get; set; } =
            new ObservableCollection<Tuple<AnimeStatus, string>>();
        public int CurrentAnimeFiltersSelectedIndex
        {
            get
            {
                return ViewModelLocator.Main.CurrentMainPage != PageIndex.PageAnimeList ||
                       ViewModelLocator.AnimeList.WorkMode == AnimeListWorkModes.Manga
                    ? -1
                    : ViewModelLocator.AnimeList.StatusSelectorSelectedIndex;
            }
            set
            {
                if(!_allowFilterNavigation) //when hamburger gets collapsed we don't want to trigger this thing
                    return;
                if (ViewModelLocator.Main.CurrentMainPage != PageIndex.PageAnimeList || ViewModelLocator.AnimeList.WorkMode != AnimeListWorkModes.Anime)
                    ViewModelLocator.Main.Navigate(PageIndex.PageAnimeList,new AnimeListPageNavigationArgs(value, AnimeListWorkModes.Anime));
                ViewModelLocator.AnimeList.StatusSelectorSelectedIndex = value;
                SetActiveButton(HamburgerButtons.AnimeList);
                RaisePropertyChanged(() => CurrentAnimeFiltersSelectedIndex);
            }
        }

        public ObservableCollection<Tuple<AnimeStatus, string>> MangaListFilters { get; set; } =
            new ObservableCollection<Tuple<AnimeStatus, string>>();

        public int CurrentMangaFiltersSelectedIndex
        {
            get
            {
                return ViewModelLocator.Main.CurrentMainPage != PageIndex.PageAnimeList ||
                       ViewModelLocator.AnimeList.WorkMode == AnimeListWorkModes.Anime
                    ? -1
                    : ViewModelLocator.AnimeList.StatusSelectorSelectedIndex;
            }
            set
            {
                if (ViewModelLocator.Main.CurrentMainPage != PageIndex.PageAnimeList || ViewModelLocator.AnimeList.WorkMode != AnimeListWorkModes.Manga)                    
                    ViewModelLocator.Main.Navigate(PageIndex.PageAnimeList, new AnimeListPageNavigationArgs(value,AnimeListWorkModes.Manga));
                ViewModelLocator.AnimeList.StatusSelectorSelectedIndex = value;
                SetActiveButton(HamburgerButtons.MangaList);
                RaisePropertyChanged(() => CurrentMangaFiltersSelectedIndex);
            }
        }

        public string LogInLabel => Credentials.Authenticated ? "Account" : "Log In";

        public BitmapImage UserImage
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
        {
            get
            {
                return _buttonNavigationCommand ?? (_buttonNavigationCommand = new RelayCommand<object>(ButtonClick));
            }
        }

        public ICommand ButtonAdCommand
        {
            get
            {
                return _buttonAdCommand ?? (_buttonAdCommand = new RelayCommand(() =>
                {
                    var ad = new InterstitialAd();
                    AdLoadingSpinnerVisibility = Visibility.Visible;
                    ad.AdReady += (sender, o1) =>
                    {
                        AdLoadingSpinnerVisibility = Visibility.Collapsed;
                        ad.Show();
                    };
                    ad.ErrorOccurred += (sender, args) =>
                    {
                        Utils.GiveStatusBarFeedback("Error. It's something on their end... :(");
                        AdLoadingSpinnerVisibility = Visibility.Collapsed;
                        (sender as InterstitialAd).Close();
                    };
                    ad.Completed += (sender, o) => Utils.GiveStatusBarFeedback("Thank you so much :D");
#if !DEBUG
                    ad.RequestAd(AdType.Video, "d25517cb-12d4-4699-8bdc-52040c712cab", "11389925");
#else
                    ad.RequestAd(AdType.Video, "98d3d081-e5b2-46ea-876d-f1d8176fb908", "291908");
#endif
                }
                    ));
            }
        }

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

        public Thickness _bottomStackPanelMargin = new Thickness(0);
        public Thickness BottomStackPanelMargin
        {
            get { return _bottomStackPanelMargin; }
            set
            {
                _bottomStackPanelMargin = value;
                RaisePropertyChanged(() => BottomStackPanelMargin);
            }
        }


        private async void ButtonClick(object o)
        {
            if (o == null)
                return;
            PageIndex page;
            if (Enum.TryParse(o as string, out page))
            {
                await
                    Utils.GetMainPageInstance()
                        .Navigate(page, GetAppropriateArgsForPage(page));
                SetActiveButton(Utils.GetButtonForPage(page));
            }
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
                    return AnimeListPageNavigationArgs.TopAnime;
                case PageIndex.PageTopManga:
                    return AnimeListPageNavigationArgs.TopManga;
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

        internal async Task UpdateProfileImg(bool dl = true)
        {
            if (Credentials.Authenticated)
            {
                try
                {
                    var file = await ApplicationData.Current.LocalFolder.GetFileAsync("UserImg.png");
                    var props = await file.GetBasicPropertiesAsync();
                    if (props.Size == 0)
                        throw new FileNotFoundException();
                    var bitmap = new BitmapImage();
                    using (var fs = (await file.OpenStreamForReadAsync()).AsRandomAccessStream())
                    {
                        bitmap.SetSource(fs);
                    }
                    UserImage = bitmap;
                    UsrImgPlaceholderVisibility = Visibility.Collapsed;
                }
                catch (FileNotFoundException)
                {
                    UserImage = new BitmapImage();
                    if (dl)
                        await Utils.DownloadProfileImg();
                    else
                        UsrImgPlaceholderVisibility = Visibility.Visible;
                }
                catch (Exception)
                {
                    UsrImgPlaceholderVisibility = Visibility.Visible;
                    UserImage = new BitmapImage();
                }

                ProfileButtonVisibility = true;
            }
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
        }

        public void SetActiveButton(HamburgerButtons val)
        {
            ResetActiveButton();
            TxtForegroundBrushes[val.ToString()] =
                Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
            TxtBorderBrushThicknesses[val.ToString()] = new Thickness(4, 0, 0, 0);
            RaisePropertyChanged(() => TxtForegroundBrushes);
            RaisePropertyChanged(() => TxtBorderBrushThicknesses);
        }

        public void HamburgerWidthChanged(bool wide)
        {
            if (wide)
            {
                AnimeListFilters = new ObservableCollection<Tuple<AnimeStatus, string>>
                {
                    new Tuple<AnimeStatus, string>(AnimeStatus.Watching, "Watching"),
                    new Tuple<AnimeStatus, string>(AnimeStatus.Completed, "Completed"),
                    new Tuple<AnimeStatus, string>(AnimeStatus.OnHold, "On Hold"),
                    new Tuple<AnimeStatus, string>(AnimeStatus.Dropped, "Dropped"),
                    new Tuple<AnimeStatus, string>(AnimeStatus.PlanToWatch, "Plan to watch"),
                    new Tuple<AnimeStatus, string>(AnimeStatus.AllOrAiring, "All")
                };
                MangaListFilters = new ObservableCollection<Tuple<AnimeStatus, string>>
                {
                    new Tuple<AnimeStatus, string>(AnimeStatus.Watching, "Reading"),
                    new Tuple<AnimeStatus, string>(AnimeStatus.Completed, "Completed"),
                    new Tuple<AnimeStatus, string>(AnimeStatus.OnHold, "On Hold"),
                    new Tuple<AnimeStatus, string>(AnimeStatus.Dropped, "Dropped"),
                    new Tuple<AnimeStatus, string>(AnimeStatus.PlanToWatch, "Plan to read"),
                    new Tuple<AnimeStatus, string>(AnimeStatus.AllOrAiring, "All")
                };
            }
            else //award winning text trimming
            {
                AnimeListFilters = new ObservableCollection<Tuple<AnimeStatus, string>>
                {
                    new Tuple<AnimeStatus, string>(AnimeStatus.Watching, "Wat..."),
                    new Tuple<AnimeStatus, string>(AnimeStatus.Completed, "Com..."),
                    new Tuple<AnimeStatus, string>(AnimeStatus.OnHold, "On H..."),
                    new Tuple<AnimeStatus, string>(AnimeStatus.Dropped, "Dro..."),
                    new Tuple<AnimeStatus, string>(AnimeStatus.PlanToWatch, "Pla..."),
                    new Tuple<AnimeStatus, string>(AnimeStatus.AllOrAiring, "All")
                };
                MangaListFilters = new ObservableCollection<Tuple<AnimeStatus, string>>
                {
                    new Tuple<AnimeStatus, string>(AnimeStatus.Watching, "Rea..."),
                    new Tuple<AnimeStatus, string>(AnimeStatus.Completed, "Com..."),
                    new Tuple<AnimeStatus, string>(AnimeStatus.OnHold, "On H..."),
                    new Tuple<AnimeStatus, string>(AnimeStatus.Dropped, "Dro..."),
                    new Tuple<AnimeStatus, string>(AnimeStatus.PlanToWatch, "Plan..."),
                    new Tuple<AnimeStatus, string>(AnimeStatus.AllOrAiring, "All")
                };
            }
            _allowFilterNavigation = false;
            RaisePropertyChanged(() => AnimeListFilters);
            RaisePropertyChanged(() => MangaListFilters);
            RaisePropertyChanged(() => CurrentAnimeFiltersSelectedIndex);
            RaisePropertyChanged(() => CurrentMangaFiltersSelectedIndex);
            _allowFilterNavigation = true;
        }

        public void UpdateLogInLabel()
        {
            RaisePropertyChanged(() => LogInLabel);
        }
    }
}