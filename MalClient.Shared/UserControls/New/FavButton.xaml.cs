using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UWP.Shared.UserControls.New
{
    public sealed partial class FavButton : UserControl
    {
        public FavButton()
        {
            this.InitializeComponent();
        }

        public new Brush Background
        {
            get { return Root.Background; }
            set { Root.Background = value; }
        }
    }
}
