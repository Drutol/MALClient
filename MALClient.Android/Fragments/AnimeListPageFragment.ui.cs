using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.CollectionAdapters;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments
{
    public partial class AnimeListPageFragment : MalFragmentBase
    {
        protected override void InitBindings()
        {
            ViewModelLocator.AnimeList.PropertyChanged+= AnimeListOnPropertyChanged;
        }

        private async void AnimeListOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.AnimeList.AnimeGridItems))
            {
                if(ViewModelLocator.AnimeList.AnimeGridItems != null)
                    AnimeListPageGridView.Adapter = new AnimeListItemsAdapter(Context as Activity, Resource.Layout.AnimeGridItem, ViewModelLocator.AnimeList.AnimeGridItems);
            }
        }


        private GridView _animeListPageGridView;

        public GridView AnimeListPageGridView => _animeListPageGridView ?? (_animeListPageGridView = FindViewById<GridView>(Resource.Id.AnimeListPageGridView));

    }
}