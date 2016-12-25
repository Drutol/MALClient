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
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.SettingsFragments
{
    public class SettingsHomepageFragment : MalFragmentBase
    {
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
        }

        protected override void InitBindings()
        {
            SettingsPageHomepageList.SetAdapter(ViewModel.SettingsPages.GetAdapter(GetTemplateDelegate));
        }

        private View GetTemplateDelegate(int i, SettingsPageEntry settingsPageEntry, View convertView)
        {
            var view = convertView;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.SettingsPageItem, null);
                view.SetOnClickListener(new OnClickListener(OnEntryClicked));
            }
            view.Tag = settingsPageEntry.Wrap();

            view.FindViewById<TextView>(Resource.Id.SettingsPageItemHeader).Text = settingsPageEntry.Header;
            view.FindViewById<TextView>(Resource.Id.SettingsPageItemSubtitle).Text = settingsPageEntry.Subtitle;

            return view;
        }

        private void OnEntryClicked(View view)
        {
            var entry = view.Tag.Unwrap<SettingsPageEntry>();
            ViewModel.RequestNavigationCommand.Execute(entry.PageType);
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageHomepage;

        #region Views

        private LinearLayout _settingsPageHomepageList;

        public LinearLayout SettingsPageHomepageList => _settingsPageHomepageList ?? (_settingsPageHomepageList = FindViewById<LinearLayout>(Resource.Id.SettingsPageHomepageList));

        #endregion
    }
}