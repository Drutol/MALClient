using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Android;
using MALClient.Android.Adapters.CollectionAdapters;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.ViewModels;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public partial class AnimeListPageFragment
    {
        private static AnimeListPageNavigationArgs _prevArgs;

        private AnimeListViewModel ViewModel => ViewModelLocator.AnimeList;
        private GridViewColumnHelper _gridViewColumnHelper;
        private AnimeListItemsAdapter _animeListItemsAdapter;


        public override int LayoutResourceId => Resource.Layout.AnimeListPage;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.AnimeList.Init(_prevArgs);         
        }

        private async void AnimeListPageGridViewOnItemClick(AnimeItemViewModel model)
        {
            await Task.Delay(75); //let's behold this ripple effect
            var args = ViewModelLocator.GeneralMain.GetCurrentListOrderParams();
            args.SelectedItemIndex = ViewModel.AnimeItems.IndexOf(model);
            model.NavigateDetails(null, args);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            _gridViewColumnHelper.OnConfigurationChanged(newConfig);
            if (AnimeListPageGridView.NumColumns == 2)
                _animeListItemsAdapter?.OnConfigurationChanged(newConfig);
            else
                _animeListItemsAdapter?.ResetConfiguration();
            base.OnConfigurationChanged(newConfig);
        }

        public static AnimeListPageFragment BuildInstance(object args)
        {
            _prevArgs = args as AnimeListPageNavigationArgs;
            return new AnimeListPageFragment();
        }
    }
}