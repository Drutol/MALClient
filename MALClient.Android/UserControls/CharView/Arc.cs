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
            private readonly Chart _chart;
            private ValueAnimator ValueChangeAnimation;

            private float _currEndValue=0;
            private float _value = 0;
            private float Value
            {
                get { return _value; }
                set
                {
                    if (value < 0) return;
                    float prev = _value;
                    _value = value;
                    _chart.currentSum += value - prev;
                }
            }

            public Paint Paint { get; set; } = new Paint();

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

            private float _lengthFraction;
            public float LengthFraction
            {
                get { return _lengthFraction; }
                set
                {
                    _lengthFraction = SexyMath.Normalize(value, 0.0f, 1.0f);
                    _dashEffect = new DashPathEffect(new float[] { _lengthTotal * _lengthFraction, 1000000 }, 0); //BIG number to be sure I get only one dash line.
                }
            }

            private float _standardStrokeWidth;
            public float StandardStrokeWidth
            {
                get { return _standardStrokeWidth; }
            }

            public float Angle { get; set; } = 0;

            private float _lengthTotal;
            private DashPathEffect _dashEffect = new DashPathEffect(new float[] { 10, 1000 }, 0);
            private float _drawingRadius;
#endregion
            public Arc(float value, Color color, Chart chart)
            {
                _chart = chart;
                _drawingRadius = ( chart.InnerCircleRadius + chart.OutterCircleRadius )/2.0f;
                _currEndValue = value;
                LengthFraction = 1;
                
                StrokeWidth = 2.0f * (chart.OutterCircleRadius - _drawingRadius);
                _standardStrokeWidth = StrokeWidth;
                ValueChangeAnimation = ValueAnimator.OfFloat();
                ValueChangeAnimation.SetDuration(2000);
                ValueChangeAnimation.Update += (sender, args) =>
                {
                    Value = (float)ValueChangeAnimation.AnimatedValue;
                };

                Paint.Color = color;
                Paint.SetStyle(Paint.Style.Stroke);
                Paint.StrokeWidth = StrokeWidth;
                Paint.AntiAlias = true;
                Paint.SetPathEffect(_dashEffect);

                Value = value;
                UpdateLength();

                //DEBUG//
                Circle circ = new Circle(0, 0, 5.0f);
                Circle newCirc = new Circle(9.00001f, 5, 4.0f);

                bool elo = circ.CheckCollision(newCirc as Shape);
                //-----//
            }

            public void Draw(Canvas canvas)
            {
                canvas.Save();
                canvas.Rotate(Angle);
                canvas.DrawCircle(0, 0, _drawingRadius, Paint);
                canvas.Restore();
            }

            public void UpdateLength()
            {
                _lengthTotal = Value * SexyMath.CirclePeremiter(_drawingRadius) / _chart.currentSum;
                _dashEffect = new DashPathEffect(new float[] { LengthFraction * _lengthTotal, 1000000 }, 0);
                Paint.SetPathEffect(_dashEffect);
            }

            public void SetValue(float value)
            {
                ValueChangeAnimation.Cancel();
                ValueChangeAnimation.SetFloatValues(new float[] { Value, _currEndValue = _currEndValue + value });
                ValueChangeAnimation.Start();
            }

            public float GetValue()
            {
                return Value;
            }
        }
    }
}