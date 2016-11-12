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
using GalaSoft.MvvmLight.Helpers;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    public class AnimeDetailsPageGeneralTabFragment : MalFragmentBase
    {
        private AnimeDetailsPageViewModel ViewModel;

        private AnimeDetailsPageGeneralTabFragment()
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            //adapting to UWp approach where this a list, I don't dare to do this on android ;d
            ViewModel.BasicDataLoaded += ViewModelOnBasicDataLoaded;
        }

        private void ViewModelOnBasicDataLoaded()
        {
            //left details
            AnimeDetailsPageGeneralTabFragmentType.Text = ViewModel.RightDetailsRow[0].Item2;
            AnimeDetailsPageGeneralTabFragmentStatus.Text = ViewModel.RightDetailsRow[1].Item2;
            AnimeDetailsPageGeneralTabFragmentEnd.Text = ViewModel.RightDetailsRow[2].Item2;
            //right details
            AnimeDetailsPageGeneralTabFragmentEpisodes.Text = ViewModel.LeftDetailsRow[0].Item2;
            AnimeDetailsPageGeneralTabFragmentScore.Text = ViewModel.LeftDetailsRow[1].Item2;
            AnimeDetailsPageGeneralTabFragmentStart.Text = ViewModel.LeftDetailsRow[2].Item2;
            //rest
            AnimeDetailsPageGeneralTabFragmentSynopsis.Text = ViewModel.Synopsis;
        }

        protected override void InitBindings()
        {

        }

        protected override void Cleanup()
        {
            ViewModel.BasicDataLoaded -= ViewModelOnBasicDataLoaded;
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageGeneralTab;

        public static AnimeDetailsPageGeneralTabFragment Instance => new AnimeDetailsPageGeneralTabFragment();

        #region Views

        private TextView _animeDetailsPageGeneralTabFragmentEpisodes;
        private TextView _animeDetailsPageGeneralTabFragmentScore;
        private TextView _animeDetailsPageGeneralTabFragmentStart;
        private TextView _animeDetailsPageGeneralTabFragmentMyStart;
        private TextView _animeDetailsPageGeneralTabFragmentType;
        private TextView _animeDetailsPageGeneralTabFragmentStatus;
        private TextView _animeDetailsPageGeneralTabFragmentEnd;
        private TextView _animeDetailsPageGeneralTabFragmentMyEnd;
        private TextView _animeDetailsPageGeneralTabFragmentSynopsis;

        public TextView AnimeDetailsPageGeneralTabFragmentEpisodes => _animeDetailsPageGeneralTabFragmentEpisodes ?? (_animeDetailsPageGeneralTabFragmentEpisodes = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentEpisodes));

        public TextView AnimeDetailsPageGeneralTabFragmentScore => _animeDetailsPageGeneralTabFragmentScore ?? (_animeDetailsPageGeneralTabFragmentScore = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentScore));

        public TextView AnimeDetailsPageGeneralTabFragmentStart => _animeDetailsPageGeneralTabFragmentStart ?? (_animeDetailsPageGeneralTabFragmentStart = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentStart));

        public TextView AnimeDetailsPageGeneralTabFragmentMyStart => _animeDetailsPageGeneralTabFragmentMyStart ?? (_animeDetailsPageGeneralTabFragmentMyStart = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentMyStart));

        public TextView AnimeDetailsPageGeneralTabFragmentType => _animeDetailsPageGeneralTabFragmentType ?? (_animeDetailsPageGeneralTabFragmentType = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentType));

        public TextView AnimeDetailsPageGeneralTabFragmentStatus => _animeDetailsPageGeneralTabFragmentStatus ?? (_animeDetailsPageGeneralTabFragmentStatus = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentStatus));

        public TextView AnimeDetailsPageGeneralTabFragmentEnd => _animeDetailsPageGeneralTabFragmentEnd ?? (_animeDetailsPageGeneralTabFragmentEnd = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentEnd));

        public TextView AnimeDetailsPageGeneralTabFragmentMyEnd => _animeDetailsPageGeneralTabFragmentMyEnd ?? (_animeDetailsPageGeneralTabFragmentMyEnd = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentMyEnd));

        public TextView AnimeDetailsPageGeneralTabFragmentSynopsis => _animeDetailsPageGeneralTabFragmentSynopsis ?? (_animeDetailsPageGeneralTabFragmentSynopsis = FindViewById<TextView>(Resource.Id.AnimeDetailsPageGeneralTabFragmentSynopsis));



        #endregion
    }
}