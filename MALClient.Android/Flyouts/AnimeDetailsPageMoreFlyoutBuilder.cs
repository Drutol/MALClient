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
using Com.Orhanobut.Dialogplus;
using Com.Shehabic.Droppy;
using MALClient.Android.DIalogs;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Flyouts
{
    public static class AnimeDetailsPageMoreFlyoutBuilder
    {
        public static DroppyMenuPopup BuildForAnimeDetailsPage(Context context,AnimeDetailsPageViewModel viewModel,View parent,Action<int> listener)
        {
            AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            AnimeListPageFlyoutBuilder.InjectAnimation(droppyBuilder);

            

            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Forum board", listener, 0)));
            if(viewModel.AnimeMode)
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Promotional videos", listener, 1)));
            if(!viewModel.AddAnimeVisibility)
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Tags", listener, 2)));
            if (viewModel.IsRewatchingButtonVisibility)
                droppyBuilder.AddMenuItem(
                    new DroppyMenuCustomItem(
                        AnimeListPageFlyoutBuilder.BuildItem(context, viewModel.RewatchingLabel, listener, 6, viewModel.IsRewatching ? (int?)ResourceExtension.OpaqueAccentColour : null)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Copy link", listener, 3)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Open in browser", listener, 4)));
            if (!viewModel.AddAnimeVisibility)
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Remove from my list", listener, 5)));


            return droppyBuilder.Build();
        }
    }
}