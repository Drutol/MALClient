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
    public partial class SettingsNotificationsFragment
    {
        private Switch _settingsPageNotificationsEnable;
        private Switch _settingsPageNotificationsCheckInRuntime;
        private LinearLayout _notificationsTypesCheckBoxGroup;
        private Spinner _settingsPageNotificationsFrequencySpinner;

        public Switch SettingsPageNotificationsEnable => _settingsPageNotificationsEnable ?? (_settingsPageNotificationsEnable = FindViewById<Switch>(Resource.Id.SettingsPageNotificationsEnable));

        public Switch SettingsPageNotificationsCheckInRuntime => _settingsPageNotificationsCheckInRuntime ?? (_settingsPageNotificationsCheckInRuntime = FindViewById<Switch>(Resource.Id.SettingsPageNotificationsCheckInRuntime));

        public LinearLayout NotificationsTypesCheckBoxGroup => _notificationsTypesCheckBoxGroup ?? (_notificationsTypesCheckBoxGroup = FindViewById<LinearLayout>(Resource.Id.NotificationsTypesCheckBoxGroup));

        public Spinner SettingsPageNotificationsFrequencySpinner => _settingsPageNotificationsFrequencySpinner ?? (_settingsPageNotificationsFrequencySpinner = FindViewById<Spinner>(Resource.Id.SettingsPageNotificationsFrequencySpinner));
    }
}