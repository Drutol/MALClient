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
    public partial class SettingsFeedsFragment : MalFragmentBase
    {
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
        }

        protected override void InitBindings()
        {
            Bindings.Add(SettingsPageFeedsAddPinnedProfilesSwitch.Id, new List<Binding>());
            Bindings[SettingsPageFeedsAddPinnedProfilesSwitch.Id].Add(
                this.SetBinding(() => ViewModel.FeedsIncludePinnedProfiles,
                    () => SettingsPageFeedsAddPinnedProfilesSwitch.Checked, BindingMode.TwoWay));
            //
            SettingsPageFeedsMaximumEntriesSlider.Progress = Settings.FeedsMaxEntries - 5;
            SettingsPageFeedsMaximumEntriesTextView.Text = (Settings.FeedsMaxEntries).ToString();

            SettingsPageFeedsMaximumEntriesSlider.ProgressChanged += (sender, args) =>
            {
                Settings.FeedsMaxEntries = SettingsPageFeedsMaximumEntriesSlider.Progress + 5;
                SettingsPageFeedsMaximumEntriesTextView.Text =
                    (SettingsPageFeedsMaximumEntriesSlider.Progress + 5).ToString();
            };
            //
            SettingsPageFeedsElderEntriesSlider.Progress = Settings.FeedsMaxEntryAge - 3;
            SettingsPageFeedsElderEntriesTextView.Text = (Settings.FeedsMaxEntryAge).ToString();

            SettingsPageFeedsElderEntriesSlider.ProgressChanged += (sender, args) =>
            {
                Settings.FeedsMaxEntryAge = SettingsPageFeedsElderEntriesSlider.Progress + 3;
                SettingsPageFeedsElderEntriesTextView.Text =
                    (SettingsPageFeedsElderEntriesSlider.Progress + 3).ToString();
            };
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageFeeds;
    }
}
 