using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace MALClient.Android.UserControls
{
    public class ExpandableGridView : GridView
    {
        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }

        public ExpandableGridView(Context context) : base(context)
        {
            VerticalScrollBarEnabled = false;
        }

        public ExpandableGridView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            VerticalScrollBarEnabled = false;
        }

        public ExpandableGridView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            VerticalScrollBarEnabled = false;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            // HACK! TAKE THAT ANDROID!
            if (IsExpanded)
            {
                // Calculate entire height by providing a very large height hint.
                // View.MEASURED_SIZE_MASK represents the largest height possible.
                int expandSpec = MeasureSpec.MakeMeasureSpec(MeasuredSizeMask,
                    MeasureSpecMode.AtMost);
                base.OnMeasure(widthMeasureSpec, expandSpec);

                ViewGroup.LayoutParams param = LayoutParameters;
                param.Height = MeasuredHeight;
            }
            else
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            }
        }
    }
}