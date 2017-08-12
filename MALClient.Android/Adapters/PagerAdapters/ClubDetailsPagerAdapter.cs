using System;
using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V13.App;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using MALClient.Android.Fragments;
using MALClient.Android.Fragments.ProfilePageFragments;
using MALClient.Android.Resources;
using Orientation = Android.Widget.Orientation;

namespace MALClient.Android.PagerAdapters
{
    public class ClubDetailsPagerAdapter : FragmentStatePagerAdapter, PagerSlidingTabStrip.ICustomTabProvider
    {
        public ClubDetailsPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ClubDetailsPagerAdapter(FragmentManager fm) : base(fm)
        {
            _generalFragment = new ProfilePageGeneralTabFragment();
            _commentsFragment = new ProfilePageFavouritesTabFragment();
            _descriptionFragment = new ProfilePageStatsTabFragment();
        }

        private MalFragmentBase _currentFragment;

        private readonly MalFragmentBase _generalFragment;
        private readonly MalFragmentBase _commentsFragment;
        private readonly MalFragmentBase _descriptionFragment;

        public override int Count => 3;

        public override Fragment GetItem(int p1)
        {
            switch (p1)
            {
                case 0:
                    return _descriptionFragment;
                case 1:
                    return _generalFragment;
                case 2:
                    return _commentsFragment;
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
                    _currentFragment = _descriptionFragment;
                    break;
                case 1:
                    _currentFragment = _generalFragment;
                    break;
                case 2:
                    _currentFragment = _commentsFragment;
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


            var txt = new TextView(p0.Context) { LayoutParameters = new LinearLayout.LayoutParams(-2, -2) { Gravity = GravityFlags.CenterHorizontal }, TextAlignment = TextAlignment.Center };
            txt.SetTextColor(new Color(ResourceExtension.BrushText));

            var img = new ImageView(p0.Context) { LayoutParameters = new LinearLayout.LayoutParams(DimensionsHelper.DpToPx(35), DimensionsHelper.DpToPx(35)) { Gravity = GravityFlags.CenterHorizontal } };
            img.SetScaleType(ImageView.ScaleType.CenterInside);
            img.ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.BrushText));

            switch (p1)
            {
                case 0:
                    txt.Text = "Description";
                    img.SetImageResource(Resource.Drawable.icon_acc_settings);
                    break;
                case 1:
                    txt.Text = "General";
                    img.SetImageResource(Resource.Drawable.icon_favourite);
                    break;
                case 2:
                    txt.Text = "Comments";
                    img.SetImageResource(Resource.Drawable.icon_message);
                    break;
            }

            holder.Tag = p1;

            holder.AddView(img);
            holder.AddView(txt);

            return holder;
        }
    }
}