using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.DIalogs;
using MALClient.Android.Listeners;
using MALClient.Android.UserControls;
using MALClient.Models.Models;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.ProfilePageFragments
{
    public class ProfilePageGeneralTabFragment : MalFragmentBase
    {
        private ProfilePageViewModel ViewModel = ViewModelLocator.ProfilePage;


        protected override void Init(Bundle savedInstanceState)
        {
        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.CurrentData)
                .WhenSourceChanges(() =>
                {
                    ProfilePageGeneralTabScrollingContainer.ScrollY = 0;

                    if (string.IsNullOrEmpty(ViewModel.CurrentData.User.ImgUrl))
                    {
                        ProfilePageGeneralTabAnimeUserImg.Visibility = ViewStates.Invisible;
                        ProfilePageGeneralTabImagePlaceholder.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        ProfilePageGeneralTabAnimeUserImg.Into(ViewModel.CurrentData.User.ImgUrl);
                        ProfilePageGeneralTabImagePlaceholder.Visibility = ViewStates.Gone;
                    }


                    ProfilePageGeneralTabDetailsList.SetAdapter(
                        ViewModel.CurrentData.Details.GetAdapter(GetDetailTemplateDelegate));

                    ProfilePageGeneralTabFriendsGrid.ItemHeight =
                        ProfilePageGeneralTabFriendsGrid.ItemWidth = DimensionsHelper.DpToPx(65);
                    ProfilePageGeneralTabFriendsGrid.SetColumnWidth((int)ProfilePageGeneralTabFriendsGrid.ItemWidth);
                    ProfilePageGeneralTabFriendsGrid.Adapter =
                        ViewModel.CurrentData.Friends.GetAdapter(GetFriendTemplateDelegate);

                    if(ProfilePageGeneralTabScrollingContainer.ScrollY > 0)
                        PopulateComments();
                    else
                        ProfilePageGeneralTabScrollingContainer.SetOnScrollChangeListener(new ScrollListener(i =>
                        {
                            if (i > 0)
                            {
                                ProfilePageGeneralTabScrollingContainer.SetOnScrollChangeListener(null);
                                PopulateComments();
                            }
                        }));
                }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.CommentText,
                    () => ProfilePageGeneralTabCommentInput.Text,BindingMode.TwoWay));

            ProfilePageGeneralTabAnimeListButton.SetCommand(ViewModel.NavigateAnimeListCommand);
            ProfilePageGeneralTabMangaListButton.SetCommand(ViewModel.NavigateMangaListCommand);
            ProfilePageGeneralTabHistoryButton.SetCommand(ViewModel.NavigateHistoryCommand);
            ProfilePageGeneralTabSendCommentButton.SetCommand(ViewModel.SendCommentCommand);
            ProfilePageGeneralTabActionButton.Click += ProfilePageGeneralTabActionButtonOnClick;
        }

        private void PopulateComments()
        {
            ProfilePageGeneralTabCommentsList.SetAdapter(
                ViewModel.CurrentData.Comments.GetAdapter(GetCommentTemplateDelegate));
        }

        private async void ProfilePageGeneralTabActionButtonOnClick(object sender, EventArgs eventArgs)
        {
            var str = await TextInputDialogBuilder.BuildInputTextDialog(Context, "Find user", "username...");
            if(!string.IsNullOrEmpty(str))
                ViewModel.NavigateProfileCommand.Execute(new MalUser{Name = str});
        }

        private View GetCommentTemplateDelegate(int i, MalComment malComment, View convertView)
        {

            var view = Activity.LayoutInflater.Inflate(Resource.Layout.ProfilePageGeneralTabCommentItem, null);

            var delButton = view.FindViewById<Button>(Resource.Id.ProfilePageGeneralTabCommentItemDeleteButton);
            var convButton = view.FindViewById<Button>(Resource.Id.ProfilePageGeneralTabCommentItemConvButton);
            var imgButton = view.FindViewById<FrameLayout>(Resource.Id.ProfilePageGeneralTabCommentItemImgButton);

            delButton.SetOnClickListener(new OnClickListener(OnCommentDeleteClick)); 
            convButton.SetOnClickListener(new OnClickListener( OnCommentConversationClick)); 
            imgButton.SetOnClickListener(new OnClickListener(OnCommentAuthorClick));

            view.FindViewById<ImageViewAsync>(Resource.Id.ProfilePageGeneralTabCommentItemUserImg).Into(malComment.User.ImgUrl);

            view.FindViewById<TextView>(Resource.Id.ProfilePageGeneralTabCommentItemUsername).Text =
                malComment.User.Name;
            view.FindViewById<TextView>(Resource.Id.ProfilePageGeneralTabCommentItemDate).Text = malComment.Date;
            view.FindViewById<TextView>(Resource.Id.ProfilePageGeneralTabCommentItemContent).Text = malComment.Content;

            view.FindViewById(Resource.Id.ProfilePageGeneralTabCommentItemDeleteButton).Visibility =
                malComment.CanDelete ? ViewStates.Visible : ViewStates.Gone;
            view.FindViewById(Resource.Id.ProfilePageGeneralTabCommentItemConvButton).Visibility =
                string.IsNullOrEmpty(malComment.ComToCom) ? ViewStates.Gone : ViewStates.Visible;

            view.Tag =
                delButton.Tag =
                    convButton.Tag = 
                        imgButton.Tag = malComment.Wrap();
            return view;
        }




        private View GetFriendTemplateDelegate(int i, MalUser malUser, View convertView)
        {
            var view = convertView;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.ProfilePageGeneralTabFriendItem, null);
                view.SetOnClickListener(new OnClickListener(FriendButtonOnClick));
            }

            var img = (view as FrameLayout).FindViewById<ImageViewAsync>(Resource.Id.ProfilePageGeneralTabFriendItemImage);

            img.Into(malUser.ImgUrl);

            view.Tag = malUser.Wrap();

            return view;
        }


        private View GetDetailTemplateDelegate(int i, Tuple<string, string> tuple, View convertView)
        {
            var view = convertView ??
                       Activity.LayoutInflater.Inflate(Resource.Layout.ProfilePageGeneralTabDetailsItem, null);

            view.FindViewById<TextView>(Resource.Id.ProfilePageGeneralTabDetailsItemLeft).Text = tuple.Item1;
            view.FindViewById<TextView>(Resource.Id.ProfilePageGeneralTabDetailsItemRight).Text = tuple.Item2;

            return view;
        }


        private void OnCommentConversationClick(View sender)
        {
            ViewModel.NavigateConversationCommand.Execute(sender.Tag.Unwrap<MalComment>());
        }

        private void OnCommentDeleteClick(View sender)
        {
            ViewModel.DeleteCommentCommand.Execute(sender.Tag.Unwrap<MalComment>());
            ProfilePageGeneralTabCommentsList.RemoveView(sender);
        }

        private void FriendButtonOnClick(View sender)
        {
            ViewModel.NavigateProfileCommand.Execute((sender as View).Tag.Unwrap<MalUser>());
        }

        private void OnCommentAuthorClick(View sender)
        {
            ViewModel.NavigateProfileCommand.Execute((sender as View).Tag.Unwrap<MalComment>().User);
        }

        public override int LayoutResourceId => Resource.Layout.ProfilePageGeneralTab;

        #region Views

        private ImageView _profilePageGeneralTabImagePlaceholder;
        private ImageViewAsync _profilePageGeneralTabAnimeUserImg;
        private LinearLayout _profilePageGeneralTabDetailsList;
        private Button _profilePageGeneralTabAnimeListButton;
        private Button _profilePageGeneralTabMangaListButton;
        private Button _profilePageGeneralTabHistoryButton;
        private ExpandableGridView _profilePageGeneralTabFriendsGrid;
        private EditText _profilePageGeneralTabCommentInput;
        private Button _profilePageGeneralTabSendCommentButton;
        private LinearLayout _profilePageGeneralTabCommentsList;
        private ScrollView _profilePageGeneralTabScrollingContainer;
        private FloatingActionButton _profilePageGeneralTabActionButton;

        public ImageView ProfilePageGeneralTabImagePlaceholder => _profilePageGeneralTabImagePlaceholder ?? (_profilePageGeneralTabImagePlaceholder = FindViewById<ImageView>(Resource.Id.ProfilePageGeneralTabImagePlaceholder));

        public ImageViewAsync ProfilePageGeneralTabAnimeUserImg => _profilePageGeneralTabAnimeUserImg ?? (_profilePageGeneralTabAnimeUserImg = FindViewById<ImageViewAsync>(Resource.Id.ProfilePageGeneralTabAnimeUserImg));

        public LinearLayout ProfilePageGeneralTabDetailsList => _profilePageGeneralTabDetailsList ?? (_profilePageGeneralTabDetailsList = FindViewById<LinearLayout>(Resource.Id.ProfilePageGeneralTabDetailsList));

        public Button ProfilePageGeneralTabAnimeListButton => _profilePageGeneralTabAnimeListButton ?? (_profilePageGeneralTabAnimeListButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabAnimeListButton));

        public Button ProfilePageGeneralTabMangaListButton => _profilePageGeneralTabMangaListButton ?? (_profilePageGeneralTabMangaListButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabMangaListButton));

        public Button ProfilePageGeneralTabHistoryButton => _profilePageGeneralTabHistoryButton ?? (_profilePageGeneralTabHistoryButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabHistoryButton));

        public ExpandableGridView ProfilePageGeneralTabFriendsGrid => _profilePageGeneralTabFriendsGrid ?? (_profilePageGeneralTabFriendsGrid = FindViewById<ExpandableGridView>(Resource.Id.ProfilePageGeneralTabFriendsGrid));

        public EditText ProfilePageGeneralTabCommentInput => _profilePageGeneralTabCommentInput ?? (_profilePageGeneralTabCommentInput = FindViewById<EditText>(Resource.Id.ProfilePageGeneralTabCommentInput));

        public Button ProfilePageGeneralTabSendCommentButton => _profilePageGeneralTabSendCommentButton ?? (_profilePageGeneralTabSendCommentButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabSendCommentButton));

        public LinearLayout ProfilePageGeneralTabCommentsList => _profilePageGeneralTabCommentsList ?? (_profilePageGeneralTabCommentsList = FindViewById<LinearLayout>(Resource.Id.ProfilePageGeneralTabCommentsList));

        public ScrollView ProfilePageGeneralTabScrollingContainer => _profilePageGeneralTabScrollingContainer ?? (_profilePageGeneralTabScrollingContainer = FindViewById<ScrollView>(Resource.Id.ProfilePageGeneralTabScrollingContainer));

        public FloatingActionButton ProfilePageGeneralTabActionButton => _profilePageGeneralTabActionButton ?? (_profilePageGeneralTabActionButton = FindViewById<FloatingActionButton>(Resource.Id.ProfilePageGeneralTabActionButton));




        #endregion
    }
}