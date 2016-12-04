using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using Android.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using MALClient.Android.BindingInformation;

namespace MALClient.Android.CollectionAdapters
{
    public abstract class DeeplyObservableCollectionAdapter<T> : BaseAdapter<T>
    {
        private IList<T> Items { get; }
        private int LayoutResource { get; }
        private Activity Context { get; }
        private Dictionary<View, T> InitializedViews { get; } = new Dictionary<View, T>();
        private bool isObservable;
        protected abstract void DetachOldView(T viewModel);

        protected abstract void PrepareView(T item, View view);

        protected abstract long GetItemId(T item);

        protected Dictionary<int, BindingInfo<T>> Bindings { get; } =
            new Dictionary<int, BindingInfo<T>>();
      
        public override T this[int position] => Items[position];

        public override int Count => this.Items.Count;

        protected DeeplyObservableCollectionAdapter(Activity context, int layoutResource, IList<T> items)
        {
            Context = context;
            LayoutResource = layoutResource;
            Items = items;
            var observable = items as ObservableCollection<T>;
            if (observable != null)
            {
                isObservable = true;
                observable.CollectionChanged += OnCollectionChanged;
            }
            NotifyDataSetChanged();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return GetItemId(Items[position]);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var viewModel = Items[position];
            var view = convertView;
            if (view == null)
            {
                view = Context.LayoutInflater.Inflate(LayoutResource, null);
                InitializedViews.Add(view,viewModel);
            }
            else
            {
                T item;
                if(InitializedViews.TryGetValue(convertView,out item))
                    DetachOldView(item);
            }

            PrepareView(viewModel, view);

            return view;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (isObservable)
                    (Items as ObservableCollection<T>).CollectionChanged -= OnCollectionChanged;
                foreach (var bindingInfo in Bindings)
                    bindingInfo.Value.Detach();
            }

            base.Dispose(disposing);
        }
    }
}