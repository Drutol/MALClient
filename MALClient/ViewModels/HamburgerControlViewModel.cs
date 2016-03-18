using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Pages;
using MALClient.UserControls;

namespace MALClient.ViewModels
{
    public interface IHamburgerControlView
    {
        double GetScrollBurgerActualHeight();
    }


    public class HamburgerControlViewModel : ViewModelBase
    {
        private bool? _prevState;
        private int _stackPanelHeightSum = Creditentials.Authenticated ? 300 : 350; //base value , we are either on log in page or list page (app bar on/off)
        private bool _subtractedHeightForButton = true;


        public IHamburgerControlView View { get; set; }


        public Dictionary<string, Brush> TxtForegroundBrushes => _brushes;
        public Dictionary<string, Thickness> TxtBorderBrushThicknesses => _thicknesses;
            
            
        private readonly Dictionary<string, Brush> _brushes = new Dictionary<string, Brush>
        {
            ["AnimeList"] =  new SolidColorBrush(Colors.Black),
            ["MangaList"] =  new SolidColorBrush(Colors.Black),
            ["AnimeSearch"] =  new SolidColorBrush(Colors.Black),
            ["MangaSearch"] =  new SolidColorBrush(Colors.Black),
            ["LogIn"] =  new SolidColorBrush(Colors.Black),
            ["Settings"] =  new SolidColorBrush(Colors.Black),
            ["Profile"] =  new SolidColorBrush(Colors.Black),
            ["Seasonal"] =  new SolidColorBrush(Colors.Black),
            ["About"] =  new SolidColorBrush(Colors.Black),
            ["Recommendations"] =  new SolidColorBrush(Colors.Black),
        };

        private readonly Dictionary<string, Thickness> _thicknesses = new Dictionary<string, Thickness>
        {
            ["AnimeList"] =  new Thickness(0),
            ["MangaList"] =  new Thickness(0),
            ["AnimeSearch"] =  new Thickness(0),
            ["MangaSearch"] =  new Thickness(0),
            ["LogIn"] =  new Thickness(0),
            ["Settings"] =  new Thickness(0),
            ["Profile"] =  new Thickness(0),
            ["Seasonal"] =  new Thickness(0),
            ["About"] =  new Thickness(0),
            ["Recommendations"] =  new Thickness(0),
        };

        public RelayCommand PaneOpenedCommand { get; private set; }

        private double _gridSeparatorHeight;
        public double GridSeparatorHeight
        {
            get { return _gridSeparatorHeight; }
            set
            {
                _gridSeparatorHeight = value;
                RaisePropertyChanged(() => GridSeparatorHeight);
            }
        }

        private double _gridBtmMarginHeight;
        public double GridBtmMarginHeight
        {
            get { return _gridBtmMarginHeight; }
            set
            {
                _gridBtmMarginHeight = value;
                RaisePropertyChanged(() => GridBtmMarginHeight);
            }
        }

        private BitmapImage _userImage;
        public BitmapImage UserImage
        {
            get { return _userImage; } 
            set
            {
                _userImage = value;
                RaisePropertyChanged(() => UserImage);
            }
        }

        private bool _profileButtonVisibility;
        public bool ProfileButtonVisibility
        {
            get { return _profileButtonVisibility; }
            set
            {
                _profileButtonVisibility = value;
                RaisePropertyChanged(() => ProfileButtonVisibility);
            }
        }

        private ICommand _buttonNavigationCommand;
        public ICommand ButtonNavigationCommand
        {
            get
            {
                return _buttonNavigationCommand ?? (_buttonNavigationCommand = new RelayCommand<Object>(ButtonClick));
            }
        }


        public Visibility _usrImgPlaceholderVisibility = Visibility.Collapsed;
        public Visibility UsrImgPlaceholderVisibility
        {
            get { return _usrImgPlaceholderVisibility; }
            set
            {
                _usrImgPlaceholderVisibility = value;
                RaisePropertyChanged(() => UsrImgPlaceholderVisibility);
            }
        }

        private async void ButtonClick(object o)
        {
            if(o == null)
                return;
            PageIndex page;
            if (PageIndex.TryParse(o as string, out page))
            {
                await
                    Utils.GetMainPageInstance()
                        .Navigate(page, page == PageIndex.PageSeasonal ? AnimeListPageNavigationArgs.Seasonal : null);
                SetActiveButton(Utils.GetButtonForPage(page));
            }
        }

        public HamburgerControlViewModel()
        {
            PaneOpenedCommand = new RelayCommand(this.PaneOpened);
        }

        public void ChangeBottomStackPanelMargin(bool up)
        {
            if (up == _prevState)
                return;

            _prevState = up;

            _stackPanelHeightSum += up ? 50 : -50;
        }    

        public void PaneOpened()
        {
            var val = Convert.ToInt32(View.GetScrollBurgerActualHeight());
            GridSeparatorHeight = val - _stackPanelHeightSum < 0 ? 0 : val - _stackPanelHeightSum;
            GridBtmMarginHeight = GridSeparatorHeight < 1 ? 50 : 0;
        }

        internal async Task UpdateProfileImg(bool dl = true)
        {
            if (Creditentials.Authenticated)
            {
                try
                {
                    StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("UserImg.png");
                    BasicProperties props = await file.GetBasicPropertiesAsync();
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
                if (_subtractedHeightForButton)
                {
                    _stackPanelHeightSum += 35;
                    _subtractedHeightForButton = false;
                }
            }
            else
            {
                ProfileButtonVisibility = false;
                if (!_subtractedHeightForButton)
                {
                    _stackPanelHeightSum -= 35;
                    _subtractedHeightForButton = true;
                }
            }
            PaneOpened();
        }

        private void ResetActiveButton()
        {
            _brushes["AnimeList"] = new SolidColorBrush(Colors.Black);
            _brushes["MangaList"] = new SolidColorBrush(Colors.Black);
            _brushes["AnimeSearch"] = new SolidColorBrush(Colors.Black);
            _brushes["MangaSearch"] = new SolidColorBrush(Colors.Black);
            _brushes["LogIn"] = new SolidColorBrush(Colors.Black);
            _brushes["Settings"] = new SolidColorBrush(Colors.Black);
            _brushes["Profile"] = new SolidColorBrush(Colors.Black);
            _brushes["Seasonal"] = new SolidColorBrush(Colors.Black);
            _brushes["About"] = new SolidColorBrush(Colors.Black);
            _brushes["Recommendations"] = new SolidColorBrush(Colors.Black);

            _thicknesses["AnimeList"] = new Thickness(0);
            _thicknesses["MangaList"] = new Thickness(0);
            _thicknesses["AnimeSearch"] = new Thickness(0);
            _thicknesses["MangaSearch"] = new Thickness(0);
            _thicknesses["LogIn"] = new Thickness(0);
            _thicknesses["Settings"] = new Thickness(0);
            _thicknesses["Profile"] = new Thickness(0);
            _thicknesses["Seasonal"] = new Thickness(0);
            _thicknesses["About"] = new Thickness(0);
            _thicknesses["Recommendations"] = new Thickness(0);
        }

        public void SetActiveButton(HamburgerButtons val)
        {
            ResetActiveButton();
            _brushes[val.ToString()] = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
            _thicknesses[val.ToString()] = new Thickness(3,0,0,0);
            RaisePropertyChanged(() => TxtForegroundBrushes);
            RaisePropertyChanged(() => TxtBorderBrushThicknesses);
        }
    }
}
