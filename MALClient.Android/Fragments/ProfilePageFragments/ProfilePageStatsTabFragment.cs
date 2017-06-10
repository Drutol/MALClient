using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Renderscripts;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.PagerAdapters;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.ProfilePageFragments
{
    public class ProfilePageStatsTabFragment : MalFragmentBase
    {
        private bool _initialized;
        private ProfilePageViewModel ViewModel = ViewModelLocator.ProfilePage;

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            
        }

        public void NavigatedTo()
        {
            if (_initialized)
                return;
            _initialized = true;

            (RootView as FrameLayout).AddView(
                Activity.LayoutInflater.Inflate(Resource.Layout.ProfilePageStatsTabContent, null));


            Bindings.Add(this.SetBinding(() => ViewModel.MangaChartValues)
                .WhenSourceChanges(() =>
                {
                    if (ViewModel.AnimeChartValues == null || ViewModel.MangaChartValues == null)
                        return;

                    var all = ViewModel.AnimeChartValues.Sum();
                    var allManga = ViewModel.MangaChartValues.Sum();

                    if (all == 0 || allManga == 0)
                        return;

                    LinearLayout.LayoutParams param;
                    /////////
                    //ANIME//
                    /////////
                    param = (LinearLayout.LayoutParams) ProfilePageStatsFragmentWatchingAnimeBar.LayoutParameters;
                    param.Weight = ViewModel.CurrentData.AnimeWatching * 100 / all;
                    ProfilePageStatsFragmentWatchingAnimeBar.LayoutParameters = param;
                    if (param.Weight > 7) ProfilePageStatsFragmentWatchingAnimeBarLabel.Text = $"{param.Weight}%";

                    param = (LinearLayout.LayoutParams) ProfilePageStatsFragmentCompletedAnimeBar.LayoutParameters;
                    param.Weight = ViewModel.CurrentData.AnimeCompleted * 100 / all;
                    ProfilePageStatsFragmentCompletedAnimeBar.LayoutParameters = param;
                    if (param.Weight > 7) ProfilePageStatsFragmentCompletedAnimeBarLabel.Text = $"{param.Weight}%";

                    param = (LinearLayout.LayoutParams) ProfilePageStatsFragmentOnHoldAnimeBar.LayoutParameters;
                    param.Weight = ViewModel.CurrentData.AnimeOnHold * 100 / all;
                    ProfilePageStatsFragmentOnHoldAnimeBar.LayoutParameters = param;
                    if (param.Weight > 7) ProfilePageStatsFragmentOnHoldAnimeBarLabel.Text = $"{param.Weight}%";

                    param = (LinearLayout.LayoutParams) ProfilePageStatsFragmentDroppedAnimeBar.LayoutParameters;
                    param.Weight = ViewModel.CurrentData.AnimeDropped * 100 / all;
                    ProfilePageStatsFragmentDroppedAnimeBar.LayoutParameters = param;
                    if (param.Weight > 7) ProfilePageStatsFragmentDroppedAnimeBarLabel.Text = $"{param.Weight}%";

                    param = (LinearLayout.LayoutParams) ProfilePageStatsFragmentPlannedAnimeBar.LayoutParameters;
                    param.Weight = ViewModel.CurrentData.AnimePlanned * 100 / all;
                    ProfilePageStatsFragmentPlannedAnimeBar.LayoutParameters = param;
                    if (param.Weight > 7) ProfilePageStatsFragmentPlannedAnimeBarLabel.Text = $"{param.Weight}%";

                    /////////
                    //MANGA//
                    /////////
                    /// 
                    param = (LinearLayout.LayoutParams) ProfilePageStatsFragmentWatchingMangaBar.LayoutParameters;
                    param.Weight = ViewModel.CurrentData.MangaReading * 100 / allManga;
                    ProfilePageStatsFragmentWatchingMangaBar.LayoutParameters = param;
                    if (param.Weight > 7) ProfilePageStatsFragmentWatchingMangaBarLabel.Text = $"{param.Weight}%";

                    param = (LinearLayout.LayoutParams) ProfilePageStatsFragmentCompletedMangaBar.LayoutParameters;
                    param.Weight = ViewModel.CurrentData.MangaCompleted * 100 / allManga;
                    ProfilePageStatsFragmentCompletedMangaBar.LayoutParameters = param;
                    if (param.Weight > 7) ProfilePageStatsFragmentCompletedMangaBarLabel.Text = $"{param.Weight}%";

                    param = (LinearLayout.LayoutParams) ProfilePageStatsFragmentOnHoldMangaBar.LayoutParameters;
                    param.Weight = ViewModel.CurrentData.MangaOnHold * 100 / allManga;
                    ProfilePageStatsFragmentOnHoldMangaBar.LayoutParameters = param;
                    if (param.Weight > 7)
                        ProfilePageStatsFragmentOnHoldMangaBarLabel.Text = $"{param.Weight}%";

                    param = (LinearLayout.LayoutParams) ProfilePageStatsFragmentDroppedMangaBar.LayoutParameters;
                    param.Weight = ViewModel.CurrentData.MangaDropped * 100 / allManga;
                    ProfilePageStatsFragmentDroppedMangaBar.LayoutParameters = param;
                    if (param.Weight > 7) ProfilePageStatsFragmentDroppedMangaBarLabel.Text = $"{param.Weight}%";

                    param = (LinearLayout.LayoutParams) ProfilePageStatsFragmentPlannedMangaBar.LayoutParameters;
                    param.Weight = ViewModel.CurrentData.MangaPlanned * 100 / allManga;
                    ProfilePageStatsFragmentPlannedMangaBar.LayoutParameters = param;
                    if (param.Weight > 7) ProfilePageStatsFragmentPlannedMangaBarLabel.Text = $"{param.Weight}%";


                    ////ANIME BOTTOM

                    WatchingAnimeCount.Text = ViewModel.CurrentData.AnimeWatching.ToString();
                    CompletedAnimeCount.Text = ViewModel.CurrentData.AnimeCompleted.ToString();
                    OnHoldAnimeCount.Text = ViewModel.CurrentData.AnimeOnHold.ToString();
                    DroppedAnimeCount.Text = ViewModel.CurrentData.AnimeDropped.ToString();
                    PlannedAnimeCount.Text = ViewModel.CurrentData.AnimePlanned.ToString();

                    TotalAnimeCount.Text = ViewModel.CurrentData.AnimeTotal.ToString();
                    RewatchedAnimeCount.Text = ViewModel.CurrentData.AnimeRewatched.ToString();
                    EpisodesAnimeCount.Text = ViewModel.CurrentData.AnimeEpisodes.ToString();

                    AnimeDaysLabel.Text = $"Days: {ViewModel.CurrentData.AnimeDays:N1}";
                    AnimeMeanLabel.Text = $"Mean: {ViewModel.CurrentData.AnimeMean:N2}";

                    ////MANGA BOTTOM

                    WatchingMangaCount.Text = ViewModel.CurrentData.MangaReading.ToString();
                    CompletedMangaCount.Text = ViewModel.CurrentData.MangaCompleted.ToString();
                    OnHoldMangaCount.Text = ViewModel.CurrentData.MangaOnHold.ToString();
                    DroppedMangaCount.Text = ViewModel.CurrentData.MangaDropped.ToString();
                    PlannedMangaCount.Text = ViewModel.CurrentData.MangaPlanned.ToString();

                    TotalMangaCount.Text = ViewModel.CurrentData.MangaTotal.ToString();
                    RereadMangaCount.Text = ViewModel.CurrentData.MangaReread.ToString();
                    ChaptersMangaCount.Text = ViewModel.CurrentData.MangaChapters.ToString();
                    VolumesMangaCount.Text = ViewModel.CurrentData.MangaVolumes.ToString();

                    MangaDaysLabel.Text = $"Days: {ViewModel.CurrentData.MangaDays:N1}";
                    MangaMeanLabel.Text = $"Mean: {ViewModel.CurrentData.MangaMean:N2}";

                    StatsApproxTimeSpentAnime.Text = ViewModel.ApproxTimeSpentOnAnime;
                    StatsApproxTimeSpentMovies.Text = ViewModel.ApproxTimeSpentOnMovies;
                    StatsApproxTimeSpentBoth.Text = ViewModel.ApproxTimeSpentOnAnimeAndMovies;

                }));
        }

        public override int LayoutResourceId => Resource.Layout.ProfilePageStatsTab;

        #region Views

        private TextView _profilePageStatsFragmentWatchingAnimeBarLabel;
        private FrameLayout _profilePageStatsFragmentWatchingAnimeBar;
        private TextView _profilePageStatsFragmentCompletedAnimeBarLabel;
        private FrameLayout _profilePageStatsFragmentCompletedAnimeBar;
        private TextView _profilePageStatsFragmentOnHoldAnimeBarLabel;
        private FrameLayout _profilePageStatsFragmentOnHoldAnimeBar;
        private TextView _profilePageStatsFragmentDroppedAnimeBarLabel;
        private FrameLayout _profilePageStatsFragmentDroppedAnimeBar;
        private TextView _profilePageStatsFragmentPlannedAnimeBarLabel;
        private FrameLayout _profilePageStatsFragmentPlannedAnimeBar;
        private TextView _watchingAnimeCount;
        private TextView _completedAnimeCount;
        private TextView _onHoldAnimeCount;
        private TextView _droppedAnimeCount;
        private TextView _plannedAnimeCount;
        private TextView _totalAnimeCount;
        private TextView _rewatchedAnimeCount;
        private TextView _episodesAnimeCount;
        private TextView _animeDaysLabel;
        private TextView _animeMeanLabel;
        private TextView _profilePageStatsFragmentWatchingMangaBarLabel;
        private FrameLayout _profilePageStatsFragmentWatchingMangaBar;
        private TextView _profilePageStatsFragmentCompletedMangaBarLabel;
        private FrameLayout _profilePageStatsFragmentCompletedMangaBar;
        private TextView _profilePageStatsFragmentOnHoldMangaBarLabel;
        private FrameLayout _profilePageStatsFragmentOnHoldMangaBar;
        private TextView _profilePageStatsFragmentDroppedMangaBarLabel;
        private FrameLayout _profilePageStatsFragmentDroppedMangaBar;
        private TextView _profilePageStatsFragmentPlannedMangaBarLabel;
        private FrameLayout _profilePageStatsFragmentPlannedMangaBar;
        private TextView _watchingMangaCount;
        private TextView _completedMangaCount;
        private TextView _onHoldMangaCount;
        private TextView _droppedMangaCount;
        private TextView _plannedMangaCount;
        private TextView _totalMangaCount;
        private TextView _rereadMangaCount;
        private TextView _chaptersMangaCount;
        private TextView _volumesMangaCount;
        private TextView _mangaDaysLabel;
        private TextView _mangaMeanLabel;
        private TextView _statsApproxTimeSpentAnime;
        private TextView _statsApproxTimeSpentMovies;
        private TextView _statsApproxTimeSpentBoth;

        public TextView ProfilePageStatsFragmentWatchingAnimeBarLabel => _profilePageStatsFragmentWatchingAnimeBarLabel ?? (_profilePageStatsFragmentWatchingAnimeBarLabel = FindViewById<TextView>(Resource.Id.ProfilePageStatsFragmentWatchingAnimeBarLabel));

        public FrameLayout ProfilePageStatsFragmentWatchingAnimeBar => _profilePageStatsFragmentWatchingAnimeBar ?? (_profilePageStatsFragmentWatchingAnimeBar = FindViewById<FrameLayout>(Resource.Id.ProfilePageStatsFragmentWatchingAnimeBar));

        public TextView ProfilePageStatsFragmentCompletedAnimeBarLabel => _profilePageStatsFragmentCompletedAnimeBarLabel ?? (_profilePageStatsFragmentCompletedAnimeBarLabel = FindViewById<TextView>(Resource.Id.ProfilePageStatsFragmentCompletedAnimeBarLabel));

        public FrameLayout ProfilePageStatsFragmentCompletedAnimeBar => _profilePageStatsFragmentCompletedAnimeBar ?? (_profilePageStatsFragmentCompletedAnimeBar = FindViewById<FrameLayout>(Resource.Id.ProfilePageStatsFragmentCompletedAnimeBar));

        public TextView ProfilePageStatsFragmentOnHoldAnimeBarLabel => _profilePageStatsFragmentOnHoldAnimeBarLabel ?? (_profilePageStatsFragmentOnHoldAnimeBarLabel = FindViewById<TextView>(Resource.Id.ProfilePageStatsFragmentOnHoldAnimeBarLabel));

        public FrameLayout ProfilePageStatsFragmentOnHoldAnimeBar => _profilePageStatsFragmentOnHoldAnimeBar ?? (_profilePageStatsFragmentOnHoldAnimeBar = FindViewById<FrameLayout>(Resource.Id.ProfilePageStatsFragmentOnHoldAnimeBar));

        public TextView ProfilePageStatsFragmentDroppedAnimeBarLabel => _profilePageStatsFragmentDroppedAnimeBarLabel ?? (_profilePageStatsFragmentDroppedAnimeBarLabel = FindViewById<TextView>(Resource.Id.ProfilePageStatsFragmentDroppedAnimeBarLabel));

        public FrameLayout ProfilePageStatsFragmentDroppedAnimeBar => _profilePageStatsFragmentDroppedAnimeBar ?? (_profilePageStatsFragmentDroppedAnimeBar = FindViewById<FrameLayout>(Resource.Id.ProfilePageStatsFragmentDroppedAnimeBar));

        public TextView ProfilePageStatsFragmentPlannedAnimeBarLabel => _profilePageStatsFragmentPlannedAnimeBarLabel ?? (_profilePageStatsFragmentPlannedAnimeBarLabel = FindViewById<TextView>(Resource.Id.ProfilePageStatsFragmentPlannedAnimeBarLabel));

        public FrameLayout ProfilePageStatsFragmentPlannedAnimeBar => _profilePageStatsFragmentPlannedAnimeBar ?? (_profilePageStatsFragmentPlannedAnimeBar = FindViewById<FrameLayout>(Resource.Id.ProfilePageStatsFragmentPlannedAnimeBar));

        public TextView WatchingAnimeCount => _watchingAnimeCount ?? (_watchingAnimeCount = FindViewById<TextView>(Resource.Id.WatchingAnimeCount));

        public TextView CompletedAnimeCount => _completedAnimeCount ?? (_completedAnimeCount = FindViewById<TextView>(Resource.Id.CompletedAnimeCount));

        public TextView OnHoldAnimeCount => _onHoldAnimeCount ?? (_onHoldAnimeCount = FindViewById<TextView>(Resource.Id.OnHoldAnimeCount));

        public TextView DroppedAnimeCount => _droppedAnimeCount ?? (_droppedAnimeCount = FindViewById<TextView>(Resource.Id.DroppedAnimeCount));

        public TextView PlannedAnimeCount => _plannedAnimeCount ?? (_plannedAnimeCount = FindViewById<TextView>(Resource.Id.PlannedAnimeCount));

        public TextView TotalAnimeCount => _totalAnimeCount ?? (_totalAnimeCount = FindViewById<TextView>(Resource.Id.TotalAnimeCount));

        public TextView RewatchedAnimeCount => _rewatchedAnimeCount ?? (_rewatchedAnimeCount = FindViewById<TextView>(Resource.Id.RewatchedAnimeCount));

        public TextView EpisodesAnimeCount => _episodesAnimeCount ?? (_episodesAnimeCount = FindViewById<TextView>(Resource.Id.EpisodesAnimeCount));

        public TextView AnimeDaysLabel => _animeDaysLabel ?? (_animeDaysLabel = FindViewById<TextView>(Resource.Id.AnimeDaysLabel));

        public TextView AnimeMeanLabel => _animeMeanLabel ?? (_animeMeanLabel = FindViewById<TextView>(Resource.Id.AnimeMeanLabel));

        public TextView ProfilePageStatsFragmentWatchingMangaBarLabel => _profilePageStatsFragmentWatchingMangaBarLabel ?? (_profilePageStatsFragmentWatchingMangaBarLabel = FindViewById<TextView>(Resource.Id.ProfilePageStatsFragmentWatchingMangaBarLabel));

        public FrameLayout ProfilePageStatsFragmentWatchingMangaBar => _profilePageStatsFragmentWatchingMangaBar ?? (_profilePageStatsFragmentWatchingMangaBar = FindViewById<FrameLayout>(Resource.Id.ProfilePageStatsFragmentWatchingMangaBar));

        public TextView ProfilePageStatsFragmentCompletedMangaBarLabel => _profilePageStatsFragmentCompletedMangaBarLabel ?? (_profilePageStatsFragmentCompletedMangaBarLabel = FindViewById<TextView>(Resource.Id.ProfilePageStatsFragmentCompletedMangaBarLabel));

        public FrameLayout ProfilePageStatsFragmentCompletedMangaBar => _profilePageStatsFragmentCompletedMangaBar ?? (_profilePageStatsFragmentCompletedMangaBar = FindViewById<FrameLayout>(Resource.Id.ProfilePageStatsFragmentCompletedMangaBar));

        public TextView ProfilePageStatsFragmentOnHoldMangaBarLabel => _profilePageStatsFragmentOnHoldMangaBarLabel ?? (_profilePageStatsFragmentOnHoldMangaBarLabel = FindViewById<TextView>(Resource.Id.ProfilePageStatsFragmentOnHoldMangaBarLabel));

        public FrameLayout ProfilePageStatsFragmentOnHoldMangaBar => _profilePageStatsFragmentOnHoldMangaBar ?? (_profilePageStatsFragmentOnHoldMangaBar = FindViewById<FrameLayout>(Resource.Id.ProfilePageStatsFragmentOnHoldMangaBar));

        public TextView ProfilePageStatsFragmentDroppedMangaBarLabel => _profilePageStatsFragmentDroppedMangaBarLabel ?? (_profilePageStatsFragmentDroppedMangaBarLabel = FindViewById<TextView>(Resource.Id.ProfilePageStatsFragmentDroppedMangaBarLabel));

        public FrameLayout ProfilePageStatsFragmentDroppedMangaBar => _profilePageStatsFragmentDroppedMangaBar ?? (_profilePageStatsFragmentDroppedMangaBar = FindViewById<FrameLayout>(Resource.Id.ProfilePageStatsFragmentDroppedMangaBar));

        public TextView ProfilePageStatsFragmentPlannedMangaBarLabel => _profilePageStatsFragmentPlannedMangaBarLabel ?? (_profilePageStatsFragmentPlannedMangaBarLabel = FindViewById<TextView>(Resource.Id.ProfilePageStatsFragmentPlannedMangaBarLabel));

        public FrameLayout ProfilePageStatsFragmentPlannedMangaBar => _profilePageStatsFragmentPlannedMangaBar ?? (_profilePageStatsFragmentPlannedMangaBar = FindViewById<FrameLayout>(Resource.Id.ProfilePageStatsFragmentPlannedMangaBar));

        public TextView WatchingMangaCount => _watchingMangaCount ?? (_watchingMangaCount = FindViewById<TextView>(Resource.Id.WatchingMangaCount));

        public TextView CompletedMangaCount => _completedMangaCount ?? (_completedMangaCount = FindViewById<TextView>(Resource.Id.CompletedMangaCount));

        public TextView OnHoldMangaCount => _onHoldMangaCount ?? (_onHoldMangaCount = FindViewById<TextView>(Resource.Id.OnHoldMangaCount));

        public TextView DroppedMangaCount => _droppedMangaCount ?? (_droppedMangaCount = FindViewById<TextView>(Resource.Id.DroppedMangaCount));

        public TextView PlannedMangaCount => _plannedMangaCount ?? (_plannedMangaCount = FindViewById<TextView>(Resource.Id.PlannedMangaCount));

        public TextView TotalMangaCount => _totalMangaCount ?? (_totalMangaCount = FindViewById<TextView>(Resource.Id.TotalMangaCount));

        public TextView RereadMangaCount => _rereadMangaCount ?? (_rereadMangaCount = FindViewById<TextView>(Resource.Id.RereadMangaCount));

        public TextView ChaptersMangaCount => _chaptersMangaCount ?? (_chaptersMangaCount = FindViewById<TextView>(Resource.Id.ChaptersMangaCount));

        public TextView VolumesMangaCount => _volumesMangaCount ?? (_volumesMangaCount = FindViewById<TextView>(Resource.Id.VolumesMangaCount));

        public TextView MangaDaysLabel => _mangaDaysLabel ?? (_mangaDaysLabel = FindViewById<TextView>(Resource.Id.MangaDaysLabel));

        public TextView MangaMeanLabel => _mangaMeanLabel ?? (_mangaMeanLabel = FindViewById<TextView>(Resource.Id.MangaMeanLabel));

        public TextView StatsApproxTimeSpentAnime => _statsApproxTimeSpentAnime ?? (_statsApproxTimeSpentAnime = FindViewById<TextView>(Resource.Id.StatsApproxTimeSpentAnime));

        public TextView StatsApproxTimeSpentMovies => _statsApproxTimeSpentMovies ?? (_statsApproxTimeSpentMovies = FindViewById<TextView>(Resource.Id.StatsApproxTimeSpentMovies));

        public TextView StatsApproxTimeSpentBoth => _statsApproxTimeSpentBoth ?? (_statsApproxTimeSpentBoth = FindViewById<TextView>(Resource.Id.StatsApproxTimeSpentBoth));

        #endregion
    }
}