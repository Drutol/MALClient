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

namespace MALClient.Android.UserControls
{
    public partial class SemiCircle
    {
        private float _value;
        private Paint _paint;
        private float _lengthTotal;
        private DashPathEffect _dashEffect;
        private float _angle = 0;
        private float _lengthFraction = 1;
        private float _angleFraction = 0.5f;
        private float _innerRadius;
        private float _outterRadius;

        public SemiCircle(int value, int sum, float innerRadius, float outterRadius, Color color)
        {
            _value = value;
            _lengthTotal = value * (innerRadius + outterRadius) * (float)System.Math.PI / sum;
            _innerRadius = innerRadius;
            _outterRadius = outterRadius;
            _dashEffect = new DashPathEffect(new float[] { _lengthTotal , 1000000 }, 0);
            setPaint(value, color);
        }

        public void Draw(Canvas canvas)
        {
            canvas.Save();
            canvas.Rotate( GetAngle() );
            canvas.DrawCircle(0, 0, (_innerRadius + _outterRadius) / 2.0f, _paint);
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
        public void SetAngleFraction(float value)
        {
            _angleFraction = value;
        }

        public void SetAngle(float angle)
        {
            _angle = angle;
            if (_angle > 360) angle -= 360;
            else if (_angle < 0) angle += 360;
        }

        public float GetAngle()
        {
            return _angle * _angleFraction;
        }

        public float GetAngleFraction()
        {
            return _angleFraction;
        }

        private void setPaint(int value, Color color)
        {
            _paint = new Paint();
            _paint.Color = color;
            _paint.SetStyle(Paint.Style.Stroke);
            _paint.StrokeWidth = (_outterRadius - _innerRadius);
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
}