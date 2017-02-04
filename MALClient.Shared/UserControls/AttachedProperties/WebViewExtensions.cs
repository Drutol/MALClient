using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MALClient.Shared.Managers;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Shared.UserControls.AttachedProperties
{
    public class WebViewExtensions : DependencyObject
    {
    
        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
            "Content", typeof(string), typeof(WebViewExtensions), new PropertyMetadata(default(string),PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var view = dependencyObject as WebView;
            if (GetResizeToFit(view))
            {
                if (view.Tag == null)
                {
                    view.ScriptNotify += ViewOnScriptNotify;
                    view.Tag = true;
                }
            }
            SetComputedHeight(view,0);
            view.NavigateToString(CssManager.WrapWithCss(dependencyPropertyChangedEventArgs.NewValue as string,GetDisableScroll(dependencyObject)));
        }

        private static async void ViewOnScriptNotify(object sender, NotifyEventArgs e)
        {
            var view = sender as WebView;
            double val;
            if(!double.TryParse(e.Value,out val))
                return;

           // val = val / 72 * _displayInformation.LogicalDpi;
            val *= 1.1;
           // val *= _scaleCoefficient;
            if (view.ActualHeight == 0) // let's recheck
            {
                await Task.Delay(1000);
                try
                {
                    await view.InvokeScriptAsync("eval", new[] { @"notifyDocumentHeightChanged('content');" });
                }
                catch (Exception)
                {
                    //null content
                }
                return;
            }
            if (view.DataContext == null || Math.Abs(val - GetComputedHeight(view)) < .1)
                return;

            if (val > view.ActualHeight)
            {
                SetComputedHeight(view, val);
                //Debug.WriteLine($"{(view.DataContext as ForumTopicMessageEntryViewModel)?.Data.MessageNumber} - {val}");
            }
            else
            {
                SetComputedHeight(view, view.ActualHeight);
            }

        }

        public static void SetContent(DependencyObject element, string value)
        {
            element.SetValue(ContentProperty, value);
        }

        public static string GetContent(DependencyObject element)
        {
            return (string) element.GetValue(ContentProperty);
        }

        public static readonly DependencyProperty DisableScrollProperty = DependencyProperty.RegisterAttached(
            "DisableScroll", typeof(bool), typeof(WebViewExtensions), new PropertyMetadata(default(bool)));

        public static void SetDisableScroll(DependencyObject element, bool value)
        {
            element.SetValue(DisableScrollProperty, value);
        }

        public static bool GetDisableScroll(DependencyObject element)
        {
            return (bool) element.GetValue(DisableScrollProperty);
        }

        public static readonly DependencyProperty ResizeToFitProperty = DependencyProperty.RegisterAttached(
            "ResizeToFit", typeof(bool), typeof(WebViewExtensions), new PropertyMetadata(default(bool)));

        public static void SetResizeToFit(DependencyObject element, bool value)
        {
            element.SetValue(ResizeToFitProperty, value);
        }

        public static bool GetResizeToFit(DependencyObject element)
        {
            return (bool) element.GetValue(ResizeToFitProperty);
        }

        public static readonly DependencyProperty ComputedHeightProperty = DependencyProperty.RegisterAttached(
            "ComputedHeight", typeof(double), typeof(WebViewExtensions), new PropertyMetadata(default(double)));


        public static void SetComputedHeight(DependencyObject element, double value)
        {
            element.SetValue(ComputedHeightProperty, value);
        }

        public static double GetComputedHeight(DependencyObject element)
        {
            return (double) element.GetValue(ComputedHeightProperty);
        }
    }
}
