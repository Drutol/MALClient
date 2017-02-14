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
    public partial class SettingsAdsFragment : MalFragmentBase
    {
        private LinearLayout _settingsPageAdsEnableAdsSwitch;
        private Spinner _settingsPageAdsMinutesDailySpinner;

        public LinearLayout SettingsPageAdsEnableAdsSwitch => _settingsPageAdsEnableAdsSwitch ?? (_settingsPageAdsEnableAdsSwitch = FindViewById<LinearLayout>(Resource.Id.SettingsPageAdsEnableAdsSwitch));

        public Spinner SettingsPageAdsMinutesDailySpinner => _settingsPageAdsMinutesDailySpinner ?? (_settingsPageAdsMinutesDailySpinner = FindViewById<Spinner>(Resource.Id.SettingsPageAdsMinutesDailySpinner));
        
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
        }

        protected override void InitBindings()
        {
            SettingsPageAdsEnableAdsSwitch.Click += (sender, args) =>
            {
                ViewModel.AdsEnable = (sender as Switch).Checked;
            };
            //
            List<int> availableTimes = new List<int>() { 0, 5, 10, 15, 20, 30 };
            SettingsPageAdsMinutesDailySpinner.Adapter = availableTimes.GetAdapter((i, i1, arg3) =>
            {
                var view = arg3;
                var text = i1 == 0 ? $"Indefinietly" : $"{i1} minutes";
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
            SettingsPageAdsMinutesDailySpinner.SetSelection(availableTimes.IndexOf(ViewModel.NotificationsRefreshTime));
            SettingsPageAdsMinutesDailySpinner.ItemSelected += (sender, args) =>
            {
                ViewModel.AdsMinutesPerDay = (int)SettingsPageAdsMinutesDailySpinner.SelectedView.Tag;
            };
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageAds;

    }
}