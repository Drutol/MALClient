using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MALClient.XShared.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UWP.Shared.Items
{
    public sealed partial class StaffItem : UserControl , IItemWithFlyout
    {
        private FavouriteViewModel ViewModel => DataContext as FavouriteViewModel;

        public StaffItem()
        {
            this.InitializeComponent();
            Loaded += async (sender, args) =>
            {
                await Task.Delay(2000);
                NoImgSymbol.Visibility = Visibility.Visible;
            };
        }

        public void ShowFlyout()
        {
            MenuFlyout.ShowAt(this);
        }

        private void Image_OnImageOpened(object sender, RoutedEventArgs e)
        {
            NoImgSymbol.Opacity = 0;
        }
    }
}
