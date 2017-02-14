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
    public partial class SettingsNotificationsFragment : MalFragmentBase
    {
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
        }

        protected override void InitBindings()
        {
            //
            SettingsPageNotificationsEnable.Checked = Settings.EnableNotifications;
            SettingsPageNotificationsEnable.CheckedChange +=
                (sender, args) => ViewModel.EnableNotifications = SettingsPageNotificationsEnable.Checked;

            SettingsPageNotificationsCheckInRuntime.Checked = ViewModel.NotificationCheckInRuntime;
            SettingsPageNotificationsCheckInRuntime.CheckedChange +=
                (sender, args) => ViewModel.NotificationCheckInRuntime = SettingsPageNotificationsCheckInRuntime.Checked;
            //
            foreach (MalNotificationsTypes malNotificationsType in Enum.GetValues(typeof(MalNotificationsTypes)))
            {
                if (malNotificationsType != MalNotificationsTypes.Generic)
                {
                    CheckBox temp = new CheckBox( Context );
                    temp.Text = malNotificationsType.GetDescription();
                    temp.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.MatchParent) { TopMargin = DimensionsHelper.DpToPx(5) };
                    temp.Tag = (int)malNotificationsType;

                    NotificationsTypesCheckBoxGroup.AddView(temp);
                    temp.CheckedChange += NotificationsCheckBoxesChange;

                    if ( ViewModel.EnabledNotificationTypes.HasFlag(malNotificationsType) )
                        temp.Checked = true;
                }
            }
            //
            List<int> availableTimes = new List<int>() { 30, 45, 60, 120 };
            SettingsPageNotificationsFrequencySpinner.Adapter = availableTimes.GetAdapter((i, i1, arg3) =>
            {
                var view = arg3;
                if (view == null)
                {
                    view = AnimeListPageFlyoutBuilder.BuildBaseItem(Activity, $"{i1} minutes",
                        ResourceExtension.BrushAnimeItemInnerBackground, null, false, GravityFlags.Center);
                }
                else
                {
                    view.FindViewById<TextView>(AnimeListPageFlyoutBuilder.TextViewTag).Text = $"{i1} minutes";
                }
                view.Tag = i1;
                return view;
            });
            SettingsPageNotificationsFrequencySpinner.SetSelection(availableTimes.IndexOf(ViewModel.NotificationsRefreshTime));
            SettingsPageNotificationsFrequencySpinner.ItemSelected += (sender, args) =>
            {
                ViewModel.NotificationsRefreshTime = (int)SettingsPageNotificationsFrequencySpinner.SelectedView.Tag;
            };
            //

        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageNotifications;

        private void NotificationsCheckBoxesChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            var val = (MalNotificationsTypes)(int)(sender as CheckBox).Tag;

            if (checkedChangeEventArgs.IsChecked == true)
                ViewModel.EnabledNotificationTypes |=  val;
            else
                ViewModel.EnabledNotificationTypes &= ~val;
        }
    }
}