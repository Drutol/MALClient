using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads.Mediation.CustomEvent;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Mopub.Mobileads;

namespace MALClient.Android.Mediation
{
    class MoPubCustomEventBannerForwarder : Java.Lang.Object, MoPubView.IBannerAdListener
    {
        private readonly ICustomEventBannerListener _listener;
        private readonly MoPubView _adBanner;

        public MoPubCustomEventBannerForwarder(ICustomEventBannerListener listener, MoPubView adBanner)
        {
            _listener = listener;
            _adBanner = adBanner;
        }

        public void OnBannerClicked(MoPubView p0)
        {
            _listener.OnAdClicked();
        }

        public void OnBannerCollapsed(MoPubView p0)
        {
            
        }

        public void OnBannerExpanded(MoPubView p0)
        {
            
        }

        public void OnBannerFailed(MoPubView p0, MoPubErrorCode p1)
        {
            _listener.OnAdFailedToLoad(p1.IntCode);
        }

        public void OnBannerLoaded(MoPubView p0)
        {
            _listener.OnAdLoaded(_adBanner);
        }
    }
}