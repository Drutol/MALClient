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
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Android.Listeners;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.Android.Mediation
{
    [Preserve(AllMembers = true)]
    public class DakimakuriAdBanner : Java.Lang.Object, ICustomEventBanner
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
            //MediationOptions options;

            //try
            //{
            //    options = JsonConvert.DeserializeObject<MediationOptions>(serverParameter);
            //}
            //catch
            //{
            //    options = null;
            //}

            //if (options == null)
            //{
            //    var banner = new ImageView(context) {LayoutParameters = new ViewGroup.LayoutParams(-2, -2)};

            //    banner.SetOnClickListener(new OnClickListener(view =>
            //    {
            //        listener.OnAdClicked();
            //        ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://cuddlyoctopus.com/"));
            //    }));
            //    banner.SetImageResource(Resource.Drawable.test_banner);
            //    listener.OnAdLoaded(banner);

            //    if (options.AdDisplayTime > 0)
            //    {
            //        await Task.Delay(options.AdDisplayTime);
            //        listener.OnAdFailedToLoad(657);
            //    }
            //}

        }
    }
}