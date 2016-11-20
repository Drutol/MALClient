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
using MALClient.Android.Adapters.CollectionAdapters;
using MALClient.Android.CollectionAdapters;

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

        public override int LayoutResourceId => Resource.Layout.AnimeListPage;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.AnimeList.Init(null);         
        }

        private async void AnimeListPageGridViewOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            await Task.Delay(100); //let's behold this ripple effect
            var adapter = AnimeListPageGridView.Adapter as AnimeListItemsAdapter;
            adapter[itemClickEventArgs.Position].NavigateDetailsCommand.Execute(null);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            var width = newConfig.ScreenWidthDp/DimensionsHelper.PxToDp(2.1f);
            width = width > 200 ? 200 : width;
            AnimeListPageGridView.SetColumnWidth((int)width);
            base.OnConfigurationChanged(newConfig);
        }

        public static AnimeListPageFragment BuildInstance(object args)
        {
            _prevArgs = args as AnimeListPageNavigationArgs;
            return new AnimeListPageFragment();
        }
    }
}