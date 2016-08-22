using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.XShared.Utils;

namespace MalClient.Shared.UserControls.AttachedProperties
{
    public class AnimeImageExtensions : DependencyObject
    {
        public static readonly DependencyProperty MalBaseImageSourceProperty =
          DependencyProperty.RegisterAttached(
              "MalBaseImageSource",
              typeof(string),
              typeof(AnimeImageExtensions),
              new PropertyMetadata("", PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var img = d as Image;
            var source = e.NewValue as string;
            if (Settings.PullHigherQualityImages)
            {
                var pos = source.IndexOf(".jpg");
                if (pos != -1)
                {
                    img.ImageFailed += ImgOnImageFailed;
                    img.Source = new BitmapImage(new Uri(source.Insert(pos, "l")));
                }
            }
            else
                img.Source = new BitmapImage(new Uri(source));

        }

        private static void ImgOnImageFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
        {
            var img = sender as Image;
            img.ImageFailed -= ImgOnImageFailed;
            img.Source = new BitmapImage(new Uri(GetMalBaseImageSource(img)));
     
        }

        public static void SetMalBaseImageSource(UIElement element, string value)
        {
            element.SetValue(MalBaseImageSourceProperty, value);
        }

        public static string GetMalBaseImageSource(UIElement element)
        {
            return (string)element.GetValue(MalBaseImageSourceProperty);
        }
    }
}
