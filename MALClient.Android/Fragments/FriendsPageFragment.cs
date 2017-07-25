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
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class FriendsPageFragment : MalFragmentBase
    {
        private readonly FriendsPageNavArgs _navArgs;

        private FriendsPageViewModel ViewModel = ViewModelLocator.Friends;

        public FriendsPageFragment(FriendsPageNavArgs args)
        {
            _navArgs = args;
        }


        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel.NavigatedTo(_navArgs);
        }

        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => LoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(this.SetBinding(() => ViewModel.Friends).WhenSourceChanges(() =>
            {
                ListView.InjectFlingAdapter(ViewModel.Friends, HolderFactory, DataTemplateFull,
                    DataTemplateFling, DataTemplateBasic, ContainerTemplate);
            }));
        }

        private FriendsItemViewHolder HolderFactory(View view) => new FriendsItemViewHolder(view);


        private View ContainerTemplate(int i)
        {
            var view =  Activity.LayoutInflater.Inflate(Resource.Layout.FriendsPageItem, null);
            view.SetOnClickListener(new OnClickListener(OnFriendClick));

            return view;
        }

        private void OnFriendClick(View view)
        {
            ViewModel.NavigateUserCommand.Execute(view.Tag.Unwrap<MalFriend>());
        }

        private void DataTemplateBasic(View view, int i, MalFriend model, FriendsItemViewHolder holder)
        {
            holder.Name.Text = model.User.Name;
            holder.FriendsSince.Text = model.FriendsSince;
            holder.LastOnline.Text = model.LastOnline;
        }

        private void DataTemplateFling(View view, int i, MalFriend model, FriendsItemViewHolder holder)
        {
            holder.Image.Visibility = holder.Image.IntoIfLoaded(model.User.ImgUrl,new CircleTransformation()) ? ViewStates.Visible : ViewStates.Gone;
        }

        private void DataTemplateFull(View view, int i, MalFriend model, FriendsItemViewHolder holder)
        {
            holder.Image.Into(model.User.ImgUrl,new CircleTransformation());
            holder.Image.Visibility = ViewStates.Visible;
        }

        public override int LayoutResourceId => Resource.Layout.FriendsPage;

        #region Views

        private ListView _listView;
        private ProgressBar _loadingSpinner;


        public ListView ListView => _listView ?? (_listView = FindViewById<ListView>(Resource.Id.ListView));

        public ProgressBar LoadingSpinner => _loadingSpinner ??
                                             (_loadingSpinner = FindViewById<ProgressBar>(Resource.Id.LoadingSpinner));

        #endregion


        class FriendsItemViewHolder
        {
            private readonly View _view;

            public FriendsItemViewHolder(View view)
            {
                _view = view;
            }

            private ProgressBar _imgPlaceholder;
            private ImageViewAsync _image;
            private TextView _name;
            private TextView _lastOnline;
            private TextView _friendsSince;

            public ProgressBar ImgPlaceholder => _imgPlaceholder ??
                                                 (_imgPlaceholder =
                                                     _view.FindViewById<ProgressBar>(Resource.Id.ImgPlaceholder));

            public ImageViewAsync Image => _image ?? (_image = _view.FindViewById<ImageViewAsync>(Resource.Id.Image));

            public TextView Name => _name ?? (_name = _view.FindViewById<TextView>(Resource.Id.Name));

            public TextView LastOnline => _lastOnline ??
                                          (_lastOnline = _view.FindViewById<TextView>(Resource.Id.LastOnline));

            public TextView FriendsSince => _friendsSince ??
                                            (_friendsSince = _view.FindViewById<TextView>(Resource.Id.FriendsSince));

        }
    }
}