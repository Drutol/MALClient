using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

    public sealed partial class HamburgerControl : UserControl
    {
        public HamburgerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private HamburgerControlViewModel _viewModel => (HamburgerControlViewModel) DataContext;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _viewModel.UpdateProfileImg();
            _viewModel.SetActiveButton(Creditentials.Authenticated ? HamburgerButtons.AnimeList : HamburgerButtons.LogIn);
        }

        private static readonly Brush _b2 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(220, 50, 50, 50)
                : Color.FromArgb(170, 150, 150, 150));

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as Button).Background = _b2;
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as Button).Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}