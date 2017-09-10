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
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.UserControls;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.FriendsPageFragments
{
    public class FriendsPageRequestsTabFragment : MalFragmentBase
    {
        private FriendsPageViewModel ViewModel = ViewModelLocator.Friends;

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.Requests).WhenSourceChanges(() =>
            {
                if(ViewModel.Requests == null)
                    return;
                ListView.InjectFlingAdapter(ViewModel.Requests,v => new FriendRequestViewHolder(v),DataTemplateFull,DataTemplateFling,DataTemplateBasic,ContainerTemplate);
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingPending,
                    () => LoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(
                this.SetBinding(() => ViewModel.RequestsEmptyNoticeVisibility,
                    () => EmptyNotice.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            var sc = RootView as ScrollableSwipeToRefreshLayout;
            sc.ScrollingView = ListView;
            sc.Refresh += ScOnRefresh;
        }

        private void ScOnRefresh(object sender, EventArgs eventArgs)
        {
            (RootView as ScrollableSwipeToRefreshLayout).Refreshing = false;
            ViewModel.RefreshPendingCommand.Execute(null);
        }

        public override void OnDestroy()
        {
            ListView.ClearFlingAdapter();
            base.OnDestroy();
        }

        private View ContainerTemplate(int i)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.FriendsPageRequestItem, null);
            view.SetOnClickListener(new OnClickListener(OnFriendClick));

            view.FindViewById(Resource.Id.AcceptButton).SetOnClickListener(new OnClickListener(view1 =>
            {
                ViewModel.AcceptFriendRequestCommand.Execute(view1.Tag.Unwrap<MalFriendRequest>());
            }));

            view.FindViewById(Resource.Id.DenyButton).SetOnClickListener(new OnClickListener(view1 =>
            {
                ViewModel.DenyFriendRequestCommand.Execute(view1.Tag.Unwrap<MalFriendRequest>());
            }));

            return view;
        }

        private void OnFriendClick(View view)
        {
            ViewModel.NavigateUserCommand.Execute(view.Tag.Unwrap<MalFriendRequest>().User);
        }

        private void DataTemplateBasic(View view, int i, MalFriendRequest model, FriendRequestViewHolder holder)
        {
            holder.Name.Text = model.User.Name;
            if (!string.IsNullOrEmpty(model.Message))
            {
                holder.Message.Text = model.Message;
                holder.Message.Visibility = ViewStates.Visible;
            }
            else
                holder.Message.Visibility = ViewStates.Gone;
            
            
            holder.AcceptButton.Tag = holder.DenyButton.Tag = view.Tag;
        }

        private void DataTemplateFling(View view, int i, MalFriendRequest model, FriendRequestViewHolder holder)
        {
            holder.Image.Visibility = holder.Image.IntoIfLoaded(model.User.ImgUrl, new CircleTransformation()) ? ViewStates.Visible : ViewStates.Gone;
        }

        private void DataTemplateFull(View view, int i, MalFriendRequest model, FriendRequestViewHolder holder)
        {
            holder.Image.Into(model.User.ImgUrl, new CircleTransformation());
            holder.Image.Visibility = ViewStates.Visible;
        }



        public override int LayoutResourceId => Resource.Layout.FriendsPageTab;

        #region Views

        private ListView _listView;
        private ProgressBar _loadingSpinner;
        private TextView _emptyNotice;

        public ListView ListView => _listView ?? (_listView = FindViewById<ListView>(Resource.Id.ListView));

        public ProgressBar LoadingSpinner => _loadingSpinner ?? (_loadingSpinner = FindViewById<ProgressBar>(Resource.Id.LoadingSpinner));

        public TextView EmptyNotice => _emptyNotice ?? (_emptyNotice = FindViewById<TextView>(Resource.Id.EmptyNotice));

        #endregion


        class FriendRequestViewHolder
        {
            private readonly View _view;

            public FriendRequestViewHolder(View view)
            {
                _view = view;
            }

            private ProgressBar _imgPlaceholder;
            private ImageViewAsync _image;
            private TextView _name;
            private TextView _message;
            private Button _acceptButton;
            private Button _denyButton;

            public ProgressBar ImgPlaceholder => _imgPlaceholder ?? (_imgPlaceholder = _view.FindViewById<ProgressBar>(Resource.Id.ImgPlaceholder));

            public ImageViewAsync Image => _image ?? (_image = _view.FindViewById<ImageViewAsync>(Resource.Id.Image));

            public TextView Name => _name ?? (_name = _view.FindViewById<TextView>(Resource.Id.Name));

            public TextView Message => _message ?? (_message = _view.FindViewById<TextView>(Resource.Id.Message));

            public Button AcceptButton => _acceptButton ?? (_acceptButton = _view.FindViewById<Button>(Resource.Id.AcceptButton));

            public Button DenyButton => _denyButton ?? (_denyButton = _view.FindViewById<Button>(Resource.Id.DenyButton));
        }
    }
}