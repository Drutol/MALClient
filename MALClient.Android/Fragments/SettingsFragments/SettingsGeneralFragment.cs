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
    public partial class SettingsGeneralFragment : MalFragmentBase
    {
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
        }

        protected override void InitBindings()
        {
            SettingsPageGeneralStartPageRadioGroup.Check(Settings.DefaultMenuTab == "anime"
                ? SettingsPageGeneralRadioAnimeList.Id
                : SettingsPageGeneralRadioMangaList.Id);
            SettingsPageGeneralStartPageRadioGroup.SetOnCheckedChangeListener(new OnCheckedListener(i =>
            {
                Settings.DefaultMenuTab = i == SettingsPageGeneralRadioAnimeList.Id ? "anime" : "manga";
            }));
            //
            SettingsPageGeneralThemeRadioGroup.Check(Settings.SelectedTheme == 1
                ? SettingsPageGeneralRadioDarkTheme.Id
                : SettingsPageGeneralRadioLightTheme.Id);
            
            SettingsPageGeneralThemeRadioGroup.SetOnCheckedChangeListener(new OnCheckedListener(i =>
            {
                Settings.SelectedTheme = i == SettingsPageGeneralRadioDarkTheme.Id ? 1 : 0;
                SettingsPageGeneralThemeChangeApply.Visibility =
                    Converters.BoolToVisibility(Settings.SelectedTheme != MainActivity.CurrentTheme || AndroidColourThemeHelper.CurrentTheme != MainActivity.CurrentAccent);
            }));
            SettingsPageGeneralThemeChangeApply.SetOnClickListener(new OnClickListener(view =>
            {
                MainActivity.CurrentContext.Recreate();
                ViewModelLocator.NavMgr.ResetMainBackNav();
            }));

            //


            Bindings.Add(
                this.SetBinding(() => ViewModel.PullHigherQualityImages,
                    () => SettingsPageGeneralPullHigherSwitch.Checked,BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.DisplaySeasonWithType,
                    () => SettingsPageGeneralSeasonSwitch.Checked,BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.AutoDescendingSorting,
                    () => SettingsPageGeneralAutoSortSwitch.Checked,BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.MangaFocusVolumes,
                    () => SettingsPageGeneralVolsImportantSwitch.Checked,BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.HamburgerHideMangaSection,
                    () => SettingsPageGeneralHideHamburgerMangaSwitch.Checked, BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.EnableSwipeToIncDec,
                    () => SettingsPageGeneralEnableSwipeSwitch.Checked, BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.PreferEnglishTitles,
                    () => SettingsPageGeneralPreferEnglishTitleSwitch.Checked, BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.MakeGridItemsSmaller,
                    () => SettingsPageGeneralSmallerGridItems.Checked, BindingMode.TwoWay));
            //
            SettingsPageGeneralAnimeSortRadioGroup.Check(GetViewIdForAnimeSortOption(Settings.AnimeSortOrder));
            SettingsPageGeneralAnimeSortRadioGroup.SetOnCheckedChangeListener(new OnCheckedListener(i =>
            {
                Settings.AnimeSortOrder = GetSortOptionForViewId(i);
            }));

            SettingsPageGeneralMangaSortRadioGroup.Check(GetViewIdForMangaSortOption(Settings.MangaSortOrder));
            SettingsPageGeneralMangaSortRadioGroup.SetOnCheckedChangeListener(new OnCheckedListener(i =>
            {
                Settings.MangaSortOrder = GetSortOptionForViewId(i);
            }));

            SettingsPageGeneralAnimeSortDescendingSwitch.Checked = Settings.IsSortDescending;
            SettingsPageGeneralAnimeSortDescendingSwitch.CheckedChange +=
                (sender, args) => Settings.IsSortDescending = SettingsPageGeneralAnimeSortDescendingSwitch.Checked;

            SettingsPageGeneralMangaSortDescendingSwitch.Checked = Settings.IsMangaSortDescending;
            SettingsPageGeneralMangaSortDescendingSwitch.CheckedChange +=
                (sender, args) => Settings.IsMangaSortDescending = SettingsPageGeneralMangaSortDescendingSwitch.Checked;
            //

            SettingsPageGeneralWatchingViewModeSpinner.Adapter = ViewModel.DisplayModes.GetAdapter(GetTemplateDelegate);
            SettingsPageGeneralWatchingViewModeSpinner.SetSelection(ViewModel.DisplayModes.IndexOf(ViewModel.SelectedDefaultViewForWatching));
            SettingsPageGeneralWatchingViewModeSpinner.ItemSelected += (sender, args) =>
            {
                ViewModel.SelectedDefaultViewForWatching = (sender as Spinner).SelectedView.Tag.Unwrap<Tuple<AnimeListDisplayModes, string>>();
            };

            SettingsPageGeneralCompletedViewModeSpinner.Adapter = ViewModel.DisplayModes.GetAdapter(GetTemplateDelegate);
            SettingsPageGeneralCompletedViewModeSpinner.SetSelection(ViewModel.DisplayModes.IndexOf(ViewModel.SelectedDefaultViewForCompleted));
            SettingsPageGeneralCompletedViewModeSpinner.ItemSelected += (sender, args) =>
            {
                ViewModel.SelectedDefaultViewForCompleted = (sender as Spinner).SelectedView.Tag.Unwrap<Tuple<AnimeListDisplayModes, string>>();
            };

            SettingsPageGeneralOnHoldViewModeSpinner.Adapter = ViewModel.DisplayModes.GetAdapter(GetTemplateDelegate);
            SettingsPageGeneralOnHoldViewModeSpinner.SetSelection(ViewModel.DisplayModes.IndexOf(ViewModel.SelectedDefaultViewForOnHold));
            SettingsPageGeneralOnHoldViewModeSpinner.ItemSelected += (sender, args) =>
            {
                ViewModel.SelectedDefaultViewForOnHold = (sender as Spinner).SelectedView.Tag.Unwrap<Tuple<AnimeListDisplayModes, string>>();
            };

            SettingsPageGeneralDroppedViewModeSpinner.Adapter = ViewModel.DisplayModes.GetAdapter(GetTemplateDelegate);
            SettingsPageGeneralDroppedViewModeSpinner.SetSelection(ViewModel.DisplayModes.IndexOf(ViewModel.SelectedDefaultViewForDropped));
            SettingsPageGeneralDroppedViewModeSpinner.ItemSelected += (sender, args) =>
            {
                ViewModel.SelectedDefaultViewForDropped = (sender as Spinner).SelectedView.Tag.Unwrap<Tuple<AnimeListDisplayModes, string>>();
            };

            SettingsPageGeneralPtwViewModeSpinner.Adapter = ViewModel.DisplayModes.GetAdapter(GetTemplateDelegate);
            SettingsPageGeneralPtwViewModeSpinner.SetSelection(ViewModel.DisplayModes.IndexOf(ViewModel.SelectedDefaultViewForPlanned));
            SettingsPageGeneralPtwViewModeSpinner.ItemSelected += (sender, args) =>
            {
                ViewModel.SelectedDefaultViewForPlanned = (sender as Spinner).SelectedView.Tag.Unwrap<Tuple<AnimeListDisplayModes, string>>();
            };

            SettingsPageGeneralAllViewModeSpinner.Adapter = ViewModel.DisplayModes.GetAdapter(GetTemplateDelegate);
            SettingsPageGeneralAllViewModeSpinner.SetSelection(ViewModel.DisplayModes.IndexOf(ViewModel.SelectedDefaultViewForAll));
            SettingsPageGeneralAllViewModeSpinner.ItemSelected += (sender, args) =>
            {
                ViewModel.SelectedDefaultViewForAll = (sender as Spinner).SelectedView.Tag.Unwrap<Tuple<AnimeListDisplayModes, string>>();
            };

           
            //
            var filters = Enum.GetValues(typeof(AnimeStatus)).Cast<int>().ToList();
            SettingsPageGeneralMangaFilerSpinner.Adapter = filters.GetAdapter(GetMangaTemplateDelegate);
            SettingsPageGeneralMangaFilerSpinner.SetSelection(filters.IndexOf(Settings.DefaultMangaFilter));
            SettingsPageGeneralMangaFilerSpinner.ItemSelected += (sender, args) =>
            {
                Settings.DefaultMangaFilter = (int) SettingsPageGeneralMangaFilerSpinner.SelectedView.Tag;
            };

            SettingsPageGeneralAnimeFilterSpinner.Adapter = filters.GetAdapter(GetAnimeTemplateDelegate);
            SettingsPageGeneralAnimeFilterSpinner.SetSelection(filters.IndexOf(Settings.DefaultAnimeFilter));
            SettingsPageGeneralAnimeFilterSpinner.ItemSelected += (sender, args) =>
            {
                Settings.DefaultAnimeFilter = (int)SettingsPageGeneralAnimeFilterSpinner.SelectedView.Tag;
            };
            //

            
            Bindings.Add(
                this.SetBinding(() => ViewModel.SetStartDateOnListAdd,
                    () => SettingsPageGeneralStartDateWhenAddCheckBox.Checked,BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.SetStartDateOnWatching,
                    () => SettingsPageGeneralStartDateWhenWatchCheckBox.Checked,BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.SetEndDateOnCompleted,
                    () => SettingsPageGeneralEndDateWhenCompleted.Checked,BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.SetEndDateOnDropped,
                    () => SettingsPageGeneralEndDateWhenDropCheckBox.Checked,BindingMode.TwoWay));
            Bindings.Add(
                this.SetBinding(() => ViewModel.OverrideValidStartEndDate,
                    () => SettingsPageGeneralAllowDateOverrideCheckBox.Checked,BindingMode.TwoWay));
            //
            SettingsPageGeneralAirDayOffsetSlider.Progress = Settings.AirDayOffset + 3;
            SettingsPageGeneralAirDayOffsetTextView.Text = (Settings.AirDayOffset).ToString();

            SettingsPageGeneralAirDayOffsetSlider.ProgressChanged += (sender, args) =>
            {
                Settings.AirDayOffset = SettingsPageGeneralAirDayOffsetSlider.Progress - 3;
                SettingsPageGeneralAirDayOffsetTextView.Text =
                    (SettingsPageGeneralAirDayOffsetSlider.Progress - 3).ToString();
            };

            UpdateColourSelection();

            SettingsPageGeneralColorOrange.Tag = (int) AndroidColorThemes.Orange;
            SettingsPageGeneralColorPurple.Tag = (int) AndroidColorThemes.Purple;
            SettingsPageGeneralColorBlue.Tag = (int)AndroidColorThemes.Blue;
            SettingsPageGeneralColorLime.Tag = (int)AndroidColorThemes.Lime;

            var colorListener = new OnClickListener(view =>
            {
                AndroidColourThemeHelper.CurrentTheme = (AndroidColorThemes) (int) view.Tag;
                UpdateColourSelection();
            });

            SettingsPageGeneralColorOrange.SetOnClickListener(colorListener);
            SettingsPageGeneralColorPurple.SetOnClickListener(colorListener);
            SettingsPageGeneralColorBlue.SetOnClickListener(colorListener);
            SettingsPageGeneralColorLime.SetOnClickListener(colorListener);

        }     

        private void UpdateColourSelection()
        {
            switch (AndroidColourThemeHelper.CurrentTheme)
            {
                case AndroidColorThemes.Orange:
                    SettingsPageGeneralColorOrange.SetImageResource(Resource.Drawable.icon_ok);
                    SettingsPageGeneralColorPurple.SetImageResource(Resource.Color.Transparent);
                    SettingsPageGeneralColorBlue.SetImageResource(Resource.Color.Transparent);
                    SettingsPageGeneralColorLime.SetImageResource(Resource.Color.Transparent);
                    break;
                case AndroidColorThemes.Purple:
                    SettingsPageGeneralColorPurple.SetImageResource(Resource.Drawable.icon_ok);
                    SettingsPageGeneralColorOrange.SetImageResource(Resource.Color.Transparent);
                    SettingsPageGeneralColorBlue.SetImageResource(Resource.Color.Transparent);
                    SettingsPageGeneralColorLime.SetImageResource(Resource.Color.Transparent);
                    break;
                case AndroidColorThemes.Blue:
                    SettingsPageGeneralColorBlue.SetImageResource(Resource.Drawable.icon_ok);
                    SettingsPageGeneralColorPurple.SetImageResource(Resource.Color.Transparent);
                    SettingsPageGeneralColorOrange.SetImageResource(Resource.Color.Transparent);
                    SettingsPageGeneralColorLime.SetImageResource(Resource.Color.Transparent);
                    break;
                case AndroidColorThemes.Lime:
                    SettingsPageGeneralColorLime.SetImageResource(Resource.Drawable.icon_ok);
                    SettingsPageGeneralColorPurple.SetImageResource(Resource.Color.Transparent);
                    SettingsPageGeneralColorOrange.SetImageResource(Resource.Color.Transparent);
                    SettingsPageGeneralColorBlue.SetImageResource(Resource.Color.Transparent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SettingsPageGeneralThemeChangeApply.Visibility =
                    Converters.BoolToVisibility(Settings.SelectedTheme != MainActivity.CurrentTheme || AndroidColourThemeHelper.CurrentTheme != MainActivity.CurrentAccent);
        }

        #region TemplateDelegates

        private View GetMangaTemplateDelegate(int i, int animeStatus, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                view = AnimeListPageFlyoutBuilder.BuildBaseItem(Activity, Utilities.StatusToString(animeStatus, true),
                    ResourceExtension.BrushAnimeItemInnerBackground, null, false);
            }
            else
            {
                view.FindViewById<TextView>(AnimeListPageFlyoutBuilder.TextViewTag).Text = Utilities.StatusToString(animeStatus,true);
            }

            view.Tag = animeStatus;

            return view;
        }

        private View GetAnimeTemplateDelegate(int i, int animeStatus, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                view = AnimeListPageFlyoutBuilder.BuildBaseItem(Activity, Utilities.StatusToString(animeStatus),
                    ResourceExtension.BrushAnimeItemInnerBackground, null, false);
            }
            else
            {
                view.FindViewById<TextView>(AnimeListPageFlyoutBuilder.TextViewTag).Text = Utilities.StatusToString(animeStatus);
            }

            view.Tag = animeStatus;

            return view;
        }

        private View GetTemplateDelegate(int i, Tuple<AnimeListDisplayModes, string> tuple, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                var par = AnimeListPageFlyoutBuilder.ParamRelativeLayout;
                AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(100),DimensionsHelper.DpToPx(38));
                view = AnimeListPageFlyoutBuilder.BuildBaseItem(Activity, tuple.Item2,
                    ResourceExtension.BrushAnimeItemInnerBackground, null, false);
                AnimeListPageFlyoutBuilder.ParamRelativeLayout = par;
            }
            else
            {
                view.FindViewById<TextView>(AnimeListPageFlyoutBuilder.TextViewTag).Text = tuple.Item2;
            }


            view.Tag = tuple.Wrap();



            return view;
        }

        #endregion


        #region SortingViewMapping

        private int GetViewIdForAnimeSortOption(SortOptions option)
        {
            switch (option)
            {
                case SortOptions.SortTitle:
                    return SettingsPageGeneralAnimeSortTitleRadioBtn.Id;
                case SortOptions.SortScore:
                    return SettingsPageGeneralAnimeScoreTitleRadioBtn.Id;
                case SortOptions.SortWatched:
                    return SettingsPageGeneralAnimeWatchedTitleRadioBtn.Id;
                case SortOptions.SortAirDay:
                    return SettingsPageGeneralAnimeSoonAiringTitleRadioBtn.Id;
                case SortOptions.SortLastWatched:
                    return SettingsPageGeneralAnimeLastWatchTitleRadioBtn.Id;
                case SortOptions.SortStartDate:
                    break;
                case SortOptions.SortEndDate:
                    break;
                case SortOptions.SortNothing:
                    return SettingsPageGeneralAnimeSortNoneRadioBtn.Id;
                case SortOptions.SortSeason:
                    break;
            }
            throw new ArgumentOutOfRangeException(nameof(option), option, "Emm... did I add stuff to default sortings?");
        }

        private int GetViewIdForMangaSortOption(SortOptions option)
        {
            switch (option)
            {
                case SortOptions.SortTitle:
                    return SettingsPageGeneralMangaSortTitleRadioBtn.Id;
                case SortOptions.SortScore:
                    return SettingsPageGeneralMangaSortScoreRadioBtn.Id;
                case SortOptions.SortWatched:
                    return SettingsPageGeneralMangaSortReadRadioBtn.Id;
                case SortOptions.SortNothing:
                    return SettingsPageGeneralMangaSortNoneRadioBtn.Id;
                case SortOptions.SortSeason:
                    break;
            }
            throw new ArgumentOutOfRangeException(nameof(option), option, "Emm... did I add stuff to default sortings?");
        }

        private SortOptions GetSortOptionForViewId(int id)
        {
            if (id == SettingsPageGeneralAnimeSortTitleRadioBtn.Id || id == SettingsPageGeneralMangaSortTitleRadioBtn.Id)
                return SortOptions.SortTitle;
            if (id == SettingsPageGeneralAnimeScoreTitleRadioBtn.Id || id == SettingsPageGeneralMangaSortScoreRadioBtn.Id)
                return SortOptions.SortScore;
            if (id == SettingsPageGeneralAnimeWatchedTitleRadioBtn.Id || id == SettingsPageGeneralMangaSortReadRadioBtn.Id)
                return SortOptions.SortWatched;
            if (id == SettingsPageGeneralAnimeSoonAiringTitleRadioBtn.Id)
                return SortOptions.SortAirDay;
            if (id == SettingsPageGeneralAnimeLastWatchTitleRadioBtn.Id)
                return SortOptions.SortLastWatched;
            return SortOptions.SortNothing;
        }

        #endregion


        public override int LayoutResourceId => Resource.Layout.SettingsPageGeneral;
    }
}