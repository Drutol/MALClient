using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MALClient.Shared.UserControls
{
    public class AlternatingListView : ListView
    {
        private static readonly Brush _b2 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(170, 44, 44, 44)
                : Color.FromArgb(170, 230, 230, 230));

        private static readonly Brush _b1 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(255, 11, 11, 11)
                : Colors.WhiteSmoke);

        public static readonly DependencyProperty FirstBrushProperty = DependencyProperty.Register("FirstBrush",
            typeof(Brush), typeof(AlternatingListView), new PropertyMetadata(_b1));

        public Brush FirstBrush
        {
            get { return (Brush)GetValue(FirstBrushProperty); }
            set { SetValue(FirstBrushProperty, value); }
        }

        public static readonly DependencyProperty SecondBrushProperty = DependencyProperty.Register("SecondBrush",
            typeof(Brush), typeof(AlternatingListView), new PropertyMetadata(_b2));

        public Brush SecondBrush
        {
            get { return (Brush) GetValue(SecondBrushProperty); }
            set {  SetValue(SecondBrushProperty, value); }
        }


        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var listViewItem = element as ListViewItem;
            if (listViewItem != null)
            {
                var index = IndexFromContainer(element);
                if ((index + 1)%2 == 0)
                {
                    listViewItem.Background = FirstBrush;
                }
                else
                {
                    listViewItem.Background = SecondBrush;
                }
            }
        }
    }
}