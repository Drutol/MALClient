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
    class ChartView : View
    {
        private int[] valuesArray = new int[] { 21, 12, 65, 10, 30 };
        private List<SemiCircle> SemiCircles = new List<SemiCircle>();
        private int selectedChartSegment = -1;
        private int[] colorsArray = new int[] { 0xdd216c, 0x21dd90, 0xffe047, 0x21b3dd, 0x74c326 };
        private Paint _innerCirclePaint = new Paint();
        private Paint _textPaint = new Paint();
        private static float _outterCircleRadius = 400;
        private static float _innerCircleRadius = 300;

        private float _globalAngle = 0;
       
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
            var touchListener = new OnTouchListener(onTouch);
            this.SetOnTouchListener(touchListener);
            int sum = valuesArray.Sum();
            int i = 0;
            float angle = 0;
            foreach(int value in valuesArray)
            {
                Color color = new Color(colorsArray[i]);
                color.A = 255;
                SemiCircles.Add(new SemiCircle(value, sum, _innerCircleRadius, _outterCircleRadius, color));
                SemiCircles[i].SetAngle(angle);
                angle += value * 360.0f / sum;
                i++;
            }

            _innerCirclePaint.SetARGB(255, 255, 255, 255);
            _innerCirclePaint.AntiAlias = true;
            _innerCirclePaint.SetShadowLayer(15, 0, 0, new Color(0, 0, 0, 120));
            SetLayerType(LayerType.Software, _innerCirclePaint);

            _textPaint.SetARGB(255, 50, 50, 50);
            _textPaint.AntiAlias = true;
            _textPaint.TextSize = 150;

            ValueAnimator initAnimation = ValueAnimator.OfFloat(new float[] { 0, 1 });
            initAnimation.SetDuration(1000);
            initAnimation.Update += (sender, args) =>
            {
                foreach( SemiCircle ob in SemiCircles)
                {
                    ob.SetLengthFraction((float)initAnimation.AnimatedValue);
                    ob.SetAngleFraction((float)initAnimation.AnimatedValue);
                    _globalAngle = 180 * (float)initAnimation.AnimatedValue - 180;
                    Invalidate();
                }
            };
            initAnimation.AnimationEnd += (sender, args) =>
            {
                _globalAngle = 0;
            };
            initAnimation.Start();

        }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.Translate(canvas.Width / 2, canvas.Height / 2);
            canvas.Save();
                canvas.Rotate(_globalAngle);
                foreach(SemiCircle semiCircle in SemiCircles)
                {
                    semiCircle.Draw(canvas);
                }
                canvas.DrawCircle(0, 0, _innerCircleRadius, _innerCirclePaint);
            canvas.Restore();
            if(selectedChartSegment >=0)
            {
                canvas.Translate(-_textPaint.MeasureText( ( (int) (SemiCircles[selectedChartSegment].GetValue()*100 / valuesArray.Sum() ) ).ToString() + '%')/2.0f, _textPaint.TextSize / 2.0f);
                canvas.DrawText( ((int)(SemiCircles[selectedChartSegment].GetValue()*100/ valuesArray.Sum())).ToString()+ '%', 0, 0, _textPaint);
            }
        }

        private void onTouch(MotionEvent e)
        {
            float x = e.GetX();
            float y = e.GetY();

            switch(e.Action)
            {
                case MotionEventActions.Up:
                    trySelectSegment(x, y);
                    break;
                case MotionEventActions.Move:
                    break;
                case MotionEventActions.Down:

                    break;
            }
        }

        private void trySelectSegment(float x, float y)
        {
            Vector2D pressedPoint = new Vector2D(x - MeasuredWidth / 2.0f, y - MeasuredHeight / 2.0f);
            float pressedPointAngle = pressedPoint.GetAngle();

            if ( pressedPoint.GetLength() <= _outterCircleRadius && pressedPoint.GetLength() >= _innerCircleRadius )
            {
                for (int i = 1; i < SemiCircles.Count; i++)
                {
                    if ( ( pressedPointAngle -_globalAngle + 720 )%360 < SemiCircles[i].GetAngle() )
                    { 
                        SelectedChartSegmentChanged(i-1);
                        return;
                    }
                }
                SelectedChartSegmentChanged(SemiCircles.Count - 1);
                return;
            }
        }

        private void SelectedChartSegmentChanged(int value)
        {
            if (selectedChartSegment == value) return;

            var previousSelectedSegment = selectedChartSegment;
            selectedChartSegment = value;

            float angleDelta = 0;

            if ( selectedChartSegment == SemiCircles.Count - 1)
                angleDelta = 90 - SemiCircles[selectedChartSegment].GetAngle() - ( 360* SemiCircles[selectedChartSegment].GetAngleFraction() - SemiCircles[selectedChartSegment].GetAngle()) / 2.0f;
            else
                angleDelta = 90-SemiCircles[selectedChartSegment].GetAngle() - ( SemiCircles[selectedChartSegment+1].GetAngle()- SemiCircles[selectedChartSegment].GetAngle() )/2.0f;

            ValueAnimator animator = ValueAnimator.OfFloat(new float[] { _globalAngle, angleDelta });
            animator.SetDuration(1000);
            animator.Update += (sender, args) =>
            {
                _globalAngle = (float)animator.AnimatedValue;
                Invalidate();
            };
            var strokeWidth = SemiCircles[value].GetStrokeWidth();
            ValueAnimator selectingAnimation = ValueAnimator.OfFloat(new float[] { strokeWidth, 1.5f*strokeWidth  });
            selectingAnimation.SetDuration(1000);
            selectingAnimation.Update += (sender, args) =>
            {
                SemiCircles[selectedChartSegment].SetStrokeWidth( (float)selectingAnimation.AnimatedValue );
                if(previousSelectedSegment >= 0)
                {
                    SemiCircles[previousSelectedSegment].SetStrokeWidth(strokeWidth - (float)selectingAnimation.AnimatedValue + 1.5f * strokeWidth);
                }
            };
            animator.Start();
            selectingAnimation.Start();
        }
    }
}
