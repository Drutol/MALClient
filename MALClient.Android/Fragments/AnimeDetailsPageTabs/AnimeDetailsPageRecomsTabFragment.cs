using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Android.UserControls;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;
using Orientation = Android.Content.Res.Orientation;

namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    public class AnimeDetailsPageRecomsTabFragment : MalFragmentBase
    {
        private AnimeDetailsPageViewModel ViewModel;


        private AnimeDetailsPageRecomsTabFragment()
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            
        }

        protected override void InitBindings()
        {

            AnimeDetailsPageRecomTabsList.InjectFlingAdapter(ViewModel.Recommendations,DataTemplateFull,DataTemplateFling,ContainerTemplate,DataTemplateBasic);
            AnimeDetailsPageRecomTabsList.OnItemClickListener = new OnItemClickListener<DirectRecommendationData>(data => ViewModel.NavigateDetailsCommand.Execute(data));
            AnimeDetailsPageRecomTabsList.EmptyView = AnimeDetailsPageReviewsTabEmptyNotice;

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingRecommendations,
                    () => AnimeDetailsPageRecomTabLoadingOverlay.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            SetUpForOrientation(Activity.Resources.Configuration.Orientation);
        }

        private void DataTemplateBasic(View view, int i, DirectRecommendationData animeRecomData)
        {
            view.FindViewById<TextView>(Resource.Id.AnimeRecomItemShowTitle).Text = animeRecomData.Title;
            view.FindViewById<TextView>(Resource.Id.AnimeRecomItemShowType).Text = animeRecomData.Type.ToString();

            var spannableString = new SpannableString(animeRecomData.Description);
            spannableString.SetSpan(new LeadingSpannableString(12, DimensionsHelper.DpToPx(140)), 0, spannableString.Length(), SpanTypes.Paragraph);
            view.FindViewById<TextView>(Resource.Id.AnimeRecomItemRecomContent).SetText(spannableString.SubSequenceFormatted(0, spannableString.Length()), TextView.BufferType.Spannable);
        }

        private View ContainerTemplate(int i)
        {
            return Activity.LayoutInflater.Inflate(Resource.Layout.AnimeRecomItem, null);
        }

        private void DataTemplateFling(View view, int i, DirectRecommendationData animeRecomData)
        {
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeRecomItemImage);
            if (img.IntoIfLoaded(animeRecomData.ImageUrl))
            {
                img.Visibility = ViewStates.Visible;
                view.FindViewById(Resource.Id.AnimeRecomItemImagePlaceholder).Visibility = ViewStates.Gone;
            }
            else
            {
                img.Visibility = ViewStates.Invisible;
                view.FindViewById(Resource.Id.AnimeRecomItemImagePlaceholder).Visibility = ViewStates.Visible;
            }
           
        }

        private void DataTemplateFull(View view, int i, DirectRecommendationData animeRecomData)
        {
            
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeRecomItemImage);
            if (img.Tag == null || (string) img.Tag != animeRecomData.ImageUrl)
            {
                img.Into(animeRecomData.ImageUrl);
            }
            else
            {
                img.Visibility = ViewStates.Visible;
            }
            view.FindViewById(Resource.Id.AnimeRecomItemImagePlaceholder).Visibility = ViewStates.Gone;

        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            SetUpForOrientation(newConfig.Orientation);
            base.OnConfigurationChanged(newConfig);
        }

        private void SetUpForOrientation(Orientation orientation)
        {
            ViewGroup.LayoutParams param;
            switch (orientation)
            {
                case Orientation.Landscape:
                    param = RootView.LayoutParameters;
                    param.Height = ViewGroup.LayoutParams.WrapContent;
                    RootView.LayoutParameters = param;
                    break;
                case Orientation.Portrait:
                case Orientation.Square:
                case Orientation.Undefined:
                    param = RootView.LayoutParameters;
                    param.Height = ViewGroup.LayoutParams.MatchParent;
                    RootView.LayoutParameters = param;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        public static AnimeDetailsPageRecomsTabFragment Instance => new AnimeDetailsPageRecomsTabFragment();

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageRecomsTab;

        #region Views

        private HeightAdjustingListView _animeDetailsPageRecomTabsList;
        private TextView _animeDetailsPageReviewsTabEmptyNotice;
        private RelativeLayout _animeDetailsPageRecomTabLoadingOverlay;

        public HeightAdjustingListView AnimeDetailsPageRecomTabsList => _animeDetailsPageRecomTabsList ?? (_animeDetailsPageRecomTabsList = FindViewById<HeightAdjustingListView>(Resource.Id.AnimeDetailsPageRecomTabsList));

        public TextView AnimeDetailsPageReviewsTabEmptyNotice => _animeDetailsPageReviewsTabEmptyNotice ?? (_animeDetailsPageReviewsTabEmptyNotice = FindViewById<TextView>(Resource.Id.AnimeDetailsPageReviewsTabEmptyNotice));

        public RelativeLayout AnimeDetailsPageRecomTabLoadingOverlay => _animeDetailsPageRecomTabLoadingOverlay ?? (_animeDetailsPageRecomTabLoadingOverlay = FindViewById<RelativeLayout>(Resource.Id.AnimeDetailsPageRecomTabLoadingOverlay));


        #endregion

    }
}