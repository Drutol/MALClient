using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Ads;
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
    public partial class  MainActivity
    {
        private Timer _timer;
        private bool _initializedAds;
        private void InitAdContainer()
        {
            ViewModelLocator.Settings.OnAdsMinutesPerDayChanged += SettingsOnOnAdsMinutesPerDayChanged;

            Bindings.Add(this.SetBinding(() => ViewModel.AdsContainerVisibility)
                .WhenSourceChanges(() =>
                {
                    if (ViewModel.AdsContainerVisibility)
                    {
                        if (!_initializedAds)
                        {
                            MobileAds.Initialize(ApplicationContext, "ca-app-pub-8220174765620095~3319675764");
                            var adRequest = new AdRequest.Builder().AddTestDevice(AdRequest.DeviceIdEmulator).Build();
                            MainPageAdView.LoadAd(adRequest);
                            _initializedAds = true;
                        }
                        MainPageAdView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        MainPageAdView.Visibility = ViewStates.Gone;
                    }
                }));

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
    }
}