using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Adapters;
using MALClient.Models.Interfaces;
using MALClient.Models.Models;
using MALClient.Models.Models.Forums;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MALClient.XShared.BL
{
    public class HandyDataStorage : IHandyDataStorage
    {
        private readonly IApplicationDataService _applicationDataService;
        private readonly IDataCache _dataCache;

        public HandyDataStorage(IApplicationDataService applicationDataService,IDataCache dataCache)
        {
            _applicationDataService = applicationDataService;
            _dataCache = dataCache;
        }

        public async void Init()
        {
            LoadPinnedUsers();
            _starredMessages =
                await _dataCache.RetrieveDataRoaming<Dictionary<string, List<StarredForumMessage>>>("starred_messages",
                    0) ?? new Dictionary<string, List<StarredForumMessage>>();

        }

        public async Task SaveData()
        {
            if (_updated)
            {
                await _dataCache.SaveDataRoaming(_starredMessages, "starred_messages");
            }
        }
        #region PinnedUsers

        public ObservableCollection<MalUser> PinnedUsers { get; private set; }

        private void LoadPinnedUsers()
        {
            var data = _applicationDataService["PinnedUsers"] as string;
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


        #region StarredMessages

        private bool _updated;
        private Dictionary<string, List<StarredForumMessage>> _starredMessages;


        public void ResetStarredMessages(MalUser user)
        {

            if (_starredMessages.ContainsKey(user.Name.ToLower()))
            {
                _starredMessages.Remove(user.Name.ToLower());
                _updated = true;
            }
        }

        public void ResetStarredMessages()
        {
            _starredMessages = new Dictionary<string, List<StarredForumMessage>>();
            _updated = true;
        }

        public void StarForumMessage(StarredForumMessage forumMessage)
        {
            var key = forumMessage.Poster.Name.ToLower();
            if (!_starredMessages.ContainsKey(key))
            {
                _starredMessages.Add(key,new List<StarredForumMessage> { forumMessage });
            }
            else
            {
                if(_starredMessages[key].All(message => message.MessageId != forumMessage.MessageId))
                {
                    _starredMessages[key].Add(forumMessage);
                    _starredMessages[key][0].Poster = forumMessage.Poster; //update first poster entry
                }
            }
            _updated = true;
        }

        public void UnstarForumMessage(string messageId, MalUser poster)
        {
            var key = poster.Name.ToLower();
            if (_starredMessages.ContainsKey(key))
            {
                _starredMessages[key].RemoveAt(
                    _starredMessages[key].FindIndex(message => message.MessageId == messageId));
                if (!_starredMessages[key].Any())
                    _starredMessages.Remove(key);
            }
            _updated = true;
        }

        public bool IsMessageStarred(string forumMessageId, MalUser poster)
        {
            var key = poster.Name.ToLower();
            if (_starredMessages.ContainsKey(key))
            {
                return _starredMessages[key].Any(message => message.MessageId == forumMessageId);
            }
            return false;
        }

        public ReadOnlyDictionary<string, List<StarredForumMessage>> StarredMessages => new ReadOnlyDictionary<string, List<StarredForumMessage>>(_starredMessages);

        #endregion
    }
}
