using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using Org.Zakariya.Flyoutmenu;

namespace MALClient.Android.Flyouts
{
    public class AnimeListFilterFlyoutItem : FlyoutMenuView.MenuItem
    {
        private readonly AnimeStatus _status;

        private readonly TextPaint _paint;

        public AnimeListFilterFlyoutItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public AnimeListFilterFlyoutItem(AnimeStatus status) : base((int)status)
        {
            _status = status;
            _paint = new TextPaint
            {
                TextSize = 22,
                TextAlign = Paint.Align.Center
            };
            _paint.SetStyle(Paint.Style.Fill);
            _paint.Color = Color.Black;
        }

        public override void OnDraw(Canvas canvas, RectF bounds, float degreeSelected)
        {
            canvas.DrawText(Utilities.StatusToString((int)_status), bounds.CenterX(), bounds.CenterY() - (_paint.Descent() + _paint.Ascent())/2, _paint);
        }
    }

    public class AnimeListFilterFlyoutRenderer : FlyoutMenuView.ButtonRenderer
    {
        public override void OnDrawButtonContent(Canvas p0, RectF p1, int p2, float p3)
        {
            
        }
    }
}