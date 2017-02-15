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

namespace MALClient.Android.UserControls
{
    class ChartView : View
    {
        private class SemiCircle
        {
            private float _value;
            private Paint _paint;
            private float _lengthTotal;
            private DashPathEffect _dashEffect;
            private float _angle = 0;
            private float _lengthFraction= 0;

            public SemiCircle(int value, int sum, Color color, int fraction = 1)
            {
                _value = value;
                _lengthTotal = value * (_innerCircleRadius + _outterCircleRadius) * (float)System.Math.PI / sum;
                SetLengthFraction(fraction);
                setPaint(value, color);
            }

            public void Draw(Canvas canvas)
            {
                canvas.Save();
                    canvas.Rotate(_angle);
                    canvas.DrawCircle(0, 0, (_innerCircleRadius + _outterCircleRadius) / 2.0f, _paint);
                canvas.Restore();
            }

            public Color GetColor()
            {
                return _paint.Color;
            }
            public void SetLengthFraction(float value)
            {
                _lengthFraction = value;
                _dashEffect = new DashPathEffect(new float[] { _lengthTotal * _lengthFraction, 1000000 }, 0); //BIG number to be sure I get only one dash line.
            }

            public void SetAngle(float angle)
            {
                _angle = angle;
                if (_angle > 360) angle -= 360;
                else if (_angle < 0) angle += 360;
            }

            public float GetAngle()
            {
                return _angle;
            }

            private void setPaint(int value, Color color)
            {
                _paint = new Paint();
                _paint.Color = color;
                _paint.SetStyle(Paint.Style.Stroke);
                _paint.StrokeWidth = (_outterCircleRadius - _innerCircleRadius);
                _paint.AntiAlias = true;
                _paint.SetPathEffect(_dashEffect);
            }

            public void SetStrokeWidth(float value)
            {
                _paint.StrokeWidth = value;
            }

            public float GetStrokeWidth()
            {
                return _paint.StrokeWidth;
            }

            public float GetValue()
            {
                return _value;
            }
        }
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
                SemiCircles.Add(new SemiCircle(value, sum, color));
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
                    selectSegment(x, y);
                    break;
                case MotionEventActions.Move:
                    break;
                case MotionEventActions.Down:

                    break;
            }
        }

        private void selectSegment(float x, float y)
        {
            float relativeX = x - MeasuredWidth / 2.0f;
            float relativeY = y - MeasuredHeight / 2.0f;

            float pressedPointAngle = 0;

            if(relativeX == 0)
            {
                pressedPointAngle = 90;
                if (relativeY < 0)
                    pressedPointAngle += 180;
            }else
            {
                float relativeAngle = 0;

                if (relativeX < 0)
                    relativeAngle += 180;
                else if (relativeY < 0)
                    relativeAngle += 360;

                float atan = (float)( System.Math.Atan(relativeY / relativeX) * (180 / System.Math.PI) );
                pressedPointAngle = atan + relativeAngle;
            }
            if (System.Math.Sqrt(System.Math.Pow(relativeX, 2) + System.Math.Pow(relativeY, 2)) <= _outterCircleRadius)
            {
                for (int i = 1; i < SemiCircles.Count; i++)
                {
                    if ( (pressedPointAngle -_globalAngle + 720)%360 < SemiCircles[i].GetAngle() )
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
                angleDelta = 90 - SemiCircles[selectedChartSegment].GetAngle() - (360 - SemiCircles[selectedChartSegment].GetAngle()) / 2.0f;
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
