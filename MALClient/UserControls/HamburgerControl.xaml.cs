using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Comm;
using MALClient.Pages;
using MALClient.ViewModels;

#pragma warning disable 4014

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
        About,
        Recommendations
    }

    public sealed partial class HamburgerControl : UserControl , IHamburgerControlView
    {


        public HamburgerControl()
        {
            InitializeComponent();
            TxtList.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
            Loaded += OnLoaded;
            (DataContext as HamburgerControlViewModel).View = this;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            new ViewModelLocator().Hamburger.UpdateProfileImg();
        }

        internal void PaneOpened()
        {

        }

        

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageSettings);
            new ViewModelLocator().Hamburger.SetActiveButton(HamburgerButtons.Settings);
        }

        private void BtnList_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageAnimeList);
            new ViewModelLocator().Hamburger.SetActiveButton(HamburgerButtons.AnimeList);
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageSearch);
            new ViewModelLocator().Hamburger.SetActiveButton(HamburgerButtons.AnimeSearch);
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageLogIn);
            new ViewModelLocator().Hamburger.SetActiveButton(HamburgerButtons.LogIn);
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageProfile);
            new ViewModelLocator().Hamburger.SetActiveButton(HamburgerButtons.Profile);
        }

        private void BtnSeasonal_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageAnimeList, new AnimeListPageNavigationArgs());
            new ViewModelLocator().Hamburger.SetActiveButton(HamburgerButtons.Seasonal);
        }

        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageAbout);
            new ViewModelLocator().Hamburger.SetActiveButton(HamburgerButtons.About);
        }

        private void BtnRecom_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageRecomendations);
            new ViewModelLocator().Hamburger.SetActiveButton(HamburgerButtons.Recommendations);
        }





        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var btn = sender as Button;
            var grid = btn.Content as Grid;
            foreach (Border item in grid.Children.OfType<Border>())
            {
                item.Visibility = Visibility.Visible;
                break;
            }
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var btn = sender as Button;
            var grid = btn.Content as Grid;
            foreach (Border item in grid.Children.OfType<Border>())
            {
                item.Visibility = Visibility.Collapsed;
                break;
            }
        }




        private MainViewModel GetMainPageInstance()
        {
            return Utils.GetMainPageInstance();
        }


        public double GetScrollBurgerActualHeight()
        {
            return ScrlBurger.ActualHeight;
        }
    }
}