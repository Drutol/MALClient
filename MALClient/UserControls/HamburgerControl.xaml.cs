using System;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Comm;
using MALClient.Pages;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls
{

    public enum HamburgerButtons
    {
        AnimeList,
        AnimeSearch,
        LogIn,
        Settings,
        Profile,
        Seasonal,
        About
    }

    public sealed partial class HamburgerControl : UserControl
    {
        private int _stackPanelHeightSum = 275; //base value

        public HamburgerControl()
        {
            InitializeComponent();
            TxtList.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
            Loaded += OnLoaded;

        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateProfileImg();
        }

        internal void PaneOpened()
        {
            var val = Convert.ToInt32(ScrlBurger.ActualHeight);
            GridSeparator.Height = val - _stackPanelHeightSum < 0 ? 0 : val - _stackPanelHeightSum;
            GridBtmMargin.Height = GridSeparator.Height == 0 ? 50 : 0;
        }

        private bool _subtractedHeightForButton = true;
        internal async void UpdateProfileImg()
        {
            if (Creditentials.Authenticated)
            {
                try
                {
                    var file = await ApplicationData.Current.LocalFolder.GetFileAsync("UserImg.png");
                    var props = await file.GetBasicPropertiesAsync();
                    if(props.Size == 0)
                        throw new FileNotFoundException();
                    var bitmap = new BitmapImage();
                    using (var fs = (await file.OpenStreamForReadAsync()).AsRandomAccessStream())
                    {
                        bitmap.SetSource(fs);
                    }

                    ImgUser.Source = bitmap;
                }
                catch (FileNotFoundException)
                {
                    Utils.DownloadProfileImg();
                }
                catch (Exception)
                {
                    // ignored
                }

                BtnProfile.Visibility = Visibility.Visible;
                if (_subtractedHeightForButton)
                {
                    _stackPanelHeightSum += 35;
                    _subtractedHeightForButton = false;
                }

            }
            else
            {
                BtnProfile.Visibility = Visibility.Collapsed;
                if (!_subtractedHeightForButton)
                {
                    _stackPanelHeightSum -= 35;
                    _subtractedHeightForButton = true;
                }

                
            }
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageSettings);
            SetActiveButton(HamburgerButtons.AnimeList);
        }

        private void BtnList_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageAnimeList);
            SetActiveButton(HamburgerButtons.AnimeList);
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageSearch);
            SetActiveButton(HamburgerButtons.AnimeSearch);
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageLogIn);
            SetActiveButton(HamburgerButtons.LogIn);
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageProfile);
            SetActiveButton(HamburgerButtons.Profile);
        }

        private void BtnSeasonal_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageAnimeList,new AnimeListPageNavigationArgs());
            SetActiveButton(HamburgerButtons.Seasonal);
        }

        private void ButtonAbout_OnClick(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageAbout);
            SetActiveButton(HamburgerButtons.About);
        }

        public void SetActiveButton(HamburgerButtons val)
        {
            ResetActiveButton();
            switch (val)
            {
                case HamburgerButtons.AnimeList:
                    TxtList.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                case HamburgerButtons.AnimeSearch:
                    TxtSearch.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                case HamburgerButtons.LogIn:
                    TxtLogin.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                case HamburgerButtons.Settings:
                    TxtSettings.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                case HamburgerButtons.Profile:
                    TxtProfile.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                case HamburgerButtons.Seasonal:
                    TxtSeasonal.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                case HamburgerButtons.About:
                    SymbolAbout.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(val), val, null);
            }
        }

        private void ResetActiveButton()
        {
            TxtSettings.Foreground = new SolidColorBrush(Colors.Black);
            TxtSearch.Foreground = new SolidColorBrush(Colors.Black);
            TxtList.Foreground = new SolidColorBrush(Colors.Black);
            TxtLogin.Foreground = new SolidColorBrush(Colors.Black);
            TxtProfile.Foreground = new SolidColorBrush(Colors.Black);
            TxtSeasonal.Foreground = new SolidColorBrush(Colors.Black);
            SymbolAbout.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = btn.Content as Grid;
            foreach (Border item in grid.Children.OfType<Border>())
            {
                item.Visibility = Visibility.Visible;
                break;
            }
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = btn.Content as Grid;
            foreach (Border item in grid.Children.OfType<Border>())
            {
                item.Visibility = Visibility.Collapsed;
                break;
            }
        }

        private bool? _prevState;
        public void ChangeBottomStackPanelMargin(bool up)
        {
            if (up == _prevState)         
                return;

            _prevState = up;
            
            _stackPanelHeightSum += up ? 50 : -50 ;
        }


        private MainPage GetMainPageInstance()
        {
            return Utils.GetMainPageInstance();
        }


    }
}
