using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Android.App;
using Android.Views;
using Android.Widget;
using MALClient.Android.BindingInformation;

namespace MALClient.Android.CollectionAdapters
{
    public abstract class ObservableCollectionAdapter<T> : BaseAdapter<T>
    {
        protected readonly ObservableCollection<T> Items;
        private readonly int _resource;

        public ObservableCollectionAdapter(Activity context, int resource, ObservableCollection<T> items)
        {
            this.Context = context;
            this._resource = resource;
            this.Items = items;
            this.Items.CollectionChanged += this.OnCollectionChanged;
            NotifyDataSetChanged();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyDataSetChanged();
        }

        private void OnItemChanged(object sender, EventArgs e)
        {
            this.NotifyDataSetChanged();
        }

        public override T this[int position] => this.Items[position];

        private Activity Context { get; set; }

        public override int Count => this.Items.Count;

        public override long GetItemId(int position)
        {
            return this.GetItemId(this.Items[position], position);
        }

        private readonly Dictionary<View, T> _initializedViews = new Dictionary<View, T>();

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView != null)
            {
                T oldItem;
                if (this._initializedViews.TryGetValue(convertView, out oldItem))
                {
                    var oldObservable = oldItem as INotifyPropertyChanged;
                    if (oldObservable != null)
                    {
                        oldObservable.PropertyChanged -= this.OnItemChanged;
                        DetachOldView(this[position]);
                    }
                }
            }

            View view = convertView;
            if (view == null)
            {
                view = this.Context.LayoutInflater.Inflate(_resource, null);
                this.InitializeNewView(view);
            }

            T item = this[position];
            this._initializedViews[view] = item;
            this.PrepareView(item, view);

            var observable = item as INotifyPropertyChanged;
            if (observable != null)
            {
                observable.PropertyChanged += this.OnItemChanged;
            }

            return view;
        }

        protected virtual void InitializeNewView(View view)
        {
        }

        protected abstract void DetachOldView(T viewModel);

        protected readonly Dictionary<int, BindingInfo<T>> Bindings =
            new Dictionary<int, BindingInfo<T>> ();

        protected abstract void PrepareView(T item, View view);

        protected abstract long GetItemId(T item, int position);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Items.CollectionChanged -= this.OnCollectionChanged;
                foreach (var bindingInfo in Bindings)
                    bindingInfo.Value.Detach();
                
            }

            base.Dispose(disposing);
        }
    }
}