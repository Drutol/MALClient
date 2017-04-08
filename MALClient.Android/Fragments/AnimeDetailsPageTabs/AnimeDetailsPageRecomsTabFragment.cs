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

            AnimeDetailsPageRecomTabsList.Adapter = ViewModel.Recommendations.GetAdapter(RecomItemDelegate);
            AnimeDetailsPageRecomTabsList.OnItemClickListener = new OnItemClickListener<DirectRecommendationData>(data => ViewModel.NavigateDetailsCommand.Execute(data));
            AnimeDetailsPageRecomTabsList.EmptyView = AnimeDetailsPageReviewsTabEmptyNotice;

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingRecommendations,
                    () => AnimeDetailsPageRecomTabLoadingOverlay.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            SetUpForOrientation(Activity.Resources.Configuration.Orientation);
        }

        private View RecomItemDelegate(int i, DirectRecommendationData animeReviewData, View convertView)
        {
            var view = convertView;
            if (view == null)
            {
                view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeRecomItem,null);
            }

            view.Tag = new JavaObjectWrapper<DirectRecommendationData>(animeReviewData);

            view.FindViewById<TextView>(Resource.Id.AnimeRecomItemShowTitle).Text = animeReviewData.Title;
            view.FindViewById<TextView>(Resource.Id.AnimeRecomItemShowType).Text = animeReviewData.Type.ToString();
            view.FindViewById<TextView>(Resource.Id.AnimeRecomItemRecomContent).Text = animeReviewData.Description; ;
                    
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeRecomItemImage);
            img.Into(animeReviewData.ImageUrl);
  
            return view;
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