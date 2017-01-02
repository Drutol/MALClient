using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MALClient.Android.UserControls
{
    public class ScrollableSwipeToRefreshLayout : SwipeRefreshLayout
    {
        public ScrollableSwipeToRefreshLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ScrollableSwipeToRefreshLayout(Context context) : base(context)
        {
        }

        public ScrollableSwipeToRefreshLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public View ScrollingView { get; set; }

        public override bool CanChildScrollUp()
        {
            return ScrollingView.CanScrollVertically(-1);
        }
    }
}