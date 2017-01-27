using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Models;
using MALClient.Models.Models.Forums;

namespace MALClient.Models.Interfaces
{
    public interface IHandyDataStorage
    {
        void Init();
        Task SaveData();
        //
        ObservableCollection<MalUser> PinnedUsers { get; }
        //
        ReadOnlyDictionary<string,List<StarredForumMessage>> StarredMessages { get; }
        void StarForumMessage(StarredForumMessage forumMessage);
        void UnstarForumMessage(string messageId, MalUser poster);
        bool IsMessageStarred(string forumMessageId, MalUser poster);
        void ResetStarredMessages(MalUser user);
        void ResetStarredMessages();
    }
}
