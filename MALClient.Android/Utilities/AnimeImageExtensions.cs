using System;
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
                if (pos != -1)
                {
                    var uri = originUrl.Insert(pos, "l");
                    var work = ImageService.Instance.LoadUrl(uri);
                    image.Tag = originUrl;
                    work.Error(exception =>
                    {
                        ImageService.Instance.LoadUrl((string)image.Tag)
                            .FadeAnimation(false)
                            .Success(image.AnimateFadeIn)
                            .Into(image);
                    }).FadeAnimation(false).Success(image.AnimateFadeIn).Into(image);
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
    }
}