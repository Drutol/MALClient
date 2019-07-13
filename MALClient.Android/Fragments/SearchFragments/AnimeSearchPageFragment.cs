using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AoLibs.Adapters.Android.Recycler;
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.SearchFragments
{
    public class AnimeSearchPageFragment : MalFragmentBase
    {
        private bool _waitForRootView;

        public AnimeSearchPageFragment(bool initBindings) : base(initBindings)
        {

        }

        private SearchPageViewModel ViewModel;


        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.SearchPage;
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
        }

        protected override void InitBindings()
        {
            if (_waitForRootView)
            {
                _waitForRootView = false;
                NavigatedTo();
            }

            SearchRecyclerView.SetAdapter(new ObservableRecyclerAdapter<AnimeSearchItemViewModel, ItemHolder>(
                ViewModel.AnimeSearchItemViewModels, DataTemplate, LayoutInflater,
                Resource.Layout.AnimeSearchItem) {StretchContentHorizonatally = true});
            SearchRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));

            Bindings.Add(this.SetBinding(() => ViewModel.Loading).WhenSourceChanges(() =>
            {
                if (ViewModel.Loading)
                {
                    AnimeSearchPageLoadingSpinner.Visibility = ViewStates.Visible;
                }
                else
                {
                    AnimeSearchPageLoadingSpinner.Visibility = ViewStates.Gone;
                }
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.EmptyNoticeVisibility).WhenSourceChanges(() =>
            {
                if (ViewModel.EmptyNoticeVisibility)
                {
                    AnimeSearchPageEmptyNotice.Visibility = ViewStates.Visible;
                }
                else
                {
                    AnimeSearchPageEmptyNotice.Visibility = ViewStates.Gone;
                }
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.IsFirstVisitGridVisible).WhenSourceChanges(() =>
            {
                if (ViewModel.IsFirstVisitGridVisible)
                {
                    AnimeSearchPageFirstSearchSection.Visibility = ViewStates.Visible;
                }
                else
                {
                    AnimeSearchPageFirstSearchSection.Visibility = ViewStates.Gone;
                }
            }));
        }

        public override void DetachBindings()
        {

        }

        private void DataTemplate(AnimeSearchItemViewModel item, ItemHolder holder, int position)
        {
            holder.AnimeSearchItemTitle.Text = item.Title;
            holder.AnimeSearchItemType.Text = item.Type;
            holder.AnimeSearchItemDescription.Text = item.Synopsis;
            holder.WatchedEpisodes.Text = item.WatchedEps;

            if (Settings.HideGlobalScoreInDetailsWhenNotRated)
            {
                if (item.IsAuth && item.MyScore > 0)
                {
                    holder.AnimeSearchItemGlobalScoreContainer.Visibility = ViewStates.Visible;
                    holder.AnimeSearchItemGlobalScore.Text = item.GlobalScoreBind;
                }
                else
                {
                    holder.AnimeSearchItemGlobalScoreContainer.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                holder.AnimeSearchItemGlobalScore.Text = item.GlobalScoreBind;
            }


            if (item.IsAuth)
            {
                holder.TopRightInfo.Visibility = ViewStates.Visible;
                holder.WatchingStatus.Text =
                    item.MyStatusBindShort;
                holder.WatchedEpisodes.Text =
                    item.MyEpisodesBindShort;
            }
            else
            {
                holder.TopRightInfo.Visibility = ViewStates.Gone;
            }

            holder.AnimeSearchItemImage.Into(item.ImgUrl);

            holder.ClickSurface.SetOnClickListener(new OnClickListener(view => item.NavigateDetails()));
        }

        public void NavigatedTo()
        {
            if (RootView == null)
            {
                _waitForRootView = true;
                return;
            }
        }
        public override int LayoutResourceId => Resource.Layout.AnimeSearchPage;


        class ItemHolder : RecyclerView.ViewHolder
        {
            private readonly View _view;

            public ItemHolder(View view) : base(view)
            {
                _view = view;
            }
            private ImageView _animeSearchItemImage;
            private TextView _watchingStatus;
            private TextView _watchedEpisodes;
            private LinearLayout _topRightInfo;
            private TextView _animeSearchItemType;
            private TextView _animeSearchItemEpisodes;
            private TextView _animeSearchItemGlobalScore;
            private RelativeLayout _animeSearchItemGlobalScoreContainer;
            private RelativeLayout _animeSearchItemBtmSection;
            private TextView _animeSearchItemTitle;
            private TextView _animeSearchItemDescription;
            private LinearLayout _clickSurface;

            public ImageView AnimeSearchItemImage => _animeSearchItemImage ?? (_animeSearchItemImage = _view.FindViewById<ImageView>(Resource.Id.AnimeSearchItemImage));
            public TextView WatchingStatus => _watchingStatus ?? (_watchingStatus = _view.FindViewById<TextView>(Resource.Id.WatchingStatus));
            public TextView WatchedEpisodes => _watchedEpisodes ?? (_watchedEpisodes = _view.FindViewById<TextView>(Resource.Id.WatchedEpisodes));
            public LinearLayout TopRightInfo => _topRightInfo ?? (_topRightInfo = _view.FindViewById<LinearLayout>(Resource.Id.TopRightInfo));
            public TextView AnimeSearchItemType => _animeSearchItemType ?? (_animeSearchItemType = _view.FindViewById<TextView>(Resource.Id.AnimeSearchItemType));
            public TextView AnimeSearchItemEpisodes => _animeSearchItemEpisodes ?? (_animeSearchItemEpisodes = _view.FindViewById<TextView>(Resource.Id.AnimeSearchItemEpisodes));
            public TextView AnimeSearchItemGlobalScore => _animeSearchItemGlobalScore ?? (_animeSearchItemGlobalScore = _view.FindViewById<TextView>(Resource.Id.AnimeSearchItemGlobalScore));
            public RelativeLayout AnimeSearchItemGlobalScoreContainer => _animeSearchItemGlobalScoreContainer ?? (_animeSearchItemGlobalScoreContainer = _view.FindViewById<RelativeLayout>(Resource.Id.AnimeSearchItemGlobalScoreContainer));
            public RelativeLayout AnimeSearchItemBtmSection => _animeSearchItemBtmSection ?? (_animeSearchItemBtmSection = _view.FindViewById<RelativeLayout>(Resource.Id.AnimeSearchItemBtmSection));
            public TextView AnimeSearchItemTitle => _animeSearchItemTitle ?? (_animeSearchItemTitle = _view.FindViewById<TextView>(Resource.Id.AnimeSearchItemTitle));
            public TextView AnimeSearchItemDescription => _animeSearchItemDescription ?? (_animeSearchItemDescription = _view.FindViewById<TextView>(Resource.Id.AnimeSearchItemDescription));
            public LinearLayout ClickSurface => _clickSurface ?? (_clickSurface = _view.FindViewById<LinearLayout>(Resource.Id.ClickSurface));
        }


        #region Views

        private RecyclerView _searchRecyclerView;
        private TextView _animeSearchPageEmptyNotice;
        private LinearLayout _animeSearchPageFirstSearchSection;
        private ProgressBar _animeSearchPageLoadingSpinner;

        public RecyclerView SearchRecyclerView => _searchRecyclerView ?? (_searchRecyclerView = FindViewById<RecyclerView>(Resource.Id.SearchRecyclerView));
        public TextView AnimeSearchPageEmptyNotice => _animeSearchPageEmptyNotice ?? (_animeSearchPageEmptyNotice = FindViewById<TextView>(Resource.Id.AnimeSearchPageEmptyNotice));
        public LinearLayout AnimeSearchPageFirstSearchSection => _animeSearchPageFirstSearchSection ?? (_animeSearchPageFirstSearchSection = FindViewById<LinearLayout>(Resource.Id.AnimeSearchPageFirstSearchSection));
        public ProgressBar AnimeSearchPageLoadingSpinner => _animeSearchPageLoadingSpinner ?? (_animeSearchPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeSearchPageLoadingSpinner));

        #endregion
    }
}