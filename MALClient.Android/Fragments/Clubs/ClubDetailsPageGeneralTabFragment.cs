using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.View.Menu;
using Android.Views;
using Android.Widget;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Listeners;
using MALClient.Models.Models;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Clubs;

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubDetailsPageGeneralTabFragment : MalFragmentBase
    {
        enum DisplayMode
        {
            Members,
            Anime,
            Manga
        }


        private ClubDetailsViewModel ViewModel = ViewModelLocator.ClubDetails;
        private DisplayMode _mode = DisplayMode.Members;

        protected override void Init(Bundle savedInstanceState)
        {
            throw new NotImplementedException();
        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.Details).WhenSourceChanges(() =>
            {
                Image.Into(ViewModel.Details.ImgUrl);
                Title.Text = ViewModel.Details.Name;

                StatsList.SetAdapter(ViewModel.Details.GeneralInfo.GetAdapter(GetTemplateDelegate));
                OfficersList.SetAdapter(ViewModel.Details.GeneralInfo.GetAdapter(GetTemplateDelegate));

                if (ViewModel.Details.Joined)
                {
                    ButtonLeave.Visibility = ViewStates.Visible;
                    ButtonLeave.Text = "Leave";
                    ButtonLeave.SetOnClickListener(new OnClickListener(view => ViewModel.LeaveClubCommand.Execute(null)));
                }
                else if (ViewModel.Details.IsPublic)
                {
                    ButtonLeave.SetOnClickListener(new OnClickListener(view => ViewModel.JoinClubCommand.Execute(null)));
                    ButtonLeave.Visibility = ViewStates.Visible;
                    ButtonLeave.Text = "Join";
                }
                else
                {
                    ButtonLeave.Visibility = ViewStates.Gone;
                }

                ButtonForum.SetOnClickListener(new OnClickListener(view => ViewModel.NavigateForumCommand.Execute(null)));

                UpdateGridView();
            }));



        }

        private void UpdateGridView()
        {
            GridView.ClearFlingAdapter();
            switch (_mode)
            {
                case DisplayMode.Members:
                    GridView.InjectFlingAdapter(ViewModel.Members, MemberDataTemplateFull, MemberDataTemplateFling, MemberContainerTemplate);
                    break;
                case DisplayMode.Anime:
                    GridView.InjectFlingAdapter(ViewModel.AnimeRelations,DataTemplateFull,DataTemplateFling,AnimeContainerTemplate,DataTemplateBasic);
                    break;
                case DisplayMode.Manga:
                    GridView.InjectFlingAdapter(ViewModel.MangaRelations, DataTemplateFull, DataTemplateFling, MangaContainerTemplate, DataTemplateBasic);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        #region MangaAndAnime


        private View MangaContainerTemplate(int arg)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.AnimeLightItem, null);
            view.Click +=
                (sender, args) => ViewModel.NavigateMangaDetailsCommand.Execute((sender as View).Tag
                    .Unwrap<Tuple<string, string>>());
            return view;
        }

        private View AnimeContainerTemplate(int i)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.AnimeLightItem, null);
            view.Click +=
                (sender, args) => ViewModel.NavigateAnimeDetailsCommand.Execute((sender as View).Tag
                    .Unwrap<Tuple<string, string>>());
            return view;
        }

        private void DataTemplateBasic(View view1, int i, Tuple<string, string> arg3)
        {
            View.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text = arg3.Item1;
        }



        private void DataTemplateFling(View view1, int i, Tuple<string, string> arg3)
        {
            var img = view1.FindViewById<ImageViewAsync>(Resource.Id.ProfilePageGeneralTabFriendItemImage);

            string link = null;
            if (AnimeImageQuery.IsCached(int.Parse(arg3.Item2), true, ref link))
                img.Visibility = img.IntoIfLoaded(link) ? ViewStates.Visible : ViewStates.Gone;

        }

        private void DataTemplateFull(View view1, int i, Tuple<string, string> arg3)
        {
            var img = view1.FindViewById<ImageViewAsync>(Resource.Id.ProfilePageGeneralTabFriendItemImage);
            string imgUrl = null;
            var id = int.Parse(arg3.Item2);
            if (AnimeImageQuery.IsCached(id, true, ref imgUrl))
                img.Into(imgUrl);
            else
                img.IntoWithTask(AnimeImageQuery.GetImageUrl(id, true));
        }

        #endregion


        #region Members

        private View MemberContainerTemplate(int i)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.ProfilePageGeneralTabFriendItem,null);
            view.Click +=
                (sender, args) => ViewModel.NavigateUserCommand.Execute((sender as View).Tag.Unwrap<MalUser>());
            return view;
        }

        private void MemberDataTemplateFling(View view1, int i, MalUser arg3)
        {
            var img = view1.FindViewById<ImageViewAsync>(Resource.Id.ProfilePageGeneralTabFriendItemImage);
            if (img.IntoIfLoaded(arg3.ImgUrl))
                img.Visibility = ViewStates.Invisible;
        }

        private void MemberDataTemplateFull(View view1, int i, MalUser arg3)
        {
            view1.FindViewById<ImageViewAsync>(Resource.Id.ProfilePageGeneralTabFriendItemImage).Into(arg3.ImgUrl);
        }

        #endregion


        private View GetTemplateDelegate(int i, (string name, string value) valueTuple, View arg3)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.HeaderedTextBlock, null);

            view.FindViewById<TextView>(Resource.Id.Header).Text = valueTuple.name;
            view.FindViewById<TextView>(Resource.Id.Text).Text = valueTuple.value;

            return view;
        }

        public override int LayoutResourceId => Resource.Layout.ClubDetailsPageGeneralTab;


        #region Views

        private ImageViewAsync _image;
        private TextView _title;
        private LinearLayout _statsList;
        private LinearLayout _officersList;
        private Button _buttonForum;
        private Button _buttonLeave;
        private GridView _gridView;
        private ToggleButton _membersToggle;
        private ToggleButton _relatedAnimeToggle;
        private ToggleButton _relatedMangaToggle;

        public ImageViewAsync Image => _image ?? (_image = FindViewById<ImageViewAsync>(Resource.Id.Image));

        public TextView Title => _title ?? (_title = FindViewById<TextView>(Resource.Id.Title));

        public LinearLayout StatsList => _statsList ?? (_statsList = FindViewById<LinearLayout>(Resource.Id.StatsList));

        public LinearLayout OfficersList => _officersList ?? (_officersList = FindViewById<LinearLayout>(Resource.Id.OfficersList));

        public Button ButtonForum => _buttonForum ?? (_buttonForum = FindViewById<Button>(Resource.Id.ButtonForum));

        public Button ButtonLeave => _buttonLeave ?? (_buttonLeave = FindViewById<Button>(Resource.Id.ButtonLeave));

        public GridView GridView => _gridView ?? (_gridView = FindViewById<GridView>(Resource.Id.GridView));

        public ToggleButton MembersToggle => _membersToggle ?? (_membersToggle = FindViewById<ToggleButton>(Resource.Id.MembersToggle));

        public ToggleButton RelatedAnimeToggle => _relatedAnimeToggle ?? (_relatedAnimeToggle = FindViewById<ToggleButton>(Resource.Id.RelatedAnimeToggle));

        public ToggleButton RelatedMangaToggle => _relatedMangaToggle ?? (_relatedMangaToggle = FindViewById<ToggleButton>(Resource.Id.RelatedMangaToggle));

        #endregion
    }
}