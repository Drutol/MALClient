using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Adapters;
using MALClient.Android.BindingInformation;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.CollectionAdapters
{
    public class AnimeListItemsAdapter : DeeplyObservableCollectionAdapter<AnimeItemViewModel>
    {
        public AnimeListItemsAdapter(Activity context, int layoutResource, ObservableCollection<AnimeItemViewModel> items)
            : base(context, layoutResource, items) {}

        protected override long GetItemId(AnimeItemViewModel item) => item.Id;

        protected override void DetachOldView(AnimeItemViewModel viewModel)
        {
            if (Bindings.ContainsKey(viewModel.Id))
            {
                Bindings[viewModel.Id].Detach();
                Bindings.Remove(viewModel.Id);
            }
        }

        protected override void PrepareView(AnimeItemViewModel item, View view)
        {
            if (!Bindings.ContainsKey(item.Id))
                Bindings.Add(item.Id, new AnimeItemBindingInfo(view, item));
            else
                Bindings[item.Id].Container = view;
        }
    }
}