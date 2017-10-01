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
using Java.Lang;
using Orientation = Android.Content.Res.Orientation;

namespace MALClient.Android.UserControls
{
    public class HeightAdjustingListView : ListView
    {
        private bool EnableAdjustments { get; set; }

        #region Constructors

        public HeightAdjustingListView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public HeightAdjustingListView(Context context) : base(context)
        {
            EnableAdjustments = context.Resources.Configuration.Orientation == Orientation.Landscape;
        }

        public HeightAdjustingListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            EnableAdjustments = context.Resources.Configuration.Orientation == Orientation.Landscape;
        }

        public HeightAdjustingListView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            EnableAdjustments = context.Resources.Configuration.Orientation == Orientation.Landscape;
        }

        public HeightAdjustingListView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            EnableAdjustments = context.Resources.Configuration.Orientation == Orientation.Landscape;
        }

        #endregion

        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            //EnableAdjustments = newConfig.Orientation == Orientation.Landscape;
            base.OnConfigurationChanged(newConfig);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (EnableAdjustments)
            {
                int expandSpec = MeasureSpec.MakeMeasureSpec(Integer.MaxValue >> 2,
                    MeasureSpecMode.AtMost);
                base.OnMeasure(widthMeasureSpec, expandSpec);
            }
            else
            {
                base.OnMeasure(widthMeasureSpec,heightMeasureSpec);
            }

        }
    }
}