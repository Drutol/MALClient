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
using Com.Mikepenz.Materialdrawer.Interfaces;
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
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Managers;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;

namespace MALClient.Android.Fragments
{

    public partial class AnimeListPageFragment
    {      
        private AnimeListPageNavigationArgs _prevArgs;

        private AnimeListViewModel ViewModel = ViewModelLocator.AnimeList;
        private GridViewColumnHelper _gridViewColumnHelper;
        public static Drawer RightDrawer { get; set; }

        public AnimeListPageFragment(AnimeListPageNavigationArgs args)
        {
            _prevArgs = args;
        }

        public override int LayoutResourceId => Resource.Layout.AnimeListPage;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.AnimeList.Init(_prevArgs);     
        }

        private void CurrentContextOnHamburgerOpened(object o, EventArgs eventArgs)
        {
            _actionMenu?.Close(true);
        }

        public override void OnResume()
        {
            RightDrawer?.DrawerLayout?.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            MainActivity.CurrentContext.HamburgerOpened += CurrentContextOnHamburgerOpened;
            base.OnResume();
        }

        protected override void Cleanup()
        {
            MainActivity.CurrentContext.HamburgerOpened -= CurrentContextOnHamburgerOpened;
            _actionMenu?.Close(false);
            base.Cleanup();
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
            var prevPosition = AnimeListPageGridView.FirstVisiblePosition;

            _gridViewColumnHelper?.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);

            var footerParam = _loadMoreFooter.LayoutParameters;
            if (ViewModel.AnimeGridItems != null)
            {
                if (_gridViewColumnHelper != null
                    && ViewModel.AnimeGridItems.Count % _gridViewColumnHelper.LastColmuns != 0)
                    footerParam.Height = DimensionsHelper.DpToPx(315);
                AnimeListPageGridView.SetSelection(prevPosition);
            }
            else
            {
                footerParam.Height = -2;
            }
            _loadMoreFooter.LayoutParameters = footerParam;
            InitActionMenu();

        }

        #region SortingFilterHamburgers

        public override void OnPause()
        {
            RightDrawer.DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            base.OnPause();
        }

        private void InitDrawer()
        {
            if(RightDrawer != null)
                return;

            var builder = new DrawerBuilder().WithActivity(Activity);
            builder.WithSliderBackgroundColorRes(ResourceExtension.BrushHamburgerBackgroundRes);
            builder.WithStickyFooterShadow(true);
            builder.WithDisplayBelowStatusBar(true);
            builder.WithDrawerGravity((int) GravityFlags.Right);

            builder.WithStickyHeaderShadow(true);
            builder.WithStickyHeader(Resource.Layout.AnimeListPageDrawerHeader);

            RightDrawer = builder.Build();
            RightDrawer.DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            RightDrawer.StickyHeader.SetBackgroundColor(new Color(ResourceExtension.BrushAppBars));
            RightDrawer.DrawerLayout.AddDrawerListener(new DrawerListener(
            onClose: () =>
            {
                OpenFiltersDrawer(false);
                ViewModelLocator.NavMgr.ResetOneTimeOverride();
            },
            onOpen: () =>
            {
                ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CloseDrawer));
                _actionMenu.Close(true);
            }));

            OpenFiltersDrawer(false);
        }

        private void OpenFiltersDrawer(bool open)
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

 
            RightDrawer.SetItems(new List<IDrawerItem> { f1, f2, f3, f4, f5, f6 });
            RightDrawer.SetSelection((int)ViewModel.GetDesiredStatus());

            RightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Filters";
            RightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_filter);



            if (open)
            {
                
                RightDrawer.OpenDrawer();
                _actionMenu.Close(true);
            }

            RightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                if(view == null)
                    return;
                ViewModel.CurrentStatus = (int)arg3.Identifier;
                ViewModel.RefreshList();
                RightDrawer.OnDrawerItemClickListener = null;
                RightDrawer.CloseDrawer();
            });
        }

        private void OpenSortingDrawer()
        {
            var items = new List<IDrawerItem>();
            var options = Enum.GetValues(typeof(SortOptions)).Cast<SortOptions>();
            if (ViewModel.IsMangaWorkMode)
                options = options.Except(new[] {SortOptions.SortAirDay, SortOptions.SortSeason});
            foreach (SortOptions sortOption in options)
            {
                var btn = HamburgerUtilities.GetBaseSecondaryItem();
                if (sortOption == SortOptions.SortWatched)
                    btn.WithName(ViewModel.Sort3Label);
                else
                    btn.WithName(sortOption.GetDescription());
                btn.WithIdentifier((int)sortOption);
                items.Add(btn);
            }

            var descendingToggle = new SwitchDrawerItem();
            descendingToggle.WithName("Descending Order");
            descendingToggle.WithChecked(ViewModel.SortDescending);
            descendingToggle.WithOnCheckedChangeListener(
                new DrawerCheckedChangeListener(DescendingToggleOnCheckedChange));
            descendingToggle.WithIdentifier(998877);
            descendingToggle.WithTextColorRes(ResourceExtension.BrushTextRes);
            items.Add(descendingToggle);

            RightDrawer.SetItems(items);
            RightDrawer.SetSelection((int) ViewModel.SortOption);

            RightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Sorting";
            RightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_sort);
            RightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                if (view == null)
                    return;
                if (arg3.Identifier == 998877)
                {
                    ViewModel.SortDescending =! ViewModel.SortDescending;
                    ViewModel.RefreshList();
                }
                else
                {
                    ViewModel.SetSortOrder((SortOptions)arg3.Identifier);
                    ViewModel.RefreshList();
                }

                RightDrawer.OnDrawerItemClickListener = null;
                RightDrawer.CloseDrawer();
            });

            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CloseDrawer));
            RightDrawer.OpenDrawer();
            _actionMenu.Close(true);
        }

        private void DescendingToggleOnCheckedChange(IDrawerItem item,bool check)
        {
            ViewModel.SortDescending = check;
            ViewModel.RefreshList();
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

            RightDrawer.SetItems(new List<IDrawerItem>(){ f1, f2, f3 });
            RightDrawer.SetSelection((int) ViewModel.DisplayMode);

            RightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Display Modes";
            RightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_eye);
            RightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                if (view == null)
                    return;
                ViewModel.CurrentlySelectedDisplayMode =
                    new Tuple<AnimeListDisplayModes, string>((AnimeListDisplayModes) arg3.Identifier, null);
                RightDrawer.OnDrawerItemClickListener = null;
                RightDrawer.CloseDrawer();
            });

            (ViewModelLocator.NavMgr as NavMgr).EnqueueOneTimeOverride(new RelayCommand(CloseDrawer));
            RightDrawer.OpenDrawer();
            _actionMenu.Close(true);
        }

        private void OpenTopTypeDrawer()
        {
            var items = new List<IDrawerItem>();
            foreach (TopAnimeType sortOption in Enum.GetValues(typeof(TopAnimeType)))
            {
                var btn = HamburgerUtilities.GetBaseSecondaryItem();
                btn.WithName(sortOption.ToString());
                btn.WithIdentifier((int)sortOption);
                items.Add(btn);
            }

            RightDrawer.SetItems(items);
            RightDrawer.SetSelection((int)ViewModel.TopAnimeWorkMode);

            RightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Top Types";
            RightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_fav_outline);
            RightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                if (view == null)
                    return;
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList,AnimeListPageNavigationArgs.TopAnime((TopAnimeType)i));
                RightDrawer.OnDrawerItemClickListener = null;
                RightDrawer.CloseDrawer();
            });

            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CloseDrawer));
            RightDrawer.OpenDrawer();
            _actionMenu.Close(true);
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
                view.FindViewById<TextView>(AnimeListPageFlyoutBuilder.TextViewTag).Text = s;
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
                    RightDrawer.OnDrawerItemClickListener = null;
                    RightDrawer.CloseDrawer();
                }));

            items.Add(new ContainerDrawerItem().WithView(seasonView));
            RightDrawer.SetItems(items);
            RightDrawer.SetSelection(
                ViewModel.SeasonSelection.FindIndex(season => season.Name == ViewModel.CurrentSeason.Name));

            RightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                if (view == null)
                    return;
                ViewModel.SeasonalUrlsSelectedIndex = i;
                RightDrawer.OnDrawerItemClickListener = null;
                RightDrawer.CloseDrawer();
            });

           

            RightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Seasonal Selection";
            RightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_calendar);

            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CloseDrawer));
            RightDrawer.OpenDrawer();
            _actionMenu.Close(true);
        }

        private void CloseDrawer()
        {
            RightDrawer.CloseDrawer();
        }

        #endregion
    }
}