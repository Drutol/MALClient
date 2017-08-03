using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Models.Enums;
using MALClient.UWP.Shared.Managers;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Shared.UserControls.AttachedProperties
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
                var pos = source?.IndexOf(".jpg") ?? -1;
                if (pos == -1)
                    pos = source?.IndexOf(".webp") ?? -1;
                if (pos != -1)
                {
                    var uri = await ImageCache.GetFromCacheAsync(new Uri(source.Insert(pos, "l")));
                    img.ImageFailed += (sender, args) =>
                    {
                        img.Source = new BitmapImage(new Uri(source));
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

        /// <summary>
        /// In order by most freqent prefix
        /// </summary>
        private static readonly List<string> ProbablePrefixes = new List<string>
        {
            "9",
            "10",
            "6",
            "7",
            "11",
            "13",
            "2",
            "5",
            "4",
            "12",
            "3",
            "8",
            "1",
            "4",
            "14",
            "15",
            "16",
        };

        public static readonly DependencyProperty GuessedImageSourceProperty = DependencyProperty.RegisterAttached(
            "GuessedImageSource", typeof(int), typeof(AnimeImageExtensions), new PropertyMetadata(default(int),GuessedSourcePropertyChangedCallback));

        private static async void GuessedSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var id = (int)e.NewValue;

            var foundAbstraction =
                ResourceLocator.AnimeLibraryDataStorage.AllLoadedAnimeItemAbstractions.FirstOrDefault(
                    abstraction => abstraction.Id == id);
            if (foundAbstraction != null)
            {
                d.SetValue(AnimeImageExtensions.MalBaseImageSourceProperty, foundAbstraction.ImgUrl);
            }
            else
            {
                d.SetValue(AnimeImageExtensions.MalBaseImageSourceProperty, await AnimeImageQuery.GetImageUrl(id, GetAnimeSource(d)));
            }
        }

        public static void SetGuessedImageSource(DependencyObject element, int value)
        {
            element.SetValue(GuessedImageSourceProperty, value);
        }

        public static int GetGuessedImageSource(DependencyObject element)
        {
            return (int) element.GetValue(GuessedImageSourceProperty);
        }

        public static readonly DependencyProperty AnimeSourceProperty = DependencyProperty.RegisterAttached(
            "AnimeSource", typeof(bool), typeof(AnimeImageExtensions), new PropertyMetadata(true));

        public static void SetAnimeSource(DependencyObject element, bool value)
        {
            element.SetValue(AnimeSourceProperty, value);
        }

        public static bool GetAnimeSource(DependencyObject element)
        {
            return (bool) element.GetValue(AnimeSourceProperty);
        }
    }
}
