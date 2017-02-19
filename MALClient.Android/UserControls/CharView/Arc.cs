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
            private float _strokeWidth;
            public float StrokeWidth
            {
                get { return _strokeWidth; }
                set
                {
                    _strokeWidth = value;
                    Paint.StrokeWidth = value;
                }
            }

            private float _lengthFraction = 1;
            public float LengthFraction
            {
                get { return _lengthFraction; }
                set
                {
                    _lengthFraction = SexyMath.Normalize(value, 0.0f, 1.0f);
                    updateDashLength();
                }
            }

            private float DrawingRadius { get; set; }
            private float _standardStrokeWidth;
            private float StandardStrokeWidth
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
                    updateDashLength();
                }
            }
            private DashPathEffect _dashEffect = new DashPathEffect(new float[] { 10, 100000 }, 0);
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
                Length = CurrentValue * SexyMath.CirclePeremiter(DrawingRadius) / sum;
            }

            private void updateDashLength()
            {
                _dashEffect = new DashPathEffect(new float[] { Length * LengthFraction, 100000 }, 0);
                Paint.SetPathEffect(_dashEffect);
            }
        }
    }
}