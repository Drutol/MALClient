using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Mediation.CustomEvent;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Facebook.Ads;
using AdView = Xamarin.Facebook.Ads.AdView;

namespace MALClient.Android.Mediation
{
    [Preserve(AllMembers = true)]
    public class FacebookCustomEventBannerForwarder : Java.Lang.Object, IAdListener
    {
        private const string TAG = "FbAdsForwarder";
        private ICustomEventBannerListener mBannerListener;
        private AdView mAdView;

        public FacebookCustomEventBannerForwarder(
            ICustomEventBannerListener listener, AdView adView)
        {
            this.mBannerListener = listener;
            this.mAdView = adView;
        }

        public void OnAdLoaded(IAd ad)
        {
            Log.Debug(TAG, "FacebookCustomEventBanner loaded!");
            mBannerListener.OnAdLoaded(mAdView);
        }

        public void OnAdClicked(IAd ad)
        {
            Log.Debug(TAG, "FacebookCustomEventBanner clicked!");
            mBannerListener.OnAdClicked();
            mBannerListener.OnAdOpened();
            mBannerListener.OnAdLeftApplication();
        }

        public void OnError(IAd ad, AdError error)
        {
            Log.Debug(TAG, "FacebookCustomEventBanner Error:" + error.ErrorMessage);
            switch (error.ErrorCode)
            {
                case AdError.InternalErrorCode:
                    mBannerListener.OnAdFailedToLoad(AdRequest.ErrorCodeInternalError);
                    break;
                case AdError.NetworkErrorCode:
                    mBannerListener.OnAdFailedToLoad(AdRequest.ErrorCodeNetworkError);
                    break;
                default:
                    mBannerListener.OnAdFailedToLoad(AdRequest.ErrorCodeNoFill);
                    break;
            }
        }

        public void OnLoggingImpression(IAd ad)
        {
            Log.Debug(TAG, "FacebookCustomEventBanner impression logged!");
        }
    }
}