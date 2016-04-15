using Windows.UI.Xaml.Controls;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeGridItem : UserControl, IAnimeItemInteractions
    {
        public AnimeGridItem(AnimeItemViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            vm.ViewGrid = this;
        }

        public AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

        public Flyout WatchedFlyout => null;

        public void MoreFlyoutHide()
        {
            FlyoutMore.Hide();
        }
    }
}