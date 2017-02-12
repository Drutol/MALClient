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

namespace MALClient.Android.Fragments.SettingsFragments
{
    public partial class SettingsFeedsFragment
    {
        private Switch _settingsPageFeedsAddPinnedProfilesSwitch;
        private TextView _settingsPageFeedsMaximumEntriesTextView;
        private SeekBar _settingsPageFeedsMaximumEntriesSlider;
        private TextView _settingsPageFeedsElderEntriesTextView;
        private SeekBar _settingsPageFeedsElderEntriesSlider;

        public Switch SettingsPageFeedsAddPinnedProfilesSwitch => _settingsPageFeedsAddPinnedProfilesSwitch ?? (_settingsPageFeedsAddPinnedProfilesSwitch = FindViewById<Switch>(Resource.Id.SettingsPageFeedsAddPinnedProfilesSwitch));

        public TextView SettingsPageFeedsMaximumEntriesTextView => _settingsPageFeedsMaximumEntriesTextView ?? (_settingsPageFeedsMaximumEntriesTextView = FindViewById<TextView>(Resource.Id.SettingsPageFeedsMaximumEntriesTextView));

        public SeekBar SettingsPageFeedsMaximumEntriesSlider => _settingsPageFeedsMaximumEntriesSlider ?? (_settingsPageFeedsMaximumEntriesSlider = FindViewById<SeekBar>(Resource.Id.SettingsPageFeedsMaximumEntriesSlider));

        public TextView SettingsPageFeedsElderEntriesTextView => _settingsPageFeedsElderEntriesTextView ?? (_settingsPageFeedsElderEntriesTextView = FindViewById<TextView>(Resource.Id.SettingsPageFeedsElderEntriesTextView));

        public SeekBar SettingsPageFeedsElderEntriesSlider => _settingsPageFeedsElderEntriesSlider ?? (_settingsPageFeedsElderEntriesSlider = FindViewById<SeekBar>(Resource.Id.SettingsPageFeedsElderEntriesSlider));
    }
}