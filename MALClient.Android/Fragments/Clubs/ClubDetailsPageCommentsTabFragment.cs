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
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Fragments.ProfilePageFragments;
using MALClient.Android.Listeners;
using MALClient.Android.UserControls;
using MALClient.Android.ViewHolders;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Clubs;

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubDetailsPageCommentsTabFragment : MalFragmentBase
    {
        private ClubDetailsViewModel ViewModel = ViewModelLocator.ClubDetails;

        protected override void Init(Bundle savedInstanceState)
        {
            
        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.Comments).WhenSourceChanges(() =>
            {
                CommentsList.ClearFlingAdapter();
                CommentsList.InjectFlingAdapter(ViewModel.Comments, view => new CommentViewHolder(view),
                    DataTemplateFull, DataTemplateFling, DataTemplateBasic, ContainerTemplate);
            }));

            var refresh = RootView as ScrollableSwipeToRefreshLayout;
            refresh.ScrollingView = CommentsList;
            refresh.Refresh -= RefreshOnRefresh;
            refresh.Refresh += RefreshOnRefresh;
        }

        private async void RefreshOnRefresh(object sender, EventArgs eventArgs)
        {
            if(ViewModel.Loading)
                return;

            await ViewModel.ReloadComments();

            (RootView as ScrollableSwipeToRefreshLayout).Refreshing = false;
        }

        private View ContainerTemplate(int i1)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.ProfilePageGeneralTabCommentItem, null);

            view.FindViewById(Resource.Id.ProfilePageGeneralTabCommentItemDeleteButton).Visibility = ViewStates.Gone;           
            view.FindViewById(Resource.Id.ProfilePageGeneralTabCommentItemConvButton).Visibility = ViewStates.Gone;
            view.FindViewById(Resource.Id.ProfilePageGeneralTabCommentItemImgButton).Visibility = ViewStates.Gone;


            return view;
        }

        private void DataTemplateBasic(View view1, int i1, MalClubComment malComment, CommentViewHolder arg4)
        {
            arg4.ProfilePageGeneralTabCommentItemUsername.Text = malComment.User.Name;
            arg4.ProfilePageGeneralTabCommentItemDate.Text = malComment.Date;
            arg4.ProfilePageGeneralTabCommentItemContent.Text = malComment.Content;

        }

        private void DataTemplateFling(View view1, int i1, MalClubComment arg3, CommentViewHolder arg4)
        {
            if (!arg4.ProfilePageGeneralTabCommentItemUserImg.IntoIfLoaded(arg3.User.ImgUrl))
                arg4.ProfilePageGeneralTabCommentItemUserImg.Visibility = ViewStates.Invisible;
        }

        private void DataTemplateFull(View view1, int i1, MalClubComment arg3, CommentViewHolder arg4)
        {
            arg4.ProfilePageGeneralTabCommentItemUserImg.Into(arg3.User.ImgUrl);
        }


        public override int LayoutResourceId => Resource.Layout.ClubDetailsPageCommentsPage;

        #region Views

        private EditText _commentTextBox;
        private Button _commentButton;
        private TextView _commentsEmptyNotice;
        private ListView _commentsList;

        public EditText CommentTextBox => _commentTextBox ?? (_commentTextBox = FindViewById<EditText>(Resource.Id.CommentTextBox));

        public Button CommentButton => _commentButton ?? (_commentButton = FindViewById<Button>(Resource.Id.CommentButton));

        public TextView CommentsEmptyNotice => _commentsEmptyNotice ?? (_commentsEmptyNotice = FindViewById<TextView>(Resource.Id.CommentsEmptyNotice));

        public ListView CommentsList => _commentsList ?? (_commentsList = FindViewById<ListView>(Resource.Id.CommentsList));

        #endregion
    }
}