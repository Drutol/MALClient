using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Models.Enums;
using MALClient.Shared.Managers;
using MALClient.XShared.Utils;

namespace MALClient.Shared.UserControls.AttachedProperties
{
    public class AnimeImageExtensions : DependencyObject
    {
        public static readonly DependencyProperty ForceStandardImageProperty = DependencyProperty.RegisterAttached(
            "ForceStandardImage", typeof(bool), typeof(AnimeImageExtensions), new PropertyMetadata(false));

        public static void SetForceStandardImage(DependencyObject element, bool value)
        {
            element.SetValue(ForceStandardImageProperty, value);
        }

        public static bool GetForceStandardImage(DependencyObject element)
        {
            return (bool) element.GetValue(ForceStandardImageProperty);
        }

        public static readonly DependencyProperty MalBaseImageSourceProperty =
            DependencyProperty.RegisterAttached(
                "MalBaseImageSource",
                typeof(string),
                typeof(AnimeImageExtensions),
                new PropertyMetadata("", PropertyChangedCallback));

        private static async void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var img = d as Image;
            var source = e.NewValue as string;
            if (Settings.PullHigherQualityImages && Settings.SelectedApiType != ApiType.Hummingbird && !GetForceStandardImage(d))
            {
                var pos = source.IndexOf(".jpg");
                if (pos != -1)
                {
                    var uri = await ImageCache.GetFromCacheAsync(new Uri(source.Insert(pos, "l")));
                    img.ImageFailed += async (sender, args) =>
                    {
                        img.Source = new BitmapImage(await ImageCache.GetFromCacheAsync(new Uri(source)));
                    };
                    img.Source = new BitmapImage(uri);
                }
                else if (!string.IsNullOrEmpty(source))
                    img.Source = new BitmapImage(await ImageCache.GetFromCacheAsync(new Uri(source)));
                else
                    img.Source = null;
            }
            else if (!string.IsNullOrEmpty(source))
                img.Source = new BitmapImage(await ImageCache.GetFromCacheAsync(new Uri(source)));
            else
                img.Source = null;

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
