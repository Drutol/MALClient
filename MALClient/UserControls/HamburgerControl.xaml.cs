using System;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
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
        TopManga,
        Calendar
    }

    public sealed partial class HamburgerControl : UserControl
    {
        public HamburgerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            if(Settings.EnableHearthAnimation)
                SupportMeStoryboard.Begin();
        }

        private HamburgerControlViewModel ViewModel => (HamburgerControlViewModel) DataContext;


        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.UpdateProfileImg();
            ViewModel.SetActiveButton(Credentials.Authenticated
                ? (Settings.DefaultMenuTab == "anime" ? HamburgerButtons.AnimeList : HamburgerButtons.MangaList)
                : HamburgerButtons.LogIn);
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


        

        private async void Donate(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as MenuFlyoutItem;
                await CurrentApp.RequestProductPurchaseAsync(btn.Tag as string, false);
            }
            catch (Exception)
            {
                // no donation
            }
        }

    }
}