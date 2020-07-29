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
using MALClient.Android.Resources;
using MALClient.Android.ViewModels;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
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
            var pages = ViewModel.SettingsPages
                .Where(entry => entry.PageType != SettingsPageIndex.Caching &&
                                entry.PageType != SettingsPageIndex.Articles &&
                                (Credentials.Authenticated || entry.PageType != SettingsPageIndex.Ads)).ToList();
            pages.Add(new SettingsPageEntry
            {
                Header = "Did you know?",
                PageType = SettingsPageIndex.Info,
                Subtitle = "Me explaining this UI...",
                Symbol = SettingsSymbolsEnum.Lightbulb,

            });
            //pages.Add(new SettingsPageEntry
            //{
            //    Header = "Dakimakura stores",
            //    PageType = SettingsPageIndex.Daki,
            //    Subtitle = "Make your life comfier and avoid filthy thieves/bootleggers!",
            //    Symbol = SettingsSymbolsEnum.Rocket,
            //});
            SettingsPageHomepageList.SetAdapter(pages.GetAdapter(GetTemplateDelegate));
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
            view.FindViewById<ImageView>(Resource.Id.SettingsPageItemIcon).SetImageResource(GetIcon(settingsPageEntry.Symbol));
            

            return view;
        }

        private void OnEntryClicked(View view)
        {
            var entry = view.Tag.Unwrap<SettingsPageEntry>();
            ViewModel.RequestNavigationCommand.Execute(entry.PageType);
        }

        private int GetIcon(SettingsSymbolsEnum symbol)
        {
            switch (symbol)
            {
                case SettingsSymbolsEnum.Setting:
                    return Resource.Drawable.icon_settings;
                case SettingsSymbolsEnum.SaveLocal:
                    return Resource.Drawable.icon_save_local;
                case SettingsSymbolsEnum.CalendarWeek:
                    return Resource.Drawable.icon_calendar;
                case SettingsSymbolsEnum.PreviewLink:
                    return Resource.Drawable.icon_newspaper;
                case SettingsSymbolsEnum.PostUpdate:
                    return Resource.Drawable.icon_newspaper;
                case SettingsSymbolsEnum.Manage:
                    return Resource.Drawable.icon_info;
                case SettingsSymbolsEnum.Contact:
                    return Resource.Drawable.icon_account;
                case SettingsSymbolsEnum.Placeholder:
                    return Resource.Drawable.icon_placeholder;
                case SettingsSymbolsEnum.Important:
                    return Resource.Drawable.icon_notification;
                case SettingsSymbolsEnum.SwitchApps:
                    return Resource.Drawable.icon_ads;
                case SettingsSymbolsEnum.ContactInfo:
                    return Resource.Drawable.icon_feeds;
                case SettingsSymbolsEnum.Lightbulb:
                    return Resource.Drawable.icon_bulb;
                case SettingsSymbolsEnum.Discord:
                    return Resource.Drawable.icon_discord;
                default:
                    throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null);
            }
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageHomepage;

        #region Views

        private LinearLayout _settingsPageHomepageList;

        public LinearLayout SettingsPageHomepageList => _settingsPageHomepageList ?? (_settingsPageHomepageList = FindViewById<LinearLayout>(Resource.Id.SettingsPageHomepageList));

        #endregion
    }
}