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
            private Paint Paint { get; set; } = new Paint();
            public float StrokeWidth
            {
                get => Paint.StrokeWidth;
                set => Paint.StrokeWidth = value;
            }
            public Color Color
            {
                get => Paint.Color;
                set => Paint.Color = value;
            }

            private DashPathEffect _arc = new DashPathEffect(new float[] { 100, int.MaxValue }, 0);
            public float Length
            {
                set
                {
                    _arc = new DashPathEffect(new float[] { value, int.MaxValue }, 0);
                    Paint.SetPathEffect(_arc);
                }
            }

            public Arc()
            {
                Init();
            }

            private void Init()
            {
                Paint.SetStyle(Paint.Style.Stroke);
                Paint.AntiAlias = true;
                Paint.SetPathEffect(_arc);
                Color = new Color(unchecked( (int)0xFF00FFFF));
                Length = int.MaxValue;
            }

            public void Draw(Canvas canvas, float radius)
            {
                canvas.DrawCircle(0, 0, radius, Paint);
            }
        }
    }
}