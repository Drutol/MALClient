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
using Android.Graphics;
using Android.Animation;
using MALClient.Android.Listeners;
using Android.Graphics.Drawables;
using MALClient.XShared.Utils;

namespace MALClient.Android.UserControls
{
    public class ChartView : View
    {
        private Chart Chart;
        public event EventHandler OnViewInitialized;

        public ChartView(Context context) : base(context)
        {
            Init(null, 0);
        }
        public ChartView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(attrs, 0);
        }
        public ChartView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Init(attrs, defStyle);
        }
        
        private void Init(IAttributeSet attrs, int defStyle)
        {
            Chart = new Chart();
            Chart.ChartLayoutUpdated += (sender, args) => Invalidate();
            //DEBUG
            Chart.Add(25, new Color(0xdd, 0x21, 0x6c, 120));
            Chart.Add(25, new Color(0x21, 0xdd, 0x90, 120));
            Chart.Add(25, new Color(0xff, 0xe0, 0x47, 120));
            Chart.Add(25, new Color(0x74, 0xc3, 0x26, 120));
            //-----//
            SetOnTouchListener( new OnTouchListener(OnTouch) );
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            Chart.Position = new Vector2D(MeasuredWidth / 2.0f, MeasuredHeight / 2.0f);//DEBUG
            Chart.CenterRadius = (MeasuredWidth / 2.0f) - 100; // DEBUG
        }
        protected override void OnDraw(Canvas canvas)
        {
            canvas.Save();
                Chart.Draw(canvas);
            canvas.Restore();
        }

        private void OnTouch(MotionEvent e)
        {
            Chart.OnClick(e);
        }
    }
}
