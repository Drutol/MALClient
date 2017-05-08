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
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.RecommendationsFragments
{
    public class RecommendationItemFragment : MalFragmentBase
    {
        private bool _delayForInit;

        private RecommendationItemViewModel ViewModel;

        public RecommendationItemFragment()
        {

        }

        public void BindModel(RecommendationItemViewModel viewModel)
        {
            ViewModel = viewModel;

            if (RootView == null)
            {
                _delayForInit = true;
                return;
            }

            Bindings.ForEach(binding => binding.Detach());
            Bindings.Clear();

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingSpinnerVisibility).WhenSourceChanges(() =>
                {
                    if (ViewModel.LoadingSpinnerVisibility)
                    {
                        RecommendationItemLoading.Visibility = ViewStates.Visible;
                        return;
                    }
                    RecommendationItemLoading.Visibility = ViewStates.Gone;
                   

                    RecommendationItemDescription.Text = ViewModel.Data.Description;
                    RecommendationItemDepTitle.Text = ViewModel.Data.DependentTitle;
                    RecommendationItemRecTitle.Text = ViewModel.Data.RecommendationTitle;
                    if (ViewModel.Data.AnimeDependentData?.ImgUrl != null)
                        RecommendationItemDepImage.Into(ViewModel.Data.AnimeDependentData.ImgUrl);
                    if (ViewModel.Data.AnimeRecommendationData?.ImgUrl != null)
                        RecommendationItemRecImage.Into(ViewModel.Data.AnimeRecommendationData.ImgUrl);

                    if (ViewModel.DetailItems.Count == 0)
                        return;

                    //Because adapter is slow here
                    //
                    RecommendationItemDetailItemType1.Text = ViewModel.DetailItems[0].Item1;
                    RecommendationItemDetailItemDepValue1.Text = ViewModel.DetailItems[0].Item2;                
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[0].Item3))
                    {
                        RecommendationItemDetailItemMyDepValue1.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyDepValue1.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyDepValue1.Text = ViewModel.DetailItems[0].Item3;
                    }
                    RecommendationItemDetailItemRecValue1.Text = ViewModel.DetailItems[0].Item4;
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[0].Item5))
                    {
                        RecommendationItemDetailItemMyRecValue1.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyRecValue1.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyRecValue1.Text = ViewModel.DetailItems[0].Item5;
                    }

                    //
                    RecommendationItemDetailItemType2.Text = ViewModel.DetailItems[1].Item1;
                    RecommendationItemDetailItemDepValue2.Text = ViewModel.DetailItems[1].Item2;    
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[1].Item3))
                    {
                        RecommendationItemDetailItemMyDepValue2.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyDepValue2.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyDepValue2.Text = ViewModel.DetailItems[1].Item3;
                    }
                    RecommendationItemDetailItemRecValue2.Text = ViewModel.DetailItems[1].Item4;
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[1].Item5))
                    {
                        RecommendationItemDetailItemMyRecValue2.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyRecValue2.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyRecValue2.Text = ViewModel.DetailItems[1].Item5;
                    }
                    //
                    RecommendationItemDetailItemType3.Text = ViewModel.DetailItems[2].Item1;
                    RecommendationItemDetailItemDepValue3.Text = ViewModel.DetailItems[2].Item2;
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[2].Item3))
                    {
                        RecommendationItemDetailItemMyDepValue3.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyDepValue3.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyDepValue3.Text = ViewModel.DetailItems[2].Item3;
                    }
                    RecommendationItemDetailItemRecValue3.Text = ViewModel.DetailItems[2].Item4;
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[2].Item5))
                    {
                        RecommendationItemDetailItemMyRecValue3.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyRecValue3.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyRecValue3.Text = ViewModel.DetailItems[2].Item5;
                    }
                    //
                    RecommendationItemDetailItemType4.Text = ViewModel.DetailItems[3].Item1;
                    RecommendationItemDetailItemDepValue4.Text = ViewModel.DetailItems[3].Item2;
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[3].Item3))
                    {
                        RecommendationItemDetailItemMyDepValue4.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyDepValue4.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyDepValue4.Text = ViewModel.DetailItems[3].Item3;
                    }
                    RecommendationItemDetailItemRecValue4.Text = ViewModel.DetailItems[3].Item4;
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[3].Item5))
                    {
                        RecommendationItemDetailItemMyRecValue4.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyRecValue4.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyRecValue4.Text = ViewModel.DetailItems[3].Item5;
                    }
                    //
                    RecommendationItemDetailItemType5.Text = ViewModel.DetailItems[4].Item1;
                    RecommendationItemDetailItemDepValue5.Text = ViewModel.DetailItems[4].Item2;                   
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[4].Item3))
                    {
                        RecommendationItemDetailItemMyDepValue5.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyDepValue5.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyDepValue5.Text = ViewModel.DetailItems[4].Item3;
                    }
                    RecommendationItemDetailItemRecValue5.Text = ViewModel.DetailItems[4].Item4;
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[4].Item5))
                    {
                        RecommendationItemDetailItemMyRecValue5.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyRecValue5.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyRecValue5.Text = ViewModel.DetailItems[4].Item5;
                    }                    
                    //
                    RecommendationItemDetailItemType6.Text = ViewModel.DetailItems[5].Item1;
                    RecommendationItemDetailItemDepValue6.Text = ViewModel.DetailItems[5].Item2;                   
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[5].Item3))
                    {
                        RecommendationItemDetailItemMyDepValue6.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyDepValue6.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyDepValue6.Text = ViewModel.DetailItems[5].Item3;
                    }
                    RecommendationItemDetailItemRecValue6.Text = ViewModel.DetailItems[5].Item4;
                    if (string.IsNullOrEmpty(ViewModel.DetailItems[5].Item5))
                    {
                        RecommendationItemDetailItemMyRecValue6.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        RecommendationItemDetailItemMyRecValue6.Visibility = ViewStates.Visible;
                        RecommendationItemDetailItemMyRecValue6.Text = ViewModel.DetailItems[5].Item5;
                    }
                }));

            RecommendationItemRecImageButton.SetOnClickListener(new OnClickListener(view => ViewModel.NavigateRecDetails.Execute(null)));
            RecommendationItemDepImageButton.SetOnClickListener(new OnClickListener(view => ViewModel.NavigateDepDetails.Execute(null)));
        }


        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            if (_delayForInit)
            {
                _delayForInit = false;
                BindModel(ViewModel);
            }

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
        private TextView _recommendationItemDetailItemType1;
        private TextView _recommendationItemDetailItemDepValue1;
        private TextView _recommendationItemDetailItemMyDepValue1;
        private TextView _recommendationItemDetailItemRecValue1;
        private TextView _recommendationItemDetailItemMyRecValue1;
        private TextView _recommendationItemDetailItemType2;
        private TextView _recommendationItemDetailItemDepValue2;
        private TextView _recommendationItemDetailItemMyDepValue2;
        private TextView _recommendationItemDetailItemRecValue2;
        private TextView _recommendationItemDetailItemMyRecValue2;
        private TextView _recommendationItemDetailItemType3;
        private TextView _recommendationItemDetailItemDepValue3;
        private TextView _recommendationItemDetailItemMyDepValue3;
        private TextView _recommendationItemDetailItemRecValue3;
        private TextView _recommendationItemDetailItemMyRecValue3;
        private TextView _recommendationItemDetailItemType4;
        private TextView _recommendationItemDetailItemDepValue4;
        private TextView _recommendationItemDetailItemMyDepValue4;
        private TextView _recommendationItemDetailItemRecValue4;
        private TextView _recommendationItemDetailItemMyRecValue4;
        private TextView _recommendationItemDetailItemType5;
        private TextView _recommendationItemDetailItemDepValue5;
        private TextView _recommendationItemDetailItemMyDepValue5;
        private TextView _recommendationItemDetailItemRecValue5;
        private TextView _recommendationItemDetailItemMyRecValue5;
        private TextView _recommendationItemDetailItemType6;
        private TextView _recommendationItemDetailItemDepValue6;
        private TextView _recommendationItemDetailItemMyDepValue6;
        private TextView _recommendationItemDetailItemRecValue6;
        private TextView _recommendationItemDetailItemMyRecValue6;
        private LinearLayout _recommendationItemDetailsContainer;
        private RelativeLayout _recommendationItemLoading;

        public ImageViewAsync RecommendationItemDepImage => _recommendationItemDepImage ?? (_recommendationItemDepImage = FindViewById<ImageViewAsync>(Resource.Id.RecommendationItemDepImage));

        public FrameLayout RecommendationItemDepImageButton => _recommendationItemDepImageButton ?? (_recommendationItemDepImageButton = FindViewById<FrameLayout>(Resource.Id.RecommendationItemDepImageButton));

        public TextView RecommendationItemDepTitle => _recommendationItemDepTitle ?? (_recommendationItemDepTitle = FindViewById<TextView>(Resource.Id.RecommendationItemDepTitle));

        public ImageViewAsync RecommendationItemRecImage => _recommendationItemRecImage ?? (_recommendationItemRecImage = FindViewById<ImageViewAsync>(Resource.Id.RecommendationItemRecImage));

        public FrameLayout RecommendationItemRecImageButton => _recommendationItemRecImageButton ?? (_recommendationItemRecImageButton = FindViewById<FrameLayout>(Resource.Id.RecommendationItemRecImageButton));

        public TextView RecommendationItemRecTitle => _recommendationItemRecTitle ?? (_recommendationItemRecTitle = FindViewById<TextView>(Resource.Id.RecommendationItemRecTitle));

        public TextView RecommendationItemDescription => _recommendationItemDescription ?? (_recommendationItemDescription = FindViewById<TextView>(Resource.Id.RecommendationItemDescription));

        public TextView RecommendationItemDetailItemType1 => _recommendationItemDetailItemType1 ?? (_recommendationItemDetailItemType1 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemType1));

        public TextView RecommendationItemDetailItemDepValue1 => _recommendationItemDetailItemDepValue1 ?? (_recommendationItemDetailItemDepValue1 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemDepValue1));

        public TextView RecommendationItemDetailItemMyDepValue1 => _recommendationItemDetailItemMyDepValue1 ?? (_recommendationItemDetailItemMyDepValue1 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyDepValue1));

        public TextView RecommendationItemDetailItemRecValue1 => _recommendationItemDetailItemRecValue1 ?? (_recommendationItemDetailItemRecValue1 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemRecValue1));

        public TextView RecommendationItemDetailItemMyRecValue1 => _recommendationItemDetailItemMyRecValue1 ?? (_recommendationItemDetailItemMyRecValue1 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyRecValue1));

        public TextView RecommendationItemDetailItemType2 => _recommendationItemDetailItemType2 ?? (_recommendationItemDetailItemType2 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemType2));

        public TextView RecommendationItemDetailItemDepValue2 => _recommendationItemDetailItemDepValue2 ?? (_recommendationItemDetailItemDepValue2 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemDepValue2));

        public TextView RecommendationItemDetailItemMyDepValue2 => _recommendationItemDetailItemMyDepValue2 ?? (_recommendationItemDetailItemMyDepValue2 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyDepValue2));

        public TextView RecommendationItemDetailItemRecValue2 => _recommendationItemDetailItemRecValue2 ?? (_recommendationItemDetailItemRecValue2 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemRecValue2));

        public TextView RecommendationItemDetailItemMyRecValue2 => _recommendationItemDetailItemMyRecValue2 ?? (_recommendationItemDetailItemMyRecValue2 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyRecValue2));

        public TextView RecommendationItemDetailItemType3 => _recommendationItemDetailItemType3 ?? (_recommendationItemDetailItemType3 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemType3));

        public TextView RecommendationItemDetailItemDepValue3 => _recommendationItemDetailItemDepValue3 ?? (_recommendationItemDetailItemDepValue3 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemDepValue3));

        public TextView RecommendationItemDetailItemMyDepValue3 => _recommendationItemDetailItemMyDepValue3 ?? (_recommendationItemDetailItemMyDepValue3 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyDepValue3));

        public TextView RecommendationItemDetailItemRecValue3 => _recommendationItemDetailItemRecValue3 ?? (_recommendationItemDetailItemRecValue3 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemRecValue3));

        public TextView RecommendationItemDetailItemMyRecValue3 => _recommendationItemDetailItemMyRecValue3 ?? (_recommendationItemDetailItemMyRecValue3 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyRecValue3));

        public TextView RecommendationItemDetailItemType4 => _recommendationItemDetailItemType4 ?? (_recommendationItemDetailItemType4 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemType4));

        public TextView RecommendationItemDetailItemDepValue4 => _recommendationItemDetailItemDepValue4 ?? (_recommendationItemDetailItemDepValue4 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemDepValue4));

        public TextView RecommendationItemDetailItemMyDepValue4 => _recommendationItemDetailItemMyDepValue4 ?? (_recommendationItemDetailItemMyDepValue4 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyDepValue4));

        public TextView RecommendationItemDetailItemRecValue4 => _recommendationItemDetailItemRecValue4 ?? (_recommendationItemDetailItemRecValue4 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemRecValue4));

        public TextView RecommendationItemDetailItemMyRecValue4 => _recommendationItemDetailItemMyRecValue4 ?? (_recommendationItemDetailItemMyRecValue4 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyRecValue4));

        public TextView RecommendationItemDetailItemType5 => _recommendationItemDetailItemType5 ?? (_recommendationItemDetailItemType5 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemType5));

        public TextView RecommendationItemDetailItemDepValue5 => _recommendationItemDetailItemDepValue5 ?? (_recommendationItemDetailItemDepValue5 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemDepValue5));

        public TextView RecommendationItemDetailItemMyDepValue5 => _recommendationItemDetailItemMyDepValue5 ?? (_recommendationItemDetailItemMyDepValue5 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyDepValue5));

        public TextView RecommendationItemDetailItemRecValue5 => _recommendationItemDetailItemRecValue5 ?? (_recommendationItemDetailItemRecValue5 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemRecValue5));

        public TextView RecommendationItemDetailItemMyRecValue5 => _recommendationItemDetailItemMyRecValue5 ?? (_recommendationItemDetailItemMyRecValue5 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyRecValue5));

        public TextView RecommendationItemDetailItemType6 => _recommendationItemDetailItemType6 ?? (_recommendationItemDetailItemType6 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemType6));

        public TextView RecommendationItemDetailItemDepValue6 => _recommendationItemDetailItemDepValue6 ?? (_recommendationItemDetailItemDepValue6 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemDepValue6));

        public TextView RecommendationItemDetailItemMyDepValue6 => _recommendationItemDetailItemMyDepValue6 ?? (_recommendationItemDetailItemMyDepValue6 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyDepValue6));

        public TextView RecommendationItemDetailItemRecValue6 => _recommendationItemDetailItemRecValue6 ?? (_recommendationItemDetailItemRecValue6 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemRecValue6));

        public TextView RecommendationItemDetailItemMyRecValue6 => _recommendationItemDetailItemMyRecValue6 ?? (_recommendationItemDetailItemMyRecValue6 = FindViewById<TextView>(Resource.Id.RecommendationItemDetailItemMyRecValue6));

        public LinearLayout RecommendationItemDetailsContainer => _recommendationItemDetailsContainer ?? (_recommendationItemDetailsContainer = FindViewById<LinearLayout>(Resource.Id.RecommendationItemDetailsContainer));

        public RelativeLayout RecommendationItemLoading => _recommendationItemLoading ?? (_recommendationItemLoading = FindViewById<RelativeLayout>(Resource.Id.RecommendationItemLoading));


        #endregion
    }
}