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
using MALClient.XShared.ViewModels.Details;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Flyouts
{
    public static class RecommendationsFlyoutBuilder
    {
        public static DroppyMenuPopup BuildForRecommendationsPage(Context context, View parent, RecommendationsViewModel viewModel,Action<int> callback)
        {
            AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(200), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            AnimeListPageFlyoutBuilder.InjectAnimation(droppyBuilder);

            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Anime recommendations", callback, 0)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Manga recommendations", callback, 1)));
           

            return droppyBuilder.Build();
        }
    }
}