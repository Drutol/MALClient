using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AoLibs.Adapters.Android.Recycler;
using AoLibs.Adapters.Core;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Listeners;
using MALClient.Android.UserControls;
using MALClient.Android.Utilities.ImageLoading;
using MALClient.Models.Models.Anime;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.DetailsFragments
{
    public class PersonDetailsPageProdTabFragment : MalFragmentBase
    {
        private StaffDetailsViewModel ViewModel = ViewModelLocator.StaffDetails;
        private GridViewColumnHelper _gridViewColumnHelper;

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            //AnimeDetailsPageCharactersTabGridView.DisableAdjust = true;
            AnimeDetailsPageCharactersTabLoadingSpinner.Visibility = ViewStates.Gone;
            //_gridViewColumnHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,170,2,3);

            //Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            //{
            //    if(ViewModel.Data == null)
            //        return;
            //    AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.Data.StaffPositions,DataTemplateFull,DataTemplateFling,ContainerTemplate,DataTemplateBasic);

            //}));

            //_gridHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,340,1);
            Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            {
                //if (ViewModel.AnimeStaffData == null)
                //    AnimeDetailsPageCharactersTabGridView.Adapter = null;
                //else
                //    AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.AnimeStaffData.AnimeCharacterPairs, DataTemplateFull, DataTemplateFling, ContainerTemplate);

                if (ViewModel.Data == null)
                    AnimeDetailsPageCharactersTabGridView.SetAdapter(null);
                else
                {
                    AnimeDetailsPageCharactersTabGridView.SetAdapter(
                        new ObservableRecyclerAdapter<
                            AnimeLightEntry, Holder>(
                            ViewModel.Data.StaffPositions,
                            DataTemplate,
                            ItemTemplate));

                    if (ViewModel.Data.StaffPositions?.Count == 0)
                    {
                        AnimeDetailsPageCharactersTabEmptyNotice.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        AnimeDetailsPageCharactersTabEmptyNotice.Visibility = ViewStates.Gone;
                    }
                }

            }));

            AnimeDetailsPageCharactersTabGridView.SetLayoutManager(new GridLayoutManager(Activity, 3));
            AnimeDetailsPageCharactersTabGridView.AddOnScrollListener(new CustomScrollListener());
        }

        private View ItemTemplate(int viewtype)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.AnimeLightItem, null);
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).SetMaxLines(1);
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemNotes).Visibility = ViewStates.Visible;

            return view;
        }

        private void DataTemplate(AnimeLightEntry item, Holder holder, int position)
        {
            holder.ItemView.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text = item.Title;
            holder.ItemView.FindViewById<TextView>(Resource.Id.AnimeLightItemNotes).Text = item.Notes;

            holder.ItemView.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Gone;
            var image = holder.ItemView.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage);
            if (image.Tag == null || (string)image.Tag != item.ImgUrl)
            {
                image.Into(item.ImgUrl);
                image.Tag = item.ImgUrl;
            }
            holder.ItemView.SetOnClickListener(new OnClickListener(view => LightItemOnClick(item)));
        }

        private void LightItemOnClick(AnimeLightEntry item)
        {
            ViewModel.NavigateAnimeDetailsCommand.Execute(item);
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageCharactersTab;

        class Holder : RecyclerView.ViewHolder
        {
            private readonly View _view;

            public Holder(View view) : base(view)
            {
                _view = view;
            }

            private ProgressBar _animeLightItemImgPlaceholder;
            private ImageViewAsync _animeLightItemImage;
            private TextView _animeLightItemTitle;
            private TextView _animeLightItemNotes;

            public ProgressBar AnimeLightItemImgPlaceholder => _animeLightItemImgPlaceholder ?? (_animeLightItemImgPlaceholder = _view.FindViewById<ProgressBar>(Resource.Id.AnimeLightItemImgPlaceholder));
            public ImageViewAsync AnimeLightItemImage => _animeLightItemImage ?? (_animeLightItemImage = _view.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage));
            public TextView AnimeLightItemTitle => _animeLightItemTitle ?? (_animeLightItemTitle = _view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle));
            public TextView AnimeLightItemNotes => _animeLightItemNotes ?? (_animeLightItemNotes = _view.FindViewById<TextView>(Resource.Id.AnimeLightItemNotes));
        }


        #region Views

        private RecyclerView _animeDetailsPageCharactersTabGridView;
        private TextView _animeDetailsPageCharactersTabEmptyNotice;
        private ProgressBar _animeDetailsPageCharactersTabLoadingSpinner;

        public RecyclerView AnimeDetailsPageCharactersTabGridView => _animeDetailsPageCharactersTabGridView ?? (_animeDetailsPageCharactersTabGridView = FindViewById<RecyclerView>(Resource.Id.AnimeDetailsPageCharactersTabGridView));

        public TextView AnimeDetailsPageCharactersTabEmptyNotice => _animeDetailsPageCharactersTabEmptyNotice ?? (_animeDetailsPageCharactersTabEmptyNotice = FindViewById<TextView>(Resource.Id.AnimeDetailsPageCharactersTabEmptyNotice));

        public ProgressBar AnimeDetailsPageCharactersTabLoadingSpinner => _animeDetailsPageCharactersTabLoadingSpinner ?? (_animeDetailsPageCharactersTabLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeDetailsPageCharactersTabLoadingSpinner));

        #endregion
    }
}