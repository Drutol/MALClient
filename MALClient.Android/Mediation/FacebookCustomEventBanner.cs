//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.Gms.Ads.Mediation;
//using Android.Gms.Ads.Mediation.CustomEvent;
//using Android.Graphics;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Xamarin.Facebook.Ads;
//using AdSize = Android.Gms.Ads.AdSize;
//using AdView = Xamarin.Facebook.Ads.AdView;

//namespace MALClient.Android.Mediation
//{
//    [Preserve(AllMembers = true)]
//    public class FacebookCustomEventBanner : Java.Lang.Object, ICustomEventBanner
//    {
//        public static event EventHandler<AdView> NewAd;

//        private AdView _adBanner;

//        public void OnDestroy()
//        {
//            _adBanner?.Destroy();
//        }

//        public void OnPause()
//        {

//        }

//        public void OnResume()
//        {

//        }

//        public void RequestBannerAd(Context context,
//            ICustomEventBannerListener listener,
//            string serverParameter,
//            AdSize size,
//            IMediationAdRequest mediationAdRequest,
//            Bundle customEventExtras)
//        {
//            _adBanner = new AdView(context, "967581563434898_967583140101407", Xamarin.Facebook.Ads.AdSize.BannerHeight50);
//            NewAd?.Invoke(this, _adBanner);
//            _adBanner.SetAdListener(new FacebookCustomEventBannerForwarder(listener, _adBanner));
//            _adBanner.LoadAd();
//        }
//    }
//}