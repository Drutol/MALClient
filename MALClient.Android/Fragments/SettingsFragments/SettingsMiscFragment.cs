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
using MALClient.Android.Listeners;
using MALClient.Android.ViewModels;

namespace MALClient.Android.Fragments.SettingsFragments
{
    public class SettingsMiscFragment : MalFragmentBase
    {
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
        }

        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.RatePopUpEnable,
                    () => SettingsPageMiscEnableReviewReminder.Checked));

            Bindings.Add(
                this.SetBinding(() => ViewModel.AskBeforeSendingCrashReports,
                    () => SettingsPageMiscAskBeforeCrashReports.Checked));

            SettingsPageMiscPageRateNowButton.SetOnClickListener(new OnClickListener(view => ViewModel.ReviewCommand.Execute(null)));
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageMisc;

        #region Views

        private Switch _settingsPageMiscEnableReviewReminder;
        private Button _settingsPageMiscPageRateNowButton;
        private Switch _settingsPageMiscAskBeforeCrashReports;

        public Switch SettingsPageMiscEnableReviewReminder => _settingsPageMiscEnableReviewReminder ?? (_settingsPageMiscEnableReviewReminder = FindViewById<Switch>(Resource.Id.SettingsPageMiscEnableReviewReminder));

        public Button SettingsPageMiscPageRateNowButton => _settingsPageMiscPageRateNowButton ?? (_settingsPageMiscPageRateNowButton = FindViewById<Button>(Resource.Id.SettingsPageMiscPageRateNowButton));

        public Switch SettingsPageMiscAskBeforeCrashReports => _settingsPageMiscAskBeforeCrashReports ?? (_settingsPageMiscAskBeforeCrashReports = FindViewById<Switch>(Resource.Id.SettingsPageMiscAskBeforeCrashReports));



        #endregion
    }
}