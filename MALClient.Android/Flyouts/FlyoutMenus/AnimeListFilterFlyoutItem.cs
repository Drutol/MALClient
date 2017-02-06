using System;
using Android.Graphics;
using Android.Runtime;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Flyouts.FlyoutMenus
{
    public class AnimeListFilterFlyoutItem : TextBasedFlyoutBase
    {
        public readonly int Status;

        public AnimeListFilterFlyoutItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) {}

        public AnimeListFilterFlyoutItem(AnimeStatus status) : base((int)status,TextColorBase)
        {
            Status = (int)status;
            Text = XShared.Utils.Utilities.StatusToString(Status);
        }

        public override void OnDraw(Canvas canvas, RectF bounds, float degreeSelected)
        {
            TextPaint.Color = ViewModelLocator.AnimeList.CurrentStatus == Status ? TextColorAccent : TextColorBase;
            base.OnDraw(canvas,bounds,degreeSelected);
        }
    }
}