using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient
{
    public class SmartObservableCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> range)
        {
            // get out if no new items
            var enumerable = range as T[] ?? range.ToArray();
            if (range == null || !enumerable.Any()) return;

            // prepare data for firing the events
            int newStartingIndex = Count;
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
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, newStartingIndex));
        }

        private bool IsObserving { get { return _isObserving; } set { _isObserving = value; } }

        private volatile bool _isObserving;

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
