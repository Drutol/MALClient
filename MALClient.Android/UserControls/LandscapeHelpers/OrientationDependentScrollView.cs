using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Orientation = Android.Content.Res.Orientation;

namespace MALClient.Android.UserControls
{
    public class OrientationDependentScrollView : ScrollView
    {
        private bool AllowScroll { get; set; } 

        #region Contructors

        public OrientationDependentScrollView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public OrientationDependentScrollView(Context context) : base(context)
        {
            AllowScroll = context.Resources.Configuration.Orientation == Orientation.Portrait;
        }

        public OrientationDependentScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            AllowScroll = context.Resources.Configuration.Orientation == Orientation.Portrait;
        }

        public OrientationDependentScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            AllowScroll = context.Resources.Configuration.Orientation == Orientation.Portrait;
        }

        public OrientationDependentScrollView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            AllowScroll = context.Resources.Configuration.Orientation == Orientation.Portrait;
        }

        #endregion

        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            AllowScroll = newConfig.Orientation == Orientation.Portrait;
            base.OnConfigurationChanged(newConfig);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if(AllowScroll)
                return base.OnTouchEvent(e);
            return false;
        }
    }
}