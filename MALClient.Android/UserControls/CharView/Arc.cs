using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using MALClient.XShared.Utils;
using Android.Animation;

namespace MALClient.Android.UserControls
{
    public partial class Chart
    {
        private class Arc
        {
            #region Properties&Fields

            public float ChartFraction { get; set; }
            private float _value;
            public float Value
            {
                get { return _value; }
                set
                {
                    OnValueSet?.Invoke(this, value);
                    _value = value;
                }
            }
            private float _currentValue = 0;
            public float CurrentValue
            {
                get { return _currentValue; }
                set
                {
                    _currentValue = value;
                    OnValueChanged?.Invoke(this, value);
                }
            }
            public Paint Paint { get; set; } = new Paint();
            public event EventHandler<float> OnValueChanged;
            public event EventHandler<float> OnValueSet;
            public event EventHandler OnNeedRefresh;
            private float _lastSum = 0;
            private float _strokeWidth;
            public float StrokeWidth
            {
                get { return _strokeWidth; }
                set
                {
                    _strokeWidth = value;
                    Paint.StrokeWidth = value;
                    OnNeedRefresh?.Invoke(this, null);
                }
            }

            private float _lengthFraction = 1;
            public float LengthFraction
            {
                get { return _lengthFraction; }
                set
                {
                    _lengthFraction = SexyMath.Normalize(value, 0.0f, 1.0f);
                    UpdateDashLength();
                    OnNeedRefresh?.Invoke(this, null);
                }
            }

            private float DrawingRadius { get; set; }
            private float _standardStrokeWidth;
            public float StandardStrokeWidth
            {
                set { _standardStrokeWidth = value; }
                get { return _standardStrokeWidth; }
            }

            public float Angle { get; set; } = 0;

            private float _length;
            private float Length
            {
                get { return _length; }
                set
                {
                    _length = value;
                    UpdateDashLength();
                }
            }
            private DashPathEffect _dashEffect = new DashPathEffect(new float[] { 10, 1000000 }, 0);
#endregion
            public Arc(float drawingRadius, float strokeWidth, Color color)
            {
                Paint.Color = color;
                Paint.SetStyle(Paint.Style.Stroke);
                DrawingRadius = drawingRadius;
                StandardStrokeWidth = strokeWidth;
                StrokeWidth = strokeWidth;
            }

            public void Draw(Canvas canvas)
            {
                canvas.Save();
                    canvas.Rotate(Angle);
                    canvas.DrawCircle(0, 0, DrawingRadius, Paint);
                canvas.Restore();
            }

            public void RescaleToSum(float sum)
            {
                Length = CurrentValue / sum * SexyMath.CirclePeremiter(DrawingRadius);
                _lastSum = sum;
                ChartFraction = Value * LengthFraction / _lastSum;
            }

            private void UpdateDashLength()
            {
                _dashEffect = new DashPathEffect(new float[] { Length * LengthFraction, 1000000 }, 0);
                Paint.SetPathEffect(_dashEffect);
            }
        }
    }
}