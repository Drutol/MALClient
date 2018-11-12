using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Shehabic.Droppy;
using Com.Shehabic.Droppy.Animations;
using MALClient.Android.Activities;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Flyouts
{
    public enum AnimeGridItemMoreFlyoutButtons
    {
        CopyLink,
        OpenInBrowser,
        SetStatus,
        SetRating,
        SetWatched,
        CopyTitle,
    }

    public static class AnimeListPageFlyoutBuilder
    {
        public const int TextViewTag = 34534;
        public static RelativeLayout.LayoutParams ParamTextView { get; }
        public static ViewGroup.LayoutParams ParamRelativeLayout { get; set; }

        static AnimeListPageFlyoutBuilder()
        {
            ParamTextView = new RelativeLayout.LayoutParams(-1,-2); //wrap content
            ParamTextView.AddRule(LayoutRules.AlignParentLeft);
            ParamTextView.AddRule(LayoutRules.CenterVertical);
        }

        public static void InjectAnimation(DroppyMenuPopup.Builder builder)
        {        
            builder.SetPopupAnimation(new DroppyFadeInAnimation());
            builder.TriggerOnAnchorClick(false);
            builder.SetXOffset(5);
            builder.SetYOffset(5);
        }

        public static View BuildItem(Context context,string text,Action<int> callback,int id,int? background = null,int? foreground = null,bool clickable = true,GravityFlags? gravity = null,bool wrapContentHeight = false)
        {
            var holder = BuildBaseItem(context, text,background,foreground,clickable,gravity,wrapContentHeight);

            holder.Click += (sender, args) =>
            {
                callback.Invoke(id);
            };

            return holder;
        }

        public static View BuildItem<T>(Context context,string text, T tag, Action<T> callback, int? background = null, int? foreground = null) where T : class
        {
            var holder = BuildBaseItem(context, text, background, foreground);
            holder.Tag = tag.Wrap();
            holder.Click += (sender, args) => callback.Invoke((sender as View).Tag.Unwrap<T>());
            return holder;
        }

        public static View BuildBaseItem(Context context, string text,int? background = null, int? foreground = null, bool clickable = true,GravityFlags ? gravity = null, bool wrapContentHeight = false)
        {
            background = background ?? ResourceExtension.BrushFlyoutBackground;
            foreground = foreground ?? ResourceExtension.BrushText;

            if (ParamRelativeLayout == null)
                ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), -2);
            else
                ParamRelativeLayout.Height = -2;

            var top = new FrameLayout(context);


            top.SetBackgroundColor(new Color(background.Value));
            var holder = new RelativeLayout(context) { LayoutParameters = wrapContentHeight ? new
                ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150),-2) : ParamRelativeLayout };
            var margin = DimensionsHelper.DpToPx(8);
            holder.SetPadding(margin, margin, margin, margin);
            holder.SetBackgroundResource(ResourceExtension.SelectableItemBackground);
            if (clickable)
            {
                top.Clickable = true;
                top.Focusable = true;
            }

            var txt = new TextView(context) { LayoutParameters = ParamTextView };
            txt.SetMaxLines(3);

            if (gravity != null)
            {
                txt.Gravity = gravity.Value;
            }
            txt.SetTextColor(new Color(foreground.Value));
            txt.Text = text;
            txt.Id = TextViewTag;

            holder.AddView(txt);
            top.AddView(holder);

            return top;
        }

       
        public static DroppyMenuPopup BuildForAnimeStatusSelection(Context context, View parent,
            Action<AnimeStatus> callback,AnimeStatus currentStatus,bool manga)
        {
            ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            InjectAnimation(droppyBuilder);

            var listener = new Action<int>(i => callback.Invoke((AnimeStatus)i));

            foreach (AnimeStatus value in Enum.GetValues(typeof(AnimeStatus)))
            {
                if (value == currentStatus)
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, Utilities.StatusToString((int)value,manga), listener, (int) value,
                            ResourceExtension.BrushSelectedDialogItem, ResourceExtension.AccentColour)));
                else //highlighted
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, Utilities.StatusToString((int)value,manga), listener, (int) value)));
            }
            droppyBuilder.SetYOffset(DimensionsHelper.DpToPx(30));
            return droppyBuilder.Build();
        }

        public static DroppyMenuPopup BuildForAnimeSortingSelection(Context context, View parent,
            Action<SortOptions> callback,SortOptions currentOption)
        {
            ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            InjectAnimation(droppyBuilder);


            var listener = new Action<int>(i => callback.Invoke((SortOptions)i));

            foreach (SortOptions value in Enum.GetValues(typeof(SortOptions)))
            {
                if (value == currentOption)
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, value.GetDescription(), listener, (int) value,
                            ResourceExtension.BrushSelectedDialogItem, ResourceExtension.AccentColour)));
                else //highlighted
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, value.GetDescription(), listener, (int) value)));
            }
            droppyBuilder.SetYOffset(DimensionsHelper.DpToPx(30));
            return droppyBuilder.Build();
        }

        public static DroppyMenuPopup BuildForAnimeListDisplayModeSelection(Context context, View parent,IEnumerable<Tuple<AnimeListDisplayModes,string>> items,
            Action<AnimeListDisplayModes> callback, AnimeListDisplayModes currentMode)
        {
            ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            InjectAnimation(droppyBuilder);


            var listener = new Action<int>(i => callback.Invoke((AnimeListDisplayModes)i));

            foreach (var item in items)
            {
                if (item.Item1 == currentMode)
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, item.Item2, listener, (int)item.Item1,
                            ResourceExtension.BrushSelectedDialogItem, ResourceExtension.AccentColour)));
                else //highlighted
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, item.Item2, listener, (int)item.Item1)));
            }

            droppyBuilder.SetYOffset(DimensionsHelper.DpToPx(30));

            return droppyBuilder.Build();
        }

        public static DroppyMenuPopup BuildForAnimeSeasonSelection(Context context, View parent,Action<int> callback,
            AnimeListViewModel viewModel)
        {
            ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            InjectAnimation(droppyBuilder);

            var listener = new Action<int>(callback.Invoke);

            int index = 0;
            foreach (var season in viewModel.SeasonSelection)
            {
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(BuildItem(context, season.Name, listener, index++)));
            }
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(Resource.Layout.SeasonSelectionPopup));

            return droppyBuilder.Build();
        }
    }    
}