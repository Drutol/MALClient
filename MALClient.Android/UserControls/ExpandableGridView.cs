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
        public float ItemHeight { get; set; }
        public float ItemWidth { get; set; } = 1;

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

            // Calculate entire height by providing a very large height hint.
            // View.MEASURED_SIZE_MASK represents the largest height possible.
            int expandSpec = MeasureSpec.MakeMeasureSpec(MeasuredSizeMask,
                MeasureSpecMode.AtMost);
            base.OnMeasure(widthMeasureSpec, expandSpec);

            if(Width == 0)
                return;
            ViewGroup.LayoutParams param = LayoutParameters;
            param.Height = DimensionsHelper.DpToPx(Adapter.Count/(Width/ItemWidth) * ItemHeight);

        }
    }
}