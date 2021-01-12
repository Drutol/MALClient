using System;
using System.Collections.Generic;
using System.Text;

namespace MALClient.Models.Enums
{
    [Flags]
    public enum ShareEvent
    {
        None = 0,
        AnimeStatusChanged = 1,
        AnimeScoreChanged = 2,
        AnimeEpisodesChanged = 4,
        StartedRewatching = 8,
        ChangedRewatchingCount = 16
    }
}
