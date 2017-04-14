using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using MALClient.XShared.Utils;

namespace MALClient.Android
{
    public static class AnimeImageExtensions
    {
        private static readonly HashSet<string> LoadedImgs = new HashSet<string>();
        private static readonly HashSet<string> FailedImgs = new HashSet<string>();

        private static string GetImgUrl(string originUrl)
        {
            if (Settings.PullHigherQualityImages && !FailedImgs.Contains(originUrl))
            {
                var pos = originUrl.IndexOf(".jpg", StringComparison.InvariantCulture);
                if (pos == -1)
                    pos = originUrl.IndexOf(".webp", StringComparison.InvariantCulture);

                if (pos != -1)
                    return originUrl.Insert(pos, "l");
                return originUrl;
            }
            return originUrl;
        }

        public static bool AnimeIntoIfLoaded(this ImageViewAsync image, string originUrl)
        {
            var url = GetImgUrl(originUrl);
            if (LoadedImgs.Contains(url))
            {
                LoadImage(image,originUrl,url,true);
                return true;
            }
            return false;
        }

        public static void AnimeInto(this ImageViewAsync image, string originUrl)
        {
            var url = GetImgUrl(originUrl);
            LoadImage(image,originUrl,url,LoadedImgs.Contains(url));
        }

        private static void LoadImage(ImageViewAsync image, string originUrl, string targetUrl,
            bool? imgLoaded)
        {
            if (string.IsNullOrEmpty(targetUrl) || string.IsNullOrEmpty(originUrl))
                return;

            var work = ImageService.Instance.LoadUrl(targetUrl);
            if (imgLoaded != true || !LoadedImgs.Contains(targetUrl))
            {
                image.Visibility = ViewStates.Invisible;
                work = work.Success(image.AnimateFadeIn);
                LoadedImgs.Add(targetUrl);
            }
            else
            {
                image.Visibility = ViewStates.Visible;
            }

            //we can fallback to lower quality image
            if (!originUrl.Equals(targetUrl))
            {
                image.Tag = originUrl;
                work.Error(exception =>
                {
                    var img = (string) image.Tag;
                    ImageService.Instance.LoadUrl(img)
                        .FadeAnimation(false)
                        .Into(image);
                    FailedImgs.Add(img);
                    LoadedImgs.Add(img);
                });
            }

            work.FadeAnimation(false).Into(image);
        }

        public static void Into(this ImageViewAsync image, string originUrl, ITransformation transformation = null,Action<ImageViewAsync> onCompleted = null)
        {
            if (string.IsNullOrEmpty(originUrl))
                return;

            image.Visibility = ViewStates.Invisible;
            try
            {
                var work = ImageService.Instance.LoadUrl(originUrl);
                work = work.Success(() => 
                {
                    image.AnimateFadeIn();
                    onCompleted?.Invoke(image);
                });
                if (transformation == null)
                    work.FadeAnimation(false).Into(image);
                else
                    work.FadeAnimation(false).Transform(transformation).Into(image);
            }
            catch (Exception)
            {
                //TODO Throws aggregate exception for some reason
            }

        }

        public static void HandleScaling(this ImageViewAsync image,float threshold = .4f)
        {
            var bounds = image.Drawable.Bounds;
            if (bounds.Right == 0 || image.Width == 0)
            {
                image.SetScaleType(ImageView.ScaleType.CenterCrop);
                return;
            }
            if (
                Math.Abs(image.Height / (float)image.Width -
                         bounds.Bottom / (float)bounds.Right) > threshold)
            {
                image.SetScaleType(ImageView.ScaleType.FitCenter);
            }
            else
            {
                image.SetScaleType(ImageView.ScaleType.CenterCrop);
            }
        }

        private static readonly Dictionary<View, CancellationTokenSource> CancellationTokenSources = new Dictionary<View, CancellationTokenSource>();
        public static async void IntoWithTask(this ImageViewAsync image, Task<string> originUrlTask, ITransformation transformation = null)
        {
            CancellationToken token;
            lock (CancellationTokenSources)
            {
                if (CancellationTokenSources.ContainsKey(image))
                {
                    CancellationTokenSources[image].Cancel();
                    CancellationTokenSources.Remove(image);
                }

                var src = new CancellationTokenSource();
                CancellationTokenSources.Add(image, src);
                token = src.Token;
            }

            var originUrl = await originUrlTask;

            if(token.IsCancellationRequested)
                return;

            lock (CancellationTokenSources)
            {
                CancellationTokenSources.Remove(image);
            }

            if (string.IsNullOrEmpty(originUrl))
                return;

            image.Visibility = ViewStates.Invisible;

            var work = ImageService.Instance.LoadUrl(originUrl);
            work = work.Success(image.AnimateFadeIn);
            if(transformation == null)
                work.FadeAnimation(false).Into(image);
            else
                work.FadeAnimation(false).Transform(transformation).Into(image);
        }
    }
}