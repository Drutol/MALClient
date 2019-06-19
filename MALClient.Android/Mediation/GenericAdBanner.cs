using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Mediation;
using Android.Gms.Ads.Mediation.CustomEvent;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using MALClient.Android.Listeners;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;
using Messenger = GalaSoft.MvvmLight.Messaging.Messenger;
using PreserveAttribute = Android.Runtime.PreserveAttribute;

namespace MALClient.Android.Mediation
{
    [Preserve(AllMembers = true)]
    public class GenericAdBanner : Java.Lang.Object, ICustomEventBanner
    {
        public void OnDestroy()
        {

        }

        public void OnPause()
        {

        }

        public void OnResume()
        {

        }

        public async void RequestBannerAd(
            Context context,
            ICustomEventBannerListener listener,
            string serverParameter,
            AdSize size,
            IMediationAdRequest mediationAdRequest,
            Bundle customEventExtras)
        {
            MediationOptions options;

            try
            {
                options = JsonConvert.DeserializeObject<MediationOptions>(serverParameter);
            }
            catch
            {
                listener.OnAdFailedToLoad(1);
                return;
            }

            var frame = new FrameLayout(context)
            { LayoutParameters = new FrameLayout.LayoutParams(-2, -1) { Gravity = GravityFlags.Center } };
            frame.SetPadding(0, DimensionsHelper.DpToPx(8), 0, 0);

            var banner =
                new ImageViewAsync(context)
                {
                    LayoutParameters = new FrameLayout.LayoutParams(context.Resources.DisplayMetrics.WidthPixels, -1)
                    {
                        Gravity = GravityFlags.Center,
                    },
                };
            banner.SetScaleType(ImageView.ScaleType.CenterInside);
            banner.SetAdjustViewBounds(true);
            banner.SetMaxHeight(DimensionsHelper.DpToPx(100));

            frame.AddView(banner);

            banner.SetOnClickListener(new OnClickListener(view =>
            {
                listener.OnAdClicked();
                ResourceLocator.TelemetryProvider.LogEvent($"{options.Label}Clicked");
                ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri(options.Link));
            }));


            if (options?.ImageUrls?.Any() ?? false)
            {
                var random = new Random();

                ImageService.Instance.LoadUrl(options.ImageUrls[random.Next(0, options.ImageUrls.Length)], TimeSpan.FromDays(3)).Into(banner);
            }
            else if (options?.ImageUrl != null)
            {
                ImageService.Instance.LoadUrl(options.ImageUrl, TimeSpan.FromDays(3)).Into(banner);
            }
            else
            {
                banner.SetImageResource(Resource.Drawable.annak);
            }

            ResourceLocator.TelemetryProvider.LogEvent($"{options.Label}Loaded");
            listener.OnAdLoaded(frame);
        }
    }
}