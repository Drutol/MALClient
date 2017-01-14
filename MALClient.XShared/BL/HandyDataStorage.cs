using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Interfaces;
using MALClient.Models.Models;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MALClient.XShared.BL
{
    public class HandyDataStorage : IHandyDataStorage
    {
        public HandyDataStorage()
        {
            LoadPinnedUsers();
        }


        #region PinnedUsers

        public ObservableCollection<MalUser> PinnedUsers { get; private set; }

        private void LoadPinnedUsers()
        {
            var data = ResourceLocator.ApplicationDataService["PinnedUsers"] as string;
            if (data != null)
                PinnedUsers = new ObservableCollection<MalUser>(JsonConvert.DeserializeObject<List<MalUser>>(data));
            else
                PinnedUsers = new ObservableCollection<MalUser>();

            PinnedUsers.CollectionChanged += PinnedUsersOnCollectionChanged;
        }

        private void PinnedUsersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            SavePinnedUsers();
        }

        private void SavePinnedUsers()
        {
            ResourceLocator.ApplicationDataService["PinnedUsers"] = JsonConvert.SerializeObject(PinnedUsers);
        }

        #endregion

    }
}
