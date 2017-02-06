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
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Flyouts
{
    public static class AnimeDetailsPageMoreFlyoutBuilder
    {
        public static DroppyMenuPopup BuildForAnimeDetailsPage(Context context,View parent,AnimeDetailsPageViewModel viewModel)
        {
            AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            AnimeListPageFlyoutBuilder.InjectAnimation(droppyBuilder);

            Action<int> listener = i =>
            {
                switch (i)
                {
                    case 0:
                        viewModel.NavigateForumBoardCommand.Execute(null);
                        break;
                    case 1:
                        AnimeDetailsPageDialogBuilder.BuildPromotionalVideoDialog(viewModel);
                        break;
                    case 2:
                        AnimeUpdateDialogBuilder.BuildTagDialog(viewModel);
                        break;
                    case 3:
                        viewModel.CopyToClipboardCommand.Execute(null);
                        break;
                    case 4:
                        viewModel.OpenInMalCommand.Execute(null);
                        break;
                    case 5:
                        viewModel.RemoveAnimeCommand.Execute(null);
                        break;
                }
            };

            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Forum board", listener, 0)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Promotional videos", listener, 1)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Tags", listener, 2)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Copy link", listener, 3)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Open in browser", listener, 4)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Remove from my list", listener, 5)));


            return droppyBuilder.Build();
        }
    }
}