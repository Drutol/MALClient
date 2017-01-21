using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MALClient.Shared.Managers;

namespace MALClient.Shared.UserControls.AttachedProperties
{
    public class WebViewExtensions : DependencyObject
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
            "Content", typeof(string), typeof(WebViewExtensions), new PropertyMetadata(default(string),PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var view = dependencyObject as WebView;
            view.NavigateToString(CssManager.WrapWithCss(dependencyPropertyChangedEventArgs.NewValue as string));
        }

        public static void SetContent(DependencyObject element, string value)
        {
            element.SetValue(ContentProperty, value);
        }

        public static string GetContent(DependencyObject element)
        {
            return (string) element.GetValue(ContentProperty);
        }
    }
}
