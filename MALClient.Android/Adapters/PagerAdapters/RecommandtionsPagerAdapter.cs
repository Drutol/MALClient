using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V13.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using MALClient.Android.Fragments.RecommendationsFragments;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.PagerAdapters
{
    class RecommandtionsPagerAdapter : FragmentStatePagerAdapter, PagerSlidingTabStrip.ICustomTabProvider
    {
        public RecommandtionsPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        private RecommendationItemFragment _currentFragment;
        private readonly List<RecommendationItemFragment> _pageFragments;
        private readonly List<RecommendationItemViewModel> _items;

        public RecommandtionsPagerAdapter(FragmentManager fm,IEnumerable<RecommendationItemViewModel> items) : base(fm)
        {
            _items = items.ToList();
            _pageFragments = new List<RecommendationItemFragment>
            {
                new RecommendationItemFragment(),
                new RecommendationItemFragment(),
                new RecommendationItemFragment(),
                new RecommendationItemFragment(),
                new RecommendationItemFragment(),
            };
        }

        public override int Count => _items.Count;

        public override Fragment GetItem(int position)
        {
            return _pageFragments[position % 5];
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var holder = new LinearLayout(p0.Context);
            holder.Orientation = Orientation.Vertical;
            holder.SetGravity(GravityFlags.Center);

            var txt = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt.Text = _items[p1].Data.DependentTitle;
            txt.SetMaxEms(13);
            txt.SetMaxLines(1);
            txt.Ellipsize = TextUtils.TruncateAt.End;

            var txt1 = new TextView(p0.Context);
            txt1.Text = _items[p1].Data.RecommendationTitle;
            txt1.SetMaxEms(13);
            txt1.SetMaxLines(1);
            txt.Ellipsize = TextUtils.TruncateAt.End;

            holder.AddView(txt);
            holder.AddView(txt1);

            holder.Tag = p1;
            return holder;
        }

        public void TabSelected(View p0)
        {
            var layout = p0 as LinearLayout;
            layout.Alpha = 1f;
            var index = (int) p0.Tag;
            var viewModel = _items[index];

            viewModel.PopulateData();

            _pageFragments[index%5].BindModel(viewModel);
        }

        public void TabUnselected(View p0)
        {
            var layout = p0 as LinearLayout;
            layout.Alpha = .5f;
        }
    }
}