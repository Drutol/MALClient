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
using MALClient.Android.Adapters.CollectionAdapters;
using MALClient.Models.Enums;
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
            Bindings.Add(RecommendationItemDepImage.Id, new List<Binding>());
            Bindings[RecommendationItemDepImage.Id].Add(
                this.SetBinding(() => ViewModel.LoadingSpinnerVisibility).WhenSourceChanges(() =>
                {
                    if (ViewModel.LoadingSpinnerVisibility)
                    {
                        RecommendationItemLoading.Visibility = ViewStates.Visible;
                        return;
                    }
                    RecommendationItemLoading.Visibility = ViewStates.Gone;

                    if(ViewModel.DetailItems.Count == 0 || RecommendationItemDetailsContainer.ChildCount > 0)
                        return;

                    RecommendationItemDetailsContainer.SetAdapter(new RecommendationsFragmentDetailsAdapter(Activity,
                        Resource.Layout.RecommendationItemDetailItem, ViewModel.DetailItems, true));
                    RecommendationItemDescription.Text = ViewModel.Data.Description;
                    RecommendationItemDepTitle.Text = ViewModel.Data.DependentTitle;
                    RecommendationItemRecTitle.Text = ViewModel.Data.RecommendationTitle;
                    if(ViewModel.Data.AnimeDependentData.ImgUrl != null)
                        ImageService.Instance.LoadUrl(ViewModel.Data.AnimeDependentData.ImgUrl).FadeAnimation(false).Success(() => RecommendationItemDepImage.AnimateFadeIn()).Into(RecommendationItemDepImage);
                    if(ViewModel.Data.AnimeRecommendationData.ImgUrl != null)
                        ImageService.Instance.LoadUrl(ViewModel.Data.AnimeRecommendationData.ImgUrl).FadeAnimation(false).Success(() => RecommendationItemRecImage.AnimateFadeIn()).Into(RecommendationItemRecImage);
                }));

            RecommendationItemRecImageButton.SetCommand("Click",ViewModel.NavigateDepDetails);
            RecommendationItemDepImageButton.SetCommand("Click",ViewModel.NavigateRecDetails);


        }

        public override int LayoutResourceId => Resource.Layout.RecommendationItem;



        #region Views

        private ImageViewAsync _recommendationItemDepImage;
        private FrameLayout _recommendationItemDepImageButton;
        private TextView _recommendationItemDepTitle;
        private ImageViewAsync _recommendationItemRecImage;
        private FrameLayout _recommendationItemRecImageButton;
        private TextView _recommendationItemRecTitle;
        private TextView _recommendationItemDescription;
        private LinearLayout _recommendationItemDetailsContainer;
        private RelativeLayout _recommendationItemLoading;

        public ImageViewAsync RecommendationItemDepImage => _recommendationItemDepImage ?? (_recommendationItemDepImage = FindViewById<ImageViewAsync>(Resource.Id.RecommendationItemDepImage));

        public FrameLayout RecommendationItemDepImageButton => _recommendationItemDepImageButton ?? (_recommendationItemDepImageButton = FindViewById<FrameLayout>(Resource.Id.RecommendationItemDepImageButton));

        public TextView RecommendationItemDepTitle => _recommendationItemDepTitle ?? (_recommendationItemDepTitle = FindViewById<TextView>(Resource.Id.RecommendationItemDepTitle));

        public ImageViewAsync RecommendationItemRecImage => _recommendationItemRecImage ?? (_recommendationItemRecImage = FindViewById<ImageViewAsync>(Resource.Id.RecommendationItemRecImage));

        public FrameLayout RecommendationItemRecImageButton => _recommendationItemRecImageButton ?? (_recommendationItemRecImageButton = FindViewById<FrameLayout>(Resource.Id.RecommendationItemRecImageButton));

        public TextView RecommendationItemRecTitle => _recommendationItemRecTitle ?? (_recommendationItemRecTitle = FindViewById<TextView>(Resource.Id.RecommendationItemRecTitle));

        public TextView RecommendationItemDescription => _recommendationItemDescription ?? (_recommendationItemDescription = FindViewById<TextView>(Resource.Id.RecommendationItemDescription));

        public LinearLayout RecommendationItemDetailsContainer => _recommendationItemDetailsContainer ?? (_recommendationItemDetailsContainer = FindViewById<LinearLayout>(Resource.Id.RecommendationItemDetailsContainer));

        public RelativeLayout RecommendationItemLoading => _recommendationItemLoading ?? (_recommendationItemLoading = FindViewById<RelativeLayout>(Resource.Id.RecommendationItemLoading));




        #endregion
    }
}