using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Listeners;
using MALClient.Android.Web;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Clubs;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubDetailsPageRelationsTabFragment : MalFragmentBase
    {
        enum DisplayMode
        {
            Members,
            Anime,
            Manga
        }


        private ClubDetailsViewModel ViewModel = ViewModelLocator.ClubDetails;

        private DisplayMode _mode = DisplayMode.Members;
        private GridViewColumnHelper _gridHelper = new GridViewColumnHelper(null, true);

        protected override void Init(Bundle savedInstanceState)
        {
            
        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.Details).WhenSourceChanges(() =>
            {
                if(ViewModel.Details == null)
                    return;

                MembersToggle.Tag = 0;
                RelatedAnimeToggle.Tag = 1;
                RelatedMangaToggle.Tag = 2;

                var listener = new OnClickListener(v =>
                {
                    _mode = (DisplayMode)(int)v.Tag;
                    UpdateGridView();
                });

                MembersToggle.SetOnClickListener(listener);
                RelatedAnimeToggle.SetOnClickListener(listener);
                RelatedMangaToggle.SetOnClickListener(listener);

                UpdateGridView();
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.Members).WhenSourceChanges(UpdateGridView));
        }

        private void UpdateGridView()
        {
            GridView.ClearFlingAdapter();
            switch (_mode)
            {
                case DisplayMode.Members:
                    MembersToggle.Checked = true;
                    RelatedAnimeToggle.Checked = RelatedMangaToggle.Checked = false;

                    _gridHelper.DetachGrid(GridView);
                    GridView.SetColumnWidth(DimensionsHelper.DpToPx(70));
                    GridView.LayoutParameters.Width = -1;
                    GridView.SetNumColumns(-1);
                    GridView.InjectFlingAdapter(ViewModel.Members, MemberDataTemplateFull, MemberDataTemplateFling, MemberContainerTemplate);
                    break;
                case DisplayMode.Anime:
                    RelatedAnimeToggle.Checked = true;
                    MembersToggle.Checked = RelatedMangaToggle.Checked = false;

                    _gridHelper.RegisterGrid(GridView);
                    GridView.SetColumnWidth(0);
                    GridView.InjectFlingAdapter(ViewModel.AnimeRelations, DataTemplateFull, DataTemplateFling, AnimeContainerTemplate, DataTemplateBasic);
                    break;
                case DisplayMode.Manga:
                    RelatedMangaToggle.Checked = true;
                    RelatedAnimeToggle.Checked = MembersToggle.Checked = false;

                    _gridHelper.RegisterGrid(GridView);
                    GridView.SetColumnWidth(0);
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
            view1.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text = arg3.Item1;
        }

        private void DataTemplateFling(View view1, int i, Tuple<string, string> arg3)
        {
            var img = view1.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage);

            string link = null;
            if (AnimeImageQuery.IsCached(int.Parse(arg3.Item2), true, ref link))
                img.Visibility = img.IntoIfLoaded(link) ? ViewStates.Visible : ViewStates.Gone;

        }

        private void DataTemplateFull(View view1, int i, Tuple<string, string> arg3)
        {
            var img = view1.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage);
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
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.ProfilePageGeneralTabFriendItem, null);
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

        public override int LayoutResourceId => Resource.Layout.ClubDetailsPageRelationsTab;

        #region Views

        private GridView _gridView;
        private ToggleButton _membersToggle;
        private ToggleButton _relatedAnimeToggle;
        private ToggleButton _relatedMangaToggle;

        public GridView GridView => _gridView ?? (_gridView = FindViewById<GridView>(Resource.Id.GridView));

        public ToggleButton MembersToggle => _membersToggle ?? (_membersToggle = FindViewById<ToggleButton>(Resource.Id.MembersToggle));

        public ToggleButton RelatedAnimeToggle => _relatedAnimeToggle ?? (_relatedAnimeToggle = FindViewById<ToggleButton>(Resource.Id.RelatedAnimeToggle));

        public ToggleButton RelatedMangaToggle => _relatedMangaToggle ?? (_relatedMangaToggle = FindViewById<ToggleButton>(Resource.Id.RelatedMangaToggle));

        #endregion
    }
}