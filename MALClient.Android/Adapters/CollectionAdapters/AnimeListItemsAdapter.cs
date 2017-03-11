using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Android.App;
using Android.Views;
using MALClient.Android.BindingInformation;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.CollectionAdapters
{
    public class AnimeListItemsAdapter : DeeplyObservableCollectionAdapter<AnimeItemViewModel>
    {
        private readonly Func<AnimeItemViewModel, View,bool, BindingInfo<AnimeItemViewModel>> _factory;

        public List<BindingInfo<AnimeItemViewModel>> BindingInfos => Bindings.Select(pair => pair.Value).ToList();

        public AnimeListItemsAdapter(Activity context, int layoutResource,
            IList<AnimeItemViewModel> items, Func<AnimeItemViewModel,View,bool, BindingInfo<AnimeItemViewModel>> factory)
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

        protected override void PrepareView(AnimeItemViewModel item, View view, int position)
        {
            if (!Bindings.ContainsKey(item.Id))
            {
                var binding = _factory.Invoke(item, view, false);
                binding.Position = position;
                Bindings.Add(item.Id, binding);
            }
            else
            {
                Bindings[item.Id].Fling = false;
                Bindings[item.Id].Position = position;
                Bindings[item.Id].Container = view;
            }
        }

        protected override void PrepareViewQuickly(AnimeItemViewModel item, View view, int position)
        {
            if (!Bindings.ContainsKey(item.Id))
            {
                var binding = _factory.Invoke(item, view, true);
                binding.Position = position;
                Bindings.Add(item.Id, binding);
            }
            else
            {
                Bindings[item.Id].Fling = true;
                Bindings[item.Id].Position = position;
                Bindings[item.Id].Container = view;
                
            }
        }
    }
}