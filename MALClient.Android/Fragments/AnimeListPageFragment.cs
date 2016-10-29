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
using MALClient.Android.CollectionAdapters;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public partial class AnimeListPageFragment
    {
        private static AnimeListPageNavigationArgs _prevArgs;

        public override int LayoutResourceId => Resource.Layout.AnimeListPage;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.AnimeList.Init(_prevArgs);         
        }


        public static AnimeListPageFragment BuildInstance(object args)
        {
            _prevArgs = args as AnimeListPageNavigationArgs;
            return new AnimeListPageFragment();
        }
    }
}