using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V13.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using MALClient.Android.Fragments.CalendarFragments;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels.Main;
using Orientation = Android.Widget.Orientation;

namespace MALClient.Android.PagerAdapters
{
    public class CalendarPagerAdapter : FragmentStatePagerAdapter, PagerSlidingTabStrip.ICustomTabProvider
    {
        private readonly List<CalendarPivotPage> _pages;
        private readonly List<Fragment> _fragments;

        public CalendarPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CalendarPagerAdapter(FragmentManager fm,IEnumerable<CalendarPivotPage> pages) : base(fm)
        {
            _pages = pages.ToList();
            _fragments = _pages.Take(_pages.Count - 1).Select(page => new CalendarPageTabFragment(page.Items) as Fragment).ToList();
            _fragments.Add(new CalendarPageSummaryTabFragment((_pages.Last() as CalendarSummaryPivotPage).Data));
        }

        public override int Count => _pages.Count;

        public override Fragment GetItem(int position)
        {
            return _fragments[position];
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var header = new LinearLayout(p0.Context);
            header.Orientation = Orientation.Vertical;
            var model = _pages[p1];
            var parameter = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.Center
            };

            parameter.SetMargins(0,3,0,3);
            View viewUpper;
            TextView viewBottom = new TextView(p0.Context) {LayoutParameters = parameter};
            viewBottom.SetTextColor(new Color(ResourceExtension.BrushText));
            if (model is CalendarSummaryPivotPage)
            {
                var img = new ImageView(p0.Context) {LayoutParameters = new LinearLayout.LayoutParams(-2,-2) {Gravity = GravityFlags.Center} };
                img.ScaleX = .7f;
                img.ScaleY = .7f;
                img.SetScaleType(ImageView.ScaleType.CenterInside);
                img.ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.BrushText));
                img.SetImageResource(Resource.Drawable.icon_list);
                viewUpper = img;
                viewBottom.Text = "Summary";
                (viewBottom.LayoutParameters as LinearLayout.LayoutParams).SetMargins(0,4,0,0);
            }
            else
            {
                var txt = new TextView(p0.Context) { LayoutParameters = parameter };
                txt.Text = model.Header;
                txt.SetTextColor(new Color(ResourceExtension.BrushText));
                txt.SetTextSize(ComplexUnitType.Sp, 18);
                viewBottom.Text = model.Sub;
                viewUpper = txt;
            }
            header.AddView(viewUpper);
            header.AddView(viewBottom);
            return header;
        }

        public void TabSelected(View p0)
        {
            var layout = p0 as LinearLayout;
            layout.Alpha = 1f;
        }

        public void TabUnselected(View p0)
        {
            var layout = p0 as LinearLayout;
            layout.Alpha = .7f;
        }
    }
}