using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using MALClient.Android.Resources;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Org.Zakariya.Flyoutmenu;

namespace MALClient.Android.Flyouts
{
    public abstract class TextBasedFlyoutBase : FlyoutMenuView.MenuItem
    {
        protected static readonly Color TextColorBase = new Color(ResourceExtension.BrushText);
        protected static readonly Color TextColorAccent = new Color(ResourceExtension.AccentColour);
        protected static readonly Paint BackgroundPaint = new Paint {Alpha = 0};

        protected string Text { get; set; }
        protected Paint TextPaint { get; } 

        protected TextBasedFlyoutBase(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) {}


        protected TextBasedFlyoutBase(int p0,Color defaultColor) : base(p0)
        {
            TextPaint = new TextPaint
            {
                TextSize = 24,
                TextAlign = Paint.Align.Center
            };
            TextPaint.AntiAlias = true;
            TextPaint.ElegantTextHeight = true;
            TextPaint.SetStyle(Paint.Style.Fill);
            TextPaint.Color = defaultColor;
        }

        public override void OnDraw(Canvas canvas, RectF bounds, float degreeSelected)
        {
            canvas.DrawRect(bounds,BackgroundPaint);
            canvas.DrawText(Text, bounds.CenterX(), bounds.CenterY() - (TextPaint.Descent() + TextPaint.Ascent()) / 2, TextPaint);
        }
    }
}