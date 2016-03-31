using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
        Recommendations,
        MangaList,
        MangaSearch,
        TopAnime,
        TopManga
    }

    public sealed partial class HamburgerControl : UserControl, IHamburgerControlView
    {
        public HamburgerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private HamburgerControlViewModel _viewModel => (HamburgerControlViewModel) DataContext;

        public double GetScrollBurgerActualHeight()
        {
            return ScrlBurger.ActualHeight;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _viewModel.UpdateProfileImg();
            _viewModel.SetActiveButton(Creditentials.Authenticated ? (Settings.DefaultMenuTab == "anime" ? HamburgerButtons.AnimeList : HamburgerButtons.MangaList) : HamburgerButtons.LogIn);
            _viewModel.View = this;
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //var btn = sender as Button;
            //var grid = btn.Content as Grid;
            //foreach (Border item in grid.Children.OfType<Border>())
            //{
            //    item.Visibility = Visibility.Visible;
            //    break;
            //}
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //var btn = sender as Button;
            //var grid = btn.Content as Grid;
            //foreach (Border item in grid.Children.OfType<Border>())
            //{
            //    item.Visibility = Visibility.Collapsed;
            //    break;
            //}
        }
    }
}