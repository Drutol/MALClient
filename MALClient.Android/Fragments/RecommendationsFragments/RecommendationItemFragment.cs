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
using MALClient.Android.Adapters.CollectionAdapters;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.RecommendationsFragments
{
    public class RecommendationItemFragment : MalFragmentBase
    {
        private readonly RecommendationItemViewModel ViewModel;

        public RecommendationItemFragment(RecommendationItemViewModel model)
        {
            ViewModel = model;
        }

        class RecommendationsFragmentDetailsAdapter : AlternatingListCollectionAdapter<Tuple<string,string,string,string,string>>
        {
            public RecommendationsFragmentDetailsAdapter(Activity context, int layoutResource, IEnumerable<Tuple<string, string, string, string, string>> items, bool startLight) : base(context, layoutResource, items, startLight)
            {
            }

            protected override void InitializeView(View view, Tuple<string, string, string, string, string> model)
            {
                view.FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemType).Text = model.Item1;
                view.FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemDepValue).Text = model.Item2;
                view.FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyDepValue).Text = model.Item3;
                view.FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemRecValue).Text = model.Item4;
                view.FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyRecValue).Text = model.Item5;
            }
        }

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            RecommendationItemDetailsContainer.SetAdapter(new RecommendationsFragmentDetailsAdapter(Activity,
                Resource.Layout.RecommendationItemDetailItem,ViewModel.DetailItems,true));
            RecommendationItemDescription.Text = ViewModel.Data.Description;
            RecommendationItemDepTitle.Text = ViewModel.Data.DependentTitle;
            RecommendationItemRecTitle.Text = ViewModel.Data.RecommendationTitle;
           // ImageService.Instance.LoadUrl(ViewModel.Data.AnimeDependentData.ImgUrl).Into(RecommendationItemDepImage);
           // ImageService.Instance.LoadUrl(ViewModel.Data.AnimeRecommendationData.ImgUrl).Into(RecommendationItemRecImage);
        }

        public override int LayoutResourceId => Resource.Layout.RecommendationItem;


        #region Views

        private ImageViewAsync _recommendationItemDepImage;
        private TextView _recommendationItemDepTitle;
        private ImageViewAsync _recommendationItemRecImage;
        private TextView _recommendationItemRecTitle;
        private TextView _recommendationItemDescription;
        private LinearLayout _recommendationItemDetailsContainer;

        public ImageViewAsync RecommendationItemDepImage => _recommendationItemDepImage ?? (_recommendationItemDepImage = FindViewById<ImageViewAsync>(Resource.Id.RecommendationItemDepImage));

        public TextView RecommendationItemDepTitle => _recommendationItemDepTitle ?? (_recommendationItemDepTitle = FindViewById<TextView>(Resource.Id.RecommendationItemDepTitle));

        public ImageViewAsync RecommendationItemRecImage => _recommendationItemRecImage ?? (_recommendationItemRecImage = FindViewById<ImageViewAsync>(Resource.Id.RecommendationItemRecImage));

        public TextView RecommendationItemRecTitle => _recommendationItemRecTitle ?? (_recommendationItemRecTitle = FindViewById<TextView>(Resource.Id.RecommendationItemRecTitle));

        public TextView RecommendationItemDescription => _recommendationItemDescription ?? (_recommendationItemDescription = FindViewById<TextView>(Resource.Id.RecommendationItemDescription));

        public LinearLayout RecommendationItemDetailsContainer => _recommendationItemDetailsContainer ?? (_recommendationItemDetailsContainer = FindViewById<LinearLayout>(Resource.Id.RecommendationItemDetailsContainer));



        #endregion
    }
}