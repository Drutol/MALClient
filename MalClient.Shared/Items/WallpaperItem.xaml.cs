using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MALClient.XShared.ViewModels.Items;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Shared.Items
{
    public sealed partial class WallpaperItem : UserControl
    {
        private WallpaperItemViewModel ViewModel => DataContext as WallpaperItemViewModel;

        public WallpaperItem()
        {
            this.InitializeComponent();
        }

        private async void ImageOnTapped(object sender, TappedRoutedEventArgs e)
        {
            if(ViewModel.IsBlurred)
                await Image.Blur().StartAsync();
        }

        private void Image_OnImageExOpened(object sender, ImageExOpenedEventArgs e)
        {

        }

        private async void Bitmap_OnImageOpened(object sender, RoutedEventArgs e)
        {
            DescriptionGrid.MaxWidth = Image.ActualWidth;
            ViewModel.Resolution = $"{Bitmap.PixelWidth}x{Bitmap.PixelHeight}";
            ResolutionGrid.Visibility = Visibility.Visible;
            ImgLoading.Visibility = Visibility.Collapsed;
            if (ViewModel.IsBlurred)
                await Image.Blur(10, 0).StartAsync();
        }

        private void WallpaperOnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var grid = sender as FrameworkElement;
            FlyoutBase.GetAttachedFlyout(grid).ShowAt(grid);
        }

        private void Bitmap_OnDownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            DlProgress.Text = $"{e.Progress}%";
        }
    }
}
