using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Model;
using MALClient.Android;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Resources;
using MALClient.Android.ViewModels;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingInformation;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.XShared.Utils;

namespace MALClient.Android.Fragments
{

    public partial class AnimeListPageFragment
    {
        private static AnimeListPageNavigationArgs _prevArgs;

        private AnimeListViewModel ViewModel => ViewModelLocator.AnimeList;
        private GridViewColumnHelper _gridViewColumnHelper;
        private AnimeListItemsAdapter _animeListItemsAdapter;
        private static Drawer _rightDrawer;


        public override int LayoutResourceId => Resource.Layout.AnimeListPage;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.AnimeList.Init(_prevArgs);         
        }

        private async void AnimeListPageGridViewOnItemClick(AnimeItemViewModel model)
        {
            await Task.Delay(75); //let's behold this ripple effect
            var args = ViewModelLocator.GeneralMain.GetCurrentListOrderParams();
            args.SelectedItemIndex = ViewModel.AnimeItems.IndexOf(model);
            model.NavigateDetails(null, args);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            if (ViewModel.DisplayMode == AnimeListDisplayModes.IndefiniteCompactList) //update vertical
                _animeListItemsAdapter.BindingInfos.Cast<AnimeCompactItemBindingInfo>().ForEach(info => info.OnConfigurationChanged(newConfig));

            _gridViewColumnHelper?.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }

        public static AnimeListPageFragment BuildInstance(object args)
        {
            _prevArgs = args as AnimeListPageNavigationArgs;
            return new AnimeListPageFragment();
        }

        #region SortingFilterHamburgers

        private void InitDrawer()
        {
            if(_rightDrawer != null)
                return;

            var builder = new DrawerBuilder().WithActivity(Activity);
            builder.WithSliderBackgroundColorRes(ResourceExtension.BrushHamburgerBackgroundRes);
            builder.WithStickyFooterShadow(true);
            builder.WithDisplayBelowStatusBar(true);
            builder.WithDrawerGravity((int) GravityFlags.Right);

            builder.WithStickyHeaderShadow(true);
            builder.WithStickyHeader(Resource.Layout.AnimeListPageDrawerHeader);
         
            _rightDrawer = builder.Build();
            _rightDrawer.DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            _rightDrawer.StickyHeader.SetBackgroundColor(new Color(ResourceExtension.BrushAppBars));

        }

        private void OpenFiltersDrawer()
        {
            var f1 = HamburgerUtilities.GetBaseSecondaryItem();
            f1.WithName(ViewModel.Filter1Label);
            f1.WithIdentifier((int)AnimeStatus.Watching);

            var f2 = HamburgerUtilities.GetBaseSecondaryItem();
            f2.WithName("Completed");
            f2.WithIdentifier((int)AnimeStatus.Completed);

            var f3 = HamburgerUtilities.GetBaseSecondaryItem();
            f3.WithName("On Hold");
            f3.WithIdentifier((int)AnimeStatus.OnHold);

            var f4 = HamburgerUtilities.GetBaseSecondaryItem();
            f4.WithName("Dropped");
            f4.WithIdentifier((int)AnimeStatus.Dropped);

            var f5 = HamburgerUtilities.GetBaseSecondaryItem();
            f5.WithName(ViewModel.Filter5Label);
            f5.WithIdentifier((int)AnimeStatus.PlanToWatch);

            var f6 = HamburgerUtilities.GetBaseSecondaryItem();
            f6.WithName(ViewModel.StatusAllLabel);
            f6.WithIdentifier((int)AnimeStatus.AllOrAiring);

 
            _rightDrawer.SetItems(new List<IDrawerItem> { f1, f2, f3, f4, f5, f6 });
            _rightDrawer.SetSelection((int)ViewModel.GetDesiredStatus());

            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Filters";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_filter);
            _rightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                ViewModel.CurrentStatus = (int) arg3.Identifier;
                ViewModel.RefreshList();
                _rightDrawer.OnDrawerItemClickListener = null;
                _rightDrawer.CloseDrawer();
            });

            _rightDrawer.OpenDrawer();
        }

        private void OpenSortingDrawer()
        {
            var items = new List<IDrawerItem>();
            foreach (SortOptions sortOption in Enum.GetValues(typeof(SortOptions)))
            {
                var btn = HamburgerUtilities.GetBaseSecondaryItem();
                if (sortOption == SortOptions.SortWatched)
                    btn.WithName(ViewModel.Sort3Label);
                else
                    btn.WithName(sortOption.GetDescription());
                btn.WithIdentifier((int)sortOption);
                items.Add(btn);
            }

            _rightDrawer.SetItems(items);
            _rightDrawer.SetSelection((int) ViewModel.SortOption);

            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Sorting";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_sort);
            _rightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                ViewModel.SortOption = (SortOptions) arg3.Identifier;
                ViewModel.RefreshList();
                _rightDrawer.OnDrawerItemClickListener = null;
                _rightDrawer.CloseDrawer();
            });
            _rightDrawer.OpenDrawer();
        }

        private void OpenViewModeDrawer()
        {
            var f1 = HamburgerUtilities.GetBaseSecondaryItem();
            f1.WithName("Grid");
            f1.WithIdentifier((int)AnimeListDisplayModes.IndefiniteGrid);

            var f2 = HamburgerUtilities.GetBaseSecondaryItem();
            f2.WithName("Detailed List");
            f2.WithIdentifier((int)AnimeListDisplayModes.IndefiniteList);

            var f3 = HamburgerUtilities.GetBaseSecondaryItem();
            f3.WithName("Compact List");
            f3.WithIdentifier((int)AnimeListDisplayModes.IndefiniteCompactList);

            _rightDrawer.SetItems(new List<IDrawerItem>(){ f1, f2, f3 });
            _rightDrawer.SetSelection((int) ViewModel.DisplayMode);

            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Display Modes";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_eye);
            _rightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                ViewModel.CurrentlySelectedDisplayMode =
                    new Tuple<AnimeListDisplayModes, string>((AnimeListDisplayModes) arg3.Identifier, null);
                _rightDrawer.OnDrawerItemClickListener = null;
                _rightDrawer.CloseDrawer();
            });

            _rightDrawer.OpenDrawer();
        }

        private void OpenSeasonalSelectionDrawer()
        {
            var items = new List<IDrawerItem>();
            int index = 0;
            foreach (var season in ViewModel.SeasonSelection)
            {
                var item = HamburgerUtilities.GetBaseSecondaryItem();
                item.WithName(season.Name);
                item.WithIdentifier(index++);

                items.Add(item);
            }
            var seasonView = Activity.LayoutInflater.Inflate(Resource.Layout.SeasonSelectionPopup,null);

            var spinnerYear = seasonView.FindViewById<Spinner>(Resource.Id.SeasonSelectionPopupYearComboBox);
            var spinnerSeason = seasonView.FindViewById<Spinner>(Resource.Id.SeasonSelectionPopupSeasonComboBox);
            spinnerYear.Adapter = ViewModel.SeasonYears.GetAdapter((i, s, arg3) =>
            {
                var view = arg3 ?? AnimeListPageFlyoutBuilder.BuildBaseItem(MainActivity.CurrentContext, s, ResourceExtension.BrushAnimeItemInnerBackground, null, false);
                view.Tag = s.Wrap();

                return view;
            });
            spinnerYear.ItemSelected += (sender, args) =>
            {
                ViewModel.CurrentlySelectedCustomSeasonYear = (sender as Spinner).SelectedView.Tag.Unwrap<string>();
            };
            spinnerSeason.Adapter = ViewModel.SeasonSeasons.GetAdapter((i, s, arg3) =>
            {
                var view = arg3 ?? AnimeListPageFlyoutBuilder.BuildBaseItem(MainActivity.CurrentContext, s, ResourceExtension.BrushAnimeItemInnerBackground, null, false);
                view.Tag = s.Wrap();

                return view;
            });
            spinnerSeason.ItemSelected += (sender, args) =>
            {
                ViewModel.CurrentlySelectedCustomSeasonSeason = (sender as Spinner).SelectedView.Tag.Unwrap<string>();
            };
            seasonView.FindViewById(Resource.Id.SeasonSelectionPopupAcceptButton).SetOnClickListener(new OnClickListener(
                view =>
                {
                    ViewModel.GoToCustomSeasonCommand.Execute(null);
                    _rightDrawer.OnDrawerItemClickListener = null;
                    _rightDrawer.CloseDrawer();
                }));

            items.Add(new ContainerDrawerItem().WithView(seasonView));
            _rightDrawer.SetItems(items);
            _rightDrawer.SetSelection(
                ViewModel.SeasonSelection.FindIndex(season => season.Name == ViewModel.CurrentSeason.Name));

            _rightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                ViewModel.SeasonalUrlsSelectedIndex = i;
                _rightDrawer.OnDrawerItemClickListener = null;
                _rightDrawer.CloseDrawer();
            });

            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Seasonal Selection";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_calendar);

            _rightDrawer.OpenDrawer();
        }

        #endregion
    }
}