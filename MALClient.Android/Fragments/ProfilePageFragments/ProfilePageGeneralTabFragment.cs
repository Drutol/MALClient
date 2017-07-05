using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.DIalogs;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
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
                .WhenSourceChanges( async () =>
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
                    if (ViewModel.CurrentData.Friends.Any())
                    {
                        ProfilePageGeneralTabFriendsGrid.Adapter =
                            ViewModel.CurrentData.Friends.GetAdapter(GetFriendTemplateDelegate);
                        ProfilePageGeneralTabFriendsEmptyNotice.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        ProfilePageGeneralTabFriendsGrid.Adapter = null;
                        ProfilePageGeneralTabFriendsEmptyNotice.Visibility = ViewStates.Visible;
                    }

                    ProfilePageGeneralTabCommentsList.RemoveAllViews();
                    await Task.Delay(500);
                    PopulateComments();
                }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.EmptyCommentsNoticeVisibility,
                    () => ProfilePageGeneralTabCommentsEmptyNotice.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(this.SetBinding(() => ViewModel.LoadingOhersLibrariesProgressVisiblity).WhenSourceChanges(() =>
            {
                if (ViewModel.LoadingOhersLibrariesProgressVisiblity)
                {
                    ProfilePageGeneralTabCompareList.Enabled = false;
                }
                else
                {
                    ProfilePageGeneralTabCompareList.Enabled = true;
                }
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.CommentInputBoxVisibility).WhenSourceChanges(() =>
            {
                if (ViewModel.CommentInputBoxVisibility)
                {
                    ProfilePageGeneralTabCommentInput.Visibility = ViewStates.Visible;
                    ProfilePageGeneralTabSendCommentButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    ProfilePageGeneralTabCommentInput.Visibility = ViewStates.Gone;
                    ProfilePageGeneralTabSendCommentButton.Visibility = ViewStates.Gone;
                }
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.IsMyProfile,
                    () => ProfilePageGeneralTabCompareList.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibilityInverted));

            

            Bindings.Add(
                this.SetBinding(() => ViewModel.CommentText,
                    () => ProfilePageGeneralTabCommentInput.Text,BindingMode.TwoWay));

            Bindings.Add(
                this.SetBinding(() => ViewModel.PinProfileVisibility,
                    () => PinButton.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(this.SetBinding(() => ViewModel.IsPinned).WhenSourceChanges(() =>
            {
                if(!ViewModel.PinProfileVisibility)
                    return;

                if (ViewModel.IsPinned)
                {
                    PinButton.SetBackgroundColor(new Color(ResourceExtension.AccentColour));
                    PinButtonIcon.Rotation = 0;
                    PinButtonIcon.SetImageResource(Resource.Drawable.icon_unpin);
                }
                else
                {
                    PinButton.SetBackgroundColor(new Color(ResourceExtension.OpaqueAccentColour));
                    PinButtonIcon.Rotation = 45;
                    PinButtonIcon.SetImageResource(Resource.Drawable.icon_pin);
                }
            }));


            PinButton.SetOnClickListener(new OnClickListener(view =>
            {
                ViewModel.IsPinned = !ViewModel.IsPinned;
                PinButtonIcon.Rotation = ViewModel.IsPinned ? 0 : 45;
            }));
            ProfilePageGeneralTabAnimeListButton.SetOnClickListener(new OnClickListener(v => ViewModel.NavigateAnimeListCommand.Execute(null)));
            ProfilePageGeneralTabMangaListButton.SetOnClickListener(new OnClickListener(v => ViewModel.NavigateMangaListCommand.Execute(null)));
            ProfilePageGeneralTabHistoryButton.SetOnClickListener(new OnClickListener(v => ViewModel.NavigateHistoryCommand.Execute(null)));
            ProfilePageGeneralTabSendCommentButton.SetOnClickListener(new OnClickListener(v => ViewModel.SendCommentCommand.Execute(null)));
            ProfilePageGeneralTabActionButton.SetOnClickListener(new OnClickListener(v => ProfilePageGeneralTabActionButtonOnClick()));
            ProfilePageGeneralTabCompareList.SetOnClickListener(new OnClickListener(v => ViewModel.NavigateComparisonCommand.Execute(null)));
        }

        private void AnimatePin(float from, float to)
        {
            var anim =
                new RotateAnimation(from, to, Dimension.RelativeToSelf, .5f, Dimension.RelativeToSelf,
                    .5f)
                {
                    Duration = 150,
                    Interpolator = new LinearInterpolator(),
                    FillAfter = true,
                    FillEnabled = true
                };
            PinButtonIcon.StartAnimation(anim);
        }


        private void PopulateComments()
        {
            ProfilePageGeneralTabCommentsList.SetAdapter(
                ViewModel.CurrentData.Comments.GetAdapter(GetCommentTemplateDelegate));
        }

        private async void ProfilePageGeneralTabActionButtonOnClick()
        {
            var str = await TextInputDialogBuilder.BuildInputTextDialog(Context, "Find user", "username...","Go");
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

            if(!string.IsNullOrEmpty(malComment.User.ImgUrl))
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
        private ImageButton _profilePageGeneralTabCompareList;
        private Button _profilePageGeneralTabMangaListButton;
        private Button _profilePageGeneralTabHistoryButton;
        private ImageView _pinButtonIcon;
        private FrameLayout _pinButton;
        private ExpandableGridView _profilePageGeneralTabFriendsGrid;
        private TextView _profilePageGeneralTabFriendsEmptyNotice;
        private EditText _profilePageGeneralTabCommentInput;
        private Button _profilePageGeneralTabSendCommentButton;
        private LinearLayout _profilePageGeneralTabCommentSection;
        private TextView _profilePageGeneralTabCommentsEmptyNotice;
        private LinearLayout _profilePageGeneralTabCommentsList;
        private ScrollView _profilePageGeneralTabScrollingContainer;
        private FloatingActionButton _profilePageGeneralTabActionButton;

        public ImageView ProfilePageGeneralTabImagePlaceholder => _profilePageGeneralTabImagePlaceholder ?? (_profilePageGeneralTabImagePlaceholder = FindViewById<ImageView>(Resource.Id.ProfilePageGeneralTabImagePlaceholder));

        public ImageViewAsync ProfilePageGeneralTabAnimeUserImg => _profilePageGeneralTabAnimeUserImg ?? (_profilePageGeneralTabAnimeUserImg = FindViewById<ImageViewAsync>(Resource.Id.ProfilePageGeneralTabAnimeUserImg));

        public LinearLayout ProfilePageGeneralTabDetailsList => _profilePageGeneralTabDetailsList ?? (_profilePageGeneralTabDetailsList = FindViewById<LinearLayout>(Resource.Id.ProfilePageGeneralTabDetailsList));

        public Button ProfilePageGeneralTabAnimeListButton => _profilePageGeneralTabAnimeListButton ?? (_profilePageGeneralTabAnimeListButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabAnimeListButton));

        public ImageButton ProfilePageGeneralTabCompareList => _profilePageGeneralTabCompareList ?? (_profilePageGeneralTabCompareList = FindViewById<ImageButton>(Resource.Id.ProfilePageGeneralTabCompareList));

        public Button ProfilePageGeneralTabMangaListButton => _profilePageGeneralTabMangaListButton ?? (_profilePageGeneralTabMangaListButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabMangaListButton));

        public Button ProfilePageGeneralTabHistoryButton => _profilePageGeneralTabHistoryButton ?? (_profilePageGeneralTabHistoryButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabHistoryButton));

        public ImageView PinButtonIcon => _pinButtonIcon ?? (_pinButtonIcon = FindViewById<ImageView>(Resource.Id.PinButtonIcon));

        public FrameLayout PinButton => _pinButton ?? (_pinButton = FindViewById<FrameLayout>(Resource.Id.PinButton));

        public ExpandableGridView ProfilePageGeneralTabFriendsGrid => _profilePageGeneralTabFriendsGrid ?? (_profilePageGeneralTabFriendsGrid = FindViewById<ExpandableGridView>(Resource.Id.ProfilePageGeneralTabFriendsGrid));

        public TextView ProfilePageGeneralTabFriendsEmptyNotice => _profilePageGeneralTabFriendsEmptyNotice ?? (_profilePageGeneralTabFriendsEmptyNotice = FindViewById<TextView>(Resource.Id.ProfilePageGeneralTabFriendsEmptyNotice));

        public EditText ProfilePageGeneralTabCommentInput => _profilePageGeneralTabCommentInput ?? (_profilePageGeneralTabCommentInput = FindViewById<EditText>(Resource.Id.ProfilePageGeneralTabCommentInput));

        public Button ProfilePageGeneralTabSendCommentButton => _profilePageGeneralTabSendCommentButton ?? (_profilePageGeneralTabSendCommentButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabSendCommentButton));

        public LinearLayout ProfilePageGeneralTabCommentSection => _profilePageGeneralTabCommentSection ?? (_profilePageGeneralTabCommentSection = FindViewById<LinearLayout>(Resource.Id.ProfilePageGeneralTabCommentSection));

        public TextView ProfilePageGeneralTabCommentsEmptyNotice => _profilePageGeneralTabCommentsEmptyNotice ?? (_profilePageGeneralTabCommentsEmptyNotice = FindViewById<TextView>(Resource.Id.ProfilePageGeneralTabCommentsEmptyNotice));

        public LinearLayout ProfilePageGeneralTabCommentsList => _profilePageGeneralTabCommentsList ?? (_profilePageGeneralTabCommentsList = FindViewById<LinearLayout>(Resource.Id.ProfilePageGeneralTabCommentsList));

        public ScrollView ProfilePageGeneralTabScrollingContainer => _profilePageGeneralTabScrollingContainer ?? (_profilePageGeneralTabScrollingContainer = FindViewById<ScrollView>(Resource.Id.ProfilePageGeneralTabScrollingContainer));

        public FloatingActionButton ProfilePageGeneralTabActionButton => _profilePageGeneralTabActionButton ?? (_profilePageGeneralTabActionButton = FindViewById<FloatingActionButton>(Resource.Id.ProfilePageGeneralTabActionButton));


        #endregion
    }
}