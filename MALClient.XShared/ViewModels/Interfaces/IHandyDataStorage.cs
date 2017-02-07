using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MALClient.Models.Models;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Interfaces;

namespace MALClient.XShared.ViewModels.Interfaces
{
    public interface IHandyDataStorage
    {
        void Init();
        Task SaveData();
        //
        IHandyDataStorageModule<MalUser> PinnedUsers { get; }
        //
        IHandyDataStorageModule<WatchedTopicModel> WatchedTopics { get; }
        //
        ReadOnlyDictionary<string, List<StarredForumMessage>> StarredMessages { get; }
        void StarForumMessage(StarredForumMessage forumMessage);
        void UnstarForumMessage(string messageId, MalUser poster);
        bool IsMessageStarred(string forumMessageId, MalUser poster);
        void ResetStarredMessages(MalUser user);
        void ResetStarredMessages();
        //
    }
}
