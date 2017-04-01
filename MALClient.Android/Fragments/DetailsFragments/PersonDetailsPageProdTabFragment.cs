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
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.UserControls;
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
            AnimeDetailsPageCharactersTabGridView.DisableAdjust = true;
            AnimeDetailsPageCharactersTabLoadingSpinner.Visibility = ViewStates.Gone;
            _gridViewColumnHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,170);

            Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            {
                if(ViewModel.Data == null)
                    return;
                AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.Data.StaffPositions,DataTemplateFull,DataTemplateFling,ContainerTemplate  );
                
            }));
        }

        private View ContainerTemplate(int i)
        {

            var view = Activity.LayoutInflater.Inflate(Resource.Layout.AnimeLightItem, null);
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).SetMaxLines(1);
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemNotes).Visibility = ViewStates.Visible;
            view.Click += LightItemOnClick;

            return view;
        }

        private void LightItemOnClick(object sender, EventArgs e)
        {
            ViewModel.NavigateAnimeDetailsCommand.Execute((sender as View).Tag.Unwrap<AnimeLightEntry>());
        }

        private void DataTemplateFling(View view, int i, AnimeLightEntry animeLightEntry)
        {
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text = animeLightEntry.Title;
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemNotes).Text = animeLightEntry.Notes;
            view.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage).Visibility = ViewStates.Invisible;
            view.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Visible;
        }

        private void DataTemplateFull(View view, int i, AnimeLightEntry animeLightEntry)
        {
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text = animeLightEntry.Title;
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemNotes).Text = animeLightEntry.Notes;
            view.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Gone;
            var image = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage);
            if (image.Tag == null || (string)image.Tag != animeLightEntry.ImgUrl)
            {
                image.Into(animeLightEntry.ImgUrl, null, img => img.HandleScaling());
                image.Tag = animeLightEntry.ImgUrl;
            }

            view.Tag = animeLightEntry.Wrap();
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageCharactersTab;


        #region Views

        private HeightAdjustingGridView _animeDetailsPageCharactersTabGridView;
        private ProgressBar _animeDetailsPageCharactersTabLoadingSpinner;

        public HeightAdjustingGridView AnimeDetailsPageCharactersTabGridView => _animeDetailsPageCharactersTabGridView ?? (_animeDetailsPageCharactersTabGridView = FindViewById<HeightAdjustingGridView>(Resource.Id.AnimeDetailsPageCharactersTabGridView));

        public ProgressBar AnimeDetailsPageCharactersTabLoadingSpinner => _animeDetailsPageCharactersTabLoadingSpinner ?? (_animeDetailsPageCharactersTabLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeDetailsPageCharactersTabLoadingSpinner));

        #endregion
    }
}