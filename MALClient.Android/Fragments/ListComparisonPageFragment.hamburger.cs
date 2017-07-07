using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Model;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using GalaSoft.MvvmLight.Command;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public partial class ListComparisonPageFragment
    {
        private Drawer _rightDrawer;

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

        private void OpenFiltersDrawer()
        {
            var items = new List<IDrawerItem>();
            var options = Enum.GetValues(typeof(ComparisonFilter)).Cast<ComparisonFilter>();

            foreach (var enumValue in options)
            {
                var btn = HamburgerUtilities.GetBaseSecondaryItem();
                btn.WithName(enumValue.GetDescription());
                btn.WithIdentifier((int)enumValue +100);
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
            _rightDrawer.SetSelection((int)ViewModel.ComparisonFilter+100);

            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Filters";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_filter);
            _rightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {

            });

            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CloseDrawer));
            _rightDrawer.OpenDrawer();
            //_actionMenu.Close(true);
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
            throw new NotImplementedException();
        }
    }
}