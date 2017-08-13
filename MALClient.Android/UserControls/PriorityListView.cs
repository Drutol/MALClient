using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V13.View;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MALClient.Android.UserControls
{
    public class PriorityListView : ListView
    {

        public PriorityListView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public PriorityListView(Context context) : base(context)
        {
        }

        public PriorityListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public PriorityListView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public PriorityListView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public override void OnNestedScroll(View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed)
        {
            base.OnNestedScroll(target, dxConsumed, dyUnconsumed, dxUnconsumed, dyConsumed);
        }

    }
}