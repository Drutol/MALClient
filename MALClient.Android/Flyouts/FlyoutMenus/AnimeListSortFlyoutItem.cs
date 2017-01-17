using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Flyouts
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