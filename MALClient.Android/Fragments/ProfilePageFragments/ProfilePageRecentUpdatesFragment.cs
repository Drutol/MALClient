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
using MALClient.Android.BindingConverters;
using MALClient.Android.BindingInformation;
using MALClient.Android.CollectionAdapters;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.ProfilePageFragments
{
    public class ProfilePageRecentUpdatesFragment : MalFragmentBase
    {
        private readonly ProfilePageViewModel ViewModel;

        public ProfilePageRecentUpdatesFragment() : base(false, false)
        {
            ViewModel = ViewModelLocator.ProfilePage;
        }

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            
            Bindings.Add(
                this.SetBinding(() => ViewModel.EmptyRecentAnimeNoticeVisibility,
                        () => ProfilePageRecentUpdatesTabAnimeListEmptyNotice.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            
            Bindings.Add(
                this.SetBinding(() => ViewModel.EmptyRecentMangaNoticeVisibility,
                    () => ProfilePageRecentUpdatesTabMangaListEmptyNotice.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            
            Bindings.Add(
                this.SetBinding(() => ViewModel.RecentAnime).WhenSourceChanges(() =>
                {
                    ProfilePageRecentUpdatesTabAnimeList.SetAnimeListAdapter(Context, ViewModel.RecentAnime,AnimeListDisplayModes.IndefiniteCompactList,OnItemClickAction);
                }));

            
            Bindings.Add(
                this.SetBinding(() => ViewModel.RecentManga).WhenSourceChanges(() =>
                {
                    ProfilePageRecentUpdatesTabAnimeList.SetAnimeListAdapter(Context, ViewModel.RecentManga, AnimeListDisplayModes.IndefiniteCompactList, OnItemClickAction);
                }));
        }

        private void OnItemClickAction(AnimeItemViewModel animeItemViewModel)
        {
            ViewModel.TemporarilySelectedAnimeItem = animeItemViewModel;
        }

        public override int LayoutResourceId => Resource.Layout.ProfilePageRecentUpdatesTab;

        #region Views

        private LinearLayout _profilePageRecentUpdatesTabAnimeList;
        private RelativeLayout _profilePageRecentUpdatesTabAnimeListEmptyNotice;
        private RelativeLayout _profilePageRecentUpdatesTabMangaListEmptyNotice;
        private LinearLayout _profilePageRecentUpdatesTabMangaList;

        public LinearLayout ProfilePageRecentUpdatesTabAnimeList => _profilePageRecentUpdatesTabAnimeList ?? (_profilePageRecentUpdatesTabAnimeList = FindViewById<LinearLayout>(Resource.Id.ProfilePageRecentUpdatesTabAnimeList));

        public RelativeLayout ProfilePageRecentUpdatesTabAnimeListEmptyNotice => _profilePageRecentUpdatesTabAnimeListEmptyNotice ?? (_profilePageRecentUpdatesTabAnimeListEmptyNotice = FindViewById<RelativeLayout>(Resource.Id.ProfilePageRecentUpdatesTabAnimeListEmptyNotice));

        public RelativeLayout ProfilePageRecentUpdatesTabMangaListEmptyNotice => _profilePageRecentUpdatesTabMangaListEmptyNotice ?? (_profilePageRecentUpdatesTabMangaListEmptyNotice = FindViewById<RelativeLayout>(Resource.Id.ProfilePageRecentUpdatesTabMangaListEmptyNotice));

        public LinearLayout ProfilePageRecentUpdatesTabMangaList => _profilePageRecentUpdatesTabMangaList ?? (_profilePageRecentUpdatesTabMangaList = FindViewById<LinearLayout>(Resource.Id.ProfilePageRecentUpdatesTabMangaList));


        #endregion
    }
}