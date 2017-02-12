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
    public partial class SettingsCalendarFragment : MalFragmentBase
    {
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
        }

        protected override void InitBindings()
        {
            Bindings.Add(SettingsPageCalendarBuildOptionsWatchingCheckBox.Id, new List<Binding>());
            Bindings[_settingsPageCalendarBuildOptionsWatchingCheckBox.Id].Add(
                this.SetBinding(() => ViewModel.CalendarIncludeWatching,
                    () => _settingsPageCalendarBuildOptionsWatchingCheckBox.Checked, BindingMode.TwoWay));
            Bindings[SettingsPageCalendarBuildOptionsWatchingCheckBox.Id].Add(
                this.SetBinding(() => ViewModel.CalendarIncludePlanned,
                    () => _settingsPageCalendarBuildOptionsPlanToWatchCheckBox.Checked, BindingMode.TwoWay));
            //
            SettingsPageCalendarStartPageRadioGroup.Check(Settings.CalendarStartOnToday == true
                ? SettingsPageCalendarStartPageRadioToday.Id
                : SettingsPageCalendarStartPageRadioSummary.Id);
            SettingsPageCalendarStartPageRadioGroup.SetOnCheckedChangeListener(new OnCheckedListener(i =>
            {
                Settings.CalendarStartOnToday = i == SettingsPageCalendarStartPageRadioToday.Id ? true : false;
            }));
            //
            Bindings.Add(SettingsPageCalendarMiscFirstDaySwitch.Id, new List<Binding>());
            Bindings[SettingsPageCalendarMiscFirstDaySwitch.Id].Add(
                this.SetBinding(() => ViewModel.CalendarSwitchMonSun,
                    () => SettingsPageCalendarMiscFirstDaySwitch.Checked, BindingMode.TwoWay));
            Bindings[SettingsPageCalendarMiscFirstDaySwitch.Id].Add(
                this.SetBinding(() => ViewModel.CalendarRemoveEmptyDays,
                    () => SettingsPageCalendarMiscRemoveEmptyDaysSwitch.Checked, BindingMode.TwoWay));
            Bindings[SettingsPageCalendarMiscFirstDaySwitch.Id].Add(
                this.SetBinding(() => ViewModel.CalendarPullExactAiringTime,
                    () => SettingsPageCalendarMiscExactAiringTimeSwitch.Checked, BindingMode.TwoWay));
            
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageCalendar;
    }
}