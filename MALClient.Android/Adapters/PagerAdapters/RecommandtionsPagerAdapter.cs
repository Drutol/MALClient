using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V13.App;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using MALClient.Android.Fragments.RecommendationsFragments;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Adapters.PagerAdapters
{
    class RecommandtionsPagerAdapter : FragmentStatePagerAdapter, PagerSlidingTabStrip.ICustomTabProvider
    {
        public RecommandtionsPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        private readonly Dictionary<int,Fragment> _pageFragments = new Dictionary<int, Fragment>();
        private readonly List<RecommendationItemViewModel> _items;

        public RecommandtionsPagerAdapter(FragmentManager fm,IEnumerable<RecommendationItemViewModel> items) : base(fm)
        {
            _items = items.ToList();
        }

        public override int Count => _items.Count;

        public override Fragment GetItem(int position)
        {
            if (!_pageFragments.ContainsKey(position))
            {
                _pageFragments.Add(position,new RecommendationItemFragment(_items[position]));
            }
            return _pageFragments[position];
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var holder = new LinearLayout(p0.Context);
            holder.Orientation = Orientation.Vertical;

            var txt = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt.Text = _items[p1].Data.DependentTitle;

            var txt1 = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt1.Text = _items[p1].Data.RecommendationTitle;

            holder.AddView(txt);
            holder.AddView(txt1);

            holder.Tag = p1;

            return holder;
        }

        public void TabSelected(View p0)
        {
            var layout = p0 as LinearLayout;
            layout.Alpha = 1f;

            _items[(int)p0.Tag].PopulateData();
        }

        public void TabUnselected(View p0)
        {
            var layout = p0 as LinearLayout;
            layout.Alpha = .7f;
        }
    }
}