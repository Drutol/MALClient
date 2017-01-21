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

        public static readonly DependencyProperty ResizeToFitProperty = DependencyProperty.RegisterAttached(
            "ResizeToFit", typeof(bool), typeof(WebViewExtensions), new PropertyMetadata(default(bool),ReizePropertyChangedCallback));

        private static void ReizePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var view = dependencyObject as WebView;
            view.DOMContentLoaded += ViewOnDomContentLoaded;


        }

        private static async void ViewOnDomContentLoaded(WebView view, WebViewDOMContentLoadedEventArgs args)
        {
            view.DOMContentLoaded -= ViewOnDomContentLoaded;
            await Task.Delay(2000);
            var winHeight = await view.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });
            var docHeight = await view.InvokeScriptAsync("eval", new[] { "document.body.getBoundingClientRect().top.toString()" });
            var dogfhfcHeight = await view.InvokeScriptAsync("eval", new[] { "Math.max( document.body.scrollHeight, document.body.offsetHeight, document.documentElement.clientHeight, document.documentElement.scrollHeight, document.documentElement.offsetHeight ).toString()" });

            int dh, wh;
            if (int.TryParse(docHeight, out dh) && int.TryParse(winHeight, out wh))
                view.Height = /*view.ActualHeight +*/ int.Parse(dogfhfcHeight);

            //var heightString = await view.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });
            //int height;
            //if (int.TryParse(heightString, out height))
            //{
            //    view.Height = height;
            //}

        }

        public static void SetResizeToFit(DependencyObject element, bool value)
        {
            element.SetValue(ResizeToFitProperty, value);
        }

        public static bool GetResizeToFit(DependencyObject element)
        {
            return (bool) element.GetValue(ResizeToFitProperty);
        }
    }
}
