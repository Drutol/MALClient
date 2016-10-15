using System;
using System.Collections.Generic;
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
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateSources();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
    }
}
