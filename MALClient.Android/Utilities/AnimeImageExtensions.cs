using System;
using Android.Views;
using FFImageLoading;
using FFImageLoading.Views;
using MALClient.XShared.Utils;

namespace MALClient.Android
{
    public static class AnimeImageExtensions
    {
        public static void AnimeInto(this ImageViewAsync image,string originUrl)
        {
            if (Settings.PullHigherQualityImages)
            {
                var pos = originUrl.IndexOf(".jpg", StringComparison.InvariantCulture);
                if (pos == -1)
                    pos = originUrl.IndexOf(".webp", StringComparison.InvariantCulture);
                image.Visibility = ViewStates.Invisible;
                if (pos != -1)
                {
                    var uri = originUrl.Insert(pos, "l");
                    var work = ImageService.Instance.LoadUrl(uri);

                    work = work.Success(image.AnimateFadeIn);

                    image.Tag = originUrl;
                    work.Error(exception =>
                    {
                        ImageService.Instance.LoadUrl((string)image.Tag)
                            .FadeAnimation(false)
                            .Success(image.AnimateFadeIn)
                            .Into(image);
                    }).FadeAnimation(false).Into(image);
                }
                else if (!string.IsNullOrEmpty(originUrl))
                    ImageService.Instance.LoadUrl(originUrl)
                        .Success(image.AnimateFadeIn)
                        .FadeAnimation(false)
                        .Into(image);
            }
            else
            {
                ImageService.Instance.LoadUrl(originUrl)
                    .Success(image.AnimateFadeIn)
                    .FadeAnimation(false)
                    .Into(image);
            }

        }

        public static void Into(this ImageViewAsync image, string originUrl)
        {
            if (string.IsNullOrEmpty(originUrl))
                return;

            image.Visibility = ViewStates.Invisible;

            var work = ImageService.Instance.LoadUrl(originUrl);
            work = work.Success(image.AnimateFadeIn);
            image.Tag = originUrl;
            work.FadeAnimation(false).Into(image);
        }
    }
}