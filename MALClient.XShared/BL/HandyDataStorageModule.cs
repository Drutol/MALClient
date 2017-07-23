using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.XShared.Interfaces;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.XShared.BL
{
    public class HandyDataStorageModule<T> : IHandyDataStorageModule<T>
    {
        private readonly string _key;
        private readonly bool _atomicObjects;
        public ObservableCollection<T> StoredItems { get; private set; }
        public bool Batch { get; set; }

        public HandyDataStorageModule(string key,bool atomicObjects)
        {
            _key = key;
            _atomicObjects = atomicObjects;
            StoredItems = new ObservableCollection<T>();
            LoadData();

            StoredItems.CollectionChanged += StoredItemsOnCollectionChanged;
        }

        private void StoredItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (!_atomicObjects)
            {
                foreach (INotifyPropertyChanged newItem in notifyCollectionChangedEventArgs.NewItems)
                {
                    newItem.PropertyChanged += NotifyOnPropertyChanged;
                }
                foreach (INotifyPropertyChanged newItem in notifyCollectionChangedEventArgs.OldItems)
                {
                    newItem.PropertyChanged -= NotifyOnPropertyChanged;
                }
            }
            if(!Batch)
                SaveData();
        }

        public void LoadData()
        {
            var data = ResourceLocator.ApplicationDataService[_key] as string;
            if (data != null)
                StoredItems = new ObservableCollection<T>(JsonConvert.DeserializeObject<List<T>>(data));
            else
                StoredItems = new ObservableCollection<T>();

            StoredItems.CollectionChanged += StoredItemsOnCollectionChanged;

            if(_atomicObjects)
                return;
            StoredItems.ForEach(obj =>
            {
                var notify = (obj as INotifyPropertyChanged);
                notify.PropertyChanged += NotifyOnPropertyChanged;
            });
        }

        public void StartBatch()
        {
            Batch = true;
        }

        public void CommitBatch()
        {
            Batch = false;
            SaveData();
        }

        private void NotifyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            SaveData();
        }

        public void SaveData()
        {
            ResourceLocator.ApplicationDataService[_key] = JsonConvert.SerializeObject(StoredItems);
        }

        public void SetCollection(IEnumerable<T> items)
        {
            StoredItems = new ObservableCollection<T>(items);
            StoredItems.CollectionChanged += StoredItemsOnCollectionChanged;

            if (_atomicObjects)
                return;
            StoredItems.ForEach(obj =>
            {
                var notify = (obj as INotifyPropertyChanged);
                notify.PropertyChanged += NotifyOnPropertyChanged;
            });
        }
    }
}
