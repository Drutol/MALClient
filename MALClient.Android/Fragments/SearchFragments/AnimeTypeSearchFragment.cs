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
using MALClient.Android.Activities;
using MALClient.Models.Enums;
using MALClient.Models.Enums.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.SearchFragments
{
    public class AnimeTypeSearchFragment : MalFragmentBase
    {
        private SearchPageViewModel ViewModel;

        private AnimeTypeSearchFragment() : base(false)
        {
            
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.SearchPage;
        }


        protected override void InitBindings()
        {
            AnimeTypeSearchPageList.Adapter = ViewModel.AvailableSelectionChoices.GetAdapter(GetTemplateDelegate);
        }

        private View GetTemplateDelegate(int i, Enum parameter, View convertView)
        {
            var view = convertView;
            if (view == null)
            {
                view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeSearchTypeItem, null);
                view.Click += ViewOnClick;
            }

            view.FindViewById<TextView>(Resource.Id.AnimeSearchTypeItemTextView).Text = parameter.GetDescription();
            view.Tag = parameter.Wrap();

            return view;
        }

        private void ViewOnClick(object sender, EventArgs eventArgs)
        {
            var item = (sender as View).Tag.Unwrap<Enum>();
            if (item is AnimeGenres)
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList, new AnimeListPageNavigationArgs((AnimeGenres)item));
            else                                                                                                             
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList, new AnimeListPageNavigationArgs((AnimeStudios)item));
        }

        public override int LayoutResourceId => Resource.Layout.AnimeTypeSearchPage;

        #region Views

        private GridView _animeTypeSearchPageList;

        public GridView AnimeTypeSearchPageList => _animeTypeSearchPageList ?? (_animeTypeSearchPageList = FindViewById<GridView>(Resource.Id.AnimeTypeSearchPageList));
 
        #endregion

        public static AnimeTypeSearchFragment Instance => new AnimeTypeSearchFragment();
    }
}