using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MALClient.UWP.Shared.UserControls.AttachedProperties
{
    public class LinkedImageExtensions : DependencyObject
    {
        public static readonly DependencyProperty TargetVisibilityProperty = DependencyProperty.RegisterAttached(
            "TargetVisibility", typeof(Visibility), typeof(LinkedImageExtensions), new PropertyMetadata(default(Visibility)));

        public static void SetTargetVisibility(DependencyObject element, Visibility value)
        {
            element.SetValue(TargetVisibilityProperty, value);
        }

        public static Visibility GetTargetVisibility(DependencyObject element)
        {
            return (Visibility)element.GetValue(TargetVisibilityProperty);
        }

        public static readonly DependencyProperty TargetProperty = DependencyProperty.RegisterAttached(
            "Target", typeof(FrameworkElement), typeof(LinkedImageExtensions), new PropertyMetadata(default(FrameworkElement), TargetPropertyChangedCallback));

        private static void TargetPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as Image;
            control.ImageOpened += ControlOnImageOpened;
        }

        private static void ControlOnImageOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var img = sender as Image;
            img.ImageOpened -= ControlOnImageOpened;
            GetTarget(img).Visibility = GetTargetVisibility(img);
        }

        public static void SetTarget(DependencyObject element, FrameworkElement value)
        {
            element.SetValue(TargetProperty, value);
        }

        public static FrameworkElement GetTarget(DependencyObject element)
        {
            return (FrameworkElement)element.GetValue(TargetProperty);
        }
    }
}