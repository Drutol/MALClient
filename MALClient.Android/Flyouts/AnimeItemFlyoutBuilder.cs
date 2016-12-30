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
using Com.Shehabic.Droppy;
using Com.Shehabic.Droppy.Views;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Flyouts
{
    public static class AnimeItemFlyoutBuilder
    {

        public static DroppyMenuPopup BuildForAnimeItem(Context context, View parent, AnimeItemViewModel viewModel, Action<AnimeGridItemMoreFlyoutButtons> callback,bool forceSmall = false)
        {
            AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            AnimeListPageFlyoutBuilder.InjectAnimation(droppyBuilder);


            var listener = new Action<int>(i => callback.Invoke((AnimeGridItemMoreFlyoutButtons)i));

            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Copy to clipboard", listener, 0)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Open in browser", listener, 1)));
            if (!forceSmall && viewModel.Auth)
            {
                droppyBuilder.AddSeparator();
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Set status", listener, 2)));
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Set score", listener, 3)));
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Set watched", listener, 4)));
            }
            return droppyBuilder.Build();
        }

        public static DroppyMenuPopup BuildForAnimeItemTags(Context context, View parent, AnimeItemViewModel viewModel, Action callback)
        {
            AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            AnimeListPageFlyoutBuilder.InjectAnimation(droppyBuilder);


            var listener = new Action<int>(i => callback.Invoke());

            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Tags:", listener, 0,ResourceExtension.BrushRowAlternate2,null,false,GravityFlags.CenterHorizontal)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(new DroppyMenuSeparatorView(context)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, viewModel.Notes , listener, 1, null, null, false, GravityFlags.CenterHorizontal)));

            return droppyBuilder.Build();
        }
    }
}