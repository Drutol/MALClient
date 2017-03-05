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
using Com.Mikepenz.Iconics;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Android.BindingInformation
{
    public class ForumBoardEntryBindingInfo : BindingInfo<ForumBoardEntryViewModel>
    {
        private static ForumIndexViewModel ParentViewModel = ViewModelLocator.ForumsIndex;

        public ForumBoardEntryBindingInfo(View container, ForumBoardEntryViewModel viewModel, bool fling) : base(container, viewModel, fling)
        {
            PrepareContainer();
        }

        private Binding _peekBinding;

        protected override void InitBindings()
        {
            if(Fling)
                return;

            Bindings.Add(new Binding<bool, ViewStates>(
                        ParentViewModel,
                        () => ParentViewModel.LoadingSideContentVisibility,
                        Container.FindViewById(Resource.Id.ForumIndexPageBoardItemBoardProgressBar),
                        () => Container.FindViewById(Resource.Id.ForumIndexPageBoardItemBoardProgressBar).Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));
            Bindings.Add(new Binding<bool, ViewStates>(
                    ParentViewModel,
                    () => ParentViewModel.LoadingSideContentVisibility,
                    Container.FindViewById(Resource.Id.ForumIndexPageBoardItemPeekPostSection),
                    () => Container.FindViewById(Resource.Id.ForumIndexPageBoardItemPeekPostSection).Visibility)
                .ConvertSourceToTarget(Converters.BoolToVisibilityInvertedLight));
            _peekBinding = this.SetBinding(() => ParentViewModel.LoadingSideContentVisibility).WhenSourceChanges(Callback);
        }

        private void Callback()
        {
            if (ViewModel.ArePeekPostsAvailable)
            {
                var pp1 = ViewModel.Entry.PeekPosts.First();
                Container.FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemPeekPost1Title).Text =
                    pp1.Title;
                Container.FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemPeekPost1Date).Text =
                    $"{pp1.PostTime} by {pp1.User.Name}";
                Container.FindViewById<ImageViewAsync>(Resource.Id.ForumIndexPageBoardItemPeekPost1Image)
                    .Into(pp1.User.ImgUrl);
                if (ViewModel.Entry.PeekPosts.Count() == 2)
                {
                    var pp2 = ViewModel.Entry.PeekPosts.Last();
                    Container.FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemPeekPost2Title).Text =
                        pp2.Title;
                    Container.FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemPeekPost2Date).Text =
                        $"{pp2.PostTime} by {pp2.User.Name}";
                    Container.FindViewById<ImageViewAsync>(Resource.Id.ForumIndexPageBoardItemPeekPost2Image)
                        .Into(pp2.User.ImgUrl);
                }
            }
        }

        protected override void InitOneTimeBindings()
        {
            Container.SetOnClickListener(new OnClickListener(view =>
                ParentViewModel.GoToLastPostCommand.Execute(
                    view.Tag.Unwrap<ForumBoardEntryViewModel>().Board)));
            var iconImg = Container.FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemIcon);
            iconImg.SetText(DummyFontAwesomeToRealFontAwesomeConverter.Convert(ViewModel.Icon));
            iconImg.Typeface = FontManager.GetTypeface(Container.Context,FontManager.TypefacePath);
            Container.FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemBoardName).Text =
                ViewModel.Entry.Name;
            Container.FindViewById<TextView>(Resource.Id.ForumIndexPageBoardItemDecription).Text =
                ViewModel.Entry.Description;

        }

        protected override void DetachInnerBindings()
        {
            
        }
    }
}