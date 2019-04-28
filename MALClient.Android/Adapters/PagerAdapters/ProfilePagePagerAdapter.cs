using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using com.refractored;
using MALClient.Android.Fragments;
using MALClient.Android.Fragments.ArticlesPageFragments;
using MALClient.Android.Fragments.ForumFragments.Tabs;
using MALClient.Android.Fragments.ProfilePageFragments;
using MALClient.Android.Resources;

using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using Orientation = Android.Widget.Orientation;

namespace MALClient.Android.PagerAdapters
{
    public class ProfilePagePagerAdapter : FragmentStatePagerAdapter, ICustomTabProvider
    {
        public ProfilePagePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ProfilePagePagerAdapter(FragmentManager fm) : base(fm)
        {
            _generalFragment = new ProfilePageGeneralTabFragment();
            _favsFragment = new ProfilePageFavouritesTabFragment();
            _statsFragment = new ProfilePageStatsTabFragment();
            _recentsFragment = new ProfilePageRecentUpdatesFragment();
        }

        private MalFragmentBase _currentFragment;

        private readonly MalFragmentBase _generalFragment;
        private readonly MalFragmentBase _favsFragment;
        private readonly MalFragmentBase _recentsFragment;
        private readonly ProfilePageStatsTabFragment _statsFragment;

        public override int Count => 4;

        public override Fragment GetItem(int p1)
        {
            switch (p1)
            {
                case 0:
                    return _generalFragment;
                case 1:
                    return _favsFragment;
                case 2:;
                    return _recentsFragment;
                case 3:
                    return _statsFragment;
            }
            throw new ArgumentException();
        }

        public void TabSelected(View p0)
        {
            p0.Alpha = 1f;
            //_currentFragment?.DetachBindings();
            switch ((int)p0.Tag)
            {
                case 0:
                    _currentFragment = _generalFragment;
                    break;
                case 1:
                    _currentFragment = _favsFragment;
                    break;
                case 2:
                    _currentFragment = _recentsFragment;
                    break;
                case 3:
                    _currentFragment = _statsFragment;
                    _statsFragment.NavigatedTo();
                    break;
            }
            _currentFragment?.ReattachBindings();
        }

        public void TabUnselected(View p0)
        {
            p0.Alpha = .7f;
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var holder = new LinearLayout(p0.Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters =
                    new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent)
            };


            var txt = new TextView(p0.Context) { LayoutParameters = new LinearLayout.LayoutParams(-2, -2) { Gravity = GravityFlags.CenterHorizontal },TextAlignment = TextAlignment.Center};
            txt.SetTextColor(new Color(ResourceExtension.BrushText));

            var img = new ImageView(p0.Context) { LayoutParameters = new LinearLayout.LayoutParams(DimensionsHelper.DpToPx(35), DimensionsHelper.DpToPx(35)) { Gravity = GravityFlags.CenterHorizontal} };
            img.SetScaleType(ImageView.ScaleType.CenterInside);
            img.ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.BrushText));
            
            switch (p1)
            {
                case 0:
                    txt.Text = "General";
                    img.SetImageResource(Resource.Drawable.icon_acc_settings);
                    break;
                case 1:
                    txt.Text = "Favourites";
                    img.SetImageResource(Resource.Drawable.icon_favourite);
                    break;
                case 2:
                    txt.Text = "Recent updates";
                    img.SetImageResource(Resource.Drawable.icon_clock);
                    break;
                case 3:
                    txt.Text = "Stats";
                    img.SetImageResource(Resource.Drawable.icon_chart);
                    break;
            }

            holder.Tag = p1;

            holder.AddView(img);
            holder.AddView(txt);

            return holder;
        }
    }
}