using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MALClient.UWP.UserControls.AttachedProperties
{
    public class ScrollViewerExtensions : DependencyObject
    {
        public static readonly DependencyProperty ShowScrollbarAtBreakpointOffsetProperty =
            DependencyProperty.RegisterAttached(
                "ShowScrollbarAtBreakpointOffset",
                typeof(double),
                typeof(ScrollViewer),
                new PropertyMetadata(-1)
                );

        public static void SetShowScrollbarAtBreakpointOffset(UIElement element, double value)
        {
            element.SetValue(ShowScrollbarAtBreakpointOffsetProperty, value);
            if (value > 0)
            {
                var sv = (ScrollViewer) element;
                sv.ViewChanging += SvOnViewChanging;
            }
            else
            {
                var sv = (ScrollViewer)element;
                sv.ViewChanging -= SvOnViewChanging;
            }
        }

        private static void SvOnViewChanging(object sender, ScrollViewerViewChangingEventArgs args)
        {
            var sv = (ScrollViewer) sender;
            if (args.FinalView.VerticalOffset > (double)sv.GetValue(ShowScrollbarAtBreakpointOffsetProperty))
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            else
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        public static double GetShowScrollbarAtBreakpointOffset(UIElement element)
        {
            return (double) element.GetValue(ShowScrollbarAtBreakpointOffsetProperty);
        }
    }
}
