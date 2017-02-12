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
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.UserControls;
using MALClient.Models.Models;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.ProfilePageFragments
{
    public class ProfilePageGeneralTabFragment : MalFragmentBase
    {
        private ProfilePageViewModel ViewModel = ViewModelLocator.ProfilePage;

        public ProfilePageGeneralTabFragment() : base(true, false)
        {
        }

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            Bindings.Add(1,new List<Binding>());

            this.SetBinding(() => ViewModel.CurrentData).WhenSourceChanges(() =>
            {
                ImageService.Instance.LoadUrl(ViewModel.CurrentData.User.ImgUrl)
                    .Success(ProfilePageGeneralTabAnimeUserImg.AnimateFadeIn)
                    .Into(ProfilePageGeneralTabAnimeUserImg);

                ProfilePageGeneralTabDetailsList.SetAdapter(
                    ViewModel.CurrentData.Details.GetAdapter(GetDetailTemplateDelegate));

                ProfilePageGeneralTabFriendsGrid.ItemHeight = ProfilePageGeneralTabFriendsGrid.ItemWidth = DimensionsHelper.DpToPx(65);
                ProfilePageGeneralTabFriendsGrid.SetColumnWidth((int)ProfilePageGeneralTabFriendsGrid.ItemWidth);
                ProfilePageGeneralTabFriendsGrid.Adapter =
                    ViewModel.CurrentData.Friends.GetAdapter(GetFriendTemplateDelegate);

                ProfilePageGeneralTabAnimeListButton.SetCommand(ViewModel.NavigateAnimeListCommand);
                ProfilePageGeneralTabMangaListButton.SetCommand(ViewModel.NavigateMangaListCommand);
                ProfilePageGeneralTabHistoryButton.SetCommand(ViewModel.NavigateHistoryCommand);
            });



        }

        private View GetFriendTemplateDelegate(int i, MalUser malUser, View convertView)
        {
            var view = convertView;
            view = view ?? new ImageViewAsync(Context) {LayoutParameters = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(65),DimensionsHelper.DpToPx(65))};

            ImageService.Instance.LoadUrl(malUser.ImgUrl).Success(view.AnimateFadeIn).Into(view as ImageViewAsync);

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

        public override int LayoutResourceId => Resource.Layout.ProfilePageGeneralTab;

        #region Views

        private ImageViewAsync _profilePageGeneralTabAnimeUserImg;
        private LinearLayout _profilePageGeneralTabDetailsList;
        private Button _profilePageGeneralTabAnimeListButton;
        private Button _profilePageGeneralTabMangaListButton;
        private Button _profilePageGeneralTabHistoryButton;
        private ExpandableGridView _profilePageGeneralTabFriendsGrid;
        private Button _profilePageGeneralTabSendCommentButton;
        private LinearLayout _profilePageGeneralTabCommentsList;

        public ImageViewAsync ProfilePageGeneralTabAnimeUserImg => _profilePageGeneralTabAnimeUserImg ?? (_profilePageGeneralTabAnimeUserImg = FindViewById<ImageViewAsync>(Resource.Id.ProfilePageGeneralTabAnimeUserImg));

        public LinearLayout ProfilePageGeneralTabDetailsList => _profilePageGeneralTabDetailsList ?? (_profilePageGeneralTabDetailsList = FindViewById<LinearLayout>(Resource.Id.ProfilePageGeneralTabDetailsList));

        public Button ProfilePageGeneralTabAnimeListButton => _profilePageGeneralTabAnimeListButton ?? (_profilePageGeneralTabAnimeListButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabAnimeListButton));

        public Button ProfilePageGeneralTabMangaListButton => _profilePageGeneralTabMangaListButton ?? (_profilePageGeneralTabMangaListButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabMangaListButton));

        public Button ProfilePageGeneralTabHistoryButton => _profilePageGeneralTabHistoryButton ?? (_profilePageGeneralTabHistoryButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabHistoryButton));

        public ExpandableGridView ProfilePageGeneralTabFriendsGrid => _profilePageGeneralTabFriendsGrid ?? (_profilePageGeneralTabFriendsGrid = FindViewById<ExpandableGridView>(Resource.Id.ProfilePageGeneralTabFriendsGrid));

        public Button ProfilePageGeneralTabSendCommentButton => _profilePageGeneralTabSendCommentButton ?? (_profilePageGeneralTabSendCommentButton = FindViewById<Button>(Resource.Id.ProfilePageGeneralTabSendCommentButton));

        public LinearLayout ProfilePageGeneralTabCommentsList => _profilePageGeneralTabCommentsList ?? (_profilePageGeneralTabCommentsList = FindViewById<LinearLayout>(Resource.Id.ProfilePageGeneralTabCommentsList));

        #endregion
    }
}