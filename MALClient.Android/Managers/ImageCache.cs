using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MALClient.Android.Managers
{
    public static class ImageCache
    {
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(3);

        public static async Task<Bitmap> GetImageBitmapFromUrl(string url)
        {
            await _semaphore.WaitAsync();
            Bitmap imageBitmap = null;

            using (var webClient = new HttpClient())
            {
                var response = await webClient.GetAsync(new Uri(url));
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            _semaphore.Release();
            return imageBitmap;
        }
    }
}