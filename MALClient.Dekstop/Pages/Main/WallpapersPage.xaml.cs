using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.Shared.UserControls.AttachedProperties;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WallpapersPage : Page
    {
        public WallpapersPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
            ViewModelLocator.Wallpapers.PropertyChanged += WallpapersOnPropertyChanged;
        }

        private void WallpapersOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModelLocator.Wallpapers.Wallpapers))
                MonitoredImageSourceExtension.ResetImageQueue();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            MonitoredImageSourceExtension.ResetImageQueue();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateSources();
            UpdateAmounts();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.Wallpapers.Init(e.Parameter as WallpaperPageNavigationArgs);
            base.OnNavigatedTo(e);
        }

        private void SourcesToggleButtonOnClick(object sender, RoutedEventArgs e)
        {
            var source = (WallpaperSources)int.Parse((sender as FrameworkElement).Tag as string);
            var sources = Settings.EnabledWallpaperSources;
            if (sources.Contains(source))
                sources.Remove(source);
            else
                sources.Add(source);
            Settings.EnabledWallpaperSources = sources;
        }

        private void UpdateSources()
        {
            var currentSources = Settings.EnabledWallpaperSources;
            SourceAnWp.IsChecked = currentSources.Contains(WallpaperSources.AnimeWallpaper);
            SourceAww.IsChecked = currentSources.Contains(WallpaperSources.Awwnime);
            SourceMoescape.IsChecked = currentSources.Contains(WallpaperSources.Moescape);
            SourceMoestash.IsChecked = currentSources.Contains(WallpaperSources.Moestash);
            SourcePatchuu.IsChecked = currentSources.Contains(WallpaperSources.Patchuu);
            SourcePixiv.IsChecked = currentSources.Contains(WallpaperSources.Pixiv);
            SourceZr.IsChecked = currentSources.Contains(WallpaperSources.ZettaiRyouiki);
        }

        private void UpdateAmounts()
        {
            switch (Settings.WallpapersBaseAmount)
            {
                case 6:
                    AmountMore.IsChecked = true;
                    break;
                case 4:
                    AmountStanard.IsChecked = true;
                    break;
                case 2:
                    AmountLess.IsChecked = true;
                    break;
            }
        }

        private void AmountOfWallpaperMenuFlyoutItemOnClick(object sender, RoutedEventArgs e)
        {
            var amount = int.Parse((sender as FrameworkElement).Tag as string);
            if (Settings.WallpapersBaseAmount == amount)
                return;

            Settings.WallpapersBaseAmount = amount;
            AnimeWallpapersQuery.BaseItemsToPull = amount;
            ViewModelLocator.Wallpapers.CurrentPage = 0;
        }
    }
}
