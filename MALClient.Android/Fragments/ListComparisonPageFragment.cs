using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Items;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public partial class ListComparisonPageFragment : MalFragmentBase
    {
        private readonly ListComparisonPageNavigationArgs _args;
        private ListComparisonViewModel ViewModel;

        public ListComparisonPageFragment(ListComparisonPageNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.Comparison;
            ViewModel.NavigatedTo(_args);
        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.MyData).WhenSourceChanges(() =>
            {
                if(ViewModel.MyData == null)
                    return;

                MyName.Text = ViewModel.MyData.User.Name;
                MyImage.Into(ViewModel.MyData.User.ImgUrl,new CircleTransformation());
                MyMean.Text = ViewModel.MyData.AnimeMean.ToString("N2");
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.OtherData).WhenSourceChanges(() =>
            {
                if (ViewModel.OtherData == null)
                    return;

                OtherName.Text = ViewModel.OtherData.User.Name;
                OtherImage.Into(ViewModel.OtherData.User.ImgUrl,new CircleTransformation());
                OtherMean.Text = ViewModel.OtherData.AnimeMean.ToString("N2");
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.CurrentItems).WhenSourceChanges(() =>
            {
                ComparisonListView.InjectFlingAdapter(ViewModel.CurrentItems, DataTemplateFull, DataTemplateFling,
                    ContainerTemplate, DataTemplateBasic);
            }));
        }

        class ComparisonViewHolder
        {
            private readonly View _view;

            public ComparisonViewHolder(View view)
            {
                _view = view;
            }
           
            private ProgressBar _comparisonItemImgPlaceholder;
            private ImageViewAsync _comparisonItemImage;
            private TextView _comparisonItemTitle;
            private TextView _comparisonItemStatus;
            private TextView _comparisonItemOnlyOtherScore;
            private TextView _comparisonItemOnlyOtherWatched;
            private ImageView _imageView;
            private FrameLayout _comparisonItemAddToListButton;
            private LinearLayout _comparisonItemOnOtherStateSection;
            private TextView _comparisonItemOnBothMyScore;
            private TextView _comparisonItemOnBothScoreDiff;
            private TextView _comparisonItemOnBothOtherScore;
            private TextView _comparisonItemOnBothMyWatched;
            private TextView _comparisonItemOnBothWatchedDiff;
            private TextView _comparisonItemOnBothOtherWatched;
            private LinearLayout _comparisonItemOnBothWatchedSection;
            private LinearLayout _comparisonItemOnBothStateSection;
            private TextView _comparisonItemNotOnListDescription;
            private LinearLayout _comparisonItemOnlyMyStateSection;

            public ProgressBar ComparisonItemImgPlaceholder => _comparisonItemImgPlaceholder ?? (_comparisonItemImgPlaceholder = _view.FindViewById<ProgressBar>(Resource.Id.ComparisonItemImgPlaceholder));

            public ImageViewAsync ComparisonItemImage => _comparisonItemImage ?? (_comparisonItemImage = _view.FindViewById<ImageViewAsync>(Resource.Id.ComparisonItemImage));

            public TextView ComparisonItemTitle => _comparisonItemTitle ?? (_comparisonItemTitle = _view.FindViewById<TextView>(Resource.Id.ComparisonItemTitle));

            public TextView ComparisonItemStatus => _comparisonItemStatus ?? (_comparisonItemStatus = _view.FindViewById<TextView>(Resource.Id.ComparisonItemStatus));

            public TextView ComparisonItemOnlyOtherScore => _comparisonItemOnlyOtherScore ?? (_comparisonItemOnlyOtherScore = _view.FindViewById<TextView>(Resource.Id.ComparisonItemOnlyOtherScore));

            public TextView ComparisonItemOnlyOtherWatched => _comparisonItemOnlyOtherWatched ?? (_comparisonItemOnlyOtherWatched = _view.FindViewById<TextView>(Resource.Id.ComparisonItemOnlyOtherWatched));

            public ImageView ImageView => _imageView ?? (_imageView = _view.FindViewById<ImageView>(Resource.Id.imageView));

            public FrameLayout ComparisonItemAddToListButton => _comparisonItemAddToListButton ?? (_comparisonItemAddToListButton = _view.FindViewById<FrameLayout>(Resource.Id.ComparisonItemAddToListButton));

            public LinearLayout ComparisonItemOnOtherStateSection => _comparisonItemOnOtherStateSection ?? (_comparisonItemOnOtherStateSection = _view.FindViewById<LinearLayout>(Resource.Id.ComparisonItemOnOtherStateSection));

            public TextView ComparisonItemOnBothMyScore => _comparisonItemOnBothMyScore ?? (_comparisonItemOnBothMyScore = _view.FindViewById<TextView>(Resource.Id.ComparisonItemOnBothMyScore));

            public TextView ComparisonItemOnBothScoreDiff => _comparisonItemOnBothScoreDiff ?? (_comparisonItemOnBothScoreDiff = _view.FindViewById<TextView>(Resource.Id.ComparisonItemOnBothScoreDiff));

            public TextView ComparisonItemOnBothOtherScore => _comparisonItemOnBothOtherScore ?? (_comparisonItemOnBothOtherScore = _view.FindViewById<TextView>(Resource.Id.ComparisonItemOnBothOtherScore));

            public TextView ComparisonItemOnBothMyWatched => _comparisonItemOnBothMyWatched ?? (_comparisonItemOnBothMyWatched = _view.FindViewById<TextView>(Resource.Id.ComparisonItemOnBothMyWatched));

            public TextView ComparisonItemOnBothWatchedDiff => _comparisonItemOnBothWatchedDiff ?? (_comparisonItemOnBothWatchedDiff = _view.FindViewById<TextView>(Resource.Id.ComparisonItemOnBothWatchedDiff));

            public TextView ComparisonItemOnBothOtherWatched => _comparisonItemOnBothOtherWatched ?? (_comparisonItemOnBothOtherWatched = _view.FindViewById<TextView>(Resource.Id.ComparisonItemOnBothOtherWatched));

            public LinearLayout ComparisonItemOnBothWatchedSection => _comparisonItemOnBothWatchedSection ?? (_comparisonItemOnBothWatchedSection = _view.FindViewById<LinearLayout>(Resource.Id.ComparisonItemOnBothWatchedSection));

            public LinearLayout ComparisonItemOnBothStateSection => _comparisonItemOnBothStateSection ?? (_comparisonItemOnBothStateSection = _view.FindViewById<LinearLayout>(Resource.Id.ComparisonItemOnBothStateSection));

            public TextView ComparisonItemNotOnListDescription => _comparisonItemNotOnListDescription ?? (_comparisonItemNotOnListDescription = _view.FindViewById<TextView>(Resource.Id.ComparisonItemNotOnListDescription));

            public LinearLayout ComparisonItemOnlyMyStateSection => _comparisonItemOnlyMyStateSection ?? (_comparisonItemOnlyMyStateSection = _view.FindViewById<LinearLayout>(Resource.Id.ComparisonItemOnlyMyStateSection));


        }
            
        private Dictionary<View,ComparisonViewHolder> _comparisonViewHolders = new Dictionary<View, ComparisonViewHolder>();
        private View ContainerTemplate(int i)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.ComparisonItem,null);
            view.Click += ViewOnClick;
            var holder = new ComparisonViewHolder(view);
            _comparisonViewHolders.Add(view,holder);

            holder.ComparisonItemAddToListButton.Click += async (sender, args) =>
            {
                var v = sender as View;
                var vm = v.Tag.Unwrap<ComparisonItemViewModel>();
                await vm.AddToMyListAsync();
                DataTemplateBasic(v, 0, vm);
            };
            return view;
        }

        private void DataTemplateBasic(View view, int i, ComparisonItemViewModel arg3)
        {
            var holder = _comparisonViewHolders[view];

            holder.ComparisonItemTitle.Text = arg3.Title;
            holder.ComparisonItemStatus.Text =
                $"{arg3.MyEntry?.MyStatusBindShort ?? "?"} - {arg3.OtherEntry?.MyStatusBindShort ?? "?"}";



            if (arg3.IsOnlyOnMyList)
            {
                holder.ComparisonItemOnlyMyStateSection.Visibility = ViewStates.Visible;
                holder.ComparisonItemOnOtherStateSection.Visibility = ViewStates.Gone;
                holder.ComparisonItemOnBothStateSection.Visibility = ViewStates.Gone;

                holder.ComparisonItemNotOnListDescription.Text =
                    $"{ViewModel.OtherData.User.Name} doesn't have this show on list...";
            }
            else if(arg3.IsOnlyOnOtherList)
            {
                holder.ComparisonItemOnlyMyStateSection.Visibility = ViewStates.Gone;
                holder.ComparisonItemOnOtherStateSection.Visibility = ViewStates.Visible;
                holder.ComparisonItemOnBothStateSection.Visibility = ViewStates.Gone;

                holder.ComparisonItemOnlyOtherScore.Text = ScoreToString(arg3.OtherEntry.MyScore);
                holder.ComparisonItemOnlyOtherWatched.Text = arg3.OtherEntry.MyEpisodesBindShort;
            }
            else //comparison
            {
                holder.ComparisonItemOnlyMyStateSection.Visibility = ViewStates.Gone;
                holder.ComparisonItemOnOtherStateSection.Visibility = ViewStates.Gone;
                holder.ComparisonItemOnBothStateSection.Visibility = ViewStates.Visible;

                holder.ComparisonItemOnBothMyScore.Text = ScoreToString(arg3.MyEntry.MyScore);
                holder.ComparisonItemOnBothScoreDiff.Text = arg3.ScoreDifferenceBind;
                holder.ComparisonItemOnBothOtherScore.Text = ScoreToString(arg3.OtherEntry.MyScore);

                holder.ComparisonItemOnBothMyWatched.Text = arg3.MyEntry.MyEpisodes.ToString();
                holder.ComparisonItemOnBothWatchedDiff.Text = arg3.WatchedDifferenceBind;
                holder.ComparisonItemOnBothOtherWatched.Text = arg3.OtherEntry.MyEpisodes.ToString();

                holder.ComparisonItemOnBothScoreDiff.SetTextColor(new Color(ResourcesCompat.GetColor(Resources,GetColorResForDiff(arg3.ScoreDifference),Activity.Theme)));
                holder.ComparisonItemOnBothWatchedDiff.SetTextColor(new Color(ResourcesCompat.GetColor(Resources,GetColorResForDiff(arg3.WatchedDifference),Activity.Theme)));
            }

            int GetColorResForDiff(int diff)
            {
                if (diff > 0)
                    return Resource.Color.LimeAccentColour;
                if (diff < 0)
                    return global::Android.Resource.Color.HoloRedDark;

                return global::Android.Resource.Color.White;
            }

            string ScoreToString(float score)
            {
                return score == 0
                    ? "?"
                    : arg3.OtherEntry.MyScore.ToString("N0");
            }
        }

        private void DataTemplateFling(View view, int i, ComparisonItemViewModel arg3)
        {
            var holder = _comparisonViewHolders[view];

            if (holder.ComparisonItemImage.IntoIfLoaded(arg3.ImgUrl))
            {
                holder.ComparisonItemImage.Visibility = ViewStates.Visible;
                holder.ComparisonItemImgPlaceholder.Visibility = ViewStates.Gone;
            }
            else
            {
                holder.ComparisonItemImage.Visibility = ViewStates.Invisible;
                holder.ComparisonItemImgPlaceholder.Visibility = ViewStates.Visible;
            }
        }

        private void DataTemplateFull(View view, int i, ComparisonItemViewModel arg3)
        {
            var holder = _comparisonViewHolders[view];

            if (holder.ComparisonItemImage.Tag == null || (string)holder.ComparisonItemImage.Tag != arg3.ImgUrl)
                holder.ComparisonItemImage.Into(arg3.ImgUrl);
            holder.ComparisonItemImgPlaceholder.Visibility = ViewStates.Gone;
        }



        private void ViewOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateDetailsCommand.Execute((sender as View).Tag.Unwrap<ComparisonItemViewModel>());
        }


        public override int LayoutResourceId => Resource.Layout.ListComparisonPage;

        #region Views

        private ImageViewAsync _myImage;
        private TextView _myName;
        private TextView _myMean;
        private TextView _otherMean;
        private ImageViewAsync _otherImage;
        private TextView _otherName;
        private ListView _comparisonListView;

        public ImageViewAsync MyImage => _myImage ?? (_myImage = FindViewById<ImageViewAsync>(Resource.Id.MyImage));

        public TextView MyName => _myName ?? (_myName = FindViewById<TextView>(Resource.Id.MyName));

        public TextView MyMean => _myMean ?? (_myMean = FindViewById<TextView>(Resource.Id.MyMean));

        public TextView OtherMean => _otherMean ?? (_otherMean = FindViewById<TextView>(Resource.Id.OtherMean));

        public ImageViewAsync OtherImage => _otherImage ?? (_otherImage = FindViewById<ImageViewAsync>(Resource.Id.OtherImage));

        public TextView OtherName => _otherName ?? (_otherName = FindViewById<TextView>(Resource.Id.OtherName));

        public ListView ComparisonListView => _comparisonListView ?? (_comparisonListView = FindViewById<ListView>(Resource.Id.ComparisonListView));



        #endregion
    }
}