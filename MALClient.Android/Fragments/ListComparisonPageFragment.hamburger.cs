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
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Model;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using Com.Oguzdev.Circularfloatingactionmenu.Library;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;

namespace MALClient.Android.Fragments
{
    public partial class ListComparisonPageFragment
    {
        private Drawer _rightDrawer;
        private FloatingActionMenu _actionMenu;

        private void InitDrawer()
        {
            if (_rightDrawer != null)
                return;

            var builder = new DrawerBuilder().WithActivity(Activity);
            builder.WithSliderBackgroundColorRes(ResourceExtension.BrushHamburgerBackgroundRes);
            builder.WithStickyFooterShadow(true);
            builder.WithDisplayBelowStatusBar(true);
            builder.WithDrawerGravity((int)GravityFlags.Right);

            builder.WithStickyHeaderShadow(true);
            builder.WithStickyHeader(Resource.Layout.AnimeListPageDrawerHeader);

            _rightDrawer = builder.Build();

            _rightDrawer.DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            _rightDrawer.StickyHeader.SetBackgroundColor(new Color(ResourceExtension.BrushAppBars));
            _rightDrawer.DrawerLayout.AddDrawerListener(new DrawerListener(() =>
            {
                ViewModelLocator.NavMgr.ResetOneTimeOverride();
                OnCloseDrawer();
            }, null));
        }

        private void OpenCompFiltersDrawer()
        {
            var items = new List<IDrawerItem>();

            foreach (var enumValue in Enum.GetValues(typeof(ComparisonFilter)).Cast<ComparisonFilter>())
            {
                var btn = HamburgerUtilities.GetBaseSecondaryItem();
                btn.WithName(enumValue.GetDescription());
                btn.WithIdentifier((int)enumValue);
                btn.WithSetSelected(enumValue == ViewModel.ComparisonFilter);
                items.Add(btn);
            }
           
            _rightDrawer.SetItems(items);
            _rightDrawer.SetSelection((int)ViewModel.ComparisonFilter+100);

            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Comaprison Filters";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_list_type);
            _rightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                ViewModel.ComparisonFilter = (ComparisonFilter) arg3.Identifier;
                CloseDrawer();
            });

            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CloseDrawer));
            _rightDrawer.OpenDrawer();
            _actionMenu.Close(true);

        }

        private void OpenSortingDrawer()
        {
            var items = new List<IDrawerItem>();

            foreach (var enumValue in Enum.GetValues(typeof(ComparisonSorting)).Cast<ComparisonSorting>())
            {
                var btn = HamburgerUtilities.GetBaseSecondaryItem();
                btn.WithName(enumValue.GetDescription());
                btn.WithIdentifier((int)enumValue);
                btn.WithSetSelected(enumValue == ViewModel.ComparisonSorting);
                items.Add(btn);
            }

            var descendingToggle = new SwitchDrawerItem();
            descendingToggle.WithName("Descending Order");
            descendingToggle.WithChecked(!ViewModel.SortAscending);
            descendingToggle.WithOnCheckedChangeListener(
                new DrawerCheckedChangeListener(DescendingToggleOnCheckedChange));
            descendingToggle.WithIdentifier(998877);
            descendingToggle.WithTextColorRes(ResourceExtension.BrushTextRes);
            items.Add(descendingToggle);

            _rightDrawer.SetItems(items);
            _rightDrawer.SetSelection((int)ViewModel.ComparisonSorting);

            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Sorting";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_list_type);
            _rightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                ViewModel.ComparisonSorting = (ComparisonSorting)arg3.Identifier;
                CloseDrawer();
            });

            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CloseDrawer));
            _rightDrawer.OpenDrawer();
            _actionMenu.Close(true);
        }

        private void OpenFiltersDrawer()
        {
            var items = new List<IDrawerItem>();

            foreach (var enumValue in Enum.GetValues(typeof(AnimeStatus)).Cast<AnimeStatus>())
            {
                var btn = HamburgerUtilities.GetBaseSecondaryItem();
                btn.WithName(enumValue.GetDescription());
                btn.WithIdentifier((int)enumValue + 100);
                btn.WithSetSelected(enumValue == ViewModel.StatusFilter);
                items.Add(btn);
            }

            var spinner = new Spinner(Context);
            spinner.Adapter = Enum.GetValues(typeof(ComparisonStatusFilterTarget)).Cast<ComparisonStatusFilterTarget>()
                .ToList().GetAdapter(GetTemplateDelegate);
            spinner.SetSelection((int)ViewModel.StatusFilterTarget);
            spinner.ItemSelected += (sender, args) =>
            {
                ViewModel.StatusFilterTarget = (ComparisonStatusFilterTarget)(int)(sender as Spinner).SelectedView.Tag;
            };

            items.Add(new ContainerDrawerItem().WithView(spinner));


            _rightDrawer.SetSelection((int)ViewModel.StatusFilter);

            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Status Filters";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_filter);

            _rightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                ViewModel.StatusFilter = (AnimeStatus) arg3.Identifier;
                CloseDrawer();
            });

            _rightDrawer.OpenDrawer();
            _actionMenu.Close(true);
        }

        private View GetTemplateDelegate(int i, ComparisonStatusFilterTarget target, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                var par = AnimeListPageFlyoutBuilder.ParamRelativeLayout;
                AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(100), DimensionsHelper.DpToPx(38));
                view = AnimeListPageFlyoutBuilder.BuildBaseItem(Activity, target.GetDescription(),
                    ResourceExtension.BrushAnimeItemInnerBackground, null, false);
                AnimeListPageFlyoutBuilder.ParamRelativeLayout = par;
            }
            else
            {
                view.FindViewById<TextView>(AnimeListPageFlyoutBuilder.TextViewTag).Text = target.GetDescription();
            }


            view.Tag = (int) target;

            return view;
        }

        private void CloseDrawer()
        {
            _rightDrawer.CloseDrawer();
        }

        private void DescendingToggleOnCheckedChange(IDrawerItem arg1, bool arg2)
        {
            ViewModel.SortAscending = !arg2;
        }

        private void OnCloseDrawer()
        {
            
        }

        private void InitActionMenu()
        {
            _actionMenu?.Close(true);
            _actionMenu?.Dispose();
            var param = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(45), DimensionsHelper.DpToPx(45));
            var builder = new FloatingActionMenu.Builder(Activity)
                .AddSubActionView(BuildFabActionButton(param, Resource.Drawable.icon_filter))
                .AddSubActionView(BuildFabActionButton(param, Resource.Drawable.icon_list_type))
                .AddSubActionView(BuildFabActionButton(param, Resource.Drawable.icon_sort));
            _actionMenu = builder.AttachTo(ComparisonPageActionButton).Build();
        }

        private View BuildFabActionButton(ViewGroup.LayoutParams param, int icon)
        {
            var b1 = new FloatingActionButton(Activity)
            {
                LayoutParameters = param,
                Clickable = true,
                Focusable = true
            };
            b1.Size = FloatingActionButton.SizeMini;
            b1.SetScaleType(ImageView.ScaleType.Center);
            b1.SetImageResource(icon);
            b1.ImageTintList = ColorStateList.ValueOf(new Color(255, 255, 255));
            b1.BackgroundTintList = ColorStateList.ValueOf(new Color(ResourceExtension.AccentColourContrast));
            b1.Tag = icon;
            b1.Click += OnFloatingActionButtonOptionClick;
            return b1;
        }

        private void OnFloatingActionButtonOptionClick(object sender, EventArgs e)
        {
            switch ((int)(sender as View).Tag)
            {
                case Resource.Drawable.icon_filter:
                    OpenFiltersDrawer();
                    break;
                case Resource.Drawable.icon_list_type:
                    OpenCompFiltersDrawer();
                    break;
                case Resource.Drawable.icon_sort:
                    OpenSortingDrawer();
                    break;
            }
        }
    }
}