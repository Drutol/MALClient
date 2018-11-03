using System;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;

namespace MALClient.XShared.Interfaces
{
    public interface IShareManager
    {
        event EventHandler<bool> TimerStateChanged;

        void EnqueueEvent(ShareEvent action, AnimeShareDiff diff);
        string GenerateMessage();
    }
}