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
            
            AnimeDetailsPageReviewsTabsList.Adapter = ViewModel.Reviews.GetAdapter(GetTemplateDelegate);

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingReviews,
                    () => LoadingOverlay.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));         
        }

        private Dictionary<AnimeReviewData,bool> _reviewStates = new Dictionary<AnimeReviewData, bool>();

        private View GetTemplateDelegate(int i, AnimeReviewData animeReviewData, View convertView)
        {
            var view = convertView;

            if(!_reviewStates.ContainsKey(animeReviewData))
                _reviewStates.Add(animeReviewData, false);

            if (view == null)
            {
                view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeReviewItemLayout,null);
                view.Click += OnReviewClick;
            }
            view.Tag = new ReviewWrapperClass() { Data = animeReviewData };

            view.FindViewById(Resource.Id.AnimeReviewItemLayoutReviewContent).Visibility =
                _reviewStates[animeReviewData] ? ViewStates.Visible : ViewStates.Gone;

            view.FindViewById<ImageViewAsync>(Resource.Id.AnimeReviewItemLayoutAvatarImage)
                .Into(animeReviewData.AuthorAvatar);
            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutAuthor).Text = animeReviewData.Author;
            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutDate).Text = animeReviewData.Date;
            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutOverallScore).Text = animeReviewData.OverallRating;
            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutEpsSeen).Text = animeReviewData.EpisodesSeen;
            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutHelpfulCount).Text = animeReviewData.HelpfulCount;
            view.FindViewById<TextView>(Resource.Id.AnimeReviewItemLayoutReviewContent).Text = animeReviewData.Review;

            var scores = view.FindViewById<LinearLayout>(Resource.Id.AnimeReviewItemLayoutMarksList);
            scores.RemoveAllViews();
            foreach (var score in animeReviewData.Score)
            {
                var txt = new TextView(view.Context);
                txt.Text = $"{score.Field} - {score.Score}";
                txt.Typeface = Typeface.Create(ResourceExtension.FontSizeLight,TypefaceStyle.Normal);
                txt.SetTextColor(new Color(ResourceExtension.BrushText));
                scores.AddView(txt);
            }

            return view;
        }

        private void OnReviewClick(object sender, EventArgs eventArgs)
        {
            var view = sender as View;
            var model = ((ReviewWrapperClass) view.Tag).Data;
            if (_reviewStates[model]) //collapse
            {
                _reviewStates[model] = false;
                view.FindViewById(Resource.Id.AnimeReviewItemLayoutReviewContent).Visibility = ViewStates.Gone;
            }
            else //expand
            {
                _reviewStates[model] = true;
                view.FindViewById(Resource.Id.AnimeReviewItemLayoutReviewContent).Visibility = ViewStates.Visible;
            }
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageReviewsTab;

        public static AnimeDetailsPageReviewsTabFragment Instance => new AnimeDetailsPageReviewsTabFragment();

        #region Views

        private ListView _animeDetailsPageReviewsTabsList;

        public ListView AnimeDetailsPageReviewsTabsList => _animeDetailsPageReviewsTabsList ?? (_animeDetailsPageReviewsTabsList = FindViewById<ListView>(Resource.Id.AnimeDetailsPageReviewsTabsList));



        private RelativeLayout _loadingOverlay;
        public RelativeLayout LoadingOverlay => _loadingOverlay ?? (_loadingOverlay = FindViewById<RelativeLayout>(Resource.Id.AnimeDetailsPageReviewsTabLoadingOverlay));
        #endregion

    }
}