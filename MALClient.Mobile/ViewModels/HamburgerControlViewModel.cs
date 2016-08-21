using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm;
using MalClient.Shared.Comm.Anime;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Main;
using MALClient.Pages;
using MALClient.UserControls;
using Microsoft.Services.Store.Engagement;

namespace MALClient.ViewModels
{
    public class HamburgerControlViewModel : ViewModelBase , IHamburgerViewModel
    {
        private Visibility _adLoadingSpinnerVisibility = Visibility.Collapsed;

        private Thickness _bottomStackPanelMargin = new Thickness(0);


        private ICommand _buttonAdCommand;
        private ICommand _buttonNavigationCommand;
        private ICommand _buttonNavigationTopAnimeCommand;

        public ICommand ButtonNavigationTopAnimeCommand
            => _buttonNavigationTopAnimeCommand ?? (_buttonNavigationTopAnimeCommand = new RelayCommand<object>(ButtonClickTopCategory));


        private double _gridBtmMarginHeight;

        private double _gridSeparatorHeight;

        private int _menuPivotSelectedIndex;
        private bool? _prevState;

        private bool _profileButtonVisibility;

        private bool _subtractedHeightForButton = true;

        private BitmapImage _userImage;

        private Visibility _usrImgPlaceholderVisibility = Visibility.Collapsed;


        public HamburgerControlViewModel()
        {
            ResetActiveButton();
            MenuPivotSelectedIndex = Settings.DefaultMenuTab == "anime" ? 0 : 1;
        }

        private Color RequestedFontColor
            => Application.Current.RequestedTheme == ApplicationTheme.Dark ? Colors.FloralWhite : Colors.Black;

        public string LogInLabel => Credentials.Authenticated ? "Account" : "Log In";
        public Visibility LogInButtonVisibility => Credentials.Authenticated ? Visibility.Collapsed : Visibility.Visible;

        public Visibility FeedbackHubButtonVisibility => Feedback.IsSupported ? Visibility.Visible : Visibility.Collapsed;

        public Dictionary<string, Brush> TxtForegroundBrushes { get; } = new Dictionary<string, Brush>();

        public Dictionary<string, Thickness> TxtBorderBrushThicknesses { get; } = new Dictionary<string, Thickness>();

        public double GridBtmMarginHeight
        {
            get { return _gridBtmMarginHeight; }
            set
            {
                _gridBtmMarginHeight = value;
                RaisePropertyChanged(() => GridBtmMarginHeight);
            }
        }

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

        public ICommand ButtonNavigationCommand => _buttonNavigationCommand ?? (_buttonNavigationCommand = new RelayCommand<object>(ButtonClick));

        public Visibility MalApiSpecificButtonsVisibility
            => Settings.SelectedApiType == ApiType.Mal ? Visibility.Visible : Visibility.Collapsed;

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


        public int MenuPivotSelectedIndex
        {
            get { return _menuPivotSelectedIndex; }
            set
            {
                _menuPivotSelectedIndex = value;
                RaisePropertyChanged(() => MenuPivotSelectedIndex);
            }
        }

        public Thickness BottomStackPanelMargin
        {
            get { return _bottomStackPanelMargin; }
            set
            {
                _bottomStackPanelMargin = value;
                RaisePropertyChanged(() => BottomStackPanelMargin);
            }
        }


        private void ButtonClick(object o)
        {
            if (o == null)
                return;
            PageIndex page;
            if (Enum.TryParse(o as string, out page))
            {
                if(page == PageIndex.PageProfile)
                    ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList,null);
                MobileViewModelLocator.Main.Navigate(page, GetAppropriateArgsForPage(page));
                SetActiveButton(Utilities.GetButtonForPage(page));
            }
        }

        private void ButtonClickTopCategory(object o)
        {
            if (o == null)
                return;
            TopAnimeType type = (TopAnimeType)int.Parse(o as string);
            MobileViewModelLocator.Main.Navigate(PageIndex.PageTopAnime, AnimeListPageNavigationArgs.TopAnime(type));
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
                case PageIndex.PageArticles:
                    return MalArticlesPageNavigationArgs.Articles;
                case PageIndex.PageNews:
                    return MalArticlesPageNavigationArgs.News;
                case PageIndex.PageProfile:
                    return new ProfilePageNavigationArgs {TargetUser = Credentials.UserName};
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

        public async Task UpdateProfileImg(bool dl = true)
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
                        await Utilities.DownloadProfileImg();
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
            else
            {
                ProfileButtonVisibility = false;
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
            TxtForegroundBrushes["Calendar"] = new SolidColorBrush(color);
            TxtForegroundBrushes["Articles"] = new SolidColorBrush(color);
            TxtForegroundBrushes["News"] = new SolidColorBrush(color);
            TxtForegroundBrushes["Forums"] = new SolidColorBrush(color);
            TxtForegroundBrushes["CharacterSearch"] = new SolidColorBrush(color);

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
        }

        public void SetActiveButton(HamburgerButtons val)
        {
            ResetActiveButton();
            TxtForegroundBrushes[val.ToString()] =
                Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
            TxtBorderBrushThicknesses[val.ToString()] = new Thickness(3, 0, 0, 0);
            RaisePropertyChanged(() => TxtForegroundBrushes);
            RaisePropertyChanged(() => TxtBorderBrushThicknesses);
        }

        public void UpdateLogInLabel()
        {
            RaisePropertyChanged(() => LogInLabel);
            RaisePropertyChanged(() => LogInButtonVisibility);
        }

        public Visibility MangaSectionVisbility { get; set; }

        public void SetActiveButton(TopAnimeType topType)
        {
            //not for mobile
        }

        public void UpdateApiDependentButtons()
        {
            RaisePropertyChanged(() => MalApiSpecificButtonsVisibility);
        }

        public void UpdateAnimeFiltersSelectedIndex()
        {
            
        }

        public List<string> PinnedProfiles => Settings.PinnedProfiles.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        public void UpdatePinnedProfiles()
        {
            RaisePropertyChanged(() => PinnedProfiles);
        }
    }
}