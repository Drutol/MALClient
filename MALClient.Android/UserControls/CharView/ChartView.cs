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
        private Chart pieChart;

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
            pieChart = new Chart(this);
            //DEBUG
            pieChart.AddArc(15, new Color(0xdd, 0x21, 0x6c, 255));
            pieChart.AddArc(5, new Color(0x21, 0xdd, 0x90, 255));
            pieChart.AddArc(10, new Color(0xff, 0xe0, 0x47, 255));
            pieChart.AddArc(30, new Color(0x74, 0xc3, 0x26, 255));
            pieChart.AddArc(4, new Color(0x21, 0xb3, 0xdd, 255));
            //-----//
            var touchListener = new OnTouchListener(onTouch);
            this.SetOnTouchListener(touchListener);
        }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.Translate(canvas.Width / 2, canvas.Height / 2);
            canvas.Save();
                pieChart.Draw(canvas);
            canvas.Restore();
        }

        private void onTouch(MotionEvent e)
        {
            float x = e.GetX();
            float y = e.GetY();

            switch(e.Action)
            {
                case MotionEventActions.Up:
                    //DEBUG//
                    if (SexyMath.CheckCollision(pieChart.InnerCircleRadius, new Vector2D(x - MeasuredWidth/2.0f, y - MeasuredHeight/2.0f)))
                    {
                        pieChart.incVal();
                    }
                    //----//
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
            /*if (_animationInProgress) return;
            Vector2D pressedPoint = new Vector2D(x - MeasuredWidth / 2.0f, y - MeasuredHeight / 2.0f);
            float pressedPointAngle = pressedPoint.GetAngle();

            if ( pressedPoint.GetLength() <= _outterCircleRadius && pressedPoint.GetLength() >= _innerCircleRadius )
            {
                for (int i = 1; i < Arcs.Count; i++)
                {
                    if ( ( pressedPointAngle -_globalAngle + 720 )%360 < Arcs[i].GetAngle() )
                    { 
                        SelectedChartSegmentChanged(i-1);
                        return;
                    }
                }
                SelectedChartSegmentChanged(Arcs.Count - 1);
                return;
            }*/
        }

        private void SelectedChartSegmentChanged(int value)
        {
            /*if (selectedChartSegment == value) return;

            var previousSelectedSegment = selectedChartSegment;
            selectedChartSegment = value;

            float angleDelta = 0;

            if ( selectedChartSegment == Arcs.Count - 1)
                angleDelta = _selectedAngle - Arcs[selectedChartSegment].GetAngle() - ( 360* Arcs[selectedChartSegment].GetAngleFraction() - Arcs[selectedChartSegment].GetAngle()) / 2.0f;
            else
                angleDelta = _selectedAngle - Arcs[selectedChartSegment].GetAngle() - ( Arcs[selectedChartSegment+1].GetAngle()- Arcs[selectedChartSegment].GetAngle() )/2.0f;

            ValueAnimator animator = ValueAnimator.OfFloat(new float[] { _globalAngle, angleDelta });
            animator.SetDuration(750);
            animator.Update += (sender, args) =>
            {
                _globalAngle = (float)animator.AnimatedValue;
                Invalidate();
            };
            var strokeWidth = Arcs[value].GetStrokeWidth();
            ValueAnimator selectingAnimation = ValueAnimator.OfFloat(new float[] { strokeWidth, 1.5f*strokeWidth  });
            selectingAnimation.SetDuration(750);
            selectingAnimation.Update += (sender, args) =>
            {
                Arcs[selectedChartSegment].SetStrokeWidth( (float)selectingAnimation.AnimatedValue );
                if(previousSelectedSegment >= 0)
                {
                    Arcs[previousSelectedSegment].SetStrokeWidth(strokeWidth - (float)selectingAnimation.AnimatedValue + 1.5f * strokeWidth);
                }
            };
            selectingAnimation.AnimationEnd += (sender, args) =>
            {
                _animationInProgress = false;
            };
            _animationInProgress = true;
            animator.Start();
            selectingAnimation.Start();*/
        }
    }
}
