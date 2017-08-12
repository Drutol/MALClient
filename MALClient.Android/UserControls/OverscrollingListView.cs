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
    public class OverscrollingListView : ListView
    {
        public event EventHandler ReachedTop;
        public event EventHandler ReachedBottom;

        public OverscrollingListView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public OverscrollingListView(Context context) : base(context)
        {
        }

        public OverscrollingListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public OverscrollingListView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public OverscrollingListView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public bool InterceptTouchEvents { get; set; }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return InterceptTouchEvents;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if(InterceptTouchEvents)
                return base.OnTouchEvent(e);
            return false;
        }

        protected override void OnOverScrolled(int scrollX, int scrollY, bool clampedX, bool clampedY)
        {
            if(clampedY && FirstVisiblePosition == 0)
                ReachedTop?.Invoke(this, EventArgs.Empty);

            base.OnOverScrolled(scrollX, scrollY, clampedX, clampedY);
        }

    }
}