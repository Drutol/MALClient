using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Reward;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Activities
{
    public partial class  MainActivity : IRewardedVideoAdListener
    {
        private IRewardedVideoAd _videoAd;
        private Timer _timer;
        private bool _initializedAds;
        private bool _adLoaded;

        public bool AdLoaded
        {
            get => _adLoaded;
            set
            {
                _adLoaded = value;
                MainPageAdView.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        private void InitAdContainer()
        {
            ViewModelLocator.Settings.OnAdsMinutesPerDayChanged += SettingsOnOnAdsMinutesPerDayChanged;


            Bindings.Add(this.SetBinding(() => ViewModel.AdsContainerVisibility)
                .WhenSourceChanges(() =>
                {
                    if (ViewModel.AdsContainerVisibility && Credentials.Authenticated)
                    {
                        if (!_initializedAds)
                        {
                            MobileAds.Initialize(ApplicationContext, "ca-app-pub-8220174765620095~3319675764");
                            var adRequest = new AdRequest.Builder()
                                .AddKeyword("anime")
                                .AddKeyword("watch")
                                .AddKeyword("manga")
                                .AddKeyword("hobby")
                                .AddKeyword("show")
                                .AddKeyword("comic")
                                .AddKeyword("book")
                                .AddKeyword("tv").Build();
                            MainPageAdView.LoadAd(adRequest);
                            MainPageAdView.AdListener = new AdsListener(this);
                            _initializedAds = true;
                        }
                        MainPageAdView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        MainPageAdView.Visibility = ViewStates.Gone;
                    }
                }));

            _videoAd = MobileAds.GetRewardedVideoAdInstance(this);
            _videoAd.RewardedVideoAdListener = this;

            StartAdsTimeMeasurements();
        }

        private void SettingsOnOnAdsMinutesPerDayChanged()
        {
            if (Settings.AdsEnable)
            {
                var passed = (int)(ResourceLocator.ApplicationDataService["AdsTimeToday"] ?? 0);
                _timer?.Dispose();
                if (passed < Settings.AdsSecondsPerDay || Settings.AdsSecondsPerDay == 0)
                {
                    ViewModelLocator.GeneralMain.AdsContainerVisibility = true;
                    _timer = new Timer(AdTimerCallback, null, 0, 10000);
                }
                else
                {
                    ViewModelLocator.GeneralMain.AdsContainerVisibility = false;
                }
            }
            else if (_timer != null && !Settings.AdsEnable)
            {
                ViewModelLocator.GeneralMain.AdsContainerVisibility = false;
                _timer?.Dispose();
                _timer = null;
            }
            else if (!Settings.AdsEnable)
                ViewModelLocator.GeneralMain.AdsContainerVisibility = false;
        }

        private void StartAdsTimeMeasurements()
        {
            var day = ResourceLocator.ApplicationDataService["AdsCurrentDay"];
            if (day != null)
            {
                if ((int)day != DateTime.Today.DayOfYear)
                    ResourceLocator.ApplicationDataService["AdsTimeToday"] = 0;
            }
            ResourceLocator.ApplicationDataService["AdsCurrentDay"] = DateTime.Today.DayOfYear;
            if (Settings.AdsEnable)
            {
                _timer = new Timer(AdTimerCallback, null, 0, 10000);
                ViewModelLocator.GeneralMain.AdsContainerVisibility = true;
            }
            else
            {
                MainPageAdView.Pause();
                MainPageAdView.Visibility = ViewStates.Gone;
            }
        }

        private void AdTimerCallback(object state)
        {
            if(!AdLoaded)
                return;

            var passed = (int)(ResourceLocator.ApplicationDataService["AdsTimeToday"] ?? 0);
            passed += 10;
            RunOnUiThread(() => MainPageAdView.Resume());
            ResourceLocator.ApplicationDataService["AdsTimeToday"] = passed;
            if (!Settings.AdsEnable || (Settings.AdsSecondsPerDay != 0 && passed > Settings.AdsSecondsPerDay))
            {
                 RunOnUiThread(() => ViewModelLocator.GeneralMain.AdsContainerVisibility = false);
                _timer?.Dispose();
                _timer = null;
            }
        }

        class AdsListener : AdListener
        {
            private MainActivity _parent;

            public AdsListener(MainActivity parent)
            {
                _parent = parent;
            }

            public override void OnAdLoaded()
            {
                _parent.AdLoaded = true;
                base.OnAdLoaded();
            }

            public override void OnAdFailedToLoad(int errorCode)
            {
                _parent.AdLoaded = false;
                base.OnAdFailedToLoad(errorCode);
            }
        }

        #region VideoAd
        private void DisplayVideoAd()
        {
#if DEBUG
            _videoAd.LoadAd("ca-app-pub-3940256099942544/5224354917", new AdRequest.Builder().Build());
#else
            _videoAd.LoadAd("ca-app-pub-8220174765620095/2143765819",new AdRequest.Builder().Build());
#endif
            ResourceLocator.SnackbarProvider.ShowText("Ad is now loading!");
        }

        public void OnRewarded(IRewardItem reward)
        {
            ResourceLocator.SnackbarProvider.ShowText("Thanks for support! ^^");
        }

        public void OnRewardedVideoAdClosed()
        {

        }

        public void OnRewardedVideoAdFailedToLoad(int errorCode)
        {
            ResourceLocator.SnackbarProvider.ShowText("Welp, google has some problems and cannot serve an ad :( Thanks for trying though!");
        }

        public void OnRewardedVideoAdLeftApplication()
        {

        }

        public void OnRewardedVideoAdLoaded()
        {
            _videoAd.Show();
        }

        public void OnRewardedVideoAdOpened()
        {

        }

        public void OnRewardedVideoStarted()
        {

        }
        #endregion


    }
}