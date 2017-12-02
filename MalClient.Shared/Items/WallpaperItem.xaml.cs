using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Items;
using Microsoft.Toolkit.Uwp.UI.Animations;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UWP.Shared.Items
{
    public sealed partial class WallpaperItem : UserControl
    {
        private WallpaperItemViewModel ViewModel => DataContext as WallpaperItemViewModel;

        public WallpaperItem()
        {
            this.InitializeComponent();
        }

        private async void Bitmap_OnImageOpened(object sender, RoutedEventArgs e)
        {
            if (Image.ActualWidth > 0)
            {
                BoundingGrid.Width = Image.ActualWidth;
                DescriptionGrid.Width = Image.ActualWidth;
                DescriptionGrid.MinWidth = 0;
                BoundingGrid.MinWidth = 0;
            }
            if (ViewModel != null)
            {
                ViewModel.Resolution = $"{Bitmap.PixelWidth}x{Bitmap.PixelHeight}";
                if (ViewModel.IsBlurred)
                    await Image.Blur(20, 0).StartAsync();
            }
            ResolutionGrid.Visibility = Visibility.Visible;
            ImgLoading.Visibility = Visibility.Collapsed;
        }

        private void WallpaperOnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if(ViewModelLocator.Mobile)
                return;
            var grid = sender as FrameworkElement;
            FlyoutBase.GetAttachedFlyout(grid).ShowAt(grid);
        }

        private void Bitmap_OnDownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            DlProgress.Text = $"{e.Progress}%";
        }

        private void WallpaperOnHolding(object sender, HoldingRoutedEventArgs e)
        {
            var grid = sender as FrameworkElement;
            FlyoutBase.GetAttachedFlyout(grid).ShowAt(ResolutionGrid);
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ViewModelLocator.Mobile)
            {
                DescriptionGrid.MaxWidth = Image.ActualWidth;
                BoundingGrid.MaxWidth = Image.ActualWidth;
            }
        }

        private async void UIElement_OnTapped(object sender, TappedRoutedEventArgs args)
        {
            if (ViewModel.IsBlurred)
                await Image.Blur().StartAsync();
        }
    }
}
