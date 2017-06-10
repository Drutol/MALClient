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
using Com.Shehabic.Droppy;
using Com.Shehabic.Droppy.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Flyouts
{
    public class ForumsPinnedPostsFlyoutContext
    {
        private readonly ForumsMainViewModel _viewModel;
        private readonly View _parent;
        private DroppyMenuPopup _menu;
        private ListView _list;

        public ForumsPinnedPostsFlyoutContext(ForumsMainViewModel viewModel,View parent)
        {
            _viewModel = viewModel;
            _parent = parent;
        }

        public void Show()
        {
            if (_menu == null)
                _menu = BuildForPinnedPosts();
            _menu.Show();

            if (_list != null)
            {
                var count = _viewModel.PinnedTopics.Count;
                count = count > 3 ? 3 : count;
                var height = 0;
                for (int i = 0; i < count; i++)
                {
                    height += DimensionsHelper.DpToPx(65);
                }
                var param = _list.LayoutParameters;
                param.Height = height;
                _list.LayoutParameters = param;
            }
        }

        private DroppyMenuPopup BuildForPinnedPosts()
        {
            AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(250), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(_parent.Context, _parent);
            AnimeListPageFlyoutBuilder.InjectAnimation(droppyBuilder);

            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(_parent.Context, "Pinned posts", i => { }, 0, ResourceExtension.BrushRowAlternate2, null, false, GravityFlags.CenterHorizontal)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(new DroppyMenuSeparatorView(_parent.Context)));
            if (_viewModel.PinnedTopics.Any())
            {
                _list = new ListView(_parent.Context) { LayoutParameters = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(250), -2), Id = 7890};
                _list.SetBackgroundColor(new Color(ResourceExtension.BrushAnimeItemInnerBackground));
                _list.Adapter = _viewModel.PinnedTopics.GetAdapter(GetTemplateDelegate);
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(_list));
            }
            else
            {
                var txt = new TextView(_parent.Context)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(-1, -2),
                    Text = "Hold topic to pin it.",                  
                    Gravity = GravityFlags.CenterHorizontal
                };
                txt.SetTypeface(Typeface.Create(ResourceExtension.FontSizeLight,TypefaceStyle.Normal),TypefaceStyle.Normal);
                txt.SetTextColor(new Color(ResourceExtension.BrushNoSearchResults));
                var padding = DimensionsHelper.DpToPx(10);
                txt.SetPadding(0,padding,0,padding);
                txt.SetBackgroundColor(new Color(ResourceExtension.BrushAnimeItemInnerBackground));
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(txt));
            }


            return droppyBuilder.Build();
        }

        private View GetTemplateDelegate(int i, ForumTopicLightEntry entry, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.ForumsPinnedPostItem, null);
                view.Click += ViewOnClick;
                view.FindViewById(Resource.Id.ForumsPinnedPostItemUnpinButton).Click += UnpinButtonOnClick;
            }

            view.Tag = entry.Wrap();

            view.FindViewById<TextView>(Resource.Id.ForumsPinnedPostItemTitle).Text = entry.Title;
            view.FindViewById<TextView>(Resource.Id.ForumsPinnedPostItemDate).Text = entry.Created;
            view.FindViewById<TextView>(Resource.Id.ForumsPinnedPostItemAuthor).Text = entry.Op;

            view.FindViewById(Resource.Id.ForumsPinnedPostItemUnpinButton).Tag = entry.Wrap();

            return view;
        }

        private void UnpinButtonOnClick(object sender, EventArgs eventArgs)
        {
            var view = sender as View;
            _viewModel.UnpinTopicCommand.Execute(view.Tag.Unwrap<ForumTopicLightEntry>());
            (view.Parent.Parent as View).Visibility = ViewStates.Gone;
            if(!_viewModel.PinnedTopics.Any())
                _menu.Dismiss(true);
        }

        private void ViewOnClick(object sender, EventArgs eventArgs)
        {
            _viewModel.GotoPinnedTopicCommand.Execute((sender as View).Tag.Unwrap<ForumTopicLightEntry>());
        }
    }
}