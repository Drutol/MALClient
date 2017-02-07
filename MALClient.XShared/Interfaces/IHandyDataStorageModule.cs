using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Interfaces
{
    public interface IHandyDataStorageModule<T> : IHandyDataStorageModuleBase
    {
        ObservableCollection<T> StoredItems { get; }
        void SetCollection(IEnumerable<T> items);
    }
}
