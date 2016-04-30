using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MALClient.UserControls
{
    class RadioToggleMenuFlyoutItem : ToggleMenuFlyoutItem
    {
        private static Dictionary<string,List<RadioToggleMenuFlyoutItem>> _groups = new Dictionary<string, List<RadioToggleMenuFlyoutItem>>();

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(
                "GroupName",
                typeof(string),
                typeof(RadioToggleMenuFlyoutItem),
                new PropertyMetadata(
                    string.Empty, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(_groups.ContainsKey((string)e.NewValue))
                _groups[(string)e.NewValue].Add(d as RadioToggleMenuFlyoutItem);
            else
            {
                _groups.Add((string)e.NewValue,new List<RadioToggleMenuFlyoutItem> {d as RadioToggleMenuFlyoutItem});
            }
        }

        public string GroupName
        {
            get { return (string) GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public RadioToggleMenuFlyoutItem()
        {
            Click += OnClick;
        }

        private void OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var btn = sender as RadioToggleMenuFlyoutItem;
            foreach (var item in _groups[btn.GroupName])
            {
                item.IsChecked = false;
            }
            btn.IsChecked = true;
        }
    }
}
