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
using MALClient.XShared.ViewModels.Items;
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

        private void ImageOnTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.RevealCommand.Execute(null);
        }

        private void Image_OnImageExOpened(object sender, ImageExOpenedEventArgs e)
        {
            DescriptionGrid.MaxWidth = Image.ActualWidth;
            ResolutionGrid.Visibility = Visibility.Visible;
            ViewModel.Resolution = $"{Bitmap.PixelWidth}x{Bitmap.PixelHeight}";
        }
    }
}
