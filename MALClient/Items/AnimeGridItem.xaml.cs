using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeGridItem : UserControl , IAnimeItemInteractions
    {
        public AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;
        public AnimeGridItem(AnimeItemViewModel vm)
        {
            this.DataContext = vm;
            this.InitializeComponent();           
        }

        public Flyout WatchedFlyout => null;
        public Flyout MoreFlyout => FlyoutMore;
    }
}
