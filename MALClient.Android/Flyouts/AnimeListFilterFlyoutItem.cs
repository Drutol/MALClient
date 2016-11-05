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
using Android.Support.V4.Content.Res;
using Android.Text;
using Android.Views;
using Android.Widget;
using MALClient.Android.Activities;
using MALClient.Android.Resources;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using Org.Zakariya.Flyoutmenu;

namespace MALClient.Android.Flyouts
{
    public class AnimeListFilterFlyoutItem : TextBasedFlyoutBase
    {
        public readonly int Status;

        public AnimeListFilterFlyoutItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) {}

        public AnimeListFilterFlyoutItem(AnimeStatus status) : base((int)status,TextColorBase)
        {
            Status = (int)status;
            Text = Utilities.StatusToString(Status);
        }

        public override void OnDraw(Canvas canvas, RectF bounds, float degreeSelected)
        {
            TextPaint.Color = ViewModelLocator.AnimeList.CurrentStatus == Status ? TextColorAccent : TextColorBase;
            base.OnDraw(canvas,bounds,degreeSelected);
        }
    }
}