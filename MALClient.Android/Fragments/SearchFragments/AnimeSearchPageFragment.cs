using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.SearchFragments
{
    public class AnimeSearchPageFragment : MalFragmentBase
    {
        public bool _waitForRootView;

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
        }

        public void NavigatedTo()
        {
            if (RootView == null)
            {
                _waitForRootView = true;
                return;
            }
            AnimeSearchPageList.InjectFlingAdapter(ViewModel.AnimeSearchItemViewModels, DataTemplateFull, DataTemplateFling, ContainerTemplate, DataTemplateBasic);
            AnimeSearchPageList.ItemClick += AnimeSearchPageListOnItemClick;

            Bindings.Add(
                this.SetBinding(() => ViewModel.EmptyNoticeVisibility,
                    () => AnimeSearchPageEmptyNotice.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => AnimeSearchPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(
                this.SetBinding(() => ViewModel.IsFirstVisitGridVisible,
                        () => AnimeSearchPageFirstSearchSection.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        private void DataTemplateBasic(View view, int i, AnimeSearchItemViewModel animeSearchItemViewModel)
        {
            view.FindViewById<TextView>(Resource.Id.AnimeSearchItemTitle).Text = animeSearchItemViewModel.Title;
            view.FindViewById<TextView>(Resource.Id.AnimeSearchItemType).Text = animeSearchItemViewModel.Type;
            view.FindViewById<TextView>(Resource.Id.AnimeSearchItemDescription).Text = animeSearchItemViewModel.Synopsis;
            view.FindViewById<TextView>(Resource.Id.AnimeSearchItemEpisodes).Text = animeSearchItemViewModel.WatchedEps;
            view.FindViewById<TextView>(Resource.Id.AnimeSearchItemGlobalScore).Text = animeSearchItemViewModel.GlobalScoreBind;

            if (animeSearchItemViewModel.IsAuth)
            {
                view.FindViewById(Resource.Id.TopRightInfo).Visibility = ViewStates.Visible;
                view.FindViewById<TextView>(Resource.Id.WatchingStatus).Text = animeSearchItemViewModel.MyStatusBindShort;
                view.FindViewById<TextView>(Resource.Id.WatchedEpisodes).Text = animeSearchItemViewModel.MyEpisodesBindShort;
            }
            else
            {
                view.FindViewById(Resource.Id.TopRightInfo).Visibility = ViewStates.Gone;
            }
        }

        private View ContainerTemplate(int i)
        {
            var view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeSearchItem, null);
            view.Click += ViewOnClick;
            return view;
        }

        private void DataTemplateFling(View view, int i, AnimeSearchItemViewModel animeSearchItemViewModel)
        {
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeSearchItemImage);
            if (img.IntoIfLoaded(animeSearchItemViewModel.ImgUrl))
            {
                img.Visibility = ViewStates.Visible;
                view.FindViewById(Resource.Id.AnimeSearchItemImgPlaceholder).Visibility = ViewStates.Gone;
            }
            else
            {
                img.Visibility = ViewStates.Invisible;
                view.FindViewById(Resource.Id.AnimeSearchItemImgPlaceholder).Visibility = ViewStates.Visible;
            }       
        }

        private void DataTemplateFull(View view, int i, AnimeSearchItemViewModel animeSearchItemViewModel)
        {
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeSearchItemImage);
            if (img.Tag == null || (string) img.Tag != animeSearchItemViewModel.ImgUrl)
            {
                img.Into(animeSearchItemViewModel.ImgUrl);
            }
            else
            {
                img.Visibility = ViewStates.Visible;
            }          
        }

        private void AnimeSearchPageListOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            var item = itemClickEventArgs.View.Tag.Unwrap<AnimeSearchItemViewModel>();
            item.NavigateDetails();
        }

        private void ViewOnClick(object sender, EventArgs eventArgs)
        {
            (sender as View).Tag.Unwrap<AnimeSearchItemViewModel>().NavigateDetailsCommand.Execute(null);
        }

        public override int LayoutResourceId => Resource.Layout.AnimeSearchPage;

        protected override void Cleanup()
        {
            if(RootView != null)
                AnimeSearchPageList?.ClearFlingAdapter();
            base.Cleanup();
        }

        #region Views

        private ListView _animeSearchPageList;
        private TextView _animeSearchPageEmptyNotice;
        private LinearLayout _animeSearchPageFirstSearchSection;
        private ProgressBar _animeSearchPageLoadingSpinner;

        public ListView AnimeSearchPageList => _animeSearchPageList ?? (_animeSearchPageList = FindViewById<ListView>(Resource.Id.AnimeSearchPageList));

        public TextView AnimeSearchPageEmptyNotice => _animeSearchPageEmptyNotice ?? (_animeSearchPageEmptyNotice = FindViewById<TextView>(Resource.Id.AnimeSearchPageEmptyNotice));

        public LinearLayout AnimeSearchPageFirstSearchSection => _animeSearchPageFirstSearchSection ?? (_animeSearchPageFirstSearchSection = FindViewById<LinearLayout>(Resource.Id.AnimeSearchPageFirstSearchSection));

        public ProgressBar AnimeSearchPageLoadingSpinner => _animeSearchPageLoadingSpinner ?? (_animeSearchPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeSearchPageLoadingSpinner));

        #endregion
    }
}