using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
using Com.Mopub.Mobileads;
using FFImageLoading;
using FFImageLoading.Views;
using MALClient.Android.Activities;
using MALClient.Android.Listeners;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;
using Messenger = GalaSoft.MvvmLight.Messaging.Messenger;
using PreserveAttribute = Android.Runtime.PreserveAttribute;

namespace MALClient.Android.Mediation
{
    [Preserve(AllMembers = true)]
    public class MoPubAdBanner : Java.Lang.Object, ICustomEventBanner
    {
        private MoPubView _adBanner;

        public void OnDestroy()
        {
            _adBanner?.Destroy();
        }

        public void OnPause()
        {

        }

        public void OnResume()
        {

        }

        public async void RequestBannerAd(Context context,
            ICustomEventBannerListener listener,
            string serverParameter,
            AdSize size,
            IMediationAdRequest mediationAdRequest,
            Bundle customEventExtras)
        {
            await MainActivity.CurrentContext.MopubSemaphore.WaitAsync();
            _adBanner = new MoPubView(context)
            {
                AdUnitId = "9d83d5eb18444b859eb2a8a6d7110f65",
                LayoutParameters = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent, FrameLayout.LayoutParams.WrapContent) { Gravity = GravityFlags.Center }
            };
            _adBanner.BannerAdListener = new MoPubCustomEventBannerForwarder(listener, _adBanner);
            _adBanner.LoadAd();
        }
    }
}