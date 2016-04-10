using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MALClient.UserControls
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

        public Brush CurrBrush1 = _b1;
        public Brush CurrBrush2 = _b2;

        public static readonly DependencyProperty InvertRowAlternationProperty =
            DependencyProperty.Register(
                "DataSource",
                typeof (bool),
                typeof (AlternatingListView),
                new PropertyMetadata(
                    false, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var list = dependencyObject as AlternatingListView;
            if ((bool)args.NewValue)
            {
                list.CurrBrush1 = _b2;
                list.CurrBrush2 = _b1;
            }
            else
            {
                list.CurrBrush1 = _b1;
                list.CurrBrush2 = _b2;
            }
        }

        public bool InvertRowAlternation
        {
            get { return (bool)GetValue(InvertRowAlternationProperty); }
            set { SetValue(InvertRowAlternationProperty, value); }
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
                    listViewItem.Background = CurrBrush1;
                }
                else
                {
                    listViewItem.Background = CurrBrush2;
                }
            }
        }
    }
}