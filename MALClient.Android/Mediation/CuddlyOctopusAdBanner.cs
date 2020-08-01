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

namespace MALClient.Android.Mediation
{
    
    [global::Android.Runtime.Preserve(AllMembers = true)]
    public class CuddlyOctopusAdBanner : Java.Lang.Object, ICustomEventBanner
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
                options = null;
            }

            var frame = new FrameLayout(context)
                {LayoutParameters = new FrameLayout.LayoutParams(-2, -1) {Gravity = GravityFlags.Center}};
            frame.SetPadding(0,DimensionsHelper.DpToPx(8),0,0);

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
                ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.ClickedCoAd);
                ResourceLocator.SystemControlsLauncherService.LaunchUri(
                    new Uri(options?.Link ?? "https://cuddlyoctopus.com/malclient/?sfw=1"));
            }));

            if (options?.ImageUrl == null)
            {
                banner.SetImageResource(Resource.Drawable.co_ad_banner);
            }
            else
            {
                ImageService.Instance.LoadUrl(options.ImageUrl, TimeSpan.FromDays(1)).Into(banner);
            }

            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.LoadedCoAd);
            listener.OnAdLoaded(frame);

            if (options?.AdDisplayTime > 0)
            {
                await Task.Delay(options.AdDisplayTime * 1000);
                listener.OnAdFailedToLoad(657);
            }
        }
    }
}