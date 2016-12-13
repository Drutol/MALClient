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
        private readonly int _numOfColumns;
        private int _widthForCurrentParams;
        private ViewGroup.LayoutParams _itemParams;

        public AnimeListItemsAdapter(Activity context, int layoutResource,
            IList<AnimeItemViewModel> items, Func<AnimeItemViewModel,View, BindingInfo<AnimeItemViewModel>> factory,int numOfColumns)
            : base(context, layoutResource, items)
        {
            _factory = factory;
            _numOfColumns = numOfColumns;
        }

        public void OnConfigurationChanged(Configuration newConfiguration)
        {
            foreach (var bindingInfo in Bindings)
            {
                var param = bindingInfo.Value.Container.LayoutParameters;
                bool overflow = false;
                param.Width = GetItemLayoutParams(ref overflow,newConfiguration.ScreenWidthDp).Width;
                bindingInfo.Value.Container.LayoutParameters = param;
            }
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

            bool overflow = false;
            var param = GetItemLayoutParams(ref overflow);
            if (overflow || _numOfColumns > 2)
                view.LayoutParameters = param;

            if (!Bindings.ContainsKey(item.Id))
                Bindings.Add(item.Id,_factory.Invoke(item,view));
            else
                Bindings[item.Id].Container = view;
        }

        private ViewGroup.LayoutParams GetItemLayoutParams(ref bool overflow ,float width = -1)
        {
            width = width != -1 ? width : MainActivity.CurrentContext.Resources.Configuration.ScreenWidthDp;
            if (width == _widthForCurrentParams)
                return _itemParams;
            _widthForCurrentParams = (int)width;
            width /= 2.05f;
            if (width > 200)
            {
                overflow = true;
                width = 200;
            }
            _itemParams = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(width),ViewGroup.LayoutParams.WrapContent);
            return _itemParams;
        }

        public void ResetConfiguration()
        {
            foreach (var bindingInfo in Bindings)
            {
                var param = bindingInfo.Value.Container.LayoutParameters;
                param.Width = ViewGroup.LayoutParams.WrapContent;
                param.Height = ViewGroup.LayoutParams.WrapContent;
                bindingInfo.Value.Container.LayoutParameters = param;
            }
        }
    }
}