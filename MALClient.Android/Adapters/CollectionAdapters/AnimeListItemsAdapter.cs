using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using MALClient.Android.Activities;
using MALClient.Android.CollectionAdapters;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Adapters.CollectionAdapters
{
    public class AnimeListItemsAdapter : DeeplyObservableCollectionAdapter<AnimeItemViewModel>
    {
        private readonly Func<AnimeItemViewModel, View, BindingInfo<AnimeItemViewModel>> _factory;

        public AnimeListItemsAdapter(Activity context, int layoutResource,
            IList<AnimeItemViewModel> items, Func<AnimeItemViewModel,View, BindingInfo<AnimeItemViewModel>> factory)
            : base(context, layoutResource, items)
        {
            _factory = factory;
        }

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
                Bindings.Add(item.Id,_factory.Invoke(item,view));
            else
                Bindings[item.Id].Container = view;
        }

  

    }
}