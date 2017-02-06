using System;
using Android.Graphics;
using Android.Runtime;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Flyouts.FlyoutMenus
{
    public class AnimeListSortFlyoutItem : TextBasedFlyoutBase
    {
        public readonly SortOptions SortOption;

        public AnimeListSortFlyoutItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public AnimeListSortFlyoutItem(SortOptions sorting) : base((int)sorting, TextColorBase)
        {
            SortOption = sorting;
            Text = sorting.GetDescription();
        }

        public override void OnDraw(Canvas canvas, RectF bounds, float degreeSelected)
        {
            TextPaint.Color = ViewModelLocator.AnimeList.SortOption == SortOption ? TextColorAccent : TextColorBase;
            base.OnDraw(canvas, bounds, degreeSelected);
        }
    }
}