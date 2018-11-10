using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Android.ViewModels;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.SettingsFragments
{
    public class SettingsAdsFragment : MalFragmentBase
    {          
        private SettingsViewModel ViewModel;
        private bool _settingAds;
        private bool _initialized;

        public SettingsAdsFragment()
        {

        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
        }

        protected override void InitBindings()
        {
            if(_initialized)
                return;

            _initialized = true;

            SettingsPageAdsEnableAdsSwitch.Checked = ViewModel.AdsEnable;
            SettingsPageAdsEnableAdsSwitch.CheckedChange += (sender, args) =>
            {
                if(_settingAds)
                    return;
                
                if (ViewModel.AdsEnable)
                {
                    _settingAds = true;
                    ResourceLocator.MessageDialogProvider.ShowMessageDialogWithInput("Well, pity to see you go :(",
                        "Are you sure?", "Yup", "Nope", () =>
                        {
                            ViewModel.AdsEnable = false;
                            _settingAds = false;
                        },
                        () =>
                        {
                            SettingsPageAdsEnableAdsSwitch.Checked = true;
                            _settingAds = false;
                        });
                }
                else
                {
                    _settingAds = true;
                    ViewModel.AdsEnable = true;
                    _settingAds = false;
                }

            };

            //
            List<int> availableTimes = new List<int>() { 0, 5, 10, 15, 20, 30 };
            SettingsPageAdsMinutesDailySpinner.Adapter = availableTimes.GetAdapter((i, i1, arg3) =>
            {
                var view = arg3;
                var text = i1 == 0 ? "Indefinietly" : $"{i1} minutes";
                if (view == null)
                {
                    view = AnimeListPageFlyoutBuilder.BuildBaseItem(Activity, text,
                        ResourceExtension.BrushAnimeItemInnerBackground, null, false, GravityFlags.Center);
                }
                else
                {
                    view.FindViewById<TextView>(AnimeListPageFlyoutBuilder.TextViewTag).Text = text;
                }
                view.Tag = i1;
                return view;
            });
            SettingsPageAdsMinutesDailySpinner.SetSelection(availableTimes.IndexOf(ViewModel.AdsSecondsPerDay/60));
            SettingsPageAdsMinutesDailySpinner.ItemSelected += (sender, args) =>
            {
                ViewModel.AdsSecondsPerDay = (int)SettingsPageAdsMinutesDailySpinner.SelectedView.Tag*60;
            };
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageAds;


        #region Views

        private CheckBox _settingsPageAdsEnableAdsSwitch;
        private Spinner _settingsPageAdsMinutesDailySpinner;

        public CheckBox SettingsPageAdsEnableAdsSwitch => _settingsPageAdsEnableAdsSwitch ?? (_settingsPageAdsEnableAdsSwitch = FindViewById<CheckBox>(Resource.Id.SettingsPageAdsEnableAdsSwitch));

        public Spinner SettingsPageAdsMinutesDailySpinner => _settingsPageAdsMinutesDailySpinner ?? (_settingsPageAdsMinutesDailySpinner = FindViewById<Spinner>(Resource.Id.SettingsPageAdsMinutesDailySpinner));


        #endregion
    }
}