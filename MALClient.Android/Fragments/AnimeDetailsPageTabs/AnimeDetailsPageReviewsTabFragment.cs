using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Resources;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    public class AnimeDetailsPageReviewsTabFragment : MalFragmentBase
    {
        private class ReviewWrapperClass : Java.Lang.Object
        {
            public AnimeReviewData Data { get; set; }
        }

        private AnimeDetailsPageViewModel ViewModel;

        private AnimeDetailsPageReviewsTabFragment()
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            
        }

        protected override void InitBindings()
        {
            
            AnimeDetailsPageReviewsTabsList.InjectFlingAdapter(ViewModel.Reviews,DataTemplateFull,DataTemplateFling,ContainerTemplate,DataTemplateBasic);

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingReviews,
                    () => AnimeDetailsPageReviewsTabLoadingOverlay.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            AnimeDetailsPageReviewsTabsList.EmptyView = AnimeDetailsPageReviewsTabEmptyNotice;
        }



        private View ContainerTemplate(int i)
        {
            var view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeReviewItemLayout, null);
            view.Click += OnReviewClick;
            return view;
        }

        private void DataTemplateBasic(View view, int i, AnimeReviewData animeReviewData)
        {
            if(!_reviewStates.ContainsKey(animeReviewData))
                _reviewStates.Add(animeReviewData,false);

            if (_reviewStates[animeReviewData])
            {
                LoadScores(view, animeReviewData);
                view.FindViewById(Resource.Id.AnimeReviewItemLayoutReviewContent).Visibility = ViewStates.Visible;
            }
            else
            {
                view.FindViewById<LinearLayout>(Resource.Id.AnimeReviewItemLayoutMarksList).RemoveAllViews();
                view.FindViewById(Resource.Id.AnimeReviewItemLayoutReviewContent).Visibility = ViewStates.Gone;
            }


            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutAuthor).Text = animeReviewData.Author;
            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutDate).Text = animeReviewData.Date;
            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutOverallScore).Text = animeReviewData.OverallRating;
            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutEpsSeen).Text = animeReviewData.EpisodesSeen;
            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutHelpfulCount).Text = animeReviewData.HelpfulCount;


        }

        private void DataTemplateFling(View view, int i, AnimeReviewData animeReviewData)
        {
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeReviewItemLayoutAvatarImage);
            if (img.IntoIfLoaded(animeReviewData.AuthorAvatar, new CircleTransformation()))
            {
                view.FindViewById(Resource.Id.AnimeReviewItemImgPlaceholder).Visibility = ViewStates.Gone;
            }
            else
            {
                view.FindViewById(Resource.Id.AnimeReviewItemImgPlaceholder).Visibility = ViewStates.Visible;              
            }
        }

        private void DataTemplateFull(View view, int i, AnimeReviewData animeReviewData)
        {
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeReviewItemLayoutAvatarImage);
            if (img.Tag == null || (string) img.Tag != animeReviewData.AuthorAvatar)
            {
                img.Into(animeReviewData.AuthorAvatar, new CircleTransformation());
                img.Tag = animeReviewData.AuthorAvatar;
            }
            else
            {
                img.Visibility = ViewStates.Visible;
            }

            view.FindViewById(Resource.Id.AnimeReviewItemImgPlaceholder).Visibility = ViewStates.Gone;
        }

        private void LoadScores(View view,AnimeReviewData animeReviewData)
        {
            var scores = view.FindViewById<LinearLayout>(Resource.Id.AnimeReviewItemLayoutMarksList);
            scores.RemoveAllViews();
            foreach (var score in animeReviewData.Score)
            {
                var txt = new TextView(view.Context)
                {
                    Text = $"{score.Field} {score.Score}",
                    Typeface = Typeface.Create(ResourceExtension.FontSizeLight, TypefaceStyle.Normal)
                };
                txt.SetTextColor(new Color(ResourceExtension.BrushText));
                scores.AddView(txt);
            }
        }

        private readonly Dictionary<AnimeReviewData,bool> _reviewStates = new Dictionary<AnimeReviewData, bool>();

        private void OnReviewClick(object sender, EventArgs eventArgs)
        {
            var view = sender as View;
            var model = view.Tag.Unwrap<AnimeReviewData>();
            if (_reviewStates[model]) //collapse
            {
                _reviewStates[model] = false;
                view.FindViewById<LinearLayout>(Resource.Id.AnimeReviewItemLayoutMarksList).RemoveAllViews();
                view.FindViewById(Resource.Id.AnimeReviewItemLayoutReviewContent).Visibility = ViewStates.Gone;
                view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutReviewContent).Text = "";
            }
            else //expand
            {
                _reviewStates[model] = true;
                LoadScores(view, model);
                view.FindViewById(Resource.Id.AnimeReviewItemLayoutReviewContent).Visibility = ViewStates.Visible;
                view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutReviewContent).Text = model.Review;
            }
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageReviewsTab;

        public static AnimeDetailsPageReviewsTabFragment Instance => new AnimeDetailsPageReviewsTabFragment();

        #region Views

        private ListView _animeDetailsPageReviewsTabsList;
        private TextView _animeDetailsPageReviewsTabEmptyNotice;
        private RelativeLayout _animeDetailsPageReviewsTabLoadingOverlay;

        public ListView AnimeDetailsPageReviewsTabsList => _animeDetailsPageReviewsTabsList ?? (_animeDetailsPageReviewsTabsList = FindViewById<ListView>(Resource.Id.AnimeDetailsPageReviewsTabsList));

        public TextView AnimeDetailsPageReviewsTabEmptyNotice => _animeDetailsPageReviewsTabEmptyNotice ?? (_animeDetailsPageReviewsTabEmptyNotice = FindViewById<TextView>(Resource.Id.AnimeDetailsPageReviewsTabEmptyNotice));

        public RelativeLayout AnimeDetailsPageReviewsTabLoadingOverlay => _animeDetailsPageReviewsTabLoadingOverlay ?? (_animeDetailsPageReviewsTabLoadingOverlay = FindViewById<RelativeLayout>(Resource.Id.AnimeDetailsPageReviewsTabLoadingOverlay));
        #endregion

    }
}