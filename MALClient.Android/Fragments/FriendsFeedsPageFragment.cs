using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Util;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.UserControls;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class FriendsFeedsPageFragment : MalFragmentBase
    {
        private FriendsFeedsViewModel ViewModel;
        private GridViewColumnHelper _gridViewColumnHelper;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.FriendsFeeds;
            ViewModel.Init();
        }

        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => FriendsFeedsPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            _gridViewColumnHelper = new GridViewColumnHelper(FriendsFeedsPageGridView,null,2,3);

            Bindings.Add(this.SetBinding(() => ViewModel.Feeds).WhenSourceChanges(() =>
            {
                if (ViewModel.Feeds != null)
                    FriendsFeedsPageGridView.InjectFlingAdapter(ViewModel.Feeds,DataTemplateFull,DataTemplateFling,ContainerTemplate,DataTemplateBasic);
                else
                    FriendsFeedsPageGridView.Adapter = null;
            }));

            var scrollToRefresh = RootView as ScrollableSwipeToRefreshLayout;
            scrollToRefresh.ScrollingView = FriendsFeedsPageGridView;
            scrollToRefresh.Refresh += ScrollToRefreshOnRefresh;
        }

        private void ScrollToRefreshOnRefresh(object sender, EventArgs e)
        {
            (RootView as ScrollableSwipeToRefreshLayout).Refreshing = false;
            ViewModelLocator.GeneralMain.RefreshDataCommand.Execute(null);
        }

        private void DataTemplateBasic(View view, int i, UserFeedEntryModel userFeedEntryModel)
        {
            var date = view.FindViewById<TextView>(Resource.Id.FriendsFeedsPageItemDate);

            if (Settings.MakeGridItemsSmaller && date.Tag == null)
            {
                view.LayoutParameters = new AbsListView.LayoutParams(-1,DimensionsHelper.DpToPx(233));
                var usrImg = view.FindViewById(Resource.Id.FriendsFeedsPageItemUserImageButton);
                usrImg.LayoutParameters.Height = usrImg.LayoutParameters.Width = DimensionsHelper.DpToPx(50);
                usrImg.RequestLayout();
                view.FindViewById<ImageViewAsync>(Resource.Id.FriendsFeedsPageItemImage).SetScaleType(ImageView.ScaleType.CenterCrop);
                view.FindViewById<TextView>(Resource.Id.FriendsFeedsPageItemUserName).SetTextSize(ComplexUnitType.Sp, 13);
                view.FindViewById<TextView>(Resource.Id.FriendsFeedsPageItemTitle).SetTextSize(ComplexUnitType.Sp, 14);
                view.FindViewById<TextView>(Resource.Id.FriendsFeedsPageItemContent).SetTextSize(ComplexUnitType.Sp, 13);
                view.FindViewById<TextView>(Resource.Id.FriendsFeedsPageItemDate).SetTextSize(ComplexUnitType.Sp, 12);
                date.Tag = true;
            }

            view.FindViewById<TextView>(Resource.Id.FriendsFeedsPageItemUserName).Text = userFeedEntryModel.User.Name;
            view.FindViewById<TextView>(Resource.Id.FriendsFeedsPageItemTitle).Text = userFeedEntryModel.Title;
            view.FindViewById<TextView>(Resource.Id.FriendsFeedsPageItemContent).Text = userFeedEntryModel.Description;
            date.Text = userFeedEntryModel.Date.ToDiffString();
        }

        private View ContainerTemplate(int i)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.FriendsFeedsPageItem, null);

            view.Click += RootFeedItemOnClick;
            view.FindViewById(Resource.Id.FriendsFeedsPageItemUserImageButton).Click+= UserButtonOnClick;

            return view;
        }


        private void DataTemplateFling(View view, int i, UserFeedEntryModel userFeedEntryModel)
        {
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.FriendsFeedsPageItemImage);
            var accImg = view.FindViewById<ImageViewAsync>(Resource.Id.FriendsFeedsPageItemUserImage);
            string link = null;
            if(AnimeImageQuery.IsCached(userFeedEntryModel.Id,true, ref link))
                img.Visibility = img.IntoIfLoaded(link) ? ViewStates.Visible : ViewStates.Gone;

            if (!accImg.IntoIfLoaded(userFeedEntryModel.User.ImgUrl,new CircleTransformation()))
                accImg.Visibility = ViewStates.Gone;
        }

        private void DataTemplateFull(View view, int i, UserFeedEntryModel userFeedEntryModel)
        {
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.FriendsFeedsPageItemImage);
            if (img.Tag == null || (string) img.Tag != $"{userFeedEntryModel.Date.Ticks}{userFeedEntryModel.User.Name}")
            {
                img.Tag = $"{userFeedEntryModel.Date.Ticks}{userFeedEntryModel.User.Name}";
                string imgUrl = null;
                if(AnimeImageQuery.IsCached(userFeedEntryModel.Id,true,ref imgUrl))
                    img.Into(imgUrl); 
                else
                    img.IntoWithTask(AnimeImageQuery.GetImageUrl(userFeedEntryModel.Id, true));

                
                view.FindViewById(Resource.Id.FriendsFeedsPageItemUserImageButton).Tag = userFeedEntryModel.User.Wrap(); 
            }

            var accImg = view.FindViewById<ImageViewAsync>(Resource.Id.FriendsFeedsPageItemUserImage);
            if (img.Tag == null || (string)img.Tag != userFeedEntryModel.User.ImgUrl)
            {
                accImg.Into(userFeedEntryModel.User.ImgUrl, new CircleTransformation());
            }
        }

        private void RootFeedItemOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateDeitalsCommand.Execute((sender as View).Tag.Unwrap<UserFeedEntryModel>());
        }

        private void UserButtonOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateProfileCommand.Execute((sender as View).Tag.Unwrap<MalUser>());
        }

        public override int LayoutResourceId => Resource.Layout.FriendsFeedsPage;

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            _gridViewColumnHelper?.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }

        #region Views

        private GridView _friendsFeedsPageGridView;
        private ProgressBar _friendsFeedsPageLoadingSpinner;

        public GridView FriendsFeedsPageGridView => _friendsFeedsPageGridView ?? (_friendsFeedsPageGridView = FindViewById<GridView>(Resource.Id.FriendsFeedsPageGridView));

        public ProgressBar FriendsFeedsPageLoadingSpinner => _friendsFeedsPageLoadingSpinner ?? (_friendsFeedsPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.FriendsFeedsPageLoadingSpinner));

        #endregion
    }
}