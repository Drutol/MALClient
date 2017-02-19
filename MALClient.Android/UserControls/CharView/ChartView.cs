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
        private Chart pieChart;
        public event EventHandler OnViewInitialized;
        public event EventHandler<MotionEvent> OnTouch;

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
            pieChart.OnChartUpdated += (sender, args) =>
            {
                Invalidate();
            }; 
            //DEBUG
            pieChart.Add(12, new Color(0, 255, 0, 120));
            pieChart.Add(16, new Color(255, 0, 0, 120));
            //-----//
            var touchListener = new OnTouchListener(onTouch);
            SetOnTouchListener(touchListener);
            OnViewInitialized?.Invoke(this, null);
        }

        protected override void OnDraw(Canvas canvas)
        {
            canvas.Save();
                pieChart.Draw(canvas);
            canvas.Restore();
        }

        private void onTouch(MotionEvent e)
        {
            OnTouch?.Invoke(this, e);
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
