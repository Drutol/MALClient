using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MALClient.Shared.UserControls.AttachedProperties
{
    public class MonitoredImageSourceExtension : DependencyObject
    {
        private const int MaxSimultaneousDownloadsAllowed = 3;

        private static readonly SemaphoreSlim DownloadSemaphore = new SemaphoreSlim(MaxSimultaneousDownloadsAllowed,
            MaxSimultaneousDownloadsAllowed);

        private static CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public static readonly DependencyProperty MonitoredSourceProperty = DependencyProperty.RegisterAttached(
            "MonitoredSource", typeof(Uri), typeof(MonitoredImageSourceExtension),
            new PropertyMetadata(default(Uri), PropertyChangedCallback));

        private static async void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                await DownloadSemaphore.WaitAsync(TimeSpan.FromSeconds(30), _tokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                TryRelease();
                return;
            }

            try
            {
                var img = d as Image;
                var bmp = img.Source as BitmapImage;
                if (bmp == null)
                {
                    bmp = new BitmapImage();
                    img.Source = bmp;
                }
                bmp.ImageOpened += BmpOnImageOpened;
                bmp.UriSource = e.NewValue as Uri;
            }
            catch (Exception)
            {
                TryRelease();
            }           
        }

        private static void BmpOnImageOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            TryRelease();
        }

        private static void TryRelease()
        {
            if (DownloadSemaphore.CurrentCount < MaxSimultaneousDownloadsAllowed)
                DownloadSemaphore.Release();
        }

        public static void SetMonitoredSource(DependencyObject element, Uri value)
        {
            element.SetValue(MonitoredSourceProperty, value);
        }

        public static Uri GetMonitoredSource(DependencyObject element)
        {
            return (Uri) element.GetValue(MonitoredSourceProperty);
        }

        public static void ResetImageQueue()
        {
            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
        }

    }
}
