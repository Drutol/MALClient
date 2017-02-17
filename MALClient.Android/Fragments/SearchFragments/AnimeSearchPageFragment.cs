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
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.SearchFragments
{
    public class AnimeSearchPageFragment : MalFragmentBase
    {
        private AnimeSearchPageFragment(bool initBindings) : base(initBindings)
        {

        }

        private SearchPageViewModel ViewModel;
        private SearchPageNavigationArgs _prevArgs;


        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.SearchPage;
        }

        protected override void InitBindings()
        {


            AnimeSearchPageList.Adapter = ViewModel.AnimeSearchItemViewModels.GetAdapter(GetTemplateDelegate);
            AnimeSearchPageList.ItemClick += AnimeSearchPageListOnItemClick;

                        Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => AnimeSearchPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        private void AnimeSearchPageListOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            var item = itemClickEventArgs.View.Tag.Unwrap<AnimeSearchItemViewModel>();
            item.NavigateDetails();
        }

        private View GetTemplateDelegate(int i, AnimeSearchItemViewModel animeSearchItemViewModel, View convertView)
        {

            var view = convertView;
            if (view == null)
            {
                view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeSearchItem,null);
                view.Click += ViewOnClick;
            }

            view.FindViewById<TextView>(Resource.Id.AnimeSearchItemTitle).Text = animeSearchItemViewModel.Title;
            view.FindViewById<TextView>(Resource.Id.AnimeSearchItemType).Text = animeSearchItemViewModel.Type;
            view.FindViewById<TextView>(Resource.Id.AnimeSearchItemDescription).Text = animeSearchItemViewModel.Synopsis;
            view.FindViewById<TextView>(Resource.Id.AnimeSearchItemEpisodes).Text = animeSearchItemViewModel.WatchedEps;
            view.FindViewById<TextView>(Resource.Id.AnimeSearchItemGlobalScore).Text = animeSearchItemViewModel.GlobalScoreBind;
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeSearchItemImage);
            ImageService.Instance.LoadUrl(animeSearchItemViewModel.ImgUrl).Into(img);
            view.Tag = animeSearchItemViewModel.Wrap();

            return view;
        }

        private void ViewOnClick(object sender, EventArgs eventArgs)
        {
            (sender as View).Tag.Unwrap<AnimeSearchItemViewModel>().NavigateDetailsCommand.Execute(null);
        }

        public override int LayoutResourceId => Resource.Layout.AnimeSearchPage;


        #region Views

        private ListView _animeSearchPageList;
        private ProgressBar _animeSearchPageLoadingSpinner;


        public ListView AnimeSearchPageList => _animeSearchPageList ?? (_animeSearchPageList = FindViewById<ListView>(Resource.Id.AnimeSearchPageList));

        public ProgressBar AnimeSearchPageLoadingSpinner => _animeSearchPageLoadingSpinner ?? (_animeSearchPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeSearchPageLoadingSpinner));

        #endregion
        public static AnimeSearchPageFragment BuildInstance(bool initBindings = true)
        {
            return new AnimeSearchPageFragment(initBindings);
        }
    }
}