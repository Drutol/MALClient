using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MALClient.Shared.UserControls.Derived
{
    public class RadioToggleMenuFlyoutItem : ToggleMenuFlyoutItem
    {
        private static readonly Dictionary<string, List<RadioToggleMenuFlyoutItem>> _groups =
            new Dictionary<string, List<RadioToggleMenuFlyoutItem>>();

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(
                "GroupName",
                typeof(string),
                typeof(RadioToggleMenuFlyoutItem),
                new PropertyMetadata(
                    string.Empty, PropertyChangedCallback));

        public RadioToggleMenuFlyoutItem()
        {
            Click += OnClick;
        }

        public string GroupName
        {
            get { return (string) GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_groups.ContainsKey((string) e.NewValue))
                _groups[(string) e.NewValue].Add(d as RadioToggleMenuFlyoutItem);
            else
            {
                _groups.Add((string) e.NewValue, new List<RadioToggleMenuFlyoutItem> {d as RadioToggleMenuFlyoutItem});
            }
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