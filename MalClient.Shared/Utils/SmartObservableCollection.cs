using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MalClient.Shared.Utils
{
    public class SmartObservableCollection<T> : ObservableCollection<T>
    {
        private volatile bool _isObserving;

        private bool IsObserving
        {
            get { return _isObserving; }
            set { _isObserving = value; }
        }

        public SmartObservableCollection()
        {
            
        }

        public SmartObservableCollection(IEnumerable<T> source) : base(source)
        {
            
        }

        public void AddRange(IEnumerable<T> range)
        {
            // get out if no new items
            var enumerable = range as T[] ?? range.ToArray();
            if (range == null || !enumerable.Any()) return;

            // prepare data for firing the events
            var newStartingIndex = Count;
            var newItems = new List<T>();
            newItems.AddRange(enumerable);

            // add the items, making sure no events are fired
            IsObserving = false;
            foreach (var item in enumerable)
            {
                Add(item);
            }
            IsObserving = true;

            // fire the events
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(action: NotifyCollectionChangedAction.Reset));
            try
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems,
                    newStartingIndex));
            }
            catch (Exception) //it'll resolve some crashes hopefully
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (IsObserving) base.OnCollectionChanged(e);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (IsObserving) base.OnPropertyChanged(e);
        }
    }
}