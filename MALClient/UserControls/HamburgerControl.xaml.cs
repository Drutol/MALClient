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
        private HamburgerControlViewModel _viewModel => (HamburgerControlViewModel)DataContext;

        public HamburgerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;

        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _viewModel.UpdateProfileImg();
            _viewModel.SetActiveButton(HamburgerButtons.AnimeList);
            _viewModel.View = this;
        }

        internal void PaneOpened()
        {

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

        public double GetScrollBurgerActualHeight()
        {
            return ScrlBurger.ActualHeight;
        }
    }
}