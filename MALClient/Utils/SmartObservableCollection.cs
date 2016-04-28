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
        public SmartObservableCollection()
            : base()
        {
        }

        public SmartObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public SmartObservableCollection(List<T> list)
            : base(list)
        {
        }

        public void AddRange(IEnumerable<T> range)
        {
            // get out if no new items
            if (range == null || !range.Any()) return;

            // prepare data for firing the events
            int newStartingIndex = Count;
            var newItems = new List<T>();
            newItems.AddRange(range);

            // add the items, making sure no events are fired
            IsObserving = false;
            foreach (var item in range)
            {
                Add(item);
            }
            IsObserving = true;

            // fire the events
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            // this is tricky: call Reset first to make sure the controls will respond properly and not only add one item
            // LOLLO NOTE I took out the following so the list viewers don't lose the position.
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
