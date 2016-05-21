using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using MALClient.ViewModels;
using VungleSDK;

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


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.AdLoadingSpinnerVisibility = Visibility.Visible;
            if (VungleAdInstance == null)
            {
                VungleAdInstance = AdFactory.GetInstance("5735f9ae0b3973633c00004b");

                VungleAdInstance.OnAdPlayableChanged += VungleAdInstanceOnOnAdPlayableChanged;

            }
        }

        public VungleAd VungleAdInstance { get; set; }

        private async void VungleAdInstanceOnOnAdPlayableChanged(object sender, AdPlayableEventArgs adPlayableEventArgs)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                ViewModel.AdLoadingSpinnerVisibility = Visibility.Collapsed;
                await
                    VungleAdInstance.PlayAdAsync(new AdConfig
                    {
                        Incentivized = true,
                        SoundEnabled = true,                       
                    });
            });

        }

    }
}