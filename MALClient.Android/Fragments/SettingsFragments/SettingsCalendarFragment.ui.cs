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
    public partial class SettingsCalendarFragment
    {
        private CheckBox _settingsPageCalendarBuildOptionsWatchingCheckBox;
        private CheckBox _settingsPageCalendarBuildOptionsPlanToWatchCheckBox;
        private RadioButton _settingsPageCalendarStartPageRadioSummary;
        private RadioButton _settingsPageCalendarStartPageRadioToday;
        private RadioGroup _settingsPageCalendarStartPageRadioGroup;
        private Switch _settingsPageCalendarMiscFirstDaySwitch;
        private Switch _settingsPageCalendarMiscRemoveEmptyDaysSwitch;
        //private Switch _settingsPageCalendarMiscExactAiringTimeSwitch;

        public CheckBox SettingsPageCalendarBuildOptionsWatchingCheckBox => _settingsPageCalendarBuildOptionsWatchingCheckBox ?? (_settingsPageCalendarBuildOptionsWatchingCheckBox = FindViewById<CheckBox>(Resource.Id.SettingsPageCalendarBuildOptionsWatchingCheckBox));

        public CheckBox SettingsPageCalendarBuildOptionsPlanToWatchCheckBox => _settingsPageCalendarBuildOptionsPlanToWatchCheckBox ?? (_settingsPageCalendarBuildOptionsPlanToWatchCheckBox = FindViewById<CheckBox>(Resource.Id.SettingsPageCalendarBuildOptionsPlanToWatchCheckBox));

        public RadioButton SettingsPageCalendarStartPageRadioSummary => _settingsPageCalendarStartPageRadioSummary ?? (_settingsPageCalendarStartPageRadioSummary = FindViewById<RadioButton>(Resource.Id.SettingsPageCalendarStartPageRadioSummary));

        public RadioButton SettingsPageCalendarStartPageRadioToday => _settingsPageCalendarStartPageRadioToday ?? (_settingsPageCalendarStartPageRadioToday = FindViewById<RadioButton>(Resource.Id.SettingsPageCalendarStartPageRadioToday));

        public RadioGroup SettingsPageCalendarStartPageRadioGroup => _settingsPageCalendarStartPageRadioGroup ?? (_settingsPageCalendarStartPageRadioGroup = FindViewById<RadioGroup>(Resource.Id.SettingsPageCalendarStartPageRadioGroup));

        public Switch SettingsPageCalendarMiscFirstDaySwitch => _settingsPageCalendarMiscFirstDaySwitch ?? (_settingsPageCalendarMiscFirstDaySwitch = FindViewById<Switch>(Resource.Id.SettingsPageCalendarMiscFirstDaySwitch));

        public Switch SettingsPageCalendarMiscRemoveEmptyDaysSwitch => _settingsPageCalendarMiscRemoveEmptyDaysSwitch ?? (_settingsPageCalendarMiscRemoveEmptyDaysSwitch = FindViewById<Switch>(Resource.Id.SettingsPageCalendarMiscRemoveEmptyDaysSwitch));

        //public Switch SettingsPageCalendarMiscExactAiringTimeSwitch => _settingsPageCalendarMiscExactAiringTimeSwitch ?? (_settingsPageCalendarMiscExactAiringTimeSwitch = FindViewById<Switch>(Resource.Id.SettingsPageCalendarMiscExactAiringTimeSwitch));
    }
}