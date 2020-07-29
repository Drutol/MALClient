using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
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
        private Random _random = new Random();

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
                Header = "Dakimakura Guide",
                PageType = SettingsPageIndex.Daki,
                Subtitle = "Make your life comfier and avoid filthy thieves and bootleggers!",
                Symbol = SettingsSymbolsEnum.Rocket,
            });

            pages.Add(new SettingsPageEntry
            {
                Header = "Did you know?",
                PageType = SettingsPageIndex.Info,
                Subtitle = "Me explaining this UI...",
                Symbol = SettingsSymbolsEnum.Lightbulb,

            });

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
            var img = view.FindViewById<ImageView>(Resource.Id.SettingsPageItemIcon);
            img.SetImageResource(GetIcon(settingsPageEntry.Symbol));

            if (settingsPageEntry.PageType == SettingsPageIndex.Daki)
            {
                img.ImageTintList = null;
                img.ScaleX = 1.5f;
                img.ScaleY = 1.5f;
            }
            else
            {
                img.ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.AccentColour));
                img.ScaleX = 1f;
                img.ScaleY = 1f;
            }

            return view;
        }

        private void OnEntryClicked(View view)
        {
            var entry = view.Tag.Unwrap<SettingsPageEntry>();
            ViewModel.RequestNavigationCommand.Execute(entry.PageType);
        }

        private int GetIcon(SettingsSymbolsEnum symbol)
        {
            return symbol switch
            {
                SettingsSymbolsEnum.Setting => Resource.Drawable.icon_settings,
                SettingsSymbolsEnum.SaveLocal => Resource.Drawable.icon_save_local,
                SettingsSymbolsEnum.CalendarWeek => Resource.Drawable.icon_calendar,
                SettingsSymbolsEnum.PreviewLink => Resource.Drawable.icon_newspaper,
                SettingsSymbolsEnum.PostUpdate => Resource.Drawable.icon_newspaper,
                SettingsSymbolsEnum.Manage => Resource.Drawable.icon_info,
                SettingsSymbolsEnum.Contact => Resource.Drawable.icon_account,
                SettingsSymbolsEnum.Placeholder => Resource.Drawable.icon_placeholder,
                SettingsSymbolsEnum.Important => Resource.Drawable.icon_notification,
                SettingsSymbolsEnum.SwitchApps => Resource.Drawable.icon_ads,
                SettingsSymbolsEnum.ContactInfo => Resource.Drawable.icon_feeds,
                SettingsSymbolsEnum.Lightbulb => Resource.Drawable.icon_bulb,
                SettingsSymbolsEnum.Discord => Resource.Drawable.icon_discord,
                SettingsSymbolsEnum.Rocket => _random.NextDouble() > .5 ? Resource.Drawable.kuri : Resource.Drawable.octo,
                _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null)
            };
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageHomepage;

        #region Views

        private LinearLayout _settingsPageHomepageList;

        public LinearLayout SettingsPageHomepageList => _settingsPageHomepageList ?? (_settingsPageHomepageList = FindViewById<LinearLayout>(Resource.Id.SettingsPageHomepageList));

        #endregion
    }
}