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

        private HamburgerControlViewModel ViewModel => (HamburgerControlViewModel) DataContext;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.UpdateProfileImg();
            if(Settings.HamburgerAnimeFiltersExpanded)
                ButtonExpandAnimeFiltersOnClick(null,null);
            if (Settings.HamburgerMangaFiltersExpanded)
                ButtonExpandMangaFiltersOnClick(null,null);
        }

        private static readonly Brush _b2 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(220, 50, 50, 50)
                : Color.FromArgb(220, 190, 190, 190));

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as Button).Background = _b2;
        }

        private bool _animeFiltersExpanded;
        private bool _mangaFiltersExpanded;

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as Button).Background = new SolidColorBrush(Colors.Transparent);
        }

        private void ButtonExpandAnimeFiltersOnClick(object sender, RoutedEventArgs e)
        {
            if (!_animeFiltersExpanded)
            {
                ExpandAnimeListFiltersStoryboard.Begin();
                RotateAnimeListFiltersStoryboard.Begin();
            }
            else
            {
                CollapseAnimeListFiltersStoryboard.Begin();
                RotateBackAnimeListFiltersStoryboard.Begin();
            }

            _animeFiltersExpanded = !_animeFiltersExpanded;
        }

        private void ButtonExpandMangaFiltersOnClick(object sender, RoutedEventArgs e)
        {
            if (!_mangaFiltersExpanded)
            {
                ExpandMangaListFiltersStoryboard.Begin();
                RotateMangaListFiltersStoryboard.Begin();
            }
            else
            {
                CollapseMangaListFiltersStoryboard.Begin();
                RotateBackMangaListFiltersStoryboard.Begin();
            }

            _mangaFiltersExpanded = !_mangaFiltersExpanded;
        }

        public AlternatingListView AnimeFilters => AlternatingListViewAnime;
        public AlternatingListView MangaFilters => AlternatingListViewManga;

        private void HamburgerControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width == 250.0)
            {
                ViewModel.HamburgerWidthChanged(true);
                MidSeparator.Width = BottomSeparator.Width = 250;
            }
            else
            {
                ViewModel.HamburgerWidthChanged(false);
                MidSeparator.Width = BottomSeparator.Width = 60;
            }


        }
    }
}